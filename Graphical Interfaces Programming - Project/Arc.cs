using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical_Interfaces_Programming___Project
{
    internal class Arc
    {
        Rectangle rect;
        float startAngle, sweepAngle;

        public Arc(Rectangle rect, float startAngle, float sweepAngle)
        {
            this.rect = rect;
            this.startAngle = startAngle;
            this.sweepAngle = sweepAngle;
        }

        public Rectangle Rect { get => rect; set => rect = value; }
        public float StartAngle { get => startAngle; set => startAngle = value; }
        public float SweepAngle { get => sweepAngle; set => sweepAngle = value; }
    }
}
