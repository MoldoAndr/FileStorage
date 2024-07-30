using System;
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

        public SignupForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnSignup = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.txtUsername.Location = new System.Drawing.Point(20, 20);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(640, 60);
            this.txtUsername.TabIndex = 0;
            this.txtUsername.Text = "Username";
            this.txtUsername.ForeColor = System.Drawing.Color.Gray;
            this.txtUsername.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Italic);
            this.txtUsername.Enter += new System.EventHandler(this.textBox_Enter);
            this.txtUsername.Leave += new System.EventHandler(this.textBox_Leave);

            this.txtPassword.Location = new System.Drawing.Point(20, 60);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(640, 60);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.Text = "Password";
            this.txtPassword.ForeColor = System.Drawing.Color.Gray;
            this.txtPassword.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Italic);
            this.txtPassword.PasswordChar = '\0';
            this.txtPassword.Enter += new System.EventHandler(this.textBox_Enter);
            this.txtPassword.Leave += new System.EventHandler(this.textBox_Leave);

            this.btnSignup.Location = new System.Drawing.Point(20, 100);
            this.btnSignup.Name = "btnSignup";
            this.btnSignup.Size = new System.Drawing.Size(640, 80);
            this.btnSignup.TabIndex = 2;
            this.btnSignup.Text = "Signup";
            this.btnSignup.UseVisualStyleBackColor = true;
            this.btnSignup.BackColor = System.Drawing.Color.FromArgb(9, 27, 84);
            this.btnSignup.ForeColor = System.Drawing.Color.White;
            this.btnSignup.Font = new System.Drawing.Font("Arial", 12F);
            this.btnSignup.Click += new System.EventHandler(this.btnSignup_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 200);
            this.Controls.Add(this.btnSignup);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SignupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Signup";
            this.BackColor = System.Drawing.Color.FromArgb(2, 8, 28);
            this.ForeColor = System.Drawing.Color.White;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnSignup_Click(object sender, EventArgs e)
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

        private void textBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.ForeColor == System.Drawing.Color.Gray)
            {
                textBox.Text = "";
                textBox.ForeColor = System.Drawing.Color.Black;
                textBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular);
                if (textBox == txtPassword)
                {
                    textBox.PasswordChar = '*';
                }
            }
        }

        private void textBox_Leave(object sender, EventArgs e)
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
                textBox.ForeColor = System.Drawing.Color.Gray;
                textBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Italic);
            }
        }

        private void CloseConnection()
        {
            writer?.Close();
            reader?.Close();
            sslStream?.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseConnection();
            }
            base.Dispose(disposing);
        }

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnSignup;
    }
}