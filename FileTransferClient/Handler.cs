using FileTransferClient.Forms;

namespace FileTransferClient
{
    internal static class Handler
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (true)
            {
                if (!ShowInitialForm())
                {
                    break;
                }
            }
        }

        private static bool ShowInitialForm()
        {
            using InitialForm initialForm = new();
            return initialForm.ShowDialog() == DialogResult.OK && (initialForm.IsLogin ? ShowLoginForm() : ShowSignupForm());
        }

        private static bool ShowLoginForm()
        {
            using LoginForm loginForm = new();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FileForm(loginForm.SslStream, loginForm.Reader, loginForm.Writer, loginForm.Username));
                return false;
            }
            return true;
        }

        private static bool ShowSignupForm()
        {
            using SignupForm signupForm = new();
            DialogResult result = signupForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                _ = MessageBox.Show("Signup successful! Please log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return true;
        }
    }

}