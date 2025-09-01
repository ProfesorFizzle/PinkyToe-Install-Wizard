using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PinkyToeInstallWizard
{
    internal static class MultiPartHelper
    {
        private static readonly Regex SevenZipNumbered = new(@"\.7z\.(\d{3})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool Validate(string firstPartPath, out List<string> parts, out string? missing)
        {
            parts = new List<string>();
            missing = null;
            var dir = Path.GetDirectoryName(firstPartPath)!;
            var file = Path.GetFileName(firstPartPath);

            var m = SevenZipNumbered.Match(file);
            if (m.Success)
            {
                var prefix = file.Substring(0, file.Length - 4);
                int idx = int.Parse(m.Groups[1].Value);
                while (true)
                {
                    var candidate = $"{prefix}{idx:000}";
                    var candidatePath = Path.Combine(dir, candidate);
                    if (File.Exists(candidatePath))
                        parts.Add(candidatePath);
                    else
                        break;
                    idx++;
                }
                return parts.Count > 0;
            }

            if (file.EndsWith(".part1.rar", StringComparison.OrdinalIgnoreCase))
            {
                var baseName = file[..file.ToLowerInvariant().IndexOf(".part1.rar", StringComparison.Ordinal)];
                int part = 1;
                while (true)
                {
                    var candidateFile = $"{baseName}.part{part}.rar";
                    var candidatePath = Path.Combine(dir, candidateFile);
                    if (File.Exists(candidatePath))
                        parts.Add(candidatePath);
                    else
                        break;
                    part++;
                }
                return parts.Count > 0;
            }

            if (file.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add(firstPartPath);
                var baseName = file[..^4];
                int part = 2;
                while (true)
                {
                    var candidate = Path.Combine(dir, $"{baseName}.part{part}.rar");
                    if (File.Exists(candidate))
                        parts.Add(candidate);
                    else
                        break;
                    part++;
                }
                return true;
            }

            if (file.EndsWith(".7z", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add(firstPartPath);
                return true;
            }

            parts.Add(firstPartPath);
            return true;
        }
    }
}