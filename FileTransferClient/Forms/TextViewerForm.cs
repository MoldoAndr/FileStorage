using System;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Drawing;

public partial class TextViewerForm : Form
{
    private readonly BinaryReader Reader;
    private readonly BinaryWriter Writer;
    private readonly string fileName;
    private System.Timers.Timer updateTimer;

    public TextViewerForm(BinaryReader reader, BinaryWriter writer, string fileName)
    {
        InitializeComponent();
        this.Reader = reader;
        this.Writer = writer;
        this.fileName = fileName;

        updateTimer = new System.Timers.Timer();
        updateTimer.Interval = 400;
        updateTimer.Elapsed += OnTimedEvent;
        updateTimer.AutoReset = true;
        updateTimer.Enabled = true;
    }

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        try
        {
            Writer.Write("VIEW");
            Writer.Write(fileName);
            Writer.Flush();

            bool fileExists = Reader.ReadBoolean();
            if (fileExists)
            {
                int fileSize = Reader.ReadInt32();
                byte[] fileContentBytes = Reader.ReadBytes(fileSize);
                string fileContent = Encoding.UTF8.GetString(fileContentBytes);

                this.Invoke((MethodInvoker)delegate
                {
                    textBoxContent.Text = fileContent;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    textBoxContent.Text = "File not found on the server.";
                });
            }
        }
        catch (Exception ex)
        {
<<<<<<< HEAD
=======
            
>>>>>>> 26f87069918a9f76a376787a60eed6403ef5982e
        }
    }

    private void TextViewerForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        updateTimer.Stop();
        updateTimer.Dispose();
    }

    private void InitializeComponent()
    {
        this.textBoxContent = new System.Windows.Forms.TextBox();
        this.panelTop = new System.Windows.Forms.Panel();
        this.btnBack = new System.Windows.Forms.Button();
        this.viewer = new System.Windows.Forms.Button();
        this.SuspendLayout();

        this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
        this.panelTop.Height = 30;
        this.panelTop.BackColor = Color.FromArgb(2, 8, 28);

        this.btnBack.Dock = System.Windows.Forms.DockStyle.Left;
        this.btnBack.Size = new System.Drawing.Size(150, 30);
        this.btnBack.FlatStyle = FlatStyle.Flat;
        this.btnBack.Text = "Back";
        this.btnBack.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
        this.btnBack.ForeColor = Color.White;
        this.btnBack.BackColor = Color.Transparent;

        this.viewer.Location = new Point(275, 0);
        this.viewer.Size = new System.Drawing.Size(250, 30);
        this.viewer.FlatStyle = FlatStyle.Flat;
        this.viewer.Text = "Content View";
        this.viewer.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
        this.viewer.ForeColor = Color.White;
        this.viewer.BackColor = Color.Transparent;
        this.viewer.FlatAppearance.BorderSize = 0;

        this.btnBack.Click += new EventHandler(this.BtnBack_Click);

        this.textBoxContent.Location = new System.Drawing.Point(0, 50);
        this.textBoxContent.Multiline = true;
        this.textBoxContent.Name = "textBoxContent";
        this.textBoxContent.ReadOnly = true;
        this.textBoxContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.textBoxContent.Size = new System.Drawing.Size(800, 750);
        this.textBoxContent.Font = new Font("Arial", 13F, FontStyle.Bold);
        this.textBoxContent.ForeColor = Color.FromArgb(236, 240, 241);
        this.textBoxContent.BackColor = Color.FromArgb(2,8,28);
        this.textBoxContent.BorderStyle = BorderStyle.None;

        this.ClientSize = new System.Drawing.Size(800, 800);
        this.Controls.Add(this.textBoxContent);
        this.Controls.Add(this.panelTop);
        this.panelTop.Controls.Add(this.btnBack);
        this.panelTop.Controls.Add(this.viewer);
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(2,8,28);
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextViewerForm_FormClosing);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void BtnBack_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private System.Windows.Forms.TextBox textBoxContent;
    private System.Windows.Forms.Panel panelTop;
    private System.Windows.Forms.Button btnBack;
    private System.Windows.Forms.Button viewer;
}
