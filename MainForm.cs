#nullable enable
using System;
using System.Linq;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Readers;
using System.Reflection;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace PinkyToeInstallWizard
{
    public partial class MainForm : Form
    {
        private string archivePath = "";
        private string baseInstallFolder = "";
        private string installFolder = "";
        private string password = "";
        private readonly string logoResourceName = "PinkyToeInstallWizard.logo.jpg";
        private bool passwordRequired = false;

        private ExeDetector.Candidate? selectedExeCandidate;
        private List<ExeDetector.Candidate> exeCandidates = new();
        private bool exeAutoSelected = false;
        private string lastCompletedInstallFolder = "";

        private const int PROGRESS_SCALE = 10000;
        private const long ICON_EXTRACTION_MAX_BYTES = 150L * 1024 * 1024;

        public MainForm()
        {
            InitializeComponent();
            this.Text = $"PinkyToe's Install Wizard {VersionInfo.UiVersion}";
            lblVersion.Text = VersionInfo.UiVersion;

            lblStatus.Text = "Welcome to PinkyToe's Install Wizard!";
            lblMultiPartHelp.Text = "For multi-part archives select the first file (.rar/.part1.rar/.7z/.7z.001/.zip).";
            progressBar.Value = 0;
            progressBar.Visible = false;
            lblMainExe.Text = "Main Executable: (not selected yet)";
            lblTimeRemaining.Text = "";

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(logoResourceName))
            {
                if (stream != null)
                    picLogo.Image = Image.FromStream(stream);
            }

            baseInstallFolder = SettingsManager.GetDefaultInstallPath();
            if (!string.IsNullOrWhiteSpace(baseInstallFolder))
                lblFolderPath.Text = $"Install To: {baseInstallFolder} (default)";

            AllowDrop = true;
            DragEnter += MainForm_DragEnter;
            DragDrop += MainForm_DragDrop;

            chkCreateDesktopShortcut.Checked = true;
            chkCreateStartMenuShortcut.Checked = false;
        }

        private void MainForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effect = (files.Length == 1 && IsSupportedArchive(files[0]))
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
            }
        }

        private void MainForm_DragDrop(object? sender, DragEventArgs e)
        {
            var files = (string[])e.Data!.GetData(DataFormats.FileDrop);
            if (files.Length == 1 && IsSupportedArchive(files[0]))
                SetArchive(files[0]);
        }

        private void btnSelectRar_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Archives (*.rar;*.part1.rar;*.7z;*.7z.*;*.zip)|*.rar;*.part1.rar;*.7z;*.7z.*;*.zip",
                Title = "Step 1: Select the archive you wish to install"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                SetArchive(ofd.FileName);
        }

        private bool IsSupportedArchive(string path)
        {
            var lowerExt = Path.GetExtension(path).ToLowerInvariant();
            return lowerExt is ".rar" or ".7z" or ".zip"
                   || path.ToLowerInvariant().EndsWith(".7z.001")
                   || path.ToLowerInvariant().Contains(".part1.rar");
        }

        private void SetArchive(string path)
        {
            ResetExeSelectionDisplay();
            archivePath = path;
            lblRarPath.Text = $"Archive Selected: {archivePath}";
            DetectPasswordStatus(archivePath);

            if (!MultiPartHelper.Validate(archivePath, out _, out var missingPart))
            {
                lblStatus.Text = $"Missing archive part: {missingPart}";
                MessageBox.Show($"Multi-part archive appears incomplete (missing {missingPart}). Place all parts together.", "Install Wizard");
                btnSelectFolder.Enabled = false;
                btnExtract.Enabled = false;
                return;
            }

            lblStatus.Text = "Step 2: Select Install Directory (or use default).";
            btnSelectFolder.Enabled = true;
            btnExtract.Enabled = !string.IsNullOrEmpty(baseInstallFolder);
        }

        private void DetectPasswordStatus(string path)
        {
            passwordRequired = false;
            try
            {
                using var archive = ArchiveFactory.Open(path);
                passwordRequired = archive.Entries.Any(e => e.IsEncrypted);
            }
            catch
            {
                passwordRequired = true;
            }

            lblPasswordStatus.Text = passwordRequired
                ? "Password detected (will try saved passwords, then prompt)."
                : "No password required.";
        }

        private void btnSelectFolder_Click(object? sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "Step 2: Select the directory you wish to install into"
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                baseInstallFolder = fbd.SelectedPath;
                SettingsManager.SetDefaultInstallPath(baseInstallFolder);
                lblFolderPath.Text = $"Install To: {baseInstallFolder}";
                lblStatus.Text = "Step 3: Click 'Extract!' to begin installation.";
                btnExtract.Enabled = true;
            }
        }

        private string GetUniqueInstallFolder(string baseDir, string archiveBaseName)
        {
            var candidate = Path.Combine(baseDir, archiveBaseName);
            if (!Directory.Exists(candidate)) return candidate;
            int n = 2;
            while (true)
            {
                var c = Path.Combine(baseDir, $"{archiveBaseName} ({n})");
                if (!Directory.Exists(c)) return c;
                n++;
            }
        }

        private string GetUniqueShortcutPath(string folder, string baseName)
        {
            var candidate = Path.Combine(folder, baseName + ".lnk");
            if (!File.Exists(candidate)) return candidate;
            int n = 2;
            while (true)
            {
                var c = Path.Combine(folder, $"{baseName} ({n}).lnk");
                if (!File.Exists(c)) return c;
                n++;
            }
        }

        private async void btnExtract_Click(object? sender, EventArgs e)
        {
            btnExtract.Enabled = false;
            lblStatus.Text = "Inspecting archive...";
            progressBar.Visible = false;
            progressBar.Value = 0;
            lblTimeRemaining.Text = "";
            selectedExeCandidate = null;
            exeAutoSelected = false;

            List<IArchiveEntry>? entriesForDetection = null;
            bool inspected = false;

            var passwordsToTry = SettingsManager.GetSavedPasswords();
            if (!passwordRequired)
            {
                if (TryGetEntries(archivePath, null, out entriesForDetection))
                {
                    inspected = true;
                    password = "";
                }
            }
            if (!inspected && passwordRequired)
            {
                foreach (var pwd in passwordsToTry)
                {
                    if (TryGetEntries(archivePath, pwd, out entriesForDetection))
                    {
                        inspected = true;
                        password = pwd;
                        break;
                    }
                }

                while (!inspected)
                {
                    using var prompt = new PasswordPrompt();
                    if (prompt.ShowDialog() == DialogResult.OK)
                    {
                        var manual = prompt.Password;
                        if (TryGetEntries(archivePath, manual, out entriesForDetection))
                        {
                            inspected = true;
                            password = manual;
                            break;
                        }
                        var retry = MessageBox.Show("Incorrect password. Try again?", "Install Wizard",
                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                        if (retry == DialogResult.Cancel)
                        {
                            ResetAfterCancel("Archive inspection cancelled.");
                            return;
                        }
                    }
                    else
                    {
                        ResetAfterCancel("Archive inspection cancelled.");
                        return;
                    }
                }
            }
            else if (!inspected)
            {
                inspected = TryGetEntries(archivePath, null, out entriesForDetection);
            }

            if (!inspected || entriesForDetection == null)
            {
                ResetAfterCancel("Unable to read archive entries.");
                return;
            }

            var archiveBaseName = Path.GetFileNameWithoutExtension(archivePath);
            var exeEntries = entriesForDetection
                .Where(e => !e.IsDirectory && e.Key.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (exeEntries.Count == 0)
            {
                ResetAfterCancel("No .exe files found.");
                MessageBox.Show("No .exe files found in this archive.", "Install Wizard");
                return;
            }

            var (chosen, all, autoSelected) = ExeDetector.DetectMainExe(exeEntries, archiveBaseName);
            exeCandidates = all;
            selectedExeCandidate = chosen;
            exeAutoSelected = autoSelected;
            UpdateExeSelectionDisplay();

            if (!autoSelected)
            {
                Dictionary<string, Image> iconMap;
                try
                {
                    lblStatus.Text = "Preparing executable icons...";
                    iconMap = ExtractCandidateIcons(exeCandidates, password);
                }
                catch
                {
                    iconMap = new Dictionary<string, Image>();
                }

                using var chooser = new ChooseExeForm(exeCandidates, selectedExeCandidate!, iconMap);
                if (chooser.ShowDialog() == DialogResult.OK && chooser.SelectedCandidate != null)
                {
                    selectedExeCandidate = chooser.SelectedCandidate;
                }
                else if (chooser.SelectedCandidate == null)
                {
                    ResetAfterCancel("Executable selection cancelled.");
                    return;
                }
                UpdateExeSelectionDisplay();
            }

            var existing = DetectExistingInstall(selectedExeCandidate!.FileName, baseInstallFolder);
            int userChoice = 0;
            if (existing != null)
            {
                var msg =
                    $"A current installation of \"{selectedExeCandidate.FileName}\" was detected at:\n{existing}\n\n" +
                    "If you continue, files will be overwritten (update).\n\nChoose an option:";
                using var upd = new UpdatePromptDialog(msg);
                _ = upd.ShowDialog();
                userChoice = upd.UserChoice;
                if (userChoice == 1)
                {
                    ResetAfterCancel("Install cancelled.");
                    return;
                }
                installFolder = userChoice == 0
                    ? existing
                    : GetUniqueInstallFolder(baseInstallFolder, archiveBaseName);
            }
            else
            {
                installFolder = GetUniqueInstallFolder(baseInstallFolder, archiveBaseName);
            }

            if (!Directory.Exists(installFolder))
                Directory.CreateDirectory(installFolder);

            lblFolderPath.Text = $"Install To: {installFolder}";
            lblStatus.Text = "Checking disk space...";

            long totalBytes = entriesForDetection.Where(e => !e.IsDirectory).Sum(e => (long)e.Size);
            TryWarnLowDisk(installFolder, totalBytes);

            bool updating = existing != null && userChoice == 0;
            lblStatus.Text = updating ? "Updating installation..." : "Extracting... please wait.";
            progressBar.Visible = true;
            progressBar.Value = 0;
            progressBar.Maximum = PROGRESS_SCALE;

            try
            {
                await ExtractArchiveAsync(archivePath, password, installFolder, updating);
            }
            catch (Exception ex)
            {
                ResetAfterCancel("Error during extraction: " + ex.Message);
                MessageBox.Show("Error during extraction: " + ex.Message, "Install Wizard");
                return;
            }

            string finalExePath = ResolveExtractedExePath(installFolder, selectedExeCandidate);
            TryCreateShortcuts(finalExePath);

            if (chkLaunchAfterInstall.Checked)
                TryLaunch(finalExePath);

            lastCompletedInstallFolder = installFolder;
            btnOpenInstallFolder.Enabled = true;

            lblStatus.Text = updating ? "Update complete!" : "Extraction complete!";
            lblTimeRemaining.Text = "Done.";
            MessageBox.Show(updating ? "Update complete!" : "Extraction complete!", "Install Wizard");

            lblFolderPath.Text = $"Install To: {baseInstallFolder}";
            installFolder = baseInstallFolder;
            btnExtract.Enabled = false;
            btnSelectFolder.Enabled = true;
            progressBar.Visible = false;
        }

        private bool TryGetEntries(string path, string? pwd, out List<IArchiveEntry> entries)
        {
            entries = new List<IArchiveEntry>();
            try
            {
                using var archive = ArchiveFactory.Open(path, new ReaderOptions { Password = pwd });
                entries = archive.Entries.ToList();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task ExtractArchiveAsync(string path, string pwd, string targetDir, bool updating)
        {
            await Task.Run(() =>
            {
                using var archive = ArchiveFactory.Open(path, new ReaderOptions { Password = string.IsNullOrEmpty(pwd) ? null : pwd });
                var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
                long totalBytes = entries.Sum(e => (long)e.Size);
                if (totalBytes <= 0) totalBytes = 1;
                var sw = Stopwatch.StartNew();
                long processed = 0;

                foreach (var entry in entries)
                {
                    var safe = PathSafety.GetValidatedTargetRooted(targetDir, entry.Key);
                    if (safe != null)
                    {
                        entry.WriteToDirectory(targetDir, new SharpCompress.Common.ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                    processed += (long)entry.Size;

                    double pct = processed * 100.0 / totalBytes;
                    int scaled = (int)(processed * PROGRESS_SCALE / totalBytes);
                    if (scaled > PROGRESS_SCALE) scaled = PROGRESS_SCALE;

                    var elapsed = sw.Elapsed.TotalSeconds;
                    string etaText;
                    if (elapsed >= 1 && processed < totalBytes)
                    {
                        double rate = processed / elapsed;
                        double remainingBytes = totalBytes - processed;
                        double remainingSec = remainingBytes / rate;
                        etaText = FormatEta(remainingSec);
                    }
                    else if (processed >= totalBytes)
                        etaText = "Done";
                    else
                        etaText = "";

                    var speedMBs = elapsed > 0 ? (processed / 1024.0 / 1024.0 / elapsed) : 0;
                    string status = updating
                        ? $"Updating... {pct:F1}%"
                        : $"Extracting... {pct:F1}%";

                    var progressInfo =
                        $"{status}  {processed / 1024.0 / 1024.0:F1} / {totalBytes / 1024.0 / 1024.0:F1} MB  " +
                        $"{speedMBs:F1} MB/s  ETA: {etaText}";

                    Invoke(new Action(() =>
                    {
                        progressBar.Value = scaled;
                        lblStatus.Text = status;
                        lblTimeRemaining.Text = progressInfo;
                    }));
                }
            });
        }

        private string FormatEta(double seconds)
        {
            if (seconds < 1) return "<1s";
            var ts = TimeSpan.FromSeconds(seconds);
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}h {ts.Minutes}m";
            if (ts.TotalMinutes >= 1)
                return $"{(int)ts.TotalMinutes}m {ts.Seconds}s";
            return $"{ts.Seconds}s";
        }

        private string ResolveExtractedExePath(string installRoot, ExeDetector.Candidate candidate)
        {
            var rel = candidate.RelativePath.Replace('/', Path.DirectorySeparatorChar);
            var full = Path.Combine(installRoot, rel);
            if (File.Exists(full))
                return full;

            var matches = Directory.GetFiles(installRoot, candidate.FileName, SearchOption.AllDirectories);
            if (matches.Length > 0) return matches[0];

            var anyExe = Directory.GetFiles(installRoot, "*.exe", SearchOption.AllDirectories).FirstOrDefault();
            return anyExe ?? full;
        }

        private void TryCreateShortcuts(string finalExePath)
        {
            try
            {
                if (chkCreateDesktopShortcut.Checked || chkCreateStartMenuShortcut.Checked)
                {
                    var baseName = Path.GetFileNameWithoutExtension(finalExePath);
                    if (chkCreateDesktopShortcut.Checked)
                    {
                        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        var desktopShortcut = GetUniqueShortcutPath(desktop, baseName);
                        ShortcutHelper.CreateShortcut(finalExePath, desktopShortcut);
                    }
                    if (chkCreateStartMenuShortcut.Checked)
                    {
                        var startMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                        var programsDir = Path.Combine(startMenu, "Programs");
                        var startMenuShortcut = GetUniqueShortcutPath(programsDir, baseName);
                        ShortcutHelper.CreateShortcut(finalExePath, startMenuShortcut);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Shortcut creation failed: " + ex.Message, "Install Wizard");
            }
        }

        private void TryLaunch(string finalExePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = finalExePath,
                    WorkingDirectory = Path.GetDirectoryName(finalExePath),
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to launch game: " + ex.Message, "Install Wizard");
            }
        }

        private string? DetectExistingInstall(string mainExeName, string baseDir)
        {
            if (!Directory.Exists(baseDir))
                return null;

            foreach (var dir in Directory.GetDirectories(baseDir))
            {
                try
                {
                    var exes = Directory.GetFiles(dir, "*.exe", SearchOption.AllDirectories);
                    foreach (var exe in exes)
                        if (Path.GetFileName(exe).Equals(mainExeName, StringComparison.OrdinalIgnoreCase))
                            return dir;
                }
                catch { }
            }
            return null;
        }

        private void ResetAfterCancel(string status)
        {
            lblStatus.Text = status;
            lblTimeRemaining.Text = "";
            btnExtract.Enabled = false;
            progressBar.Visible = false;
        }

        private void ResetExeSelectionDisplay()
        {
            selectedExeCandidate = null;
            exeCandidates.Clear();
            exeAutoSelected = false;
            lblMainExe.Text = "Main Executable: (not selected yet)";
            lnkChangeExe.Enabled = false;
        }

        private void UpdateExeSelectionDisplay()
        {
            if (selectedExeCandidate == null)
            {
                lblMainExe.Text = "Main Executable: (none)";
                lnkChangeExe.Enabled = false;
            }
            else
            {
                lblMainExe.Text = $"Main Executable: {selectedExeCandidate.FileName}" +
                                  (exeAutoSelected ? " (auto)" : "");
                lnkChangeExe.Enabled = exeCandidates.Count > 1;
            }
        }

        private Dictionary<string, Image> ExtractCandidateIcons(List<ExeDetector.Candidate> candidates, string pwd)
        {
            var icons = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
            string tempDir = Path.Combine(Path.GetTempPath(), "PinkyToeInstallWizard_IconCache_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                using var archive = ArchiveFactory.Open(archivePath, new ReaderOptions { Password = string.IsNullOrEmpty(pwd) ? null : pwd });
                foreach (var c in candidates)
                {
                    Image img = SystemIcons.Application.ToBitmap();
                    try
                    {
                        var entry = archive.Entries.FirstOrDefault(e =>
                            !e.IsDirectory &&
                            e.Key.Replace('\\', '/').Equals(c.RelativePath, StringComparison.OrdinalIgnoreCase));

                        if (entry != null)
                        {
                            if (entry.Size > ICON_EXTRACTION_MAX_BYTES)
                            {
                                c.Reasons.Add("Icon skipped (EXE very large)");
                            }
                            else
                            {
                                string tempFile = Path.Combine(tempDir, c.FileName);
                                using (var fs = File.Create(tempFile))
                                {
                                    entry.WriteTo(fs);
                                }
                                var icon = Icon.ExtractAssociatedIcon(tempFile);
                                if (icon != null)
                                    img = new Bitmap(icon.ToBitmap(), new Size(32, 32));
                            }
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                    icons[c.FileName] = img;
                }
            }
            catch
            {
                // ignore
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
            }

            return icons;
        }

        private void TryWarnLowDisk(string targetPath, long requiredBytes)
        {
            try
            {
                var root = Path.GetPathRoot(targetPath);
                if (!string.IsNullOrEmpty(root))
                {
                    var drive = new DriveInfo(root);
                    long free = drive.AvailableFreeSpace;
                    if (free < requiredBytes)
                    {
                        var neededMB = (requiredBytes / 1024.0 / 1024.0).ToString("F0");
                        var freeMB = (free / 1024.0 / 1024.0).ToString("F0");
                        var result = MessageBox.Show(
                            $"Estimated uncompressed size: {neededMB} MB\nAvailable free space: {freeMB} MB\nContinue anyway?",
                            "Low Disk Space", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.No)
                            ResetAfterCancel("Cancelled due to insufficient disk space.");
                    }
                }
            }
            catch { }
        }

        private void btnPasswordManager_Click(object? sender, EventArgs e)
        {
            using var pm = new PasswordManagerForm();
            pm.ShowDialog();
        }

        private void lnkChangeExe_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            if (exeCandidates.Count == 0 || selectedExeCandidate == null) return;

            Dictionary<string, Image> iconMap;
            try
            {
                iconMap = ExtractCandidateIcons(exeCandidates, password);
            }
            catch
            {
                iconMap = new Dictionary<string, Image>();
            }

            using var chooser = new ChooseExeForm(exeCandidates, selectedExeCandidate, iconMap);
            if (chooser.ShowDialog() == DialogResult.OK && chooser.SelectedCandidate != null)
            {
                selectedExeCandidate = chooser.SelectedCandidate;
                exeAutoSelected = false;
                UpdateExeSelectionDisplay();
            }
        }

        private void btnOpenInstallFolder_Click(object? sender, EventArgs e)
        {
            try
            {
                var folder = string.IsNullOrWhiteSpace(lastCompletedInstallFolder)
                    ? baseInstallFolder
                    : lastCompletedInstallFolder;
                if (Directory.Exists(folder))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = folder,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open folder: " + ex.Message, "Install Wizard");
            }
        }
    }
}