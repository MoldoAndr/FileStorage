using System;
using System.Drawing;
using System.Windows.Forms;

namespace FileTransferClient.Forms
{
    public partial class InputDialog : Form
    {
        public string InputText { get; private set; }

        public InputDialog(string prompt)
        {
            InitializeComponent();
            labelPrompt.Text = prompt;
            this.BackColor = Color.FromArgb(22, 33, 62);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(550, 150);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            InputText = textBoxInput.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
