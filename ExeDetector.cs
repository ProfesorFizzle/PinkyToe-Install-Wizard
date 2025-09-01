using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SharpCompress.Archives;

namespace PinkyToeInstallWizard
{
    internal static class ExeDetector
    {
        internal class Candidate
        {
            public string FileName { get; set; } = "";
            public string RelativePath { get; set; } = "";
            public long Size { get; set; }
            public int Score { get; set; }
            public List<string> Reasons { get; set; } = new();
            public bool IsExactBaseMatch { get; set; }
            public bool IsNearExactBaseMatch { get; set; }
            public bool HasIcon { get; set; }
        }

        private static readonly string[] HardExclusionKeywords =
        {
            "setup","install","installer","uninstall","unins","updater","update","patch","vcredist","dxsetup",
            "crash","report","crashhandler","configtool","dep","redistributable","redist","prereq"
        };

        private static readonly string[] SoftNegativeKeywords =
        {
            "benchmark","server","editor","dedicated","tool","patcher","checker","diag","diagnostic"
        };

        private static readonly string[] PlatformTokens =
        {
            "win64","x64","x86_64","win32","shipping","client","game","win64shipping"
        };

        private static readonly string[] NoiseTokens =
        {
            "part1","part2","part3","part4","part5",
            "fitgirl","repack","repackby","fg","dodi","elamigos","gog","multi",
            "steamrip","online","onlinefix","online-fix","fix","cracked","crack",
            "vcredist","dxsetup","setup","installer","update","patch","bin","binaries"
        };

        private static readonly char[] TokenSplitChars =
            { '-', '_', '.', ' ', '[', ']', '(', ')', '{', '}', '+', '!' };

        private const int WEIGHT_EXACT = 70;
        private const int WEIGHT_NEAR_EXACT = 55;
        private const int WEIGHT_ICON = 28;
        private const int WEIGHT_LAUNCHER = 30;
        private const int WEIGHT_LAUNCHER_WITH_EXACT_PRESENT_PENALTY = -5;
        private const int WEIGHT_PLATFORM = 15;
        private const int WEIGHT_SHIPPING_SUFFIX = 15;
        private const int WEIGHT_LARGEST = 18;
        private const int WEIGHT_NEAR_LARGEST = 10;
        private const int PENALTY_SOFT_NEG = -18;
        private const int PENALTY_VERY_SMALL = -25;
        private const int PENALTY_VERY_SMALL_EXACT_REDUCED = -5;

        private const long MAX_ICON_SCAN_BYTES = 80L * 1024 * 1024;
        private const int MAX_SECTION_HEADERS = 40;

