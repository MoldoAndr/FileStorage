using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace FileTransferClient
{
    public partial class LoginForm : System.Windows.Forms.Form
    {
        public SslStream SslStream { get; private set; }
        public BinaryReader Reader { get; private set; }
        public BinaryWriter Writer { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginForm()
        {
            InitializeComponent();
            AddPlaceholderText();
        }

        public LoginForm(string username, string password)
            : this()
        {
            Username = username;
            Password = password;
            AuthenticateAndClose();
        }

        private void InitializeComponent()
        {
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.textBoxUsername.Location = new System.Drawing.Point(20, 20);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(640, 60);
            this.textBoxUsername.TabIndex = 0;
            this.textBoxUsername.Text = "Username";
            this.textBoxUsername.ForeColor = System.Drawing.Color.DarkBlue;
            this.textBoxUsername.Font = new System.Drawing.Font("Arial", 12F);
            this.textBoxUsername.Enter += new System.EventHandler(this.TextBox_Enter);
            this.textBoxUsername.Leave += new System.EventHandler(this.TextBox_Leave);

            this.textBoxPassword.Location = new System.Drawing.Point(20, 60);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(640, 60);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.Text = "Password";
            this.textBoxPassword.ForeColor = System.Drawing.Color.DarkBlue;
            this.textBoxPassword.Font = new System.Drawing.Font("Arial", 12F);
            this.textBoxPassword.PasswordChar = '\0';
            this.textBoxPassword.Enter += new System.EventHandler(this.TextBox_Enter);
            this.textBoxPassword.Leave += new System.EventHandler(this.TextBox_Leave);

            this.buttonLogin.Location = new System.Drawing.Point(20, 100);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(640, 80);
            this.buttonLogin.TabIndex = 2;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.BackColor = System.Drawing.Color.FromArgb(9, 27, 84);
            this.buttonLogin.ForeColor = System.Drawing.Color.White;
            this.buttonLogin.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonLogin.Click += new System.EventHandler(this.ButtonLogin_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 200);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.BackColor = System.Drawing.Color.FromArgb(2, 8, 28);
            this.ForeColor = System.Drawing.Color.White;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            if (username == "Username" || password == "Password")
            {
                MessageBox.Show("Please enter a valid username and password.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Username = username;
            Password = password;

            if (AuthenticateUser(username, password))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetLoginForm();
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient("192.168.0.190", 8888);
                SslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                SslStream.AuthenticateAsClient("FileTransferServer");

                Reader = new BinaryReader(SslStream);
                Writer = new BinaryWriter(SslStream);

                Writer.Write("LOGIN");
                Writer.Write(username);
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
                MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void AddPlaceholderText()
        {
            textBoxUsername.Text = "Username";
            textBoxUsername.ForeColor = System.Drawing.Color.DarkBlue;

            textBoxPassword.Text = "Password";
            textBoxPassword.ForeColor = System.Drawing.Color.DarkBlue;
            textBoxPassword.PasswordChar = '\0';
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.ForeColor == System.Drawing.Color.DarkBlue)
            {
                textBox.Text = "";
                textBox.ForeColor = System.Drawing.Color.Black;
                if (textBox == textBoxPassword)
                {
                    textBox.PasswordChar = '*';
                }
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
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
                textBox.ForeColor = System.Drawing.Color.DarkBlue;
            }
        }

        private void ResetLoginForm()
        {
            textBoxPassword.Clear();
            textBoxPassword.Text = "Password";
            textBoxPassword.ForeColor = System.Drawing.Color.DarkBlue;
            textBoxPassword.PasswordChar = '\0';

            if (string.IsNullOrWhiteSpace(textBoxUsername.Text))
            {
                textBoxUsername.Text = "Username";
                textBoxUsername.ForeColor = System.Drawing.Color.DarkBlue;
            }
        }

        private void CloseConnection()
        {
            Writer?.Close();
            Reader?.Close();
            SslStream?.Close();
        }

        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonLogin;
    }
}
