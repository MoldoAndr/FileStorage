using FileTransferClient.Forms;
using System;
using System.Windows.Forms;

namespace FileTransferClient
{
    internal static class Handler
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new InitialFormHandler());
        }

        private class InitialFormHandler : Form
        {
            public InitialFormHandler()
            {
                LoadInitialForm();
            }

            private void LoadInitialForm()
            {
                while (true)
                {
                    using InitialForm initialForm = new();
                    if (initialForm.ShowDialog() != DialogResult.OK)
                        break;

                    if (initialForm.IsLogin)
                    {
                        using LoginForm loginForm = new();
                        if (loginForm.ShowDialog() == DialogResult.OK)
                        {
                            Application.Run(new FileForm(loginForm.SslStream, loginForm.Reader, loginForm.Writer, loginForm.Username, loginForm.Password));
                        }
                    }
                    else
                    {
                        using SignupForm signupForm = new();
                        DialogResult result = signupForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            _ = MessageBox.Show("Signup successful! Please log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                this.Close();
            }
        }
    }
}
