using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using System.Windows.Forms;
using static Graphical_Interfaces_Programming___Project.Arc;
using static System.Windows.Forms.LinkLabel;

namespace Graphical_Interfaces_Programming___Project
{
    public partial class GolfForm : Form
    {
        Pen golfPen, holePen, wallPen, arcPen;
        List<Rectangle> walls;
        List<Arc> arcs;
        SolidBrush holeBrush, golfBrush, wallBrush, arcBrush;
        int ballRadius, holeRadius, holeRadiusPadding, sideNumber, totalShots;
        bool isLevelEnded, mousePressed, mouseOnScreen, isIntersecting;
        Vector2 ballPos, vel, holePos, mousePos2;
        Random rand;
        Vector2[] sides;
        int[] angles;

        public GolfForm()
        {
            InitializeComponent();
            initValues();
        }

        public void initValues()
        {
            // Function called for every game reset and at start of the game
            rand = new Random(new Random().Next());
            drawingPanel.BackColor = Color.FromArgb(192, 255, 192);
            totalShots = 0;
            sides = new Vector2[2];
            angles = new int[4];
            angles[0] = 0; angles[1] = 90; angles[2] = 180; angles[3] = 270;

            // Pen and Brushes initialization
            golfPen = new Pen(Color.Black, 2);
            holePen = new Pen(Color.White, 2);
            wallPen = new Pen(Color.Brown, 2);
            arcPen = new Pen(Color.Purple, 3);
            holeBrush = new SolidBrush(Color.Brown);
            golfBrush = new SolidBrush(Color.White);
            wallBrush = new SolidBrush(Color.BurlyWood);
            arcBrush = new SolidBrush(Color.MediumPurple);

            // Sides initialization
            sideNumber = rand.Next(0, 2);
            sides[0] = new Vector2(rand.Next(10, drawingPanel.Width / 2), rand.Next(10, drawingPanel.Height));
            sides[1] = new Vector2(rand.Next(drawingPanel.Width / 2, drawingPanel.Width - 10), rand.Next(10, drawingPanel.Height));
            ballPos = sides[sideNumber];
            holePos = sides[Math.Abs(sideNumber - 1)];

            // Radiuses initialization
            ballRadius = 5;
            holeRadius = 7;
            holeRadiusPadding = 4;
            
            // Flag used for mouse click event
            mousePressed = false;
            
            // Flag used for level reseting
            isLevelEnded = false;

            // Obstacles generating
            walls = new List<Rectangle>();
            arcs = new List<Arc>();
            for (int i = 0; i < 3; i++)
            {
                isIntersecting = true;
                while (isIntersecting)
                {
                    isIntersecting = false;
                    int width = 10;
                    int height = 100;
                    if (rand.Next(0, 2) == 1)
                    {
                        width = 100;
                        height = 10;
                    }
                    Rectangle r1 = new Rectangle(rand.Next(drawingPanel.Width - 20), rand.Next(drawingPanel.Height - 120), width, height);
                    foreach (Rectangle rectangle in walls)
                    {
                        if (!Rectangle.Intersect(r1, rectangle).IsEmpty)
                        {
                            isIntersecting = true;
                            break;
                        }

                    }
                    foreach (Arc arc in arcs)
                    {
                        if (!Rectangle.Intersect(r1, arc.Rect).IsEmpty)
                        {
                            isIntersecting = true;
                            break;
                        }
                    }
                    if (!isIntersecting)
                    {
                        createRectangle(r1);
                    }
                }
                isIntersecting = true;
                while (isIntersecting)
                {
                    isIntersecting = false;
                    int[] angles = { 0, 90, 180, 270 };
                    float startAngle = angles[rand.Next(0, angles.Length)];
                    Rectangle a1 = new Rectangle(rand.Next(drawingPanel.Width - 65), rand.Next(drawingPanel.Height - 60), 65, 60); foreach (Rectangle rectangle in walls)
                    {
                        foreach (Rectangle rectangle1 in walls)
                        {
                            if (!Rectangle.Intersect(a1, rectangle1).IsEmpty)
                            {
                                isIntersecting = true;
                                break;
                            }

                        }
                        foreach (Arc arc in arcs)
                        {
                            if (!Rectangle.Intersect(a1, arc.Rect).IsEmpty)
                            {
                                isIntersecting = true;
                                break;
                            }
                        }
                    }
                    if (!isIntersecting)
                    {
                        createArc(new Arc(a1, startAngle, 180));
                    }
                }
            }
        }

