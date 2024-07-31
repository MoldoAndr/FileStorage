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

        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonShare;
        private System.Windows.Forms.TextBox textBoxRecipient;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxSendRecipient;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelTitle;

        public FileForm()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.panelTop = new System.Windows.Forms.Panel();
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 60;
            this.panelTop.BackColor = Color.FromArgb(44, 62, 80);

            this.labelTitle = new System.Windows.Forms.Label();
            this.labelTitle.Text = "File Transfer Client";
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = Color.FromArgb(236, 240, 241);
            this.labelTitle.Location = new System.Drawing.Point(20, 10);
            this.labelTitle.AutoSize = true;
            this.panelTop.Controls.Add(this.labelTitle);

            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.listBoxFiles.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 25;
            this.listBoxFiles.Location = new System.Drawing.Point(20, 80);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(760, 300);
            this.listBoxFiles.TabIndex = 0;
            this.listBoxFiles.BackColor = Color.FromArgb(52, 73, 94);
            this.listBoxFiles.ForeColor = Color.FromArgb(236, 240, 241);
            this.listBoxFiles.BorderStyle = BorderStyle.None;

            this.buttonUpload = CreateStyledButton("Upload", 20, 400);
            this.buttonDownload = CreateStyledButton("Download", 210, 400);
            this.buttonDelete = CreateStyledButton("Delete", 400, 400);
            this.buttonRefresh = CreateStyledButton("Refresh", 590, 400);
            this.buttonShare = CreateStyledButton("Share", 20, 460);
            this.buttonSend = CreateStyledButton("Send", 20, 520);

            this.textBoxRecipient = CreateStyledTextBox("Enter recipient username", 210, 470);
            this.textBoxSendRecipient = CreateStyledTextBox("Enter recipient username for sending", 210, 530);

            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.textBoxRecipient);
            this.Controls.Add(this.buttonShare);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.textBoxSendRecipient);
            this.Controls.Add(this.buttonSend);
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Transfer Client";
            this.BackColor = Color.FromArgb(34, 47, 62);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Button CreateStyledButton(string text, int x, int y)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            button.Location = new Point(x, y);
            button.Size = new Size(170, 40);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(41, 128, 185);
            button.ForeColor = Color.FromArgb(236, 240, 241);
            button.Cursor = Cursors.Hand;
            button.Click += GetButtonClickHandler(text);
            return button;
        }

        private TextBox CreateStyledTextBox(string placeholder, int x, int y)
        {
            TextBox textBox = new TextBox();
            textBox.Font = new Font("Segoe UI", 11F);
            textBox.Location = new Point(x, y);
            textBox.Size = new Size(570, 30);
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;
            textBox.BackColor = Color.FromArgb(52, 73, 94); 
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
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(44, 62, 80), Color.FromArgb(52, 73, 94), 90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
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

    } 
}