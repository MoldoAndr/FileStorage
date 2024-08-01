using System.Drawing.Drawing2D;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

#pragma warning disable CS8622

namespace FileTransferClient.Forms
{
    public partial class FileForm : Form
    {
        private const int Port = 8888;
        private const string ServerIp = "192.168.0.190";

        public static int Port1 => Port2;

        public static string ServerIp1 => ServerIp2;

        public SslStream SslStream { get => SslStream1; set => SslStream1 = value; }
        public BinaryReader Reader { get => Reader1; set => Reader1 = value; }
        public BinaryWriter Writer { get => Writer1; set => Writer1 = value; }
        public TextBox TextBoxRecipient { get => TextBoxRecipient1; set => TextBoxRecipient1 = value; }
        public TextBox TextBoxSendRecipient { get => TextBoxSendRecipient1; set => TextBoxSendRecipient1 = value; }
        public Button ButtonUpload { get => ButtonUpload1; set => ButtonUpload1 = value; }
        public Button ButtonDownload { get => ButtonDownload1; set => ButtonDownload1 = value; }
        public Button ButtonDelete { get => ButtonDelete1; set => ButtonDelete1 = value; }
        public Button ButtonRefresh { get => ButtonRefresh1; set => ButtonRefresh1 = value; }
        public Button ButtonShare { get => ButtonShare1; set => ButtonShare1 = value; }
        public Button ButtonSend { get => ButtonSend1; set => ButtonSend1 = value; }
        public Button BtnClose { get => BtnClose1; set => BtnClose1 = value; }
        public ListBox ListBoxFiles { get => ListBoxFiles1; set => ListBoxFiles1 = value; }
        public Panel PanelTop { get => PanelTop1; set => PanelTop1 = value; }
        public Panel LogoPanel { get => LogoPanel1; set => LogoPanel1 = value; }
        public string Username1 { get => Username2; set => Username2 = value; }

        public static int Port2 => Port4;

        public static string ServerIp2 => ServerIp4;

        public SslStream SslStream1 { get => SslStream2; set => SslStream2 = value; }
        public BinaryReader Reader1 { get => Reader2; set => Reader2 = value; }
        public BinaryWriter Writer1 { get => Writer2; set => Writer2 = value; }
        public TextBox TextBoxRecipient1 { get => TextBoxRecipient2; set => TextBoxRecipient2 = value; }
        public TextBox TextBoxSendRecipient1 { get => TextBoxSendRecipient2; set => TextBoxSendRecipient2 = value; }
        public Button ButtonUpload1 { get => ButtonUpload2; set => ButtonUpload2 = value; }
        public Button ButtonDownload1 { get => ButtonDownload2; set => ButtonDownload2 = value; }
        public Button ButtonDelete1 { get => ButtonDelete2; set => ButtonDelete2 = value; }
        public Button ButtonRefresh1 { get => ButtonRefresh2; set => ButtonRefresh2 = value; }
        public Button ButtonShare1 { get => ButtonShare2; set => ButtonShare2 = value; }
        public Button ButtonSend1 { get => ButtonSend2; set => ButtonSend2 = value; }
        public Button BtnClose1 { get => BtnClose2; set => BtnClose2 = value; }
        public ListBox ListBoxFiles1 { get => ListBoxFiles2; set => ListBoxFiles2 = value; }
        public Panel PanelTop1 { get => PanelTop2; set => PanelTop2 = value; }
        public Panel LogoPanel1 { get => LogoPanel2; set => LogoPanel2 = value; }
        public string Username2 { get => Username3; set => Username3 = value; }

        public static int Port3 => Port4;

        public static string ServerIp3 => ServerIp4;

        public SslStream SslStream2 { get; set; }
        public BinaryReader Reader2 { get; set; }
        public BinaryWriter Writer2 { get; set; }
        public TextBox TextBoxRecipient2 { get; set; }
        public TextBox TextBoxSendRecipient2 { get; set; }
        public Button ButtonUpload2 { get; set; }
        public Button ButtonDownload2 { get; set; }
        public Button ButtonDelete2 { get; set; }
        public Button ButtonRefresh2 { get; set; }
        public Button ButtonShare2 { get; set; }
        public Button ButtonSend2 { get; set; }
        public Button BtnClose2 { get; set; }
        public ListBox ListBoxFiles2 { get; set; }
        public Panel PanelTop2 { get; set; }
        public Panel LogoPanel2 { get; set; }
        public string Username3 { get; set; }

