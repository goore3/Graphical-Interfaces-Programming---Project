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
using System.Web;
using System.Windows.Forms;

namespace Graphical_Interfaces_Programming___Project
{
    public partial class GolfForm : Form
    {
        Pen golfPen = new Pen(Color.Black, 2);
        Pen holePen = new Pen(Color.White, 2);
        SolidBrush holeBrush = new SolidBrush(Color.Brown);
        SolidBrush golfBrush = new SolidBrush(Color.AntiqueWhite);
        Boolean mousePressed, mouseOnScreen, moveBall;
        float ballSpeed, ballAngle, accel;
        int ballRadius, holeRadius, holeRadiusPadding, levelNumber;
        bool isLevelEnded;
        Vector2 ballPos, vel, holePos, mousePos2;

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
            holePos = new Point();
            holePos.X = 40;
            holePos.Y = 40;
            holeRadius = 7;
            holeRadiusPadding = 5;
            mousePressed = false;
            golfBrush = new SolidBrush(Color.White);
            golfPen = new Pen(Color.Black, 2);
            isLevelEnded = false;
            levelNumber = 1;
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
            DrawBall(g, holePen, holeBrush, holePos, holeRadius);
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
            EndgameListener();
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

        private void level1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //change level to 1
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
        
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // initialize Options form
        }

        public bool isBallInHole(Vector2 ballPos, Vector2 holePos, float ballRadius, float holeRadius)
        {
            bool isXOnBall = false;
            bool isYOnBall = false;

            if (holePos.X - holeRadius - holeRadiusPadding < ballPos.X - ballRadius && holePos.X + holeRadius + holeRadiusPadding > ballPos.X + ballRadius)
            {
                isXOnBall = true;
            }
            if (holePos.Y - holeRadius - holeRadiusPadding < ballPos.Y - ballRadius && holePos.Y + holeRadius + holeRadiusPadding > ballPos.Y + ballRadius)
            {
                isYOnBall = true;
            }
            return isXOnBall && isYOnBall;
        }

        public void EndgameListener()
        {
            // generate You Win string OR go to next level

            if (isBallInHole(ballPos, holePos, ballRadius, holeRadius))
            {
                // disappear ball, change hole color
                isLevelEnded = false;
                ballRadius = 0;
                holeBrush = new SolidBrush(Color.NavajoWhite);
                golfBrush = new SolidBrush(drawingPanel.BackColor);
                golfPen = new Pen(golfBrush, 0);
            }
        }
        private void fToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initValues();
        }
    }
}
