using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PinkyToeInstallWizard
{
    public partial class PasswordManagerForm : Form
    {
        public PasswordManagerForm()
        {
            InitializeComponent();
            LoadPasswords();
        }

        private void LoadPasswords()
        {
            lstPasswords.Items.Clear();
            foreach (var pwd in SettingsManager.GetSavedPasswords())
                lstPasswords.Items.Add(pwd);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lstPasswords.Items.Add(txtPassword.Text.Trim());
                txtPassword.Clear();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstPasswords.SelectedItem != null)
                lstPasswords.Items.Remove(lstPasswords.SelectedItem);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var pwds = new List<string>();
            foreach (var item in lstPasswords.Items)
                pwds.Add(item.ToString());
            SettingsManager.SavePasswords(pwds);
            this.Close();
        }
    }
}