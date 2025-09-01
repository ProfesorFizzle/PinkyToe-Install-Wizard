#nullable enable
namespace PinkyToeInstallWizard
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.PictureBox picLogo = null!;
        private System.Windows.Forms.Button btnSelectRar = null!;
        private System.Windows.Forms.Label lblRarPath = null!;
        private System.Windows.Forms.Button btnSelectFolder = null!;
        private System.Windows.Forms.Label lblFolderPath = null!;
        private System.Windows.Forms.Button btnExtract = null!;
        private System.Windows.Forms.Label lblStatus = null!;
        private System.Windows.Forms.Label lblMultiPartHelp = null!;
        private System.Windows.Forms.ProgressBar progressBar = null!;
        private System.Windows.Forms.Button btnPasswordManager = null!;
        private System.Windows.Forms.CheckBox chkCreateStartMenuShortcut = null!;
        private System.Windows.Forms.Label lblPasswordStatus = null!;
        private System.Windows.Forms.Label lblMainExe = null!;
        private System.Windows.Forms.LinkLabel lnkChangeExe = null!;
        private System.Windows.Forms.CheckBox chkCreateDesktopShortcut = null!;
        private System.Windows.Forms.CheckBox chkLaunchAfterInstall = null!;
        private System.Windows.Forms.Label lblTimeRemaining = null!;
        private System.Windows.Forms.Button btnOpenInstallFolder = null!;
        private System.Windows.Forms.Label lblVersion = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.btnSelectRar = new System.Windows.Forms.Button();
            this.lblRarPath = new System.Windows.Forms.Label();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblMultiPartHelp = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnPasswordManager = new System.Windows.Forms.Button();
            this.chkCreateStartMenuShortcut = new System.Windows.Forms.CheckBox();
            this.lblPasswordStatus = new System.Windows.Forms.Label();
            this.lblMainExe = new System.Windows.Forms.Label();
            this.lnkChangeExe = new System.Windows.Forms.LinkLabel();
            this.chkCreateDesktopShortcut = new System.Windows.Forms.CheckBox();
            this.chkLaunchAfterInstall = new System.Windows.Forms.CheckBox();
            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.btnOpenInstallFolder = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(20, 20);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(120, 120);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // 
            // btnSelectRar
            // 
            this.btnSelectRar.Location = new System.Drawing.Point(160, 20);
            this.btnSelectRar.Name = "btnSelectRar";
            this.btnSelectRar.Size = new System.Drawing.Size(180, 30);
            this.btnSelectRar.Text = "Step 1: Select Archive";
            this.btnSelectRar.UseVisualStyleBackColor = true;
            this.btnSelectRar.Click += new System.EventHandler(this.btnSelectRar_Click);
            // 
            // lblRarPath
            // 
            this.lblRarPath.Location = new System.Drawing.Point(160, 60);
            this.lblRarPath.Name = "lblRarPath";
            this.lblRarPath.Size = new System.Drawing.Size(420, 20);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(160, 90);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(180, 30);
            this.btnSelectFolder.Text = "Step 2: Select Install Directory";
            this.btnSelectFolder.Enabled = false;
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // lblFolderPath
            // 
            this.lblFolderPath.Location = new System.Drawing.Point(160, 130);
            this.lblFolderPath.Name = "lblFolderPath";
            this.lblFolderPath.Size = new System.Drawing.Size(420, 20);
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(160, 260);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(180, 30);
            this.btnExtract.Text = "Step 3: Extract && Install!";
            this.btnExtract.Enabled = false;
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(20, 360);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(560, 24);
            this.lblStatus.Text = "Status...";
            // 
            // lblMultiPartHelp
            // 
            this.lblMultiPartHelp.Location = new System.Drawing.Point(20, 390);
            this.lblMultiPartHelp.Name = "lblMultiPartHelp";
            this.lblMultiPartHelp.Size = new System.Drawing.Size(560, 30);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(20, 430);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(560, 23);
            this.progressBar.Visible = false;
            // 
            // btnPasswordManager
            // 
            this.btnPasswordManager.Location = new System.Drawing.Point(400, 20);
            this.btnPasswordManager.Name = "btnPasswordManager";
            this.btnPasswordManager.Size = new System.Drawing.Size(180, 30);
            this.btnPasswordManager.Text = "Password Manager";
            this.btnPasswordManager.UseVisualStyleBackColor = true;
            this.btnPasswordManager.Click += new System.EventHandler(this.btnPasswordManager_Click);
            // 
            // chkCreateStartMenuShortcut
            // 
            this.chkCreateStartMenuShortcut.Location = new System.Drawing.Point(360, 220);
            this.chkCreateStartMenuShortcut.Name = "chkCreateStartMenuShortcut";
            this.chkCreateStartMenuShortcut.Size = new System.Drawing.Size(220, 24);
            this.chkCreateStartMenuShortcut.Text = "Create Start Menu Shortcut";
            this.chkCreateStartMenuShortcut.UseVisualStyleBackColor = true;
            // 
            // lblPasswordStatus
            // 
            this.lblPasswordStatus.Location = new System.Drawing.Point(160, 150);
            this.lblPasswordStatus.Name = "lblPasswordStatus";
            this.lblPasswordStatus.Size = new System.Drawing.Size(420, 20);
            // 
            // lblMainExe
            // 
            this.lblMainExe.Location = new System.Drawing.Point(160, 170);
            this.lblMainExe.Name = "lblMainExe";
            this.lblMainExe.Size = new System.Drawing.Size(420, 20);
            this.lblMainExe.Text = "Main Executable: (not selected yet)";
            // 
            // lnkChangeExe
            // 
            this.lnkChangeExe.Location = new System.Drawing.Point(160, 190);
            this.lnkChangeExe.Name = "lnkChangeExe";
            this.lnkChangeExe.Size = new System.Drawing.Size(120, 20);
            this.lnkChangeExe.TabStop = true;
            this.lnkChangeExe.Text = "Change...";
            this.lnkChangeExe.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkChangeExe_LinkClicked);
            // 
            // chkCreateDesktopShortcut
            // 
            this.chkCreateDesktopShortcut.Location = new System.Drawing.Point(160, 220);
            this.chkCreateDesktopShortcut.Name = "chkCreateDesktopShortcut";
            this.chkCreateDesktopShortcut.Size = new System.Drawing.Size(180, 24);
            this.chkCreateDesktopShortcut.Text = "Create Desktop Shortcut";
            this.chkCreateDesktopShortcut.UseVisualStyleBackColor = true;
            // 
            // chkLaunchAfterInstall
            // 
            this.chkLaunchAfterInstall.Location = new System.Drawing.Point(160, 240);
            this.chkLaunchAfterInstall.Name = "chkLaunchAfterInstall";
            this.chkLaunchAfterInstall.Size = new System.Drawing.Size(180, 24);
            this.chkLaunchAfterInstall.Text = "Launch After Install";
            this.chkLaunchAfterInstall.UseVisualStyleBackColor = true;
            // 
            // lblTimeRemaining
            // 
            this.lblTimeRemaining.Location = new System.Drawing.Point(20, 330);
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new System.Drawing.Size(560, 24);
            // 
            // btnOpenInstallFolder
            // 
            this.btnOpenInstallFolder.Location = new System.Drawing.Point(360, 260);
            this.btnOpenInstallFolder.Name = "btnOpenInstallFolder";
            this.btnOpenInstallFolder.Size = new System.Drawing.Size(220, 30);
            this.btnOpenInstallFolder.Text = "Open Install Folder";
            this.btnOpenInstallFolder.Enabled = false;
            this.btnOpenInstallFolder.UseVisualStyleBackColor = true;
            this.btnOpenInstallFolder.Click += new System.EventHandler(this.btnOpenInstallFolder_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.lblVersion.Location = new System.Drawing.Point(480, 455);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(100, 15);
            this.lblVersion.Text = "v1.2.2beta1";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 8F);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(600, 470);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnOpenInstallFolder);
            this.Controls.Add(this.lblTimeRemaining);
            this.Controls.Add(this.chkLaunchAfterInstall);
            this.Controls.Add(this.chkCreateDesktopShortcut);
            this.Controls.Add(this.lnkChangeExe);
            this.Controls.Add(this.lblMainExe);
            this.Controls.Add(this.lblPasswordStatus);
            this.Controls.Add(this.chkCreateStartMenuShortcut);
            this.Controls.Add(this.btnPasswordManager);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblMultiPartHelp);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.lblFolderPath);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.lblRarPath);
            this.Controls.Add(this.btnSelectRar);
            this.Controls.Add(this.picLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
        }
    }
}