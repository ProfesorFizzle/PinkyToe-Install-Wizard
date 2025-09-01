using System;
using System.Windows.Forms;

namespace PinkyToeInstallWizard
{
    public partial class UpdatePromptDialog : Form
    {
        // 0 = Update existing, 1 = Cancel, 2 = New Install
        public int UserChoice { get; private set; } = 1;

        public UpdatePromptDialog(string message)
        {
            InitializeComponent();
            txtMessage.Text = message;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UserChoice = 0;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UserChoice = 1;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            UserChoice = 2;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}