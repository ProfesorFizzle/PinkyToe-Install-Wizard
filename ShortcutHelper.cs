using System.Runtime.InteropServices;
using IWshRuntimeLibrary;

namespace PinkyToeInstallWizard
{
    internal static class ShortcutHelper
    {
        public static void CreateShortcut(string exePath, string shortcutPath)
        {
            WshShell? shell = null;
            IWshShortcut? shortcut = null;
            try
            {
                shell = new WshShell();
                shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = exePath;
                shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(exePath);
                shortcut.Save();
            }
            finally
            {
                if (shortcut != null) Marshal.FinalReleaseComObject(shortcut);
                if (shell != null) Marshal.FinalReleaseComObject(shell);
            }
        }
    }
}