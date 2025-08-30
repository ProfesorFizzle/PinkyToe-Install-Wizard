using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PinkyToeInstallWizard
{
    public partial class ChooseExeForm : Form
    {
        public string SelectedExe { get; private set; }
        private string suggestedExe;

        public ChooseExeForm(string[] exeFiles, string folderName)
        {
            InitializeComponent();

            suggestedExe = exeFiles
                .Where(f => !IsExactMatch(folderName, System.IO.Path.GetFileNameWithoutExtension(f)))
                .OrderByDescending(f => SimilarityScore(folderName, System.IO.Path.GetFileNameWithoutExtension(f)))
                .FirstOrDefault();

            foreach (var exe in exeFiles)
            {
                lstExeFiles.Items.Add(exe);
            }

            if (suggestedExe != null)
                lstExeFiles.SelectedItem = suggestedExe;

            if (suggestedExe != null)
            {
                lblSuggestion.Text = $"Suggested: Closest match to folder name \"{folderName}\" is:\n{System.IO.Path.GetFileName(suggestedExe)}";
            }
            else
            {
                lblSuggestion.Text = "No suggestion available.";
            }
        }

        private static bool IsExactMatch(string folder, string file)
        {
            return string.Equals(folder, file, StringComparison.OrdinalIgnoreCase);
        }

        private static int SimilarityScore(string folder, string file)
        {
            if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(file))
                return 0;

            int score = 0;
            foreach (var ch in folder.ToLower())
            {
                if (file.ToLower().Contains(ch))
                    score++;
            }
            return score;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstExeFiles.SelectedItem != null)
            {
                SelectedExe = lstExeFiles.SelectedItem.ToString();
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Please select an .exe file.", "PinkyToe's Install Wizard");
            }
        }
    }
}