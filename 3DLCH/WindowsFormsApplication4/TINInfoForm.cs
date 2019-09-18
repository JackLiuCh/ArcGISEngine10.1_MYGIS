using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication4
{
    public partial class TINInfoForm : Form
    {
        public TINInfoForm()
        {
            InitializeComponent();
        }
        public string TextBox_X
        {
            get
            {
                return textBox_X.Text;
            }
            set
            {
                textBox_X.Text = value;
            }
        }
        public string TextBox_Y
        {
            get
            {
                return textBox_Y.Text;
            }
            set
            {
                textBox_Y.Text = value;
            }
        }
        public string TextBox_Elevation
        {
            get
            {
                return textBox_Elevation.Text;
            }
            set
            {
                textBox_Elevation.Text = value;
            }
        }
        public string TextBox_Slope
        {
            get
            {
                return textBox_Slope.Text;
            }
            set
            {
                textBox_Slope.Text = value;
            }
        }
        public string TextBox_Aspect
        {
            get
            {
                return textBox_Aspect.Text;
            }
            set
            {
                textBox_Aspect.Text = value;
            }
        }
    }
}
