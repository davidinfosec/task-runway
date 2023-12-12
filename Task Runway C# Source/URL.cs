using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskRunway
{
    public partial class URL : Form
    {
        public URL()
        {
            InitializeComponent();
            this.AcceptButton = button3;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Disable resizing
            this.MaximizeBox = false; // Hide maximize button

            // Handle KeyDown event
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(URL_KeyDown);
        }

        private void URL_Load(object sender, EventArgs e)
        {
            // This event handler is called when the form loads.
            // You can add any additional initialization code here.
        }


        private void URL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        public string GetURL()
        {
            return textBox1.Text; // Assuming textBox1 is the name of the TextBox for URL input
        }

        public string EnteredURL { get; private set; }

        private void button3_Click(object sender, EventArgs e)
        {
            // Assuming validation and logic here
            EnteredURL = textBox1.Text;

            // Close the form with OK result after setting the URL
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void URL_Load_1(object sender, EventArgs e)
        {

        }
    }
}

