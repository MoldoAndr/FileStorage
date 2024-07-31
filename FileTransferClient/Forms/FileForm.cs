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

        public FileForm()
        {
            InitializeComponent();
            ConnectToServer();
        }

        public FileForm(SslStream sslStream, BinaryReader reader, BinaryWriter writer)
        {
            InitializeComponent();
            this.sslStream = sslStream;
            this.reader = reader;
            this.writer = writer;
            UpdateFileList();
        }

        private void InitializeComponent()
        {
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonShare = new System.Windows.Forms.Button();
            this.textBoxRecipient = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxSendRecipient = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            this.listBoxFiles.Font = new System.Drawing.Font("Arial", 12F);
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 30;
            this.listBoxFiles.Location = new System.Drawing.Point(12, 12);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(776, 340);
            this.listBoxFiles.TabIndex = 0;

            this.buttonUpload.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonUpload.Location = new System.Drawing.Point(12, 358);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(180, 45);
            this.buttonUpload.TabIndex = 1;
            this.buttonUpload.Text = "Upload";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);

            this.buttonDownload.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonDownload.Location = new System.Drawing.Point(206, 358);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(180, 45);
            this.buttonDownload.TabIndex = 2;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);

            this.buttonDelete.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonDelete.Location = new System.Drawing.Point(400, 358);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(180, 45);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);

            this.buttonRefresh.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonRefresh.Location = new System.Drawing.Point(594, 358);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(180, 45);
            this.buttonRefresh.TabIndex = 4;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);

            this.buttonShare.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonShare.Location = new System.Drawing.Point(12, 409);
            this.buttonShare.Name = "buttonShare";
            this.buttonShare.Size = new System.Drawing.Size(180, 45);
            this.buttonShare.TabIndex = 5;
            this.buttonShare.Text = "Share";
            this.buttonShare.UseVisualStyleBackColor = true;
            this.buttonShare.Click += new System.EventHandler(this.buttonShare_Click);

            this.textBoxRecipient.Font = new System.Drawing.Font("Arial", 12F);
            this.textBoxRecipient.Location = new System.Drawing.Point(206, 419);
            this.textBoxRecipient.Name = "textBoxRecipient";
            this.textBoxRecipient.Size = new System.Drawing.Size(568, 26);
            this.textBoxRecipient.TabIndex = 6;
            this.textBoxRecipient.Text = "Enter recipient username";
            this.textBoxRecipient.ForeColor = System.Drawing.Color.Gray;
            this.textBoxRecipient.Enter += new System.EventHandler(this.textBoxRecipient_Enter);
            this.textBoxRecipient.Leave += new System.EventHandler(this.textBoxRecipient_Leave);

            this.buttonSend.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonSend.Location = new System.Drawing.Point(12, 460);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(180, 45);
            this.buttonSend.TabIndex = 7;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);

            this.textBoxSendRecipient.Font = new System.Drawing.Font("Arial", 12F);
            this.textBoxSendRecipient.Location = new System.Drawing.Point(206, 470);
            this.textBoxSendRecipient.Name = "textBoxSendRecipient";
            this.textBoxSendRecipient.Size = new System.Drawing.Size(568, 26);
            this.textBoxSendRecipient.TabIndex = 8;
            this.textBoxSendRecipient.Text = "Enter recipient username for sending";
            this.textBoxSendRecipient.ForeColor = System.Drawing.Color.Gray;
            this.textBoxSendRecipient.Enter += new System.EventHandler(this.textBoxSendRecipient_Enter);
            this.textBoxSendRecipient.Leave += new System.EventHandler(this.textBoxSendRecipient_Leave);

            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 520);
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
            this.ResumeLayout(false);
            this.PerformLayout();
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

        private void textBoxRecipient_Enter(object sender, EventArgs e)
        {
            if (textBoxRecipient.Text == "Enter recipient username")
            {
                textBoxRecipient.Text = "";
                textBoxRecipient.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void textBoxRecipient_Leave(object sender, EventArgs e)
        {
            if (textBoxRecipient.Text == "")
            {
                textBoxRecipient.Text = "Enter recipient username";
                textBoxRecipient.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void textBoxSendRecipient_Enter(object sender, EventArgs e)
        {
            if (textBoxSendRecipient.Text == "Enter recipient username for sending")
            {
                textBoxSendRecipient.Text = "";
                textBoxSendRecipient.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void textBoxSendRecipient_Leave(object sender, EventArgs e)
        {
            if (textBoxSendRecipient.Text == "")
            {
                textBoxSendRecipient.Text = "Enter recipient username for sending";
                textBoxSendRecipient.ForeColor = System.Drawing.Color.Gray;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.DarkBlue, Color.SteelBlue, 90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonShare;
        private System.Windows.Forms.TextBox textBoxRecipient;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxSendRecipient;
    }
}