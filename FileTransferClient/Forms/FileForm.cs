    using System.ComponentModel.Design;
    using System.Drawing.Drawing2D;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    #pragma warning disable CS8622

    namespace FileTransferClient.Forms
    {
        public partial class FileForm : Form
        {
            private const int Port = 8888;
            private const string ServerIp = "192.168.0.190";
            private string Password { get; set; }

            public FileForm()
            {
                InitializeComponent();
                ConnectToServer();
            }

            public FileForm(SslStream sslStream, BinaryReader reader, BinaryWriter writer, string username, string password)
            {
                Username = username;
                Password = password;
                InitializeComponent();
                SslStream = sslStream;
                Reader = reader;
                Writer = writer;
                UpdateFileList();
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

            private EventHandler GetButtonClickHandler(string buttonText)
            {
                return buttonText switch
                {
                    "Upload" => buttonUpload_Click,
                    "Download" => buttonDownload_Click,
                    "Delete" => buttonDelete_Click,
                    "View" => buttonView_Click,
                    "Share" => buttonShare_Click,
                    "Send" => buttonSend_Click,
                    "Modify" => buttonModify_Click,
                    "Refresh" => buttonRefresh_Click,
                    "Rename" => buttonRename_Click,
                    _ => null,
                };
            }

            private void ConnectToServer()
            {
                try
                {
                    System.Net.Sockets.TcpClient client = new(ServerIp, Port);
                    var localEndPoint = (System.Net.IPEndPoint)client.Client.LocalEndPoint;
                    
                    
                
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

            private void Disconnect()
            {
                try
                {
                    Writer?.Close();
                    Reader?.Close();
                    SslStream?.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error disconnecting: {ex.Message}", "Disconnection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
        
            private void BtnBack_Click(object sender, EventArgs e)
            {
                Disconnect();
                Close();
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
                    Writer.Write("DELETE");
                    Writer.Write(fileName);
                    
                    UpdateFileList();
                }
            }

            private void buttonModify_Click(object sender, EventArgs e)
            {
                if (ListBoxFiles.SelectedItem != null)
                {
                    string fileName = ListBoxFiles.SelectedItem.ToString();
                    Writer.Write("VIEW");
                    Writer.Write(fileName);
                    
                    Writer.Flush();

                    bool fileExists = Reader.ReadBoolean();
                    if (fileExists)
                    {
                        try
                        {
                            int fileBytesLength = Reader.ReadInt32();
                            byte[] fileBytes = Reader.ReadBytes(fileBytesLength);
                            string content = Encoding.UTF8.GetString(fileBytes);

                            TextEditorForm modifyForm = new TextEditorForm(fileName, content, Reader, Writer);
                            modifyForm.Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred while reading the file bytes: {ex.Message}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File not found on the server.", "View Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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
                        if (!shareSuccess)
                        {
                            _ = MessageBox.Show("Failed to share the file.", "Share Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            _ = MessageBox.Show("File shared successfully.", "Share Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        _ = MessageBox.Show("Please enter a valid recipient username.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            private void buttonView_Click(object sender, EventArgs e)
            {
                if (ListBoxFiles.SelectedItem != null)
                {
                    string fileName = ListBoxFiles.SelectedItem.ToString();
                    Writer.Write("VIEW");
                    Writer.Write(fileName);

                    Writer.Flush();

                    bool fileExists = Reader.ReadBoolean();
                    if (fileExists)
                    {
                        try
                        {
                            int fileBytesLength = Reader.ReadInt32();
                            byte[] fileBytes = Reader.ReadBytes(fileBytesLength);
                            string content = Encoding.UTF8.GetString(fileBytes);

                            TextViewerForm viewerForm = new TextViewerForm(Reader,Writer,fileName);
                            viewerForm.Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred while reading the file bytes: {ex.Message}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File not found on the server.", "View Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            private void buttonRename_Click(object sender, EventArgs e)
            {
                if (ListBoxFiles.SelectedItem != null)
                {
                    string oldFileName = ListBoxFiles.SelectedItem.ToString();

                    using (InputDialog inputDialog = new InputDialog("Enter the new file name:"))
                    {
                        if (inputDialog.ShowDialog() == DialogResult.OK)
                        {
                            string newFileName = inputDialog.InputText;

                                try
                                {
                                    Writer.Write("RENAME");
                                    Writer.Write(oldFileName);
                                    Writer.Write(newFileName);
                                    bool renameSuccess = Reader.ReadBoolean();
                                    if (renameSuccess)
                                    {
                                    UpdateFileList();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Error renaming file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("No file selected. Please select a file to rename.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            private void buttonRefresh_Click(object sender, EventArgs e)
            {
                UpdateFileList();
            }
        }
    }
    #pragma warning restore CS8622