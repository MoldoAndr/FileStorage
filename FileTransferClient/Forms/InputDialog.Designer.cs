namespace FileTransferClient.Forms
{
    partial class InputDialog
    {
        private System.ComponentModel.IContainer components = null;
        private Label labelPrompt;
        private TextBox textBoxInput;
        private Button buttonOK;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.labelPrompt = new System.Windows.Forms.Label();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.labelPrompt.AutoSize = true;
            this.labelPrompt.Font = new Font("Times New Roman", 14F, FontStyle.Bold);
            this.labelPrompt.ForeColor = Color.FromArgb(236, 240, 241);
            this.labelPrompt.Location = new System.Drawing.Point(12, 20);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(70, 21);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Prompt";

            this.textBoxInput.Font = new Font("Times New Roman", 14F, FontStyle.Bold);
            this.textBoxInput.ForeColor = Color.White;
            this.textBoxInput.BackColor = Color.FromArgb(22, 33, 62);
            this.textBoxInput.Location = new System.Drawing.Point(16, 50);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(360, 29);
            this.textBoxInput.TabIndex = 1;

            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Font = new Font("Times New Roman", 14F, FontStyle.Bold);
            this.buttonOK.ForeColor = Color.FromArgb(236, 240, 241);
            this.buttonOK.BackColor = Color.FromArgb(17, 10, 115);
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOK.Location = new System.Drawing.Point(301, 85);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 30);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 150);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.labelPrompt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputDialog";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
