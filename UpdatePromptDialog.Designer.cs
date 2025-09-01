namespace PinkyToeInstallWizard
{
    partial class UpdatePromptDialog
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.TextBox txtMessage = null!;
        private System.Windows.Forms.Button btnUpdate = null!;
        private System.Windows.Forms.Button btnCancel = null!;
        private System.Windows.Forms.Button btnNew = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(12, 12);
            this.txtMessage.Multiline = true;
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(400, 150);
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(12, 170);
            this.btnUpdate.Size = new System.Drawing.Size(120, 30);
            this.btnUpdate.Text = "Update Existing";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(148, 170);
            this.btnCancel.Size = new System.Drawing.Size(120, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(292, 170);
            this.btnNew.Size = new System.Drawing.Size(120, 30);
            this.btnNew.Text = "New Install";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // UpdatePromptDialog
            // 
            this.ClientSize = new System.Drawing.Size(424, 212);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNew);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Existing Installation Detected";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}