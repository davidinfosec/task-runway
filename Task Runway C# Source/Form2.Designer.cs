namespace TaskRunway
{
    partial class TaskRunwayExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskRunwayExplorer));
            label2 = new Label();
            label4 = new Label();
            checkedListBox1 = new CheckedListBox();
            label1 = new Label();
            label3 = new Label();
            checkedListBox2 = new CheckedListBox();
            button1 = new Button();
            checkedListBox3 = new CheckedListBox();
            label6 = new Label();
            button2 = new Button();
            SuspendLayout();
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Bold);
            label2.ImageAlign = ContentAlignment.MiddleLeft;
            label2.Location = new Point(19, 9);
            label2.Name = "label2";
            label2.Size = new Size(458, 25);
            label2.TabIndex = 15;
            label2.Text = "Downloads / Website Explorer";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.Font = new Font("Microsoft JhengHei", 7F, FontStyle.Bold);
            label4.Location = new Point(19, 34);
            label4.Margin = new Padding(10, 0, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(458, 18);
            label4.TabIndex = 17;
            label4.Text = "Download tools from external sources, straight into Task Runway (Source: GitHub)";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(19, 80);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(142, 148);
            checkedListBox1.TabIndex = 18;
            // 
            // label1
            // 
            label1.Font = new Font("Microsoft JhengHei", 7F, FontStyle.Bold);
            label1.Location = new Point(19, 59);
            label1.Margin = new Padding(10, 0, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(216, 18);
            label1.TabIndex = 19;
            label1.Text = "Add Programs/Scripts:";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.Font = new Font("Microsoft JhengHei", 7F, FontStyle.Bold);
            label3.Location = new Point(167, 59);
            label3.Margin = new Padding(10, 0, 3, 0);
            label3.Name = "label3";
            label3.Size = new Size(143, 18);
            label3.TabIndex = 21;
            label3.Text = "Add Websites:";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // checkedListBox2
            // 
            checkedListBox2.FormattingEnabled = true;
            checkedListBox2.Location = new Point(167, 80);
            checkedListBox2.Name = "checkedListBox2";
            checkedListBox2.Size = new Size(143, 148);
            checkedListBox2.TabIndex = 20;
            // 
            // button1
            // 
            button1.Location = new Point(20, 252);
            button1.Name = "button1";
            button1.Size = new Size(290, 28);
            button1.TabIndex = 22;
            button1.Text = "Download / Add";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // checkedListBox3
            // 
            checkedListBox3.FormattingEnabled = true;
            checkedListBox3.Location = new Point(334, 80);
            checkedListBox3.Name = "checkedListBox3";
            checkedListBox3.Size = new Size(143, 166);
            checkedListBox3.TabIndex = 23;
            // 
            // label6
            // 
            label6.Font = new Font("Microsoft JhengHei", 7F, FontStyle.Bold);
            label6.Location = new Point(334, 59);
            label6.Margin = new Padding(10, 0, 3, 0);
            label6.Name = "label6";
            label6.Size = new Size(143, 18);
            label6.TabIndex = 25;
            label6.Text = "Uninstall Programs/Scripts:";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button2.Location = new Point(334, 252);
            button2.Name = "button2";
            button2.Size = new Size(143, 28);
            button2.TabIndex = 26;
            button2.Text = "Uninstall";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // TaskRunwayExplorer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(497, 292);
            Controls.Add(button2);
            Controls.Add(label6);
            Controls.Add(checkedListBox3);
            Controls.Add(button1);
            Controls.Add(label3);
            Controls.Add(checkedListBox2);
            Controls.Add(label1);
            Controls.Add(checkedListBox1);
            Controls.Add(label4);
            Controls.Add(label2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "TaskRunwayExplorer";
            Text = "Task Runway - Download Explorer";
            ResumeLayout(false);
        }

        #endregion

        private Label label2;
        private Label label4;
        private CheckedListBox checkedListBox1;
        private Label label1;
        private Label label3;
        private CheckedListBox checkedListBox2;
        private Button button1;
        private CheckedListBox checkedListBox3;
        private Label label6;
        private Button button2;
        private Label label5;
    }
}