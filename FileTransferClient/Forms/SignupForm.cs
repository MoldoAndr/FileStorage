using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace FileTransferClient
{
    public partial class SignupForm : Form
    {
        private SslStream sslStream;
        private BinaryReader reader;
        private BinaryWriter writer;

        public string Username { get; private set; }
        public string Password { get; private set; }

        private Button btnMinimize;
        private Button btnClose;

        public SignupForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Paint += new PaintEventHandler(SetBackgroundGradient);
            AddPlaceholderText();
        }

        private void InitializeComponent()
        {
            this.txtUsername = new CustomTextBox();
            this.txtPassword = new CustomTextBox();
            this.btnSignup = new CustomButton();
            this.btnMinimize = new Button();
            this.btnClose = new Button();

            this.SuspendLayout();

            this.txtUsername.Location = new Point(20, 80);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new Size(640, 60);
            this.txtUsername.TabIndex = 0;
            this.txtUsername.Text = "Username";
            this.txtUsername.Enter += new EventHandler(this.TextBox_Enter);
            this.txtUsername.Leave += new EventHandler(this.TextBox_Leave);

            this.txtPassword.Location = new Point(20, 150);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new Size(640, 60);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.Text = "Password";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Enter += new EventHandler(this.TextBox_Enter);
            this.txtPassword.Leave += new EventHandler(this.TextBox_Leave);

            this.btnSignup.Location = new Point(20, 220);
            this.btnSignup.Name = "btnSignup";
            this.btnSignup.Size = new Size(640, 80);
            this.btnSignup.TabIndex = 2;
            this.btnSignup.Text = "Signup";
            this.btnSignup.Click += new EventHandler(this.BtnSignup_Click);

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
            this.Controls.Add(this.btnSignup);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "SignupForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Signup";
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

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            Username = txtUsername.Text;
            Password = txtPassword.Text;

            if (Username == "Username" || Password == "Password")
            {
                MessageBox.Show("Please enter a valid username and password.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (SignupUser(Username, Password))
            {
                MessageBox.Show("Signup successful! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Signup failed. Username may already exist.", "Signup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool SignupUser(string username, string password)
        {
            try
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient("192.168.0.190", 8888);
                sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                sslStream.AuthenticateAsClient("FileTransferServer");

                reader = new BinaryReader(sslStream);
                writer = new BinaryWriter(sslStream);

                writer.Write("SIGNUP");
                writer.Write(username);
                writer.Write(password);

                bool signupSuccess = reader.ReadBoolean();
                return signupSuccess;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void AddPlaceholderText()
        {
            txtUsername.Text = "Username";
            txtUsername.ForeColor = Color.White;

            txtPassword.Text = "Password";
            txtPassword.ForeColor = Color.White;
            txtPassword.PasswordChar = '\0';
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.ForeColor == Color.White)
            {
                textBox.Text = "";
                textBox.Font = new Font("Arial", 16F);
                if (textBox == txtPassword)
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
                if (textBox == txtUsername)
                {
                    textBox.Text = "Username";
                }
                else if (textBox == txtPassword)
                {
                    textBox.Text = "Password";
                    textBox.PasswordChar = '\0';
                }
                textBox.ForeColor = Color.White;
                textBox.Font = new Font("Arial", 16F, FontStyle.Italic);
            }
        }

        private void CloseConnection()
        {
            writer?.Close();
            reader?.Close();
            sslStream?.Close();
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseConnection();
            }
            base.Dispose(disposing);
        }

        private CustomTextBox txtUsername;
        private CustomTextBox txtPassword;
        private CustomButton btnSignup;
    }
}
