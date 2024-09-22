﻿using System.CodeDom.Compiler;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace KGLaba1
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        Bitmap bitmap;

        CustomPoint[] points = [];
        CyrcleService service;


        public Form1()
        {
            InitializeComponent();

            service = new CyrcleService(pictureBox1.Width, pictureBox1.Height);

            timer1.Start();

        }

        private void clearForm()
        {

            graphics.FillRectangle(Brushes.Tan, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);

        }

        private void timerStart_Tick(object sender, EventArgs e)
        {
            CircleMove();
            pictureBox1.Image = bitmap;
        }

        private void CircleMove()
        {
            if (service.countCrash < 2)
            {
                clearForm();

                points = service.ChangeFigurePosition();

                if (!service.InForm(points))
                {
                    if (service.countCrash == 1)
                    {
                        points = service.ChangeFigurePosition();
                    }
                    else
                    {
                        clearForm();
                        return;
                    }
                }

                for (int i = 0; i < points.Length; i++)
                {
                    graphics.DrawRectangle(new Pen(service.cyrcleColor), points[i].x, points[i].y, 1, 1);
                }
            }
        }


        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Tan;
            pictureBox1.Location = new Point(1, 6);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(983, 454);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 10;
            timer1.Tick += timerStart_Tick;
            // 
            // Form1
            // 
            BackColor = Color.White;
            ClientSize = new Size(984, 461);
            Controls.Add(pictureBox1);
            Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }
    }

    class CyrcleService
    {
        private int width;
        private int height;

        private CustomPoint center;
        private int radius;

        private int stepX = 1;

        private float N;
        private float M;

        public Brush cyrcleColor = Brushes.Green;
        public int countCrash = 0;

        public CyrcleService(int width, int height)
        {
            this.width = width;
            this.height = height;

            Random r = new Random();
            N = r.Next(100, width - 100);
            M = r.Next(100, height - 100);

            GenerateCyrcle();
        }

        private void ChangeCenter()
        {
            // X = X0 + Vx*t
            center.x += stepX;

            // y = kx + b
            center.y = (int)(M - M * center.x / N);
        }

        public CustomPoint[] ChangeFigurePosition()
        {
            ChangeCenter();
            List<CustomPoint> points = new List<CustomPoint>(); // один элемент в списке - центр круга

            int x = 0, y = radius, gap = 0, delta = (2 - 2 * radius);
            while (y >= 0)
            {
                points.Add(new CustomPoint(center.x + x, center.y + y));
                points.Add(new CustomPoint(center.x + x, center.y - y));
                points.Add(new CustomPoint(center.x - x, center.y - y));
                points.Add(new CustomPoint(center.x - x, center.y + y));

                gap = 2 * (delta + y) - 1;
                if (delta < 0 && gap <= 0)
                {
                    x++;
                    delta += 2 * x + 1;
                    continue;
                }
                if (delta > 0 && gap > 0)
                {
                    y--;
                    delta -= 2 * y + 1;
                    continue;
                }
                x++;
                delta += 2 * (x - y);
                y--;
            }

            return points.ToArray();
        }

        public bool InForm(CustomPoint[] points)
        {
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].x > width || points[i].x < 0 || points[i].y > height || points[i].y < 0)
                {
                    Random r = new Random();

                    if (points[i].x >= width)
                    {
                        stepX *= -1;

                        M = r.Next(-1000, 1000);
                        N = (float)(center.x / (1f - ((float)center.y / M)));
                    }
                    if (points[i].x <= 0)
                    {
                        stepX *= -1;
                        N = r.Next(-1500, 1500);
                        M = (float)(center.y / (1f - ((float)center.x / N)));
                    }
                    if (points[i].y <= 0)
                    {
                        M = r.Next(-1000, 1000);
                        N = (float)(center.x / (1f - ((float)center.y / M)));
                        stepX = M <= 0 ? Math.Abs(stepX) : -Math.Abs(stepX);
                    }
                    if (points[i].y >= height)
                    {
                        N = r.Next(-1500, 1500);
                        M = (float)(center.y / (1f - ((float)center.x / N)));
                        if (N >= center.x) stepX = Math.Abs(stepX);
                        else stepX = -Math.Abs(stepX);
                    }

                    Console.WriteLine("N " + N + " M " + M + " step " + stepX);
                    countCrash += 1;
                    return false;
                }

            }

            return true;
        }

        public void GenerateCyrcle()
        {
            Random random = new Random();
            radius = random.Next(10, 100);

            int x, y;
            do
            {
                x = random.Next(radius, (int)N);
                y = (int)(M * (1f - (float)((float)x / (float)N)));

            } while (y <= radius || y >= height - radius);

            center = new CustomPoint(x, y);
        }

    }
    class CustomPoint
    {
        public int x, y;

        public CustomPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public CustomPoint(CustomPoint p)
        {
            this.x = p.x;
            this.y = p.y;
        }

        public static CustomPoint operator +(CustomPoint v1, CustomPoint v2) => new CustomPoint(v1.x + v2.x, v1.y + v2.y);
        public static CustomPoint operator -(CustomPoint v1, CustomPoint v2) => new CustomPoint(v1.x - v2.x, v1.y - v2.y);


    }
}
