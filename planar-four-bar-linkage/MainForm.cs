using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace planar_four_bar_linkage
{
    public partial class MainForm : Form
    {
        PlanarFourBarLinkage myLinkage = new PlanarFourBarLinkage();
        List<PointF> pois = new List<PointF>();
        double currentAngle = 0;
        public MainForm()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = myLinkage;
            propertyGrid2.SelectedObject = myLinkage;
            object[] pars = { ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true };
            MethodInfo getFunction = typeof(Panel).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            getFunction.Invoke(splitContainer1.Panel2, pars);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double delta = myLinkage.Delta;
            richTextBox1.Clear();
            for (double angle = 0.0; angle < 6.28; angle += delta)
            {
                myLinkage.UpdateConfiguration(angle);
                richTextBox1.AppendText(myLinkage.ToString());
                richTextBox1.AppendText("\n");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            if (button2.Text == "Animate")
            {
                button2.Text = "Stop";
                button2.ForeColor = Color.Red;
            } else
            {
                button2.Text = "Animate";
                button2.ForeColor = Color.Black;
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            myLinkage.UpdateConfiguration(currentAngle);
            myLinkage.DrawLinkage(e.Graphics, splitContainer1.Panel2.ClientRectangle);

            pois.Add(myLinkage.GetPp());
            Rectangle rect = new Rectangle(0, 0, 6, 6);
            foreach (PointF point in pois)
            {
                rect.X = (int)(point.X - 3);
                rect.Y = (int)(point.Y - 3);
                e.Graphics.DrawRectangle(Pens.Black, rect);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = myLinkage.TimerInterval;
            currentAngle += myLinkage.Delta;
            currentAngle = currentAngle >= 6.28 ? 0 : currentAngle;
            splitContainer1.Panel2.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pois.Clear();
            splitContainer1.Panel2.Refresh();
        }
    }
}
