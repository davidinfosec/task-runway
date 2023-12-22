namespace TaskRunway
{
    partial class PathForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathForm));
            buttonSave = new Button();
            textBoxPath = new TextBox();
            labelPath = new Label();
            SuspendLayout();
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(209, 39);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(101, 24);
            buttonSave.TabIndex = 22;
            buttonSave.Text = "Save Path";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // textBoxPath
            // 
            textBoxPath.Location = new Point(29, 40);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.Size = new Size(151, 23);
            textBoxPath.TabIndex = 21;
            textBoxPath.TextChanged += textBoxPath_TextChanged;
            // 
            // labelPath
            // 
            labelPath.Font = new Font("Microsoft YaHei", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelPath.Location = new Point(12, 13);
            labelPath.Name = "labelPath";
            labelPath.Size = new Size(327, 25);
            labelPath.TabIndex = 20;
            labelPath.Text = "Enter Path:";
            labelPath.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PathForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(351, 88);
            Controls.Add(buttonSave);
            Controls.Add(textBoxPath);
            Controls.Add(labelPath);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PathForm";
            Text = "Edit Path";
            Load += PathForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonSave;
        private TextBox textBoxPath;
        private Label labelPath;
    }
}
