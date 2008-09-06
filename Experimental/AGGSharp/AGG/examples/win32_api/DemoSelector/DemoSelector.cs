using System;
using System.Windows.Forms;
using NPack.Interfaces;

namespace DemoSelector
{
    public partial class DemoSelector<T> : Form
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public DemoSelector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AGG.lion_application<T>.StartDemo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AGG.image1_application<T>.StartDemo();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AGG.rounded_rect_application<T>.StartDemo();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AGG.component_rendering_application<T>.StartDemo();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AGG.blur_application<T>.StartDemo();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AGG.perspective_application<T>.StartDemo();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AGG.lion_outline_application<T>.StartDemo();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            AGG.gouraud_application<T>.StartDemo();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            AGG.image_filters_application<T>.StartDemo();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            AGG.image_resample_application<T>.StartDemo();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            AGG.alpha_mask2_application<T>.StartDemo();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            AGG.NeHeLesson5<T>.StartDemo();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            AGG.GradientsApplication<T>.StartDemo();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            RockBlaster.RockBlasterGame<T>.StartDemo();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            SmartSweeper.SmartSweeperApplication<T>.StartDemo();
        }
    }
}
