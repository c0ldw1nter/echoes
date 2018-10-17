namespace Echoes
{
    partial class EnterTextForm
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
            this.modifiedButton1 = new ModifiedControls.ModifiedButton();
            this.enteredText = new System.Windows.Forms.TextBox();
            this.modifiedButton2 = new ModifiedControls.ModifiedButton();
            this.SuspendLayout();
            // 
            // modifiedButton1
            // 
            this.modifiedButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.modifiedButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.modifiedButton1.Font = new System.Drawing.Font("White Rabbit", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modifiedButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.modifiedButton1.Location = new System.Drawing.Point(12, 37);
            this.modifiedButton1.Margin = new System.Windows.Forms.Padding(2);
            this.modifiedButton1.Name = "modifiedButton1";
            this.modifiedButton1.Size = new System.Drawing.Size(63, 25);
            this.modifiedButton1.TabIndex = 25;
            this.modifiedButton1.Text = "OK";
            this.modifiedButton1.UseVisualStyleBackColor = false;
            this.modifiedButton1.Click += new System.EventHandler(this.modifiedButton1_Click);
            // 
            // enteredText
            // 
            this.enteredText.Location = new System.Drawing.Point(12, 12);
            this.enteredText.Name = "enteredText";
            this.enteredText.Size = new System.Drawing.Size(260, 20);
            this.enteredText.TabIndex = 26;
            // 
            // modifiedButton2
            // 
            this.modifiedButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.modifiedButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.modifiedButton2.Font = new System.Drawing.Font("White Rabbit", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modifiedButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.modifiedButton2.Location = new System.Drawing.Point(209, 37);
            this.modifiedButton2.Margin = new System.Windows.Forms.Padding(2);
            this.modifiedButton2.Name = "modifiedButton2";
            this.modifiedButton2.Size = new System.Drawing.Size(63, 25);
            this.modifiedButton2.TabIndex = 27;
            this.modifiedButton2.Text = "Cancel";
            this.modifiedButton2.UseVisualStyleBackColor = false;
            this.modifiedButton2.Click += new System.EventHandler(this.modifiedButton2_Click);
            // 
            // EnterNameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 68);
            this.ControlBox = false;
            this.Controls.Add(this.modifiedButton2);
            this.Controls.Add(this.enteredText);
            this.Controls.Add(this.modifiedButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "EnterNameForm";
            this.Text = "Enter Name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ModifiedControls.ModifiedButton modifiedButton1;
        public System.Windows.Forms.TextBox enteredText;
        private ModifiedControls.ModifiedButton modifiedButton2;
    }
}