namespace PinkyToeInstallWizard
{
    partial class ChooseExeForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox lstExeFiles;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblSuggestion;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lstExeFiles = new System.Windows.Forms.ListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblSuggestion = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // lblSuggestion
            this.lblSuggestion.Location = new System.Drawing.Point(12, 8);
            this.lblSuggestion.Size = new System.Drawing.Size(360, 40);
            this.lblSuggestion.Text = "Suggested:";

            // lstExeFiles
            this.lstExeFiles.Location = new System.Drawing.Point(12, 58);
            this.lstExeFiles.Size = new System.Drawing.Size(360, 120);

            // btnOK
            this.btnOK.Location = new System.Drawing.Point(292, 190);
            this.btnOK.Size = new System.Drawing.Size(80, 30);
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);

            // ChooseExeForm
            this.ClientSize = new System.Drawing.Size(384, 231);
            this.Controls.Add(this.lblSuggestion);
            this.Controls.Add(this.lstExeFiles);
            this.Controls.Add(this.btnOK);
            this.Text = "Select Main .exe - PinkyToe's Install Wizard";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            this.ResumeLayout(false);
        }
    }
}