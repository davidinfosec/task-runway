using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace TaskRunway
{
    public partial class PathForm : Form
    {
        public string EnteredPath { get; private set; }

        public PathForm()
        {
            InitializeComponent();
            this.AcceptButton = buttonSave;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Disable resizing
            this.MaximizeBox = false; // Hide maximize button

            // Handle KeyDown event
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Path_KeyDown);
        }

        private void Path_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        public System.Windows.Forms.TextBox GetPathTextBox()
        {
            return textBoxPath; // Access to the TextBox for path input
        }


        public string GetPath()
        {
            return textBoxPath.Text; // Assuming textBoxPath is the name of the TextBox for path input
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Assuming validation and logic here
            EnteredPath = textBoxPath.Text;

            // Close the form with OK result after setting the path
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void PathForm_Load(object sender, EventArgs e)
        {

        }
    }
}