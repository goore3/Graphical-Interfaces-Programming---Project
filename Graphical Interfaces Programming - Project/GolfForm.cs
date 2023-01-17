﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        float ballSpeed;
        Point ballPos;
        Point mousePos;
        int ballRadius;

        public GolfForm()
        {
            InitializeComponent();
            initValues();
        }

        public void initValues()
        {
            ballPos = new Point();
            ballPos.X = 30;
            ballPos.Y = 30;
            ballRadius = 5;
            mousePressed = false;
            golfBrush = new SolidBrush(Color.White);
            golfPen = new Pen(Color.Black, 2);
        }

        public static void DrawBall(Graphics g, Pen pen, Brush brush, Point ballPos, float radius)
        {
            g.DrawEllipse(pen, ballPos.X - radius, ballPos.Y - radius, radius + radius, radius + radius);
            g.FillEllipse(brush, ballPos.X - radius, ballPos.Y - radius, radius + radius, radius + radius);
        }

        private void ballKick(float ballSpeed)
        {
            double ballAngleRad = Math.Atan2(mousePos.Y - ballPos.Y, mousePos.X - ballPos.X);
            double ballAngleDeg = 180 - ((180 / Math.PI) * ballAngleRad);
            ballPos.X += (int)(ballSpeed * Math.Cos(ballAngleDeg));
            ballPos.Y += (int)(ballSpeed * Math.Sin(ballAngleDeg));
            infoLabel.Text = "Angle(Rad):" + ballAngleRad + " Angle(Deg):" + ballAngleDeg + " ballX:" + ballPos.X + " BallY:" + ballPos.Y;
        }

        private void drawPanel_Paint(Graphics g)
        {
            DrawBall(g, golfPen, golfBrush, ballPos, ballRadius);
            if (mouseOnScreen & mousePressed) {
                g.DrawLine(golfPen, mousePos, ballPos);
            }
        }

        public bool detectBall(Point ballPos, Point mousePos,float ballRadius)
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
            if (detectBall(ballPos, mousePos, ballRadius))
            {
                drawingPanel.BackColor = Color.Aquamarine;
                mousePressed = true;
            }
        }

        private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            //infoLabel.Text = "Ball Loc - X:" + ballPos.X + " Y:" + ballPos.Y + " | Cursor Loc - X:" + e.Location.X + " Y:" + e.Location.Y;
            //drawLine(e.Location.X, e.Location.Y, ballX, ballY);
            mouseOnScreen = true;
            mousePos.X = e.Location.X;
            mousePos.Y = e.Location.Y;
            if (e.Location.Y > drawingPanel.Height)
            {
                mousePos.Y = drawingPanel.Height;
            }
            if(e.Location.X > drawingPanel.Width)
            {
                mousePos.X = drawingPanel.Width;
            }
            if(e.Location.X < 0)
            {
                mousePos.X = 0;      
            }
            if(e.Location.Y < 0)
            {
                mousePos.Y = 0;
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

        private float getLineLength(Point ballPos, Point mousePos)
        {
            float lineLength = (float)Math.Sqrt(Math.Abs(ballPos.X-mousePos.X)^2 + Math.Abs(ballPos.Y-mousePos.Y)^2);
            return lineLength;
        }

        private void drawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if(mousePressed)
            {
                drawingPanel.BackColor = Color.FromArgb(192, 255, 192);
                ballKick(getLineLength(ballPos,mousePos));
                mousePressed = false;
            }
        }
    }
}