        public static int Port4 => Port;

        public static string ServerIp4 => ServerIp;

        public FileForm()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            PanelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(2, 8, 28)
            };

            Label labelUser = new()
            {
                Text = $"Connected as: {Username1}",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(236, 240, 241),
                Location = new Point(10, 5),
                AutoSize = true
            };
            PanelTop.Controls.Add(labelUser);

            LogoPanel = new Panel
            {
                Location = new Point(0, 30),
                Height = 150,
                Width = 800,
                BackColor = Color.Transparent,
                BackgroundImageLayout = ImageLayout.Center
            };
            LogoPanel.Paint += new PaintEventHandler(DrawLogo);
            BtnClose = new Button
            {
                Size = new Size(30, 30),
                Location = new Point(770, 0),
                FlatStyle = FlatStyle.Flat,
                Text = "×",
                Font = new Font("Arial", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            BtnClose.FlatAppearance.BorderSize = 0;
            BtnClose.Click += new EventHandler(BtnClose_Click);
            PanelTop.Controls.Add(BtnClose);

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
            ButtonDownload = CreateStyledButton("Download", 210, 590);
            ButtonDelete = CreateStyledButton("Delete", 400, 590);
            ButtonRefresh = CreateStyledButton("Refresh", 590, 590);
            ButtonShare = CreateStyledButton("Share", 20, 650);
            ButtonSend = CreateStyledButton("Send", 20, 710);

            TextBoxRecipient = CreateStyledTextBox("Enter recipient username", 210, 650);
            TextBoxSendRecipient = CreateStyledTextBox("Enter recipient username for sending", 210, 710);

            ClientSize = new Size(800, 800);
            Controls.Add(PanelTop);
            Controls.Add(LogoPanel);
            Controls.Add(labelListOfFiles);
            Controls.Add(ListBoxFiles);
            Controls.Add(ButtonUpload);
            Controls.Add(ButtonDownload);
            Controls.Add(ButtonDelete);
            Controls.Add(ButtonRefresh);
            Controls.Add(ButtonShare);
            Controls.Add(ButtonSend);
            Controls.Add(TextBoxRecipient);
            Controls.Add(TextBoxSendRecipient);
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
                Size = new Size(170, 40),
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
                Size = new Size(570, 30),
                Text = placeholder,
                ForeColor = Color.Gray,
                BackColor = Color.FromArgb(22, 33, 62)
            };
            textBox.Enter += (sender, e) => TextBox_Enter(sender, e, placeholder);
            textBox.Leave += (sender, e) => TextBox_Leave(sender, e, placeholder);
            return textBox;
        }

        private void TextBox_Enter(object sender, EventArgs e, string placeholder)
        {
            if (((TextBox)sender).Text == placeholder)
            {
                ((TextBox)sender).Text = "";
                ((TextBox)sender).ForeColor = Color.FromArgb(236, 240, 241);
            }
        }

