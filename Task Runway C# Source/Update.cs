using System;
using System.Drawing;
using System.Windows.Forms;

namespace TaskRunway
{
    public partial class Update : Form
    {
        // Define the actions to be triggered by button clicks
        public Action OnViewChanges { get; set; }
        public Action OnUpdateNow { get; set; }

        // Button declarations
        private Button button1;
        private Button button2;
        private Button button3;
        private Label label2;
        private Label label4;

        public Update()
        {
            InitializeComponent();

            // Attach event handlers to the buttons
            button1.Click += (sender, e) => OnViewChanges?.Invoke();
            button2.Click += (sender, e) => OnUpdateNow?.Invoke();
            button3.Click += (sender, e) => this.Close();
        }
    }
}