        public static void DrawBall(Graphics g, Pen pen, Brush brush, Vector2 ballPos, float radius)
        {
            g.DrawEllipse(pen, ballPos.X - radius, ballPos.Y - radius, radius + radius, radius + radius);
            g.FillEllipse(brush, ballPos.X - radius, ballPos.Y - radius, radius + radius, radius + radius);
        }

        private void ballKick(float ballSpeed)
        {
            vel = ballPos - mousePos2;
        }
        private bool LineIntersectsLine(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

        private bool LineIntersectsRectangle(Vector2 prevPoint, Vector2 newPoint, Rectangle rect)
        {
            Vector2 leftTop = new Vector2(rect.X, rect.Y);
            Vector2 leftBottom = new Vector2(rect.X, rect.Bottom);
            Vector2 rightTop = new Vector2(rect.Right, rect.Y);
            Vector2 rightBottom = new Vector2(rect.Right, rect.Bottom);
            bool leftLine = LineIntersectsLine(prevPoint, newPoint, leftTop, leftBottom);
            bool rightLine = LineIntersectsLine(prevPoint, newPoint, rightTop, rightBottom);
            bool topLine = LineIntersectsLine(prevPoint, newPoint, leftTop, rightTop);
            bool bottomLine = LineIntersectsLine(prevPoint, newPoint, leftBottom, rightBottom);
            bool final = leftLine || rightLine || topLine || bottomLine;
            if (final)
            {
                return true;
            } else
            {
                return false;
            }
        }


        private void intersectWalls()
        {
            foreach(Rectangle rect in walls)
            {
                if(ballPos.X >= rect.Left - ballRadius && ballPos.X <= rect.Right && ballPos.Y >= rect.Top - ballRadius && ballPos.Y <= rect.Bottom)
                {
                    List<float> differences = new List<float>();
                    differences.Add(Math.Abs(ballPos.Y - rect.Top));
                    differences.Add(Math.Abs(ballPos.Y - rect.Bottom));
                    differences.Add(Math.Abs(ballPos.X - rect.Right));
                    differences.Add(Math.Abs(ballPos.X - rect.Left));
                    int diffIndex = differences.IndexOf(differences.Min());
                    switch(diffIndex)
                    {
                        case 0:
                            vel.Y *= -1; ballPos.Y = rect.Top - ballRadius - 1; break;
                        case 1:
                            vel.Y *= -1; ballPos.Y = rect.Bottom + 1; break;
                        case 2:
                            vel.X *= -1; ballPos.X = rect.Right + ballRadius + 1; break;
                        case 3:
                            vel.X *= -1; ballPos.X = rect.Left - ballRadius - 1; break;
                    }
                }
            }
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

        private void drawRectangles(Graphics g)
        {
            g.DrawRectangles(wallPen, walls.ToArray());
            g.FillRectangles(wallBrush, walls.ToArray());
        }

        private void ballMove()
        {
            var newBallPos = ballPos + vel;
            if (!checkRectangleCollision(newBallPos, ballPos))
            {
                ballPos += vel;
                vel /= 1.5f;
            }
        }

        private bool checkRectangleCollision(Vector2 newballPos, Vector2 ballPos)
        {
            foreach(Rectangle rect in walls)
            {
                if(LineIntersectsRectangle(ballPos, newballPos, rect))
                {
                    intersectRectangle(rect);
                    return true;
                }
            }
            return false;
        }

        private void intersectRectangle(Rectangle rect)
        {
            List<float> differences = new List<float>();
            differences.Add(Math.Abs(ballPos.Y - rect.Top));
            differences.Add(Math.Abs(ballPos.Y - rect.Bottom));
            differences.Add(Math.Abs(ballPos.X - rect.Right));
            differences.Add(Math.Abs(ballPos.X - rect.Left));
            int diffIndex = differences.IndexOf(differences.Min());
            switch (diffIndex)
            {
                case 0:
                    vel.Y *= -1; ballPos.Y = rect.Top - ballRadius - 1; break;
                case 1:
                    vel.Y *= -1; ballPos.Y = rect.Bottom + 1; break;
                case 2:
                    vel.X *= -1; ballPos.X = rect.Right + ballRadius + 1; break;
                case 3:
                    vel.X *= -1; ballPos.X = rect.Left - ballRadius - 1; break;
            }
        }

        private void drawPanel_Paint(Graphics g)
        {
            ballMove();
            intersectBoundries();
            drawObstacles(g);
            //intersectWalls();
            DrawBall(g, golfPen, golfBrush, ballPos, ballRadius);
            DrawBall(g, holePen, holeBrush, holePos, holeRadius);
            if (mouseOnScreen & mousePressed) {
                g.DrawLine(golfPen, mousePos2.X,mousePos2.Y, ballPos.X, ballPos.Y);
            }
        }

        public bool detectBall(Vector2 ballPos, Vector2 mousePos, float ballRadius)
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
            if (!isLevelEnded)
                infoLabel.Text = $"Ball X:{ballPos.X} Ball Y:{ballPos.Y} Total shots: {totalShots}";
            if (detectBall(ballPos, mousePos2, ballRadius))
            {
                drawingPanel.BackColor = Color.Aquamarine;
                mousePressed = true;
            }
        }

        private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isLevelEnded)
                infoLabel.Text = $"Ball Loc - X:{ballPos.X} Y:{ballPos.Y} Total shots: {totalShots}";
            mouseOnScreen = true;
            mousePos2.X = e.Location.X;
            mousePos2.Y = e.Location.Y;
            if (e.Location.Y > drawingPanel.Height)
            {
                mousePos2.Y = drawingPanel.Height;
            }
            if (e.Location.X > drawingPanel.Width)
            {
                mousePos2.X = drawingPanel.Width;
            }
            if (e.Location.X < 0)
            {
                mousePos2.X = 0;      
            }
            if (e.Location.Y < 0)
            {
                mousePos2.Y = 0;
            }
        }

