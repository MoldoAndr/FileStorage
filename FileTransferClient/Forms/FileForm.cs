using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace FileTransferClient
{
    public partial class FileForm : Form
    {
        private const int Port = 8888;
        private const string ServerIp = "192.168.0.190";
        private SslStream sslStream;
        private BinaryReader reader;
        private BinaryWriter writer;

        private TextBox textBoxRecipient;
        private TextBox textBoxSendRecipient;
        private Button buttonUpload;
        private Button buttonDownload;
        private Button buttonDelete;
        private Button buttonRefresh;
        private Button buttonShare;
        private Button buttonSend;
        private Button btnClose;
        private ListBox listBoxFiles;
        private Panel panelTop;
        private Panel logoPanel;

        public FileForm()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(2,8,28)
            };

            this.logoPanel = new Panel
            {
                Location = new Point(0, 30),
                Height = 150,
                Width = 800,
                BackColor = Color.Transparent,
                BackgroundImageLayout = ImageLayout.Center
            };
            this.logoPanel.Paint += new PaintEventHandler(DrawLogo);

            this.btnClose = new Button
            {
                Size = new Size(30, 30),
                Location = new Point(770, 0),
                FlatStyle = FlatStyle.Flat,
                Text = "×",
                Font = new Font("Arial", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(22, 33, 62),
                Cursor = Cursors.Hand
            };
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Click += new EventHandler(BtnClose_Click);
            this.panelTop.Controls.Add(this.btnClose);

            this.listBoxFiles = new ListBox
            {
                Font = new Font("Segoe UI", 14F),
                ItemHeight = 32,
                Location = new Point(20, 280),
                Size = new Size(760, 300),
                BackColor = Color.FromArgb(22, 33, 62),
                ForeColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.None
            };

            // Buttons
            this.buttonUpload = CreateStyledButton("Upload", 20, 590);
            this.buttonDownload = CreateStyledButton("Download", 210, 590);
            this.buttonDelete = CreateStyledButton("Delete", 400, 590);
            this.buttonRefresh = CreateStyledButton("Refresh", 590, 590);
            this.buttonShare = CreateStyledButton("Share", 20, 650);
            this.buttonSend = CreateStyledButton("Send", 20, 710);

            // TextBoxes
            this.textBoxRecipient = CreateStyledTextBox("Enter recipient username", 210, 650);
            this.textBoxSendRecipient = CreateStyledTextBox("Enter recipient username for sending", 210, 710);

            // Form properties
            this.ClientSize = new Size(800, 800);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.logoPanel);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonShare);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxRecipient);
            this.Controls.Add(this.textBoxSendRecipient);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(22, 33, 62);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Button CreateStyledButton(string text, int x, int y)
        {
            Button button = new Button
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
            TextBox textBox = new TextBox
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
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(2, 8, 28), Color.FromArgb(22, 33, 62), 90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private EventHandler GetButtonClickHandler(string buttonText)
        {
            switch (buttonText)
            {
                case "Upload": return buttonUpload_Click;
                case "Download": return buttonDownload_Click;
                case "Delete": return buttonDelete_Click;
                case "Refresh": return buttonRefresh_Click;
                case "Share": return buttonShare_Click;
                case "Send": return buttonSend_Click;
                default: return null;
            }
        }

        private void ConnectToServer()
        {
            try
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(ServerIp, Port);
                sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                sslStream.AuthenticateAsClient("FileTransferServer");

                reader = new BinaryReader(sslStream);
                writer = new BinaryWriter(sslStream);
                UpdateFileList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void UpdateFileList()
        {
            listBoxFiles.Items.Clear();
            writer.Write("LIST");
            int fileCount = reader.ReadInt32();
            for (int i = 0; i < fileCount; i++)
            {
                listBoxFiles.Items.Add(reader.ReadString());
            }
        }

        public FileForm(SslStream sslStream, BinaryReader reader, BinaryWriter writer)
        {
            InitializeComponent();
            this.sslStream = sslStream;
            this.reader = reader;
            this.writer = writer;
            UpdateFileList();
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    writer.Write("UPLOAD");
                    writer.Write(fileName);
                    writer.Write(new FileInfo(openFileDialog.FileName).Length);

                    using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, bytesRead);
                        }
                    }

                    MessageBox.Show("File uploaded successfully.", "Upload Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateFileList();
                }
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem != null)
            {
                string fileName = listBoxFiles.SelectedItem.ToString();
                writer.Write("DOWNLOAD");
                writer.Write(fileName);

                bool fileExists = reader.ReadBoolean();
                if (fileExists)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.FileName = fileName;
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            long fileSize = reader.ReadInt64();
                            using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                            {
                                byte[] buffer = new byte[4096];
                                int bytesRead;
                                long totalBytesRead = 0;

                                while (totalBytesRead < fileSize && (bytesRead = reader.Read(buffer, 0, (int)Math.Min(buffer.Length, fileSize - totalBytesRead))) > 0)
                                {
                                    fs.Write(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;
                                }
                            }

                            MessageBox.Show("File downloaded successfully.", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("File not found on the server.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem != null)
            {
                string fileName = listBoxFiles.SelectedItem.ToString();
                if (MessageBox.Show($"Are you sure you want to delete {fileName}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    writer.Write("DELETE");
                    writer.Write(fileName);
                    MessageBox.Show("File deleted successfully.", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (listBoxFiles.SelectedItem != null)
            {
                string fileName = listBoxFiles.SelectedItem.ToString();
                string recipient = textBoxRecipient.Text.Trim();

                if (!string.IsNullOrEmpty(recipient) && recipient != "Enter recipient username")
                {
                    writer.Write("SHARE");
                    writer.Write(fileName);
                    writer.Write(recipient);

                    bool shareSuccess = reader.ReadBoolean();
                    if (shareSuccess)
                    {
                        MessageBox.Show("File shared successfully.", "Share Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to share the file.", "Share Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid recipient username.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem != null)
            {
                string fileName = listBoxFiles.SelectedItem.ToString();
                string recipient = textBoxSendRecipient.Text.Trim();

                if (!string.IsNullOrEmpty(recipient) && recipient != "Enter recipient username for sending")
                {
                    writer.Write("SEND");
                    writer.Write(fileName);
                    writer.Write(recipient);

                    bool sendSuccess = reader.ReadBoolean();
                    if (sendSuccess)
                    {
                        MessageBox.Show("File sent successfully.", "Send Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateFileList();
                    }
                    else
                    {
                        MessageBox.Show("Failed to send the file.", "Send Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid recipient username.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DrawLogo(object sender, PaintEventArgs e)
        {
            Panel logoPanel = (Panel)sender;

            string imagePath = "../../cloud.png";
            if (File.Exists(imagePath))
            {
                using (Image logoImage = Image.FromFile(imagePath))
                {
                    int panelWidth = 800;
                    int panelHeight = 200;

                    int imageWidth = logoImage.Width;
                    int imageHeight = logoImage.Height;

                    float scaleX = (float)(panelWidth / 2) / imageWidth;
                    float scaleY = (float)(panelHeight / 2) / imageHeight;
                    float scale = Math.Min(scaleX, scaleY);

                    int newWidth = (int)(imageWidth * scale * 1.5);
                    int newHeight = (int)(imageHeight * scale * 1.5);

                    int imageX = 400 - newWidth/2;
                    int imageY = 0;
                    e.Graphics.DrawImage(logoImage, imageX, imageY, newWidth, newHeight);
                }
            }
            else
            {
                MessageBox.Show("Logo image not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