        public static (Candidate chosen, List<Candidate> all, bool autoSelected) DetectMainExe(
            IEnumerable<IArchiveEntry> entries,
            string archiveBaseName)
        {
            var exeEntries = entries
                .Where(e => !e.IsDirectory && e.Key.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (exeEntries.Count == 0)
                throw new InvalidOperationException("No exe entries supplied to detector.");

            var sanitizedBase = SanitizeArchiveBaseName(archiveBaseName);
            var baseTokens = Tokenize(sanitizedBase);

            var candidates = new List<Candidate>();
            foreach (var entry in exeEntries)
            {
                var fileName = Path.GetFileName(entry.Key);
                var lower = fileName.ToLowerInvariant();
                bool hardExcluded = HardExclusionKeywords.Any(k => lower.Contains(k));
                if (hardExcluded && exeEntries.Count > 1)
                    continue;

                candidates.Add(new Candidate
                {
                    FileName = fileName,
                    RelativePath = entry.Key.Replace('\\', '/'),
                    Size = (long)entry.Size
                });
            }

            if (candidates.Count == 0)
            {
                candidates = exeEntries.Select(e => new Candidate
                {
                    FileName = Path.GetFileName(e.Key),
                    RelativePath = e.Key.Replace('\\', '/'),
                    Size = (long)e.Size
                }).ToList();
            }

            if (candidates.Count == 1)
            {
                var only = candidates[0];
                only.Score = 100;
                only.IsExactBaseMatch = true;
                only.Reasons.Add("Only executable candidate");
                return (only, candidates, true);
            }

            long largestSize = candidates.Max(c => c.Size);
            var nearExactForms = GenerateNearExactVariants(sanitizedBase);

            using var archiveForIcons = ReopenArchiveForIcons(entries);

            foreach (var c in candidates)
            {
                var nameLower = c.FileName.ToLowerInvariant();
                var nameNoExt = Path.GetFileNameWithoutExtension(c.FileName);
                var nameNoExtLower = nameNoExt.ToLowerInvariant();
                int score = 0;

                if (nameNoExtLower == sanitizedBase)
                {
                    score += WEIGHT_EXACT;
                    c.IsExactBaseMatch = true;
                    c.Reasons.Add("Exact match to sanitized archive name");
                }
                else if (nearExactForms.Contains(nameNoExtLower))
                {
                    score += WEIGHT_NEAR_EXACT;
                    c.IsNearExactBaseMatch = true;
                    c.Reasons.Add("Near-exact (architecture suffix) match");
                }
                else
                {
                    var exeTokens = Tokenize(nameNoExtLower);
                    int matches = exeTokens.Intersect(baseTokens).Count();
                    if (matches > 0)
                    {
                        int tokenScore = Math.Min(30, matches * 12);
                        score += tokenScore;
                        c.Reasons.Add($"Shares {matches} base name token(s)");
                    }
                }

                bool hasLauncher = nameLower.Contains("launcher");
                if (hasLauncher)
                {
                    score += WEIGHT_LAUNCHER;
                    c.Reasons.Add("Contains 'launcher'");
                }

                if (PlatformTokens.Any(k => nameLower.Contains(k)))
                {
                    score += WEIGHT_PLATFORM;
                    c.Reasons.Add("Contains platform/client hint");
                }

                if (nameNoExtLower.EndsWith("-win64-shipping") || nameNoExtLower.EndsWith("-win64"))
                {
                    score += WEIGHT_SHIPPING_SUFFIX;
                    c.Reasons.Add("Win64 shipping suffix");
                }

                if (c.Size == largestSize)
                {
                    score += WEIGHT_LARGEST;
                    c.Reasons.Add("Largest size");
                }
                else if (largestSize > 0 && c.Size >= largestSize * 0.85)
                {
                    score += WEIGHT_NEAR_LARGEST;
                    c.Reasons.Add("Near-largest size");
                }

                if (SoftNegativeKeywords.Any(k => nameLower.Contains(k)))
                {
                    score += PENALTY_SOFT_NEG;
                    c.Reasons.Add("Likely non-launch (benchmark/server/editor/etc.)");
                }

                if (c.Size < 200 * 1024)
                {
                    if (c.IsExactBaseMatch)
                    {
                        score += PENALTY_VERY_SMALL_EXACT_REDUCED;
                        c.Reasons.Add("Very small executable (reduced penalty due to exact match)");
                    }
                    else
                    {
                        score += PENALTY_VERY_SMALL;
                        c.Reasons.Add("Very small executable");
                    }
                }

                try
                {
                    c.HasIcon = TryScanIconResource(archiveForIcons, c, MAX_ICON_SCAN_BYTES);
                    if (c.HasIcon)
                        score += WEIGHT_ICON;
                    c.Reasons.Add(c.HasIcon ? "Embedded icon resource found" : "No icon resource detected");
                }
                catch
                {
                    c.Reasons.Add("Icon scan failed");
                }

                c.Score = score;
            }

            bool anyExact = candidates.Any(c => c.IsExactBaseMatch || c.IsNearExactBaseMatch);
            if (anyExact)
            {
                foreach (var c in candidates.Where(c =>
                             c.FileName.ToLowerInvariant().Contains("launcher") &&
                             !c.IsExactBaseMatch && !c.IsNearExactBaseMatch))
                {
                    c.Score += WEIGHT_LAUNCHER_WITH_EXACT_PRESENT_PENALTY;
                    c.Reasons.Add("Launcher adjusted (exact/near-exact present)");
                }
            }

            var ordered = candidates
                .OrderByDescending(c => c.Score)
                .ThenByDescending(c => c.HasIcon)
                .ThenByDescending(c => c.IsExactBaseMatch || c.IsNearExactBaseMatch)
                .ThenBy(c => c.FileName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var top = ordered[0];
            var second = ordered.Count > 1 ? ordered[1] : null;
            bool auto = false;

            var exactGroup = ordered.Where(c => c.IsExactBaseMatch || c.IsNearExactBaseMatch).ToList();
            if (exactGroup.Count == 1 && (top.IsExactBaseMatch || top.IsNearExactBaseMatch))
            {
                auto = true;
                top.Reasons.Add("Auto-selected: sole (near) exact match");
            }
            else if (top.IsExactBaseMatch && top.HasIcon && (second == null || top.Score - second.Score >= 8))
            {
                auto = true;
                top.Reasons.Add("Auto-selected: exact match with icon lead");
            }
            else if (top.Score >= 65 && (second == null || top.Score - second.Score >= 12))
            {
                auto = true;
                top.Reasons.Add("Auto-selected: high confidence lead");
            }
            else if ((top.IsExactBaseMatch || top.IsNearExactBaseMatch) && (second == null || top.Score - second.Score >= 10))
            {
                auto = true;
                top.Reasons.Add("Auto-selected: (near) exact match lead");
            }
            else if (top.HasIcon && !ordered.Skip(1).Any(c => c.HasIcon && c.Score >= top.Score - 5))
            {
                auto = true;
                top.Reasons.Add("Auto-selected: only strong icon-bearing candidate");
            }

            return (top, ordered, auto);
        }

        private static string SanitizeArchiveBaseName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            var lower = raw.ToLowerInvariant();
            lower = Regex.Replace(lower, @"\.part\d+$", "");
            lower = Regex.Replace(lower, @"\.r\d+$", "");
            lower = Regex.Replace(lower, @"\.7z\.\d{3}$", "");
            lower = Regex.Replace(lower, @"([-_\.])v?\d+(\.\d+){0,3}$", "");
            foreach (var ch in TokenSplitChars)
                lower = lower.Replace(ch, ' ');
            lower = Regex.Replace(lower, @"\s+", " ").Trim();
            var tokens = lower.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Where(t => !NoiseTokens.Contains(t) && t.Length > 1)
                              .ToList();
            return string.Join(" ", tokens);
        }

        private static HashSet<string> GenerateNearExactVariants(string baseName)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(baseName))
                return set;

