namespace VirtualChat
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sendText = new System.Windows.Forms.Button();
            this.message = new System.Windows.Forms.TextBox();
            this.listMessages = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listFile = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.UploadFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sendText
            // 
            this.sendText.Location = new System.Drawing.Point(405, 190);
            this.sendText.Name = "sendText";
            this.sendText.Size = new System.Drawing.Size(108, 24);
            this.sendText.TabIndex = 0;
            this.sendText.Text = "שליחת הודעה";
            this.sendText.UseVisualStyleBackColor = true;
            this.sendText.Click += new System.EventHandler(this.SendText_Click);
            // 
            // message
            // 
            this.message.Location = new System.Drawing.Point(405, 141);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(108, 20);
            this.message.TabIndex = 1;
            // 
            // listMessages
            // 
            this.listMessages.Location = new System.Drawing.Point(50, 93);
            this.listMessages.Multiline = true;
            this.listMessages.Name = "listMessages";
            this.listMessages.Size = new System.Drawing.Size(164, 124);
            this.listMessages.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(183, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "צ\'אט";
            // 
            // listFile
            // 
            this.listFile.FormattingEnabled = true;
            this.listFile.Location = new System.Drawing.Point(248, 93);
            this.listFile.MultiColumn = true;
            this.listFile.Name = "listFile";
            this.listFile.Size = new System.Drawing.Size(143, 121);
            this.listFile.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(356, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "קבצים";
            // 
            // UploadFile
            // 
            this.UploadFile.Location = new System.Drawing.Point(406, 93);
            this.UploadFile.Name = "UploadFile";
            this.UploadFile.Size = new System.Drawing.Size(107, 22);
            this.UploadFile.TabIndex = 6;
            this.UploadFile.Text = "שליחת קובץ";
            this.UploadFile.UseVisualStyleBackColor = true;
            this.UploadFile.Click += new System.EventHandler(this.UploadFile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 270);
            this.Controls.Add(this.UploadFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listMessages);
            this.Controls.Add(this.message);
            this.Controls.Add(this.sendText);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendText;
        private System.Windows.Forms.TextBox message;
        private System.Windows.Forms.TextBox listMessages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button UploadFile;
    }
}

