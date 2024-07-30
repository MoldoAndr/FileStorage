using System;
using System.Windows.Forms;

namespace FileTransferClient
{
    static class Handler
    {
        [STAThread]
        static void Main()
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

        static bool ShowInitialForm()
        {
            using (InitialForm initialForm = new InitialForm())
            {
                return initialForm.ShowDialog() == DialogResult.OK ? initialForm.IsLogin ? ShowLoginForm() : ShowSignupForm() : false;
            }
        }

        static bool ShowLoginForm()
        {
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new FileForm(loginForm.SslStream, loginForm.Reader, loginForm.Writer));
                    return false;
                }
                return true;
            }
        }

        static bool ShowSignupForm()
        {
            using (SignupForm signupForm = new SignupForm())
            {
                DialogResult result = signupForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    MessageBox.Show("Signup successful! Please log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
        }
    }
}