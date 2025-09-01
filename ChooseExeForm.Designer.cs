namespace PinkyToeInstallWizard
{
    internal partial class ChooseExeForm
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.ListView listViewExe = null!;
        private System.Windows.Forms.Button btnOK = null!;
        private System.Windows.Forms.Button btnCancel = null!;
        private System.Windows.Forms.Label lblHint = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.listViewExe = new System.Windows.Forms.ListView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewExe
            // 
            this.listViewExe.Location = new System.Drawing.Point(12, 40);
            this.listViewExe.Name = "listViewExe";
            this.listViewExe.Size = new System.Drawing.Size(600, 280);
            this.listViewExe.TabIndex = 0;
            this.listViewExe.UseCompatibleStateImageBehavior = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(452, 330);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 28);
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(537, 330);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblHint
            // 
            this.lblHint.Location = new System.Drawing.Point(12, 9);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(600, 28);
            this.lblHint.Text = "Select main executable.";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ChooseExeForm
            // 
            this.ClientSize = new System.Drawing.Size(624, 370);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.listViewExe);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Main Executable";
            this.ResumeLayout(false);
        }
    }
}