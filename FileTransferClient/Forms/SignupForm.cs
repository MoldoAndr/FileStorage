using System.Drawing.Drawing2D;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace FileTransferClient.Forms
{
    public partial class SignupForm : Form
    {
        private SslStream sslStream;
        private BinaryReader reader;
        private BinaryWriter writer;

        public string Username { get; private set; }
        public string Password { get; private set; }
        private string ServerIP = "192.169.0.190";
        private Button btnClose;

        public SignupForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Paint += new PaintEventHandler(SetBackgroundGradient);
            AddPlaceholderText();
        }

        private void InitializeComponent()
        {
            txtUsername = new CustomTextBox();
            txtPassword = new CustomTextBox();
            btnSignup = new CustomButton();
            btnClose = new Button();

            SuspendLayout();

            txtUsername.Location = new Point(20, 80);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(640, 60);
            txtUsername.TabIndex = 0;
            txtUsername.Text = "Username";
            txtUsername.Enter += new EventHandler(TextBox_Enter);
            txtUsername.Leave += new EventHandler(TextBox_Leave);

            txtPassword.Location = new Point(20, 150);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(640, 60);
            txtPassword.TabIndex = 1;
            txtPassword.Text = "Password";
            txtPassword.PasswordChar = '*';
            txtPassword.Enter += new EventHandler(TextBox_Enter);
            txtPassword.Leave += new EventHandler(TextBox_Leave);

            btnSignup.Location = new Point(20, 220);
            btnSignup.Name = "btnSignup";
            btnSignup.Size = new Size(640, 80);
            btnSignup.TabIndex = 2;
            btnSignup.Text = "Signup";
            btnSignup.Click += new EventHandler(BtnSignup_Click);

            btnClose.Size = new Size(30, 30);
            btnClose.Location = new Point(650, 10);
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
            Controls.Add(btnSignup);
            Controls.Add(txtPassword);
            Controls.Add(txtUsername);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SignupForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Signup";
            BackColor = Color.FromArgb(2, 8, 28);
            ForeColor = Color.White;
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetBackgroundGradient(object sender, PaintEventArgs e)
        {
            using LinearGradientBrush brush = new(ClientRectangle,
                                                                       Color.FromArgb(2, 8, 28),
                                                                       Color.FromArgb(22, 33, 62),
                                                                       90F);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            Username = txtUsername.Text;
            Password = txtPassword.Text;

            if (Username == "Username" || Password == "Password")
            {
                _ = MessageBox.Show("Please enter a valid username and password.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (SignupUser(Username, Password))
            {
                _ = MessageBox.Show("Signup successful! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                _ = MessageBox.Show("Signup failed. Username may already exist.", "Signup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool SignupUser(string username, string password)
        {
            try
            {
                System.Net.Sockets.TcpClient client = new(this.ServerIP, 8888);
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
                _ = MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            TextBox? textBox = sender as TextBox;
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
            TextBox? textBox = sender as TextBox;
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

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
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
