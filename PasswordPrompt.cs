using System.Windows.Forms;

namespace PinkyToeInstallWizard
{
    public partial class PasswordPrompt : Form
    {
        public string Password { get; private set; }
        public PasswordPrompt()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            Password = txtPassword.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}