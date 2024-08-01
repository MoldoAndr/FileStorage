using System.Drawing.Drawing2D;

namespace FileTransferClient.Forms
{
    public partial class InitialForm : Form
    {
        public bool IsLogin { get; private set; }

        public InitialForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Paint += new PaintEventHandler(SetBackgroundGradient);
        }

        private void SetBackgroundGradient(object sender, PaintEventArgs e)
        {
            using LinearGradientBrush brush = new(ClientRectangle,
                                                                       Color.FromArgb(2, 8, 28),
                                                                       Color.FromArgb(22, 33, 62),
                                                                       90F);
            e.Graphics.FillRectangle(brush, ClientRectangle);
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
            btnLogin = new CustomButton();
            btnSignup = new CustomButton();
            logoPanel = new Panel();
            titleBar = new Panel();
            btnClose = new Button();
            SuspendLayout();

            titleBar.Size = new Size(800, 30);
            titleBar.Location = new Point(0, 0);
            titleBar.BackColor = Color.FromArgb(1, 4, 14);
            titleBar.MouseDown += new MouseEventHandler(TitleBar_MouseDown);

            btnClose.Size = new Size(30, 30);
            btnClose.Location = new Point(770, 0);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Text = "×";
            btnClose.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.BackColor = Color.Transparent;
            btnClose.Click += new EventHandler(BtnClose_Click);

            logoPanel.Size = new Size(800, 200);
            logoPanel.Location = new Point(0, 60);
            logoPanel.BackColor = Color.Transparent;
            logoPanel.Paint += new PaintEventHandler(DrawLogo);

            btnLogin.Location = new Point(200, 300);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(400, 80);
            btnLogin.TabIndex = 0;
            btnLogin.Text = "Login";
            btnLogin.Click += new EventHandler(btnLogin_Click);

            btnSignup.Location = new Point(200, 400);
            btnSignup.Name = "btnSignup";
            btnSignup.Size = new Size(400, 80);
            btnSignup.TabIndex = 1;
            btnSignup.Text = "Signup";
            btnSignup.Click += new EventHandler(btnSignup_Click);

            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 520);
            Controls.Add(btnClose);
            Controls.Add(titleBar);
            Controls.Add(logoPanel);
            Controls.Add(btnSignup);
            Controls.Add(btnLogin);
            FormBorderStyle = FormBorderStyle.None;
            Name = "InitialForm";
            Text = "Welcome";
            ResumeLayout(false);
        }

        private void DrawLogo(object sender, PaintEventArgs e)
        {
            Panel logoPanel = (Panel)sender;

            string imagePath = "../../../cloud.png";

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
                _ = ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(nint hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private CustomButton btnLogin;
        private CustomButton btnSignup;
        private Panel logoPanel;
        private Panel titleBar;
        private Button btnClose;
    }

    public class CustomButton : Button
    {
        public CustomButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(65, 105, 225);
            ForeColor = Color.White;
            Font = new Font("Arial", 16F, FontStyle.Bold);
            Cursor = Cursors.Hand;

            MouseEnter += (sender, e) => BackColor = Color.FromArgb(100, 140, 250);
            MouseLeave += (sender, e) => BackColor = Color.FromArgb(65, 105, 225);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            GraphicsPath grPath = new();
            grPath.AddRectangle(new Rectangle(0, 0, Width, Height));
            Region = new Region(grPath);
        }
    }
}