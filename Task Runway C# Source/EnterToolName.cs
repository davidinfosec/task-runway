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
    public partial class EnterToolName : Form
    {
        public EnterToolName()
        {
            InitializeComponent();
            this.AcceptButton = button3;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(EnterToolName_KeyDown);
        }

        public EnterToolName(string defaultName) : this()
        {
            textBox1.Text = defaultName; // Set default tool name
        }


        private void EnterToolName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Retrieve the entered tool name from the textbox
            // You might want to add validation or trimming here if necessary
            string enteredName = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredName))
            {
                MessageBox.Show("Please enter a valid tool name.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Set DialogResult to OK to close the form and return success
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        public string GetToolName()
        {
            return textBox1.Text; // Assuming textBox1 is the name of the TextBox for tool name input
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
