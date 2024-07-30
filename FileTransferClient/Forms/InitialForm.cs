using System;
using System.Windows.Forms;

namespace FileTransferClient
{
    public partial class InitialForm : System.Windows.Forms.Form
    {
        public bool IsLogin { get; private set; }

        public InitialForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(2, 8, 28);
            this.ForeColor = System.Drawing.Color.White;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            IsLogin = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            IsLogin = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void InitializeComponent()
        {
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnSignup = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.btnLogin.Location = new System.Drawing.Point(20, 20);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(640, 80);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(9, 27, 84);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Font = new System.Drawing.Font("Arial", 12F);
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            this.btnSignup.Location = new System.Drawing.Point(20, 100);
            this.btnSignup.Name = "btnSignup";
            this.btnSignup.Size = new System.Drawing.Size(640, 80);
            this.btnSignup.TabIndex = 1;
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
            this.Controls.Add(this.btnLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InitialForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Initial Form";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnSignup;
    }
}
