using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskRunway;

namespace TaskRunway
{
    public partial class Timer : Form
    {
        private int countdownValue;
        public int SelectedCountdownTime { get; set; }
        public Timer()
        {
            InitializeComponent();
        }


        public int GetCountdownValue()
        {
            return countdownValue;
        }

        public void StartTimer(int selectedTimeMinutes)
        {
            countdownValue = selectedTimeMinutes;
        }
        }
}

