using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sessao2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

      

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Start Recording....");
           
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Stop Recording....");
        }

        private void isel_Click(object sender, EventArgs e)
        {
            textbox.Text = ((Button)sender).Text;
        }

        private void prompt_Click(object sender, EventArgs e)
        {
            textbox.Text = ((Button)sender).Text;
        }

       
    }
}