            set.Add(baseName);
            var compact = baseName.Replace(" ", "");
            set.Add(compact);

            var suffixes = new[] { "64", "32", "x64", "x86", "_x64", "_x86", "-win64", "-win32" };
            foreach (var s in suffixes)
            {
                set.Add(compact + s);
                set.Add(baseName + s);
            }
            return set;
        }

        private static List<string> Tokenize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            var raw = input
                .Split(TokenSplitChars, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => t.Length > 1)
                .Select(t => t.ToLowerInvariant())
                .Where(t => !NoiseTokens.Contains(t));

            return raw.Distinct().ToList();
        }

        private static IArchive ReopenArchiveForIcons(IEnumerable<IArchiveEntry> entries)
        {
            var first = entries.First();
            return first.Archive;
        }

        private static bool TryScanIconResource(IArchive archive, Candidate c, long maxBytes)
        {
            var entry = archive.Entries.FirstOrDefault(e =>
                !e.IsDirectory &&
                e.Key.Replace('\\', '/').Equals(c.RelativePath, StringComparison.OrdinalIgnoreCase));

            if (entry == null) return false;
            if (entry.Size > maxBytes) return false;

            using var ms = new MemoryStream();
            entry.WriteTo(ms);
            if (ms.Length < 512) return false;
            ms.Position = 0;
            var data = ms.GetBuffer();
            return PeHasIconResource(data, (int)ms.Length);
        }

        private static bool PeHasIconResource(byte[] buf, int length)
        {
            try
            {
                if (length < 0x100) return false;
                if (buf[0] != 'M' || buf[1] != 'Z') return false;
                int peOffset = BitConverter.ToInt32(buf, 0x3C);
                if (peOffset <= 0 || peOffset > length - 0x200) return false;
                if (peOffset + 4 > length) return false;
                if (buf[peOffset] != 'P' || buf[peOffset + 1] != 'E' || buf[peOffset + 2] != 0 || buf[peOffset + 3] != 0)
                    return false;

                int coff = peOffset + 4;
                if (coff + 20 > length) return false;
                ushort numberOfSections = BitConverter.ToUInt16(buf, coff + 2);
                ushort optHeaderSize = BitConverter.ToUInt16(buf, coff + 16);
                int optHeaderStart = coff + 20;
                if (optHeaderStart + optHeaderSize > length) return false;
                ushort magic = BitConverter.ToUInt16(buf, optHeaderStart);
                bool isPE32Plus = magic == 0x20B;
                int resourceDirRvaOffset = isPE32Plus ? optHeaderStart + 112 : optHeaderStart + 96;
                if (resourceDirRvaOffset + 8 > length) return false;
                int resourceRva = BitConverter.ToInt32(buf, resourceDirRvaOffset);
                int resourceSize = BitConverter.ToInt32(buf, resourceDirRvaOffset + 4);
                if (resourceRva <= 0 || resourceSize <= 0) return false;

                int sectionTableStart = optHeaderStart + optHeaderSize;
                if (sectionTableStart + numberOfSections * 40 > length) return false;

                int rsrcRawPtr = 0;
                int rsrcRawSize = 0;

                for (int i = 0; i < numberOfSections && i < MAX_SECTION_HEADERS; i++)
                {
                    int secOffset = sectionTableStart + i * 40;
                    if (secOffset + 40 > length) break;
                    int virtualAddress = BitConverter.ToInt32(buf, secOffset + 12);
                    int rawSize = BitConverter.ToInt32(buf, secOffset + 16);
                    int rawPtr = BitConverter.ToInt32(buf, secOffset + 20);
                    if (rawPtr <= 0 || rawSize <= 0 || rawPtr + rawSize > length) continue;

                    if (resourceRva >= virtualAddress && resourceRva < virtualAddress + rawSize)
                    {
                        int delta = resourceRva - virtualAddress;
                        if (delta >= 0 && delta < rawSize)
                        {
                            rsrcRawPtr = rawPtr + delta;
                            rsrcRawSize = rawSize - delta;
                        }
                        break;
                    }
                }

                if (rsrcRawPtr == 0 || rsrcRawPtr >= length) return false;
                if (rsrcRawPtr + 16 > length) return false;
                ushort numberOfNameEntries = BitConverter.ToUInt16(buf, rsrcRawPtr + 12);
                ushort numberOfIdEntries = BitConverter.ToUInt16(buf, rsrcRawPtr + 14);
                int totalEntries = numberOfNameEntries + numberOfIdEntries;
                int entryStart = rsrcRawPtr + 16;
                int entrySize = 8;

                for (int i = 0; i < totalEntries; i++)
                {
                    int eOff = entryStart + i * entrySize;
                    if (eOff + 8 > length) break;
                    int nameOrId = BitConverter.ToInt32(buf, eOff);
                    if ((nameOrId & 0x80000000) == 0)
                    {
                        int id = nameOrId;
                        if (id == 14 || id == 3)
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}