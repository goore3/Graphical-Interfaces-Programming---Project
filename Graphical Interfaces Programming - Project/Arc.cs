using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphical_Interfaces_Programming___Project
{
    internal class Arc
    {
        Rectangle rect;
        float startAngle, sweepAngle;
        Vector2 p1, pM, p2, middle;
        int slideSide;

        public Arc(Rectangle rect, float startAngle, float sweepAngle)
        {
            this.rect = rect;
            this.startAngle = startAngle;
            this.sweepAngle = sweepAngle;
            middle = new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            switch(startAngle)
            {
                case 0:
                    slideSide = 0;
                    p1 = new Vector2(rect.X, rect.Y + rect.Height/2);
                    pM = new Vector2(rect.X + rect.Width/2, rect.Y + rect.Height);
                    p2 = new Vector2(rect.X + rect.Width, rect.Y + rect.Height/2);
                    break;
                case 90:
                    slideSide = 1;
                    p1 = new Vector2(rect.X + rect.Width / 2, rect.Y);
                    pM = new Vector2(rect.X, rect.Y + rect.Height / 2);
                    p2 = new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height);
                    break;
                case 180:
                    slideSide = 2;
                    p1 = new Vector2(rect.X, rect.Y + rect.Height/2);
                    pM = new Vector2(rect.X + rect.Width / 2, rect.Y);
                    p2 = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
                    break;
                case 270:
                    slideSide = 3;
                    p1 = new Vector2(rect.X + rect.Width / 2, rect.Y);
                    pM = new Vector2(rect.X + rect.Width, rect.Y + rect.Height / 2);
                    p2 = new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height);
                    break;
            }
        }

        public Rectangle Rect { get => rect; set => rect = value; }
        public float StartAngle { get => startAngle; set => startAngle = value; }
        public float SweepAngle { get => sweepAngle; set => sweepAngle = value; }
        public Vector2 P1 { get => p1; set => p1 = value; }
        public Vector2 PM { get => pM; set => pM = value; }
        public Vector2 P2 { get => p2; set => p2 = value; }
        public int SlideSide { get => slideSide; set => slideSide = value; }
        public Vector2 Middle { get => middle; set => middle = value; }
    }
}
