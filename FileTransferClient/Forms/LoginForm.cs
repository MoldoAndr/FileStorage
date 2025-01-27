﻿using System.Drawing.Drawing2D;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace FileTransferClient.Forms
{
    public partial class LoginForm : Form
    {
        public SslStream SslStream;
        public BinaryReader Reader;
        public BinaryWriter Writer;
        
        public string Username { get; private set; }
        public string Password { get; private set; }

        private string ServerIP;
        private Button btnClose;

        public LoginForm()
        {
            string filePath = "../../../Server/IP.txt";
            ServerIP = File.ReadAllText(filePath);
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Paint += new PaintEventHandler(SetBackgroundGradient);
            AddPlaceholderText();
        }

        public LoginForm(string username, string password)
            : this()
        {
            string filePath = "../../../Server/IP.txt";
            ServerIP = File.ReadAllText(filePath);
            Username = username;
            Password = password;
            AuthenticateAndClose();
        }

        private void InitializeComponent()
        {
            textBoxUsername = new CustomTextBox();
            textBoxPassword = new CustomTextBox();
            buttonLogin = new CustomButton();
            btnClose = new Button();

            SuspendLayout();

            textBoxUsername.Location = new Point(20, 80);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(640, 60);
            textBoxUsername.TabIndex = 0;
            textBoxUsername.Text = "Username";
            textBoxUsername.Enter += new EventHandler(TextBox_Enter);
            textBoxUsername.Leave += new EventHandler(TextBox_Leave);

            textBoxPassword.Location = new Point(20, 150);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(640, 60);
            textBoxPassword.TabIndex = 1;
            textBoxPassword.Text = "Password";
            textBoxPassword.PasswordChar = '*';
            textBoxPassword.Enter += new EventHandler(TextBox_Enter);
            textBoxPassword.Leave += new EventHandler(TextBox_Leave);

            buttonLogin.Location = new Point(20, 220);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(640, 80);
            buttonLogin.TabIndex = 2;
            buttonLogin.Text = "Login";
            buttonLogin.Click += new EventHandler(ButtonLogin_Click);

            btnClose.Size = new Size(30, 30);
            btnClose.Location = new Point(650, 0);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Text = "×";
            btnClose.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.BackColor = Color.Transparent;
            btnClose.Click += new EventHandler(BtnClose_Click);

            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(680, 350);
            Controls.Add(buttonLogin);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxUsername);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            BackColor = Color.FromArgb(2, 8, 28);
            ForeColor = Color.White;
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetBackgroundGradient(object sender, PaintEventArgs e)
        {
            using LinearGradientBrush brush = new(ClientRectangle, Color.FromArgb(2, 8, 28), Color.FromArgb(22, 33, 62), 90F);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            if (username == "Username" || password == "Password")
            {
                _ = MessageBox.Show("Please enter a valid username and password.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Username = username;
            Password = password;

            if (AuthenticateUser(username, password))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                _ = MessageBox.Show("Invalid username or password.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetLoginForm();
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                string PortPath = "../../../Server/Port.txt";
                int Port = int.Parse(File.ReadAllText(PortPath));
                System.Net.Sockets.TcpClient client = new(this.ServerIP, Port);
                SslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                SslStream.AuthenticateAsClient("FileTransferServer");

                var localEndPoint = (System.Net.IPEndPoint)client.Client.LocalEndPoint;
                string filePath = "../../../ClientPort/Port.txt";
                string portInfo = $"{localEndPoint.Port}";
                File.WriteAllText(filePath, portInfo);

                Reader = new BinaryReader(SslStream);
                Writer = new BinaryWriter(SslStream);

                Writer.Write("LOGIN");
                Writer.Write(username);
                Username = username;
                Writer.Write(password);
                bool authenticated = Reader.ReadBoolean();
                if (!authenticated)
                {
                    CloseConnection();
                }
                return authenticated;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void AuthenticateAndClose()
        {
            if (AuthenticateUser(Username, Password))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                _ = MessageBox.Show("Invalid username or password.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void AddPlaceholderText()
        {
            textBoxUsername.Text = "Username";
            textBoxUsername.ForeColor = Color.White;

            textBoxPassword.Text = "Password";
            textBoxPassword.ForeColor = Color.White;
            textBoxPassword.PasswordChar = '\0';
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox.ForeColor == Color.White)
            {
                textBox.Text = "";
                if (textBox == textBoxPassword)
                {
                    textBox.PasswordChar = '*';
                }
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == textBoxUsername)
                {
                    textBox.Text = "Username";
                }
                else if (textBox == textBoxPassword)
                {
                    textBox.Text = "Password";
                    textBox.PasswordChar = '\0';
                }
                textBox.ForeColor = Color.White;
            }
        }

        private void ResetLoginForm()
        {
            textBoxPassword.Clear();
            textBoxPassword.Text = "Password";
            textBoxPassword.ForeColor = Color.White;
            textBoxPassword.PasswordChar = '\0';

            if (string.IsNullOrWhiteSpace(textBoxUsername.Text))
            {
                textBoxUsername.Text = "Username";
                textBoxUsername.ForeColor = Color.White;
            }
        }

        private void CloseConnection()
        {
            Writer?.Close();
            Reader?.Close();
            SslStream?.Close();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private CustomTextBox textBoxUsername;
        private CustomTextBox textBoxPassword;
        private CustomButton buttonLogin;
    }

    public class CustomTextBox : TextBox
    {
        public CustomTextBox()
        {
            BorderStyle = BorderStyle.None;
            BackColor = Color.FromArgb(22, 33, 62);
            ForeColor = Color.White;
            Font = new Font("Arial", 16F);
        }
    }

}
