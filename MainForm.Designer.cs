namespace PinkyToeInstallWizard
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnSelectRar;
        private System.Windows.Forms.Label lblRarPath;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Label lblFolderPath;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblMultiPartHelp;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnPasswordManager;
        private System.Windows.Forms.CheckBox chkCreateStartMenuShortcut;
        private System.Windows.Forms.Label lblPasswordStatus;

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

            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();

            // picLogo
            this.picLogo.Location = new System.Drawing.Point(20, 20);
            this.picLogo.Size = new System.Drawing.Size(120, 120);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // btnSelectRar
            this.btnSelectRar.Location = new System.Drawing.Point(160, 20);
            this.btnSelectRar.Size = new System.Drawing.Size(180, 30);
            this.btnSelectRar.Text = "Step 1: Select Archive";
            this.btnSelectRar.Click += new System.EventHandler(this.btnSelectRar_Click);

            // lblRarPath
            this.lblRarPath.Location = new System.Drawing.Point(160, 60);
            this.lblRarPath.Size = new System.Drawing.Size(400, 20);

            // btnSelectFolder
            this.btnSelectFolder.Location = new System.Drawing.Point(160, 90);
            this.btnSelectFolder.Size = new System.Drawing.Size(180, 30);
            this.btnSelectFolder.Text = "Step 2: Select Install Directory";
            this.btnSelectFolder.Enabled = false;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);

            // btnPasswordManager
            this.btnPasswordManager.Location = new System.Drawing.Point(400, 20);
            this.btnPasswordManager.Size = new System.Drawing.Size(160, 30);
            this.btnPasswordManager.Text = "Password Manager";
            this.btnPasswordManager.Click += new System.EventHandler(this.btnPasswordManager_Click);

            // lblFolderPath
            this.lblFolderPath.Location = new System.Drawing.Point(160, 130);
            this.lblFolderPath.Size = new System.Drawing.Size(400, 20);

            // chkCreateStartMenuShortcut
            this.chkCreateStartMenuShortcut.Location = new System.Drawing.Point(160, 190);
            this.chkCreateStartMenuShortcut.Size = new System.Drawing.Size(220, 24);
            this.chkCreateStartMenuShortcut.Text = "Create Start Menu Shortcut";
            this.chkCreateStartMenuShortcut.Checked = false;

            // btnExtract
            this.btnExtract.Location = new System.Drawing.Point(160, 220);
            this.btnExtract.Size = new System.Drawing.Size(180, 30);
            this.btnExtract.Text = "Step 3: Extract & Install!";
            this.btnExtract.Enabled = false;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);

            // lblStatus
            this.lblStatus.Location = new System.Drawing.Point(20, 270);
            this.lblStatus.Size = new System.Drawing.Size(500, 40);
            this.lblStatus.Text = "Status...";

            // lblMultiPartHelp
            this.lblMultiPartHelp.Location = new System.Drawing.Point(20, 310);
            this.lblMultiPartHelp.Size = new System.Drawing.Size(560, 40);
            this.lblMultiPartHelp.Text = "For multi-part .rar files, select the first file (.rar or .part1.rar). Make sure all parts are in the same folder.";

            // progressBar
            this.progressBar.Location = new System.Drawing.Point(20, 360);
            this.progressBar.Size = new System.Drawing.Size(560, 23);
            this.progressBar.Visible = false;

            // lblPasswordStatus
            this.lblPasswordStatus.Location = new System.Drawing.Point(160, 160);
            this.lblPasswordStatus.Size = new System.Drawing.Size(400, 24);
            this.lblPasswordStatus.Text = "";

            // MainForm
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.btnSelectRar);
            this.Controls.Add(this.lblRarPath);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.lblFolderPath);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblMultiPartHelp);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnPasswordManager);
            this.Controls.Add(this.chkCreateStartMenuShortcut);
            this.Controls.Add(this.lblPasswordStatus);
            this.Text = "PinkyToe's Install Wizard";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}