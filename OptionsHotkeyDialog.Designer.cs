namespace Echoes
{
    partial class OptionsHotkeyDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsHotkeyDialog));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.funcCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.enabledCheck = new System.Windows.Forms.CheckBox();
            this.ctrlCheck = new System.Windows.Forms.CheckBox();
            this.altCheck = new System.Windows.Forms.CheckBox();
            this.shiftCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(160, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 121);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // funcCombo
            // 
            this.funcCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.funcCombo.FormattingEnabled = true;
            this.funcCombo.Location = new System.Drawing.Point(66, 12);
            this.funcCombo.Name = "funcCombo";
            this.funcCombo.Size = new System.Drawing.Size(169, 21);
            this.funcCombo.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Function:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mod:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Key:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Enabled:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(66, 68);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(60, 20);
            this.textBox1.TabIndex = 11;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // enabledCheck
            // 
            this.enabledCheck.AutoSize = true;
            this.enabledCheck.Location = new System.Drawing.Point(66, 96);
            this.enabledCheck.Name = "enabledCheck";
            this.enabledCheck.Size = new System.Drawing.Size(15, 14);
            this.enabledCheck.TabIndex = 12;
            this.enabledCheck.UseVisualStyleBackColor = true;
            // 
            // ctrlCheck
            // 
            this.ctrlCheck.AutoSize = true;
            this.ctrlCheck.Location = new System.Drawing.Point(66, 41);
            this.ctrlCheck.Name = "ctrlCheck";
            this.ctrlCheck.Size = new System.Drawing.Size(54, 17);
            this.ctrlCheck.TabIndex = 13;
            this.ctrlCheck.Text = "CTRL";
            this.ctrlCheck.UseVisualStyleBackColor = true;
            // 
            // altCheck
            // 
            this.altCheck.AutoSize = true;
            this.altCheck.Location = new System.Drawing.Point(126, 41);
            this.altCheck.Name = "altCheck";
            this.altCheck.Size = new System.Drawing.Size(46, 17);
            this.altCheck.TabIndex = 14;
            this.altCheck.Text = "ALT";
            this.altCheck.UseVisualStyleBackColor = true;
            // 
            // shiftCheck
            // 
            this.shiftCheck.AutoSize = true;
            this.shiftCheck.Location = new System.Drawing.Point(178, 41);
            this.shiftCheck.Name = "shiftCheck";
            this.shiftCheck.Size = new System.Drawing.Size(57, 17);
            this.shiftCheck.TabIndex = 15;
            this.shiftCheck.Text = "SHIFT";
            this.shiftCheck.UseVisualStyleBackColor = true;
            // 
            // OptionsHotkeyDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 152);
            this.Controls.Add(this.shiftCheck);
            this.Controls.Add(this.altCheck);
            this.Controls.Add(this.ctrlCheck);
            this.Controls.Add(this.enabledCheck);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.funcCombo);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OptionsHotkeyDialog";
            this.Text = "Hotkey";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.ComboBox funcCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.CheckBox enabledCheck;
        private System.Windows.Forms.CheckBox ctrlCheck;
        private System.Windows.Forms.CheckBox altCheck;
        private System.Windows.Forms.CheckBox shiftCheck;
    }
}