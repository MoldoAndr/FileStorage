using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace FileTransferClient
{
    public partial class LoginForm : Form
    {
        public SslStream SslStream { get; private set; }
        public BinaryReader Reader { get; private set; }
        public BinaryWriter Writer { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }

        private Button btnMinimize;
        private Button btnClose;

        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Paint += new PaintEventHandler(SetBackgroundGradient);
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
            this.textBoxUsername = new CustomTextBox();
            this.textBoxPassword = new CustomTextBox();
            this.buttonLogin = new CustomButton();
            this.btnMinimize = new Button();
            this.btnClose = new Button();

            this.SuspendLayout();

            this.textBoxUsername.Location = new Point(20, 80);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new Size(640, 60);
            this.textBoxUsername.TabIndex = 0;
            this.textBoxUsername.Text = "Username";
            this.textBoxUsername.Enter += new EventHandler(this.TextBox_Enter);
            this.textBoxUsername.Leave += new EventHandler(this.TextBox_Leave);

            this.textBoxPassword.Location = new Point(20, 150);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new Size(640, 60);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.Text = "Password";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Enter += new EventHandler(this.TextBox_Enter);
            this.textBoxPassword.Leave += new EventHandler(this.TextBox_Leave);

            this.buttonLogin.Location = new Point(20, 220);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new Size(640, 80);
            this.buttonLogin.TabIndex = 2;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.Click += new EventHandler(this.ButtonLogin_Click);

            this.btnMinimize.Size = new Size(30, 30);
            this.btnMinimize.Location = new Point(620, 10);
            this.btnMinimize.FlatStyle = FlatStyle.Flat;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.Text = "—";
            this.btnMinimize.Font = new Font("Arial", 12F, FontStyle.Bold);
            this.btnMinimize.ForeColor = Color.White;
            this.btnMinimize.BackColor = Color.Transparent;
            this.btnMinimize.Click += new EventHandler(BtnMinimize_Click);

            this.btnClose.Size = new Size(30, 30);
            this.btnClose.Location = new Point(650, 10);
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Text = "×";
            this.btnClose.Font = new Font("Arial", 12F, FontStyle.Bold);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.BackColor = Color.Transparent;
            this.btnClose.Click += new EventHandler(BtnClose_Click);

            this.AutoScaleDimensions = new SizeF(9F, 18F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(680, 350);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "LoginForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.BackColor = Color.FromArgb(2, 8, 28);
            this.ForeColor = Color.White;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetBackgroundGradient(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                                                                       Color.FromArgb(2, 8, 28),
                                                                       Color.FromArgb(22, 33, 62),
                                                                       90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
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
            textBoxUsername.ForeColor = Color.White;

            textBoxPassword.Text = "Password";
            textBoxPassword.ForeColor = Color.White;
            textBoxPassword.PasswordChar = '\0';
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
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
                textBox.ForeColor = Color.White;
            }
        }

        private void ResetLoginForm()
        {
            textBoxPassword.Clear();
            textBoxPassword.Text = "Password";
            textBoxPassword.ForeColor = Color.DarkBlue;
            textBoxPassword.PasswordChar = '\0';

            if (string.IsNullOrWhiteSpace(textBoxUsername.Text))
            {
                textBoxUsername.Text = "Username";
                textBoxUsername.ForeColor = Color.DarkBlue;
            }
        }

        private void CloseConnection()
        {
            Writer?.Close();
            Reader?.Close();
            SslStream?.Close();
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private CustomTextBox textBoxUsername;
        private CustomTextBox textBoxPassword;
        private CustomButton buttonLogin;
    }

    public class CustomTextBox : TextBox
    {
        public CustomTextBox()
        {
            this.BorderStyle = BorderStyle.None;
            this.BackColor = Color.FromArgb(22, 33, 62);
            this.ForeColor = Color.White;
            this.Font = new Font("Arial", 16F);
        }
    }

}