        private void TextBox_Leave(object sender, EventArgs e, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(((TextBox)sender).Text))
            {
                ((TextBox)sender).Text = placeholder;
                ((TextBox)sender).ForeColor = Color.Gray;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using LinearGradientBrush brush = new(ClientRectangle, Color.FromArgb(2, 8, 28), Color.FromArgb(22, 33, 62), 90F);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        private EventHandler GetButtonClickHandler(string buttonText)
        {
            return buttonText switch
            {
                "Upload" => buttonUpload_Click,
                "Download" => buttonDownload_Click,
                "Delete" => buttonDelete_Click,
                "Refresh" => buttonRefresh_Click,
                "Share" => buttonShare_Click,
                "Send" => buttonSend_Click,
                _ => null,
            };
        }

        private void ConnectToServer()
        {
            try
            {
                System.Net.Sockets.TcpClient client = new(ServerIp1, Port1);
                SslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                SslStream.AuthenticateAsClient("FileTransferServer");

                Reader = new BinaryReader(SslStream);
                Writer = new BinaryWriter(SslStream);
                UpdateFileList();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error connecting to server: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void UpdateFileList()
        {
            ListBoxFiles.Items.Clear();
            Writer.Write("LIST");
            int fileCount = Reader.ReadInt32();
            for (int i = 0; i < fileCount; i++)
            {
                _ = ListBoxFiles.Items.Add(Reader.ReadString());
            }
        }

        public FileForm(SslStream sslStream, BinaryReader reader, BinaryWriter writer, string Username)

        {
            Username1 = Username;
            InitializeComponent();
            SslStream = sslStream;
            Reader = reader;
            Writer = writer;
            UpdateFileList();
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileName(openFileDialog.FileName);
                Writer.Write("UPLOAD");
                Writer.Write(fileName);
                Writer.Write(new FileInfo(openFileDialog.FileName).Length);

                using (FileStream fs = new(openFileDialog.FileName, FileMode.Open))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Writer.Write(buffer, 0, bytesRead);
                    }
                }

                _ = MessageBox.Show("File uploaded successfully.", "Upload Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateFileList();
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (ListBoxFiles.SelectedItem != null)
            {
                string fileName = ListBoxFiles.SelectedItem.ToString();
                Writer.Write("DOWNLOAD");
                Writer.Write(fileName);

                bool fileExists = Reader.ReadBoolean();
                if (fileExists)
                {
                    using SaveFileDialog saveFileDialog = new();
                    saveFileDialog.FileName = fileName;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        long fileSize = Reader.ReadInt64();
                        using (FileStream fs = new(saveFileDialog.FileName, FileMode.Create))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            long totalBytesRead = 0;

                            while (totalBytesRead < fileSize && (bytesRead = Reader.Read(buffer, 0, (int)Math.Min(buffer.Length, fileSize - totalBytesRead))) > 0)
                            {
                                fs.Write(buffer, 0, bytesRead);
                                totalBytesRead += bytesRead;
                            }
                        }

                        _ = MessageBox.Show("File downloaded successfully.", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    _ = MessageBox.Show("File not found on the server.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (ListBoxFiles.SelectedItem != null)
            {
                string fileName = ListBoxFiles.SelectedItem.ToString();
                if (MessageBox.Show($"Are you sure you want to delete {fileName}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Writer.Write("DELETE");
                    Writer.Write(fileName);
                    _ = MessageBox.Show("File deleted successfully.", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateFileList();
                }
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            UpdateFileList();
        }

        private void buttonShare_Click(object sender, EventArgs e)
        {
            if (ListBoxFiles.SelectedItem != null)
            {
                string fileName = ListBoxFiles.SelectedItem.ToString();
                string recipient = TextBoxRecipient.Text.Trim();

                if (!string.IsNullOrEmpty(recipient) && recipient != "Enter recipient username")
                {
                    Writer.Write("SHARE");
                    Writer.Write(fileName);
                    Writer.Write(recipient);

                    bool shareSuccess = Reader.ReadBoolean();
                    _ = shareSuccess
                        ? MessageBox.Show("File shared successfully.", "Share Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        : MessageBox.Show("Failed to share the file.", "Share Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _ = MessageBox.Show("Please enter a valid recipient username.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (ListBoxFiles.SelectedItem != null)
            {
                string fileName = ListBoxFiles.SelectedItem.ToString();
                string recipient = TextBoxSendRecipient.Text.Trim();

                if (!string.IsNullOrEmpty(recipient) && recipient != "Enter recipient username for sending")
                {
                    Writer.Write("SEND");
                    Writer.Write(fileName);
                    Writer.Write(recipient);

                    bool sendSuccess = Reader.ReadBoolean();
                    if (sendSuccess)
                    {
                        _ = MessageBox.Show("File sent successfully.", "Send Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateFileList();
                    }
                    else
                    {
                        _ = MessageBox.Show("Failed to send the file.", "Send Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    _ = MessageBox.Show("Please enter a valid recipient username.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
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