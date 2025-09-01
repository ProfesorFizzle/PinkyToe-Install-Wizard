namespace PinkyToeInstallWizard
{
    partial class PasswordPrompt
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.TextBox txtPassword = null!;
        private System.Windows.Forms.Button btnOK = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(12, 12);
            this.txtPassword.Size = new System.Drawing.Size(260, 20);
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(197, 38);
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PasswordPrompt
            // 
            this.ClientSize = new System.Drawing.Size(284, 71);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnOK);
            this.Text = "Enter Archive Password";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}