namespace PinkyToeInstallWizard
{
    partial class PasswordManagerForm
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.ListBox lstPasswords = null!;
        private System.Windows.Forms.TextBox txtPassword = null!;
        private System.Windows.Forms.Button btnAdd = null!;
        private System.Windows.Forms.Button btnRemove = null!;
        private System.Windows.Forms.Button btnSave = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lstPasswords = new System.Windows.Forms.ListBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstPasswords
            // 
            this.lstPasswords.FormattingEnabled = true;
            this.lstPasswords.Location = new System.Drawing.Point(12, 12);
            this.lstPasswords.Size = new System.Drawing.Size(260, 160);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(12, 180);
            this.txtPassword.Size = new System.Drawing.Size(180, 20);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(200, 178);
            this.btnAdd.Size = new System.Drawing.Size(72, 23);
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(12, 210);
            this.btnRemove.Size = new System.Drawing.Size(80, 23);
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(192, 210);
            this.btnSave.Size = new System.Drawing.Size(80, 23);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // PasswordManagerForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 251);
            this.Controls.Add(this.lstPasswords);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Password Manager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}