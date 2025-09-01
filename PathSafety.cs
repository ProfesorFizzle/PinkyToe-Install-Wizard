using System;
using System.IO;

namespace PinkyToeInstallWizard
{
    internal static class PathSafety
    {
        public static string? GetValidatedTargetRooted(string baseDir, string entryKey)
        {
            var relative = entryKey.Replace('\\', '/');
            while (relative.StartsWith("/"))
                relative = relative[1..];

            if (relative.Contains("..") || relative.Contains(":"))
                return null;

            var combined = Path.Combine(baseDir, relative);
            var fullBase = Path.GetFullPath(baseDir);
            var fullTarget = Path.GetFullPath(combined);

            return fullTarget.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase) ? fullTarget : null;
        }
    }
}