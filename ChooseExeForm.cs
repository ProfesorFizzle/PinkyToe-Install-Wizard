using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PinkyToeInstallWizard
{
    internal partial class ChooseExeForm : Form
    {
        public ExeDetector.Candidate? SelectedCandidate { get; private set; }
        private readonly ImageList _iconList = new ImageList
        {
            ColorDepth = ColorDepth.Depth32Bit,
            ImageSize = new Size(32, 32)
        };

        public ChooseExeForm(List<ExeDetector.Candidate> candidates,
                             ExeDetector.Candidate preselect,
                             IDictionary<string, Image>? icons)
        {
            InitializeComponent();

            listViewExe.Columns.Clear();
            listViewExe.Columns.Add("Score", 60);
            listViewExe.Columns.Add("File", 200);
            listViewExe.Columns.Add("Reasons", 320);
            listViewExe.FullRowSelect = true;
            listViewExe.MultiSelect = false;
            listViewExe.View = View.Details;
            listViewExe.HideSelection = false;
            listViewExe.SmallImageList = _iconList;

            foreach (var c in candidates)
            {
                Image img;
                if (icons != null && icons.TryGetValue(c.FileName, out var provided))
                    img = provided;
                else
                    img = SystemIcons.Application.ToBitmap();

                if (!_iconList.Images.ContainsKey(c.FileName))
                    _iconList.Images.Add(c.FileName, img);
            }

            foreach (var c in candidates)
            {
                var reasonsShort = string.Join("; ",
                    c.Reasons.Count > 2 ? c.Reasons.GetRange(0, 2) : c.Reasons);

                var item = new ListViewItem(new[]
                {
                    c.Score.ToString(),
                    c.FileName,
                    reasonsShort
                })
                {
                    Tag = c,
                    ImageKey = c.FileName
                };

                listViewExe.Items.Add(item);
                if (c == preselect)
                    item.Selected = true;
            }

            lblHint.Text = "Select the main executable (double-click to choose).";
            listViewExe.DoubleClick += (s, e) => ConfirmSelection();
        }

        private void ConfirmSelection()
        {
            if (listViewExe.SelectedItems.Count == 0) return;
            SelectedCandidate = listViewExe.SelectedItems[0].Tag as ExeDetector.Candidate;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e) => ConfirmSelection();

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _iconList?.Dispose();
            base.OnFormClosed(e);
        }
    }
}