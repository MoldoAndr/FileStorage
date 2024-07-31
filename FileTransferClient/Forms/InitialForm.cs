using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FileTransferClient
{
    public partial class InitialForm : Form
    {
        public bool IsLogin { get; private set; }

        public InitialForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Paint += new PaintEventHandler(SetBackgroundGradient);
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
            this.btnLogin = new CustomButton();
            this.btnSignup = new CustomButton();
            this.logoPanel = new Panel();
            this.titleBar = new Panel();
            this.btnMinimize = new Button();
            this.btnClose = new Button();
            this.SuspendLayout();

            this.titleBar.Size = new Size(800, 30);
            this.titleBar.Location = new Point(0, 0);
            this.titleBar.BackColor = Color.FromArgb(1, 4, 14);
            this.titleBar.MouseDown += new MouseEventHandler(TitleBar_MouseDown);

            this.btnMinimize.Size = new Size(30, 30);
            this.btnMinimize.Location = new Point(740, 0);
            this.btnMinimize.FlatStyle = FlatStyle.Flat;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.Text = "—";
            this.btnMinimize.Font = new Font("Arial", 12F, FontStyle.Bold);
            this.btnMinimize.ForeColor = Color.White;
            this.btnMinimize.BackColor = Color.Transparent;
            this.btnMinimize.Click += new EventHandler(BtnMinimize_Click);

            this.btnClose.Size = new Size(30, 30);
            this.btnClose.Location = new Point(770, 0);
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Text = "×";
            this.btnClose.Font = new Font("Arial", 12F, FontStyle.Bold);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.BackColor = Color.Transparent;
            this.btnClose.Click += new EventHandler(BtnClose_Click);

            this.logoPanel.Size = new Size(800, 200);
            this.logoPanel.Location = new Point(0, 60);
            this.logoPanel.BackColor = Color.Transparent;
            this.logoPanel.Paint += new PaintEventHandler(DrawLogo);

            this.btnLogin.Location = new Point(200, 300);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new Size(400, 80);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "Login";
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            this.btnSignup.Location = new Point(200, 400);
            this.btnSignup.Name = "btnSignup";
            this.btnSignup.Size = new Size(400, 80);
            this.btnSignup.TabIndex = 1;
            this.btnSignup.Text = "Signup";
            this.btnSignup.Click += new EventHandler(this.btnSignup_Click);

            this.AutoScaleDimensions = new SizeF(9F, 18F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 520);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.titleBar);
            this.Controls.Add(this.logoPanel);
            this.Controls.Add(this.btnSignup);
            this.Controls.Add(this.btnLogin);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "InitialForm";
            this.Text = "Welcome";
            this.ResumeLayout(false);
        }

        private void DrawLogo(object sender, PaintEventArgs e)
        {
            Panel logoPanel = (Panel)sender;

            string imagePath = "../../cloud.png";

            Image logoImage = Image.FromFile(imagePath);

            int panelWidth = logoPanel.ClientSize.Width;
            int panelHeight = logoPanel.ClientSize.Height;

            int imageWidth = logoImage.Width;
            int imageHeight = logoImage.Height;

            float scaleX = (float)panelWidth / imageWidth;
            float scaleY = (float)panelHeight / imageHeight;
            float scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(imageWidth * scale);
            int newHeight = (int)(imageHeight * scale);

            int imageX = (panelWidth - newWidth) / 2;
            int imageY = (panelHeight - newHeight) / 2;

            e.Graphics.DrawImage(logoImage, imageX, imageY, newWidth, newHeight);

            logoImage.Dispose();
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private CustomButton btnLogin;
        private CustomButton btnSignup;
        private Panel logoPanel;
        private Panel titleBar;
        private Button btnMinimize;
        private Button btnClose;
    }

    public class CustomButton : Button
    {
        public CustomButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.FromArgb(65, 105, 225);
            this.ForeColor = Color.White;
            this.Font = new Font("Arial", 16F, FontStyle.Bold);
            this.Cursor = Cursors.Hand;

            this.MouseEnter += (sender, e) => this.BackColor = Color.FromArgb(100, 140, 250);
            this.MouseLeave += (sender, e) => this.BackColor = Color.FromArgb(65, 105, 225);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            GraphicsPath grPath = new GraphicsPath();
            grPath.AddRectangle(new Rectangle(0, 0, this.Width, this.Height));
            this.Region = new Region(grPath);
        }
    }
}