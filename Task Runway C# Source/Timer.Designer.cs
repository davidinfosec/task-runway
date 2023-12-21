namespace TaskRunway
{
    partial class Timer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Timer));
            label4 = new Label();
            label2 = new Label();
            numericUpDown1 = new NumericUpDown();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // label4
            // 
            label4.Font = new Font("Calibri", 9F, FontStyle.Bold);
            label4.ForeColor = SystemColors.ActiveCaptionText;
            label4.Location = new Point(64, 34);
            label4.Margin = new Padding(10, 0, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(215, 18);
            label4.TabIndex = 18;
            label4.Text = "Take flight into your favorite tools.";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Calibri", 15F, FontStyle.Bold);
            label2.ForeColor = SystemColors.ActiveCaptionText;
            label2.Location = new Point(39, 9);
            label2.Name = "label2";
            label2.Size = new Size(265, 24);
            label2.TabIndex = 17;
            label2.Text = "Task Runway Pomodoro Timer";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // numericUpDown1
            // 
            numericUpDown1.BorderStyle = BorderStyle.None;
            numericUpDown1.Font = new Font("Segoe UI", 30F);
            numericUpDown1.Location = new Point(64, 67);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(99, 57);
            numericUpDown1.TabIndex = 19;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Calibri", 25F, FontStyle.Bold);
            label1.ForeColor = SystemColors.ActiveCaptionText;
            label1.Location = new Point(169, 76);
            label1.Name = "label1";
            label1.Size = new Size(135, 41);
            label1.TabIndex = 20;
            label1.Text = "Minutes";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Timer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(347, 146);
            Controls.Add(label1);
            Controls.Add(numericUpDown1);
            Controls.Add(label4);
            Controls.Add(label2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Timer";
            Text = "Form3";
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label4;
        private Label label2;
        private NumericUpDown numericUpDown1;
        private Label label1;
    }
}