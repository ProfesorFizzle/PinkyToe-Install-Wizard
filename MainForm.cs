using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Readers;
using IWshRuntimeLibrary;
using System.Reflection;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PinkyToeInstallWizard
{
    public partial class MainForm : Form
    {
        string rarPath = "";
        string baseInstallFolder = "";
        string installFolder = "";
        string password = "";
        string logoResourceName = "PinkyToeInstallWizard.logo.jpg";
        List<string> multiPartFiles = new List<string>();

        // Password notification state
        bool passwordRequired = false;
        bool passwordFound = false;
        string passwordStatusText = "";

        public MainForm()
        {
            InitializeComponent();
            lblStatus.Text = "Welcome to PinkyToe's Install Wizard!";
            lblMultiPartHelp.Text = "For multi-part .rar files, select the first file (.rar or .part1.rar). Make sure all parts are in the same folder.";
            progressBar.Value = 0;
            progressBar.Visible = false;

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(logoResourceName))
            {
                if (stream != null)
                    picLogo.Image = Image.FromStream(stream);
            }

            baseInstallFolder = SettingsManager.GetDefaultInstallPath();
            if (!string.IsNullOrEmpty(baseInstallFolder))
            {
                lblFolderPath.Text = $"Install To: {baseInstallFolder} (default)";
            }

            // Enable drag & drop
            this.AllowDrop = true;
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 && files[0].ToLower().EndsWith(".rar"))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1 && files[0].ToLower().EndsWith(".rar"))
            {
                SetArchive(files[0]);
            }
        }

        private void btnSelectRar_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "RAR files (*.rar;*.part1.rar)|*.rar;*.part1.rar",
                Title = "Step 1: Select the archive you wish to install"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SetArchive(openFileDialog.FileName);
            }
        }

        private void SetArchive(string path)
        {
            rarPath = path;
            lblRarPath.Text = $"Archive Selected: {rarPath}";
            DetectPasswordStatus(rarPath);

            if (!ValidateMultiPartRarFiles(rarPath, out multiPartFiles, out string missingPart))
            {
                lblStatus.Text = $"Missing RAR part: {missingPart}";
                MessageBox.Show($"Multi-part archive detected, but part '{missingPart}' is missing. Please ensure all parts are in the same folder.", "PinkyToe's Install Wizard");
                btnSelectFolder.Enabled = false;
                btnExtract.Enabled = false;
                return;
            }
            lblStatus.Text = "Step 2: Select Install Directory (or use default).";
            btnSelectFolder.Enabled = true;
            btnExtract.Enabled = !string.IsNullOrEmpty(baseInstallFolder);
        }

        private void DetectPasswordStatus(string archivePath)
        {
            passwordRequired = false;
            passwordFound = false;
            passwordStatusText = "";

            try
            {
                using (var archive = ArchiveFactory.Open(archivePath))
                {
                    // If any entry is encrypted, password is required
                    passwordRequired = archive.Entries.Any(e => e.IsEncrypted);
                }
            }
            catch
            {
                // Sometimes SharpCompress will throw if password is required
                passwordRequired = true;
            }

            if (passwordRequired)
            {
                // Is a password in our list likely to work? (We can't test -- so just say "will try saved passwords")
                passwordFound = SettingsManager.GetSavedPasswords().Count > 0;
                if (passwordFound)
                {
                    passwordStatusText = "Password detected: Will try saved passwords first. You will be prompted if needed.";
                }
                else
                {
                    passwordStatusText = "Password detected: No saved passwords found. You will be prompted to enter one.";
                }
            }
            else
            {
                passwordStatusText = "No password required for this archive.";
            }

            lblPasswordStatus.Text = passwordStatusText;
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Step 2: Select the directory you wish to install into"
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                baseInstallFolder = folderBrowserDialog.SelectedPath;
                SettingsManager.SetDefaultInstallPath(baseInstallFolder);
                lblFolderPath.Text = $"Install To: {baseInstallFolder}";
                lblStatus.Text = "Step 3: Click 'Extract!' to begin installation.";
                btnExtract.Enabled = true;
            }
        }

        private async void btnExtract_Click(object sender, EventArgs e)
        {
            installFolder = System.IO.Path.Combine(baseInstallFolder, System.IO.Path.GetFileNameWithoutExtension(rarPath));
            if (!System.IO.Directory.Exists(installFolder))
                System.IO.Directory.CreateDirectory(installFolder);

            lblFolderPath.Text = $"Install To: {installFolder}";
            lblStatus.Text = "Extracting... please wait.";
            btnExtract.Enabled = false;
            progressBar.Visible = true;
            progressBar.Value = 0;
            Application.DoEvents();

            bool needsPassword = false;
            int entryCount = 0;

            List<string> passwordsToTry = SettingsManager.GetSavedPasswords();
            bool extracted = false;
            foreach (var pwd in passwordsToTry)
            {
                try
                {
                    using (var archive = ArchiveFactory.Open(rarPath, new ReaderOptions() { Password = pwd }))
                    {
                        entryCount = archive.Entries.Count(e => !e.IsDirectory);
                        progressBar.Maximum = entryCount > 0 ? entryCount : 1;
                        int extractedCount = 0;
                        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
                        {
                            entry.WriteToDirectory(installFolder, new SharpCompress.Common.ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                            extractedCount++;
                            progressBar.Value = extractedCount;
                            lblStatus.Text = $"Extracting... ({extractedCount}/{entryCount})";
                            Application.DoEvents();
                        }
                        extracted = true;
                        password = pwd;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("password") || ex.Message.ToLower().Contains("encrypted"))
                    {
                        needsPassword = true;
                    }
                    else
                    {
                        lblStatus.Text = "Error: " + ex.Message;
                        MessageBox.Show("Error: " + ex.Message, "PinkyToe's Install Wizard");
                        progressBar.Visible = false;
                        btnExtract.Enabled = true;
                        return;
                    }
                }
            }

            if (!extracted && needsPassword)
            {
                using (var prompt = new PasswordPrompt())
                {
                    if (prompt.ShowDialog() == DialogResult.OK)
                    {
                        password = prompt.Password;
                        lblStatus.Text = "Trying with password...";
                        Application.DoEvents();

                        try
                        {
                            using (var archive = ArchiveFactory.Open(rarPath, new ReaderOptions() { Password = password }))
                            {
                                entryCount = archive.Entries.Count(e => !e.IsDirectory);
                                progressBar.Maximum = entryCount > 0 ? entryCount : 1;
                                int extractedCount = 0;
                                foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
                                {
                                    entry.WriteToDirectory(installFolder, new SharpCompress.Common.ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                                    extractedCount++;
                                    progressBar.Value = extractedCount;
                                    lblStatus.Text = $"Extracting... ({extractedCount}/{entryCount})";
                                    Application.DoEvents();
                                }
                                extracted = true;
                            }
                        }
                        catch (Exception ex2)
                        {
                            lblStatus.Text = "Password incorrect or extraction failed: " + ex2.Message;
                            MessageBox.Show("Password incorrect or extraction failed: " + ex2.Message, "PinkyToe's Install Wizard");
                            progressBar.Visible = false;
                            btnExtract.Enabled = true;
                            return;
                        }
                    }
                    else
                    {
                        lblStatus.Text = "Extraction cancelled by user.";
                        progressBar.Visible = false;
                        btnExtract.Enabled = true;
                        return;
                    }
                }
            }

            if (!extracted)
            {
                lblStatus.Text = "Could not extract the archive with saved passwords.";
                MessageBox.Show("Could not extract the archive with saved passwords. Please try again.", "PinkyToe's Install Wizard");
                progressBar.Visible = false;
                btnExtract.Enabled = true;
                return;
            }

            progressBar.Value = progressBar.Maximum;
            lblStatus.Text = "Extraction complete!";
            await Task.Delay(500);
            progressBar.Visible = false;

            string[] exeFiles = System.IO.Directory.GetFiles(installFolder, "*.exe", SearchOption.AllDirectories);
            if (exeFiles.Length == 0)
            {
                lblStatus.Text = "No .exe found after extraction.";
                MessageBox.Show("No .exe found after extraction. Please check the archive contents.", "PinkyToe's Install Wizard");
                btnExtract.Enabled = true;
                return;
            }

            string mainExe;
            if (exeFiles.Length == 1)
            {
                mainExe = exeFiles[0];
            }
            else
            {
                var chooseExeForm = new ChooseExeForm(exeFiles, System.IO.Path.GetFileName(installFolder));
                if (chooseExeForm.ShowDialog() == DialogResult.OK)
                {
                    mainExe = chooseExeForm.SelectedExe;
                }
                else
                {
                    lblStatus.Text = "Shortcut creation cancelled by user.";
                    btnExtract.Enabled = true;
                    return;
                }
            }

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = System.IO.Path.Combine(desktop, System.IO.Path.GetFileNameWithoutExtension(mainExe) + ".lnk");
            try
            {
                CreateShortcut(mainExe, shortcutPath);

                if (chkCreateStartMenuShortcut.Checked)
                {
                    string startMenuPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                        "Programs",
                        System.IO.Path.GetFileNameWithoutExtension(mainExe) + ".lnk"
                    );
                    CreateShortcut(mainExe, startMenuPath);
                }

                lblStatus.Text = "Success! Shortcut(s) created.";
                MessageBox.Show("Extraction complete and shortcut(s) created!", "PinkyToe's Install Wizard");
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Shortcut creation failed: " + ex.Message;
                MessageBox.Show("Shortcut creation failed: " + ex.Message, "PinkyToe's Install Wizard");
            }
            btnExtract.Enabled = true;
        }

        static void CreateShortcut(string exePath, string shortcutPath)
        {
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = exePath;
            shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(exePath);
            shortcut.Save();
        }

        private bool ValidateMultiPartRarFiles(string firstPartPath, out List<string> allParts, out string missingPart)
        {
            allParts = new List<string>();
            missingPart = null;
            var folder = System.IO.Path.GetDirectoryName(firstPartPath);
            var firstPartName = System.IO.Path.GetFileName(firstPartPath);
            var baseName = firstPartName;
            int partIdx = baseName.ToLower().IndexOf(".part1.rar");
            if (partIdx != -1)
            {
                baseName = baseName.Substring(0, partIdx);
                int partNum = 1;
                while (true)
                {
                    string partName = $"{baseName}.part{partNum}.rar";
                    string partPath = System.IO.Path.Combine(folder, partName);
                    if (System.IO.File.Exists(partPath))
                    {
                        allParts.Add(partPath);
                        partNum++;
                    }
                    else
                    {
                        if (partNum == 1)
                        {
                            allParts.Add(firstPartPath);
                            return true;
                        }
                        else
                        {
                            missingPart = partName;
                            return false;
                        }
                    }
                    if (!System.IO.File.Exists(System.IO.Path.Combine(folder, $"{baseName}.part{partNum}.rar"))) break;
                }
                return true;
            }
            else
            {
                string rarBase = firstPartName;
                if (rarBase.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                {
                    rarBase = rarBase.Substring(0, rarBase.Length - 4);
                    allParts.Add(firstPartPath);
                    int partNum = 2;
                    while (true)
                    {
                        string partName = $"{rarBase}.part{partNum}.rar";
                        string partPath = System.IO.Path.Combine(folder, partName);
                        if (System.IO.File.Exists(partPath))
                        {
                            allParts.Add(partPath);
                            partNum++;
                        }
                        else
                        {
                            if (partNum > 2)
                            {
                                missingPart = partName;
                                return false;
                            }
                            break;
                        }
                    }
                    return true;
                }
                else
                {
                    allParts.Add(firstPartPath);
                    return true;
                }
            }
        }

        private void btnPasswordManager_Click(object sender, EventArgs e)
        {
            var pm = new PasswordManagerForm();
            pm.ShowDialog();
        }
    }
}