        private void reDrawTotal()
        {
            BufferedGraphicsContext currentContext;
            BufferedGraphics myBuffer;
            currentContext = BufferedGraphicsManager.Current;
            myBuffer = currentContext.Allocate(drawingPanel.CreateGraphics(), drawingPanel.DisplayRectangle);
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

        private void drawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if(mousePressed)
            {
                drawingPanel.BackColor = Color.FromArgb(192, 255, 192);
                ballKick(getLineLength(ballPos,mousePos2));
                totalShots++;
                mousePressed = false;
            }
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
            if (isBallInHole(ballPos, holePos, ballRadius, holeRadius))
            {
                // disappear ball, change hole color
                isLevelEnded = true;
                ballRadius = 0;
                holeBrush = new SolidBrush(Color.Red);
                drawingPanel.BackColor = Color.LightBlue;
                golfBrush = new SolidBrush(drawingPanel.BackColor);
                golfPen = new Pen(golfBrush, 0);
                infoLabel.Text = $"You win! Total shots: {totalShots}. Reset level or exit the game.";
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

        private void drawObstacles(Graphics g)
        {
            g.DrawRectangles(wallPen, walls.ToArray());
            foreach (Arc arc in arcs)
            {
                g.DrawArc(arcPen, arc.Rect, arc.StartAngle, arc.SweepAngle);
            }
            g.FillRectangles(wallBrush, walls.ToArray());
        }

        private void createRectangle(Rectangle r1)
        {
            walls.Add(r1);
        }
        private void createArc(Arc a1)
        {
            arcs.Add(a1);
        }
    }
}
