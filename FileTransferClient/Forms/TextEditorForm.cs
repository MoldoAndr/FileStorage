using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

public partial class TextEditorForm : Form
{
    private readonly BinaryReader Reader;
    private readonly BinaryWriter Writer;
    private readonly string fileName;

    public TextEditorForm(string fileName, string content, BinaryReader reader, BinaryWriter writer)
    {
        InitializeComponent();
        this.Reader = reader;
        this.Writer = writer;
        this.fileName = fileName;

        textBoxContent.Text = content;
    }

    private void SaveFileContent()
    {
        try
        {
            Writer.Write("SAVE");
            Writer.Write(fileName);
            byte[] fileContentBytes = Encoding.UTF8.GetBytes(textBoxContent.Text);
            Writer.Write(fileContentBytes.Length);
            Writer.Write(fileContentBytes);
            Writer.Flush();

            bool saveSuccess = Reader.ReadBoolean();
            if (saveSuccess)
            {

                ; ; 
            }
            else
            {
                MessageBox.Show("Failed to save the file.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }

    private void TextEditorForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        SaveFileContent();
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        SaveFileContent();
    }

    private void InitializeComponent()
    {
        this.textBoxContent = new System.Windows.Forms.TextBox();
        this.panelTop = new System.Windows.Forms.Panel();
        this.btnSave = new System.Windows.Forms.Button();
        this.btnBack = new System.Windows.Forms.Button();
        this.viewer = new System.Windows.Forms.Button();
        this.SuspendLayout();

        this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
        this.panelTop.Height = 30;
        this.panelTop.BackColor = Color.FromArgb(2, 8, 28);

        this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
        this.btnSave.Size = new System.Drawing.Size(150, 30);
        this.btnSave.FlatStyle = FlatStyle.Flat;
        this.btnSave.Text = "Save";
        this.btnSave.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
        this.btnSave.ForeColor = Color.White;
        this.btnSave.BackColor = Color.Transparent;
        this.btnSave.Click += new EventHandler(this.BtnSave_Click);

        this.btnBack.Dock = System.Windows.Forms.DockStyle.Left;
        this.btnBack.Size = new System.Drawing.Size(150, 30);
        this.btnBack.FlatStyle = FlatStyle.Flat;
        this.btnBack.Text = "Back";
        this.btnBack.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
        this.btnBack.ForeColor = Color.White;
        this.btnBack.BackColor = Color.Transparent;
        this.btnBack.Click += new EventHandler(this.BtnBack_Click);

        this.viewer.Location = new Point(275, 0);
        this.viewer.Size = new System.Drawing.Size(250, 30);
        this.viewer.FlatStyle = FlatStyle.Flat;
        this.viewer.Text = "Content Modify";
        this.viewer.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
        this.viewer.ForeColor = Color.White;
        this.viewer.BackColor = Color.Transparent;
        this.viewer.FlatAppearance.BorderSize = 0;

        this.textBoxContent.Location = new System.Drawing.Point(0, 60);
        this.textBoxContent.Multiline = true;
        this.textBoxContent.Name = "textBoxContent";
        this.textBoxContent.ReadOnly = false;
        this.textBoxContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.textBoxContent.Size = new System.Drawing.Size(800, 740);
        this.textBoxContent.Font = new Font("Arial", 13F, FontStyle.Bold);
        this.textBoxContent.ForeColor = Color.FromArgb(236, 240, 241);
        this.textBoxContent.BackColor = Color.FromArgb(2, 8, 28);
        this.textBoxContent.BorderStyle = BorderStyle.None;

        this.ClientSize = new System.Drawing.Size(800, 800);
        this.Controls.Add(this.textBoxContent);
        this.Controls.Add(this.panelTop);
        this.panelTop.Controls.Add(this.btnSave);
        this.panelTop.Controls.Add(this.btnBack);
        this.panelTop.Controls.Add(this.viewer);
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(2, 8, 28);
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextEditorForm_FormClosing);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void BtnBack_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private System.Windows.Forms.TextBox textBoxContent;
    private System.Windows.Forms.Panel panelTop;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnBack;
    private System.Windows.Forms.Button viewer;
}
