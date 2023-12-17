namespace TaskRunway
{
    partial class Update
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Update));
            label2 = new Label();
            button2 = new Button();
            button3 = new Button();
            button1 = new Button();
            label4 = new Label();
            SuspendLayout();
            // 
            // label2
            // 
            label2.Font = new Font("Microsoft YaHei", 10F, FontStyle.Bold);
            label2.Location = new Point(45, 9);
            label2.Name = "label2";
            label2.Size = new Size(258, 25);
            label2.TabIndex = 15;
            label2.Text = "Task Runway Update Available";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            button2.Location = new Point(127, 78);
            button2.Name = "button2";
            button2.Size = new Size(95, 27);
            button2.TabIndex = 17;
            button2.Text = "Update Now";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(235, 78);
            button3.Name = "button3";
            button3.Size = new Size(95, 27);
            button3.TabIndex = 18;
            button3.Text = "Cancel";
            button3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(18, 78);
            button1.Name = "button1";
            button1.Size = new Size(95, 27);
            button1.TabIndex = 20;
            button1.Text = "View Changes";
            button1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.Font = new Font("Microsoft JhengHei", 7F, FontStyle.Bold);
            label4.Location = new Point(19, 45);
            label4.Margin = new Padding(10, 0, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(311, 18);
            label4.TabIndex = 21;
            label4.Text = "An update is available! Your current version: 1.0.4.2";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            label4.Click += label4_Click;
            // 
            // Update
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(351, 122);
            Controls.Add(label4);
            Controls.Add(button1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Update";
            Text = "Check for Updates";
            ResumeLayout(false);
        }
    }
}

#endregion // Make sure this is present