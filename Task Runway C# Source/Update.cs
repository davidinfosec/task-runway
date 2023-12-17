using System;
using System.Drawing;
using System.Windows.Forms;

namespace TaskRunway
{
    public partial class Update : Form
    {
        public Action OnViewChanges { get; set; }
        public Action OnUpdateNow { get; set; }

        private Button button1;
        private Button button2;
        private Button button3;
        private Label label2;
        private Label label4;

        public string VersionText
        {
            set { label4.Text = $"Version: {value}"; }
        }

        public Update()
        {
            InitializeComponent();

            // Make window fixed size and hide maximize button
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Attach event handlers to the buttons
            button1.Click += (sender, e) => OnViewChanges?.Invoke();
            button2.Click += (sender, e) => OnUpdateNow?.Invoke();
            button3.Click += (sender, e) => this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Event handler for label4 click
        }
    }
}
