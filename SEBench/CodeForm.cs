using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEBench
{
    public partial class CodeForm : Form
    {
        public CodeForm(string code, string className = null)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(className))
                this.Text = className + " - " + this.Text;

            richTextBox1.Text = code;
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressed(e);
        }

        private void CodeForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressed(e);
        }

        private void KeyPressed(KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                this.Close();
        }
    }
}
