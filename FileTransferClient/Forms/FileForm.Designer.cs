using System.Drawing.Drawing2D;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Forms;

#pragma warning disable CS8622

namespace FileTransferClient.Forms
{
    public partial class FileForm : Form
    {
        public SslStream SslStream { get; set; }
        public BinaryReader Reader { get; set; }
        public BinaryWriter Writer { get; set; }
        public TextBox TextBoxRecipient { get; set; }
        public TextBox TextBoxSendRecipient { get; set; }
        public Button ButtonUpload { get; set; }
        public Button ButtonDownload { get; set; }
        public Button ButtonDelete { get; set; }
        public Button ButtonView { get; set; }
        public Button ButtonShare { get; set; }
        public Button ButtonSend { get; set; }
        public Button BtnBack { get; set; } 
        public Button ButtonRename { get; set; }
        public Button ButtonModify { get; set; }
        public Button User { get; set; }
        public ListBox ListBoxFiles { get; set; }
        public Panel PanelTop { get; set; }
        public Panel LogoPanel { get; set; }
        public string Username { get; set; }

        private void InitializeComponent()
        {
            SuspendLayout();
            PanelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(2, 8, 28)
            };

            string usernameDisplay = $"Connected as {this.Username}";

            User = new Button
            {
                Size = new Size(500, 30),
                Location = new Point(150, 0),
                FlatStyle = FlatStyle.Flat,
                Text = usernameDisplay,
                Font = new Font("Arial", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            User.FlatAppearance.BorderSize = 0;
            PanelTop.Controls.Add(User);

            LogoPanel = new Panel
            {
                Location = new Point(0, 30),
                Height = 150,
                Width = 800,
                BackColor = Color.Transparent,
                BackgroundImageLayout = ImageLayout.Center
            };
            LogoPanel.Paint += new PaintEventHandler(DrawLogo);

            BtnBack = new Button
            {
                Size = new Size(150, 30),
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                Text = "Disconnect",
                Font = new Font("Arial", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            BtnBack.FlatAppearance.BorderSize = 0;
            BtnBack.Click += new EventHandler(BtnBack_Click);
      
            PanelTop.Controls.Add(BtnBack);

            Label labelListOfFiles = new()
            {
                Text = "List of files",
                Font = new Font("Times New Roman", 26F, FontStyle.Bold),
                ForeColor = Color.FromArgb(236, 240, 241),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(316, 240)
            };

            ListBoxFiles = new ListBox
            {
                Font = new Font("Segoe UI", 14F),
                ItemHeight = 32,
                Location = new Point(20, 280),
                Size = new Size(760, 300),
                BackColor = Color.FromArgb(22, 33, 62),
                ForeColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.None
            };

            ButtonUpload = CreateStyledButton("Upload", 20, 590);
            ButtonDownload = CreateStyledButton("Download", 149, 590);
            ButtonDelete = CreateStyledButton("Delete", 278, 590);
            ButtonView = CreateStyledButton("View", 407, 590);
            ButtonRename = CreateStyledButton("Rename", 536, 590);
            ButtonModify = CreateStyledButton("Modify", 665, 590);
            ButtonShare = CreateStyledButton("Share", 20, 650);
            ButtonSend = CreateStyledButton("Send", 20, 710);
            TextBoxRecipient = CreateStyledTextBox("Enter recipient username for sharing", 150, 650);
            TextBoxSendRecipient = CreateStyledTextBox("Enter recipient username for sending", 150, 710);

            ClientSize = new Size(800, 800);
            Controls.Add(PanelTop);
            Controls.Add(LogoPanel);
            Controls.Add(labelListOfFiles);
            Controls.Add(ListBoxFiles);
            Controls.Add(ButtonUpload);
            Controls.Add(ButtonDownload);
            Controls.Add(ButtonDelete);
            Controls.Add(ButtonView);
            Controls.Add(ButtonShare);
            Controls.Add(ButtonSend);
            Controls.Add(TextBoxRecipient);
            Controls.Add(TextBoxSendRecipient);
            Controls.Add(ButtonRename);
            Controls.Add(ButtonModify);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(22, 33, 62);

            ResumeLayout(false);
            PerformLayout();
        }

        private Button CreateStyledButton(string text, int x, int y)
        {
            Button button = new()
            {
                Text = text,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(x, y),
                Size = new Size(115, 30),
                BackColor = Color.FromArgb(17, 10, 115),
                ForeColor = Color.FromArgb(236, 240, 241),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += GetButtonClickHandler(text);
            return button;
        }

        private TextBox CreateStyledTextBox(string placeholder, int x, int y)
        {
            TextBox textBox = new()
            {
                Font = new Font("Segoe UI", 13F),
                Location = new Point(x, y),
                Size = new Size(630, 40),
                Text = placeholder,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(22, 33, 62)
            };
            textBox.Enter += (sender, e) => TextBox_Enter(sender, e, placeholder);
            textBox.Leave += (sender, e) => TextBox_Leave(sender, e, placeholder);

            textBox.Invalidate();
            textBox.Refresh();

            return textBox;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using LinearGradientBrush brush = new(ClientRectangle, Color.FromArgb(2, 8, 28), Color.FromArgb(22, 33, 62), 90F);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        private void DrawLogo(object sender, PaintEventArgs e)
        {
            string imagePath = "../../../cloud.png";
            if (File.Exists(imagePath))
            {
                using Image logoImage = Image.FromFile(imagePath);
                int panelWidth = 800;
                int panelHeight = 200;

                int imageWidth = logoImage.Width;
                int imageHeight = logoImage.Height;

                float scaleX = (float)(panelWidth / 2) / imageWidth;
                float scaleY = (float)(panelHeight / 2) / imageHeight;
                float scale = Math.Min(scaleX, scaleY);

                int newWidth = (int)(imageWidth * scale * 1.5);
                int newHeight = (int)(imageHeight * scale * 1.5);

                int imageX = 400 - (newWidth / 2);
                int imageY = 0;
                e.Graphics.DrawImage(logoImage, imageX, imageY, newWidth, newHeight);
            }
            else
            {
                _ = MessageBox.Show("Logo image not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
#pragma warning restore CS8622