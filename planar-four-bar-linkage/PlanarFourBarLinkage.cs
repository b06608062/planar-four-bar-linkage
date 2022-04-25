using System;
using System.ComponentModel;
using System.Drawing;

namespace planar_four_bar_linkage
{
    internal class PlanarFourBarLinkage
    {
        double Lg = 20.0, Ld = 10.0, Lc = 35.0, Lf = 25.0;
        double penLength = 17.5, penAngle = 0.2, delta = 0.1;
        double alpha = 1.0;
        double scale;
        int offsetX, offsetY;
        int timerInterval = 100;
        PointF P1 = new PointF(0, 0), P2 = new PointF(0, 0), Pc = new PointF(0, 0), Pf = new PointF(0, 0), Pp = new PointF(0, 0);

        public PlanarFourBarLinkage()
        {
            UpdateConfiguration(alpha);
        }

        public override string ToString()
        {
            return $"Angle = {alpha:0.00}, P1 = ({P1.X}, {P1.Y}), P2 = ({P2.X}, {P2.Y}), Pc = ({Pc.X:0.00}, {Pc.Y:0.00}), Pf = ({Pf.X:0.00}, {Pf.Y:0.00}), Pp = ({Pp.X:0.00}, {Pp.Y:0.00})";
        }

        [Category("四連桿")]
        [DisplayName("GroundLength")]
        [Description("The length of the ground")]
        public double GroundLength
        {
            get => Lg;
            set
            {
                double tmp = Lg;
                Lg = value;
                if (!isQuadrilateralFeasible() || !isLegal())
                    Lg = tmp;
            }
        }

        [Category("四連桿")]
        [DisplayName("DriverLength")]
        [Description("The length of the driver")]
        public double DriverLength
        {
            get => Ld;
            set
            {
                double tmp = Ld;
                Ld = value;
                if (!isQuadrilateralFeasible() || !isLegal())
                    Ld = tmp;
            }
        }

        [Category("四連桿")]
        [DisplayName("ConnecterLength")]
        [Description("The length of the connector")]
        public double ConnecterLength
        {
            get => Lc;
            set
            {
                double tmp = Lc;
                Lc = value;
                if (!isQuadrilateralFeasible() || !isLegal())
                    Lc = tmp;
            }
        }

        [Category("四連桿")]
        [DisplayName("FollowerLength")]
        [Description("The length of the follower")]
        public double FollowerLength
        {
            get => Lf;
            set
            {
                double tmp = Lf;
                Lf = value;
                if (!isQuadrilateralFeasible() || !isLegal())
                    Lf = tmp;
            }
        }

        [Category("畫筆")] 
        [DisplayName("PenLength")]
        [Description("The length of the pen")]
        public double PenLength
        {
            get => penLength;
            set
            {
                penLength = value;
            }
        }

        [Category("畫筆")]
        [DisplayName("PenAngle(rad)")]
        [Description("The angle of the pen")]
        public double PenAngle
        {
            get => penAngle;
            set
            {
                penAngle = value;
            }
        }

        [Category("轉速與轉角")]
        [DisplayName("Delta(rad)")]
        [Description("The rotate angle")]
        public double Delta
        {
            get => delta;
            set
            {
                if (value > 0 && value <= 1)
                    delta = value;
            }
        }

        [Category("轉速與轉角")]
        [DisplayName("Interval(ms)")]
        [Description("The rotate speed")]
        public int TimerInterval
        {
            get => timerInterval;
            set
            {
                if (value > 25 && value < 200)
                    timerInterval = value;
            }
        }

        private bool isQuadrilateralFeasible()
        {
            if (Lg > Ld + Lc + Lf || Ld > Lg + Lc + Lf || Lc > Lg + Ld + Lf || Lf > Lg + Ld + Lc) return false;

            return true;
        }

        public void UpdateConfiguration(double newAlpha)
        {
            alpha = newAlpha;

            P2.X = (float)Lg;
            P2.Y = 0.0f;
            Pc.X = (float)(Ld * Math.Cos(alpha));
            Pc.Y = (float)(Ld * Math.Sin(alpha));

            double L = Math.Sqrt(Math.Pow(Lg - Pc.X, 2) + Pc.Y * Pc.Y);
            double omega = L == 0 ? 0 : Math.Acos((Lf * Lf + L * L - Lc * Lc) / (2 * Lf * L));
            double theta = Math.Atan2(Pc.Y, Pc.X - Lg) - omega;

            Pf.X = (float)(Lf * Math.Cos(theta) + Lg);
            Pf.Y = (float)(Lf * Math.Sin(theta));

            float unitX = (float)((Pf.X - Pc.X) / Lc);
            float unitY = (float)((Pf.Y - Pc.Y) / Lc);
            Pp.X = Pc.X + (float)((unitX * Math.Cos(penAngle) - unitY * Math.Sin(penAngle)) * penLength);
            Pp.Y = Pc.Y + (float)((unitX * Math.Sin(penAngle) + unitY * Math.Cos(penAngle)) * penLength);
        }

        public void DrawLinkage(Graphics g, Rectangle bound)
        {
            scale = bound.Width / (Lc + Lf + Lg);
            offsetY = bound.Height / 2;
            offsetX = bound.Width / 3;
            PointF S1, S2, Sc, Sf, Sp;
            S1 = Transform(P1);
            S2 = Transform(P2);
            Sc = Transform(Pc);
            Sf = Transform(Pf);
            Sp = Transform(Pp);

            g.FillRectangle(Brushes.Gray, S1.X - 20, S1.Y - 20, 40, 40);
            g.FillRectangle(Brushes.Gray, S2.X - 20, S2.Y - 20, 40, 40);

            Pen myPen1 = new Pen(Color.Red, 20)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round
            };

            Pen myPen2 = new Pen(Color.Black, 15)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor
            };

            Pen myPen3 = new Pen(Color.Red, 1);

            g.DrawLine(myPen1, S1, Sc);
            myPen1.Color = Color.Green;
            g.DrawLine(myPen1, Sc, Sf);
            myPen1.Color = Color.Blue;
            g.DrawLine(myPen1, S2, Sf);
            g.DrawLine(myPen2, Sc, Sp);
            g.DrawLine(myPen3, S1, S2);

            g.FillEllipse(Brushes.White, S1.X - 5, S1.Y - 5, 10, 10);
            g.FillEllipse(Brushes.White, S2.X - 5, S2.Y - 5, 10, 10);
            g.FillEllipse(Brushes.White, Sc.X - 5, Sc.Y - 5, 10, 10);
            g.FillEllipse(Brushes.White, Sf.X - 5, Sf.Y - 5, 10, 10);
        }

        private PointF Transform(PointF point)
        {
            return new PointF((float)(offsetX + point.X * scale), (float)(offsetY + point.Y * scale));
        }

        public PointF GetPp()
        {
            return Transform(Pp);
        }

        private bool isLegal()
        {
            PointF testPc = new PointF();
            for (double angle = 0.0; angle < 6.28; angle += delta)
            {
                testPc.X = (float)(Ld * Math.Cos(angle));
                testPc.Y = (float)(Ld * Math.Sin(angle));
                double L = Math.Sqrt(Math.Pow(Lg - testPc.X, 2) + testPc.Y * testPc.Y);
                if (Lf + Lc < L || L + Lc < Lf || L + Lf < Lc) return false;
            }
            return true;
        }
    }
}
