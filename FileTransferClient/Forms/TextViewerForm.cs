using System;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;

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
            Console.WriteLine("Error: " + ex.Message);
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
        this.SuspendLayout();
        this.textBoxContent.Dock = System.Windows.Forms.DockStyle.Fill;
        this.textBoxContent.Location = new System.Drawing.Point(0, 0);
        this.textBoxContent.Multiline = true;
        this.textBoxContent.Name = "textBoxContent";
        this.textBoxContent.ReadOnly = true;
        this.textBoxContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.textBoxContent.Size = new System.Drawing.Size(800, 450);
        this.textBoxContent.TabIndex = 0;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.textBoxContent);
        this.Name = "TextViewerForm";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextViewerForm_FormClosing);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.TextBox textBoxContent;
}
