using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphical_Interfaces_Programming___Project
{
    public partial class GolfForm : Form
    {
        Pen golfPen = new Pen(Color.Black, 2);
        SolidBrush golfBrush = new SolidBrush(Color.AntiqueWhite);
        Boolean mousePressed;
        Boolean mouseOnScreen;
        Boolean moveBall;
        //Point ballPos;
        //Point mousePos;
        Vector2 ballPos, vel;
        float ballAngle;
        float accel;
        Vector2 mousePos2;
        int ballRadius;

        public GolfForm()
        {
            InitializeComponent();
            initValues();
        }

        public void initValues()
        {
            //ballPos = new Point(30, 30);
            ballPos = new Vector2(30, 30);
            ballAngle = 0;
            accel = 0;
            ballRadius = 5;
            mousePressed = false;
            golfBrush = new SolidBrush(Color.White);
            golfPen = new Pen(Color.Black, 2);
        }

        public static void DrawBall(Graphics g, Pen pen, Brush brush, Vector2 ballPos, float radius)
        {
            g.DrawEllipse(pen, ballPos.X - radius, ballPos.Y - radius, radius + radius, radius + radius);
            g.FillEllipse(brush, ballPos.X - radius, ballPos.Y - radius, radius + radius, radius + radius);
        }

        private void ballKick(float ballSpeed)
        {
            ballAngle= (float) Math.Atan2(mousePos2.Y - ballPos.Y, mousePos2.X - ballPos.X);
            vel = ballPos - mousePos2;
        }

        private void intersectBoundries()
        {

            int width = drawingPanel.Width;
            int height = drawingPanel.Height;

            if (ballPos.X < ballRadius)
            {
                vel.X *= -1; ballPos.X = ballRadius;
            }

            if (ballPos.X > width - ballRadius)
            {
                vel.X *= -1; ballPos.X = width - ballRadius;
            }

            if (ballPos.Y < ballRadius)
            {
                vel.Y *= -1; ballPos.Y = ballRadius;
            }

            if (ballPos.Y >= height - ballRadius)
            {
                vel.Y *= -1; ballPos.Y = height - ballRadius;
            }
        }

        private void ballMove()
        {
            ballPos += vel;
            vel /= 1.5f;
        }

        private void drawPanel_Paint(Graphics g)
        {
            ballMove();
            intersectBoundries();
            DrawBall(g, golfPen, golfBrush, ballPos, ballRadius);
            if (mouseOnScreen & mousePressed) {
                g.DrawLine(golfPen, mousePos2.X,mousePos2.Y, ballPos.X, ballPos.Y);
            }
        }

        public bool detectBall(Vector2 ballPos, Vector2 mousePos,float ballRadius)
        {
            bool isXOnBall = false;
            bool isYOnBall = false;
            if (mousePos.X > (ballPos.X - ballRadius) && mousePos.X < (ballPos.X + ballRadius))
            {
                isXOnBall = true;
            }
            if (mousePos.Y > (ballPos.Y - ballRadius) && mousePos.Y < (ballPos.Y + ballRadius))
            {
                isYOnBall = true;
            }
            return isXOnBall && isYOnBall;
        }

        private void drawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            infoLabel.Text = "BallX:" + ballPos.X + " BallY:" + ballPos.Y;
            if (detectBall(ballPos, mousePos2, ballRadius))
            {
                drawingPanel.BackColor = Color.Aquamarine;
                mousePressed = true;
            }
        }

        private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            infoLabel.Text = "Ball Loc - X:" + ballPos.X + " Y:" + ballPos.Y + " | Angle:" + ballAngle + " | Acceleration:" + accel;
            //drawLine(e.Location.X, e.Location.Y, ballX, ballY);
            mouseOnScreen = true;
            mousePos2.X = e.Location.X;
            mousePos2.Y = e.Location.Y;
            if (e.Location.Y > drawingPanel.Height)
            {
                mousePos2.Y = drawingPanel.Height;
            }
            if(e.Location.X > drawingPanel.Width)
            {
                mousePos2.X = drawingPanel.Width;
            }
            if(e.Location.X < 0)
            {
                mousePos2.X = 0;      
            }
            if(e.Location.Y < 0)
            {
                mousePos2.Y = 0;
            }
        }

        private void reDrawTotal()
        {
            BufferedGraphicsContext currentContext;
            BufferedGraphics myBuffer;
            currentContext = BufferedGraphicsManager.Current;
            myBuffer = currentContext.Allocate(this.drawingPanel.CreateGraphics(), this.drawingPanel.DisplayRectangle);
            Graphics g = myBuffer.Graphics;

            drawAll(g);

            myBuffer.Render();
            myBuffer.Dispose();
        }

        private void drawAll(Graphics g)
        {
            g.Clear(drawingPanel.BackColor);
            drawPanel_Paint(g);
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            reDrawTotal();
        }

        private void drawingPanel_MouseLeave(object sender, EventArgs e)
        {
            mouseOnScreen = false;
        }

        private float getLineLength(Vector2 ballPos, Vector2 mousePos)
        {
            float locX = Math.Abs((ballPos.X - mousePos.X));
            float locY = Math.Abs((ballPos.Y - mousePos.Y));
            float lineLength = (float)Math.Sqrt(locX*locX + locY*locY);
            return lineLength;
        }

        private void drawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if(mousePressed)
            {
                drawingPanel.BackColor = Color.FromArgb(192, 255, 192);
                ballKick(getLineLength(ballPos,mousePos2));
                mousePressed = false;
            }
        }


    }
}
