namespace TaskRunway
{
    partial class quickActions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(quickActions));
            label2 = new Label();
            listBox1 = new ListBox();
            listBox2 = new ListBox();
            button6 = new Button();
            button3 = new Button();
            label4 = new Label();
            SuspendLayout();
            // 
            // label2
            // 
            label2.Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Bold);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(517, 25);
            label2.TabIndex = 15;
            label2.Text = "Quick Actions Configuration Menu";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(12, 65);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(190, 184);
            listBox1.TabIndex = 16;
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(339, 65);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(190, 184);
            listBox2.TabIndex = 17;
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            // 
            // button6
            // 
            button6.Location = new Point(272, 135);
            button6.Name = "button6";
            button6.Size = new Size(49, 24);
            button6.TabIndex = 19;
            button6.Text = "-";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click_1;
            // 
            // button3
            // 
            button3.Location = new Point(217, 135);
            button3.Name = "button3";
            button3.Size = new Size(49, 24);
            button3.TabIndex = 18;
            button3.Text = "+";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label4
            // 
            label4.Font = new Font("Microsoft JhengHei", 7F, FontStyle.Bold);
            label4.Location = new Point(80, 34);
            label4.Margin = new Padding(10, 0, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(380, 18);
            label4.TabIndex = 20;
            label4.Text = "Add items to your quick action tray menu by using the buttons below.";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // quickActions
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(542, 262);
            Controls.Add(label4);
            Controls.Add(button6);
            Controls.Add(button3);
            Controls.Add(listBox2);
            Controls.Add(listBox1);
            Controls.Add(label2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "quickActions";
            Text = "Task Runway - Quick Actions";
            Load += quickActions_Load;
            ResumeLayout(false);
        }

        #endregion

        private Label label2;
        private ListBox listBox1;
        private ListBox listBox2;
        private Button button6;
        private Button button3;
        private Label label4;
    }
}