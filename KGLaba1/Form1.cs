using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace KGLaba1
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        Bitmap bitmap;

        CustomPoint[] points1 = [];
        CustomPoint[] points2 = [];

        CyrcleService service1;
        CyrcleService service2;


        public Form1()
        {
            InitializeComponent();

            service1 = new CyrcleService(pictureBox1.Width, pictureBox1.Height);

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
            service1.timer+= 1f;
        }

        private void CircleMove()
        {
            clearForm();

            points1 = service1.ChangeFigurePosition();

            for (int i = 0; i < points1.Length; i++)
            {
                graphics.DrawRectangle(new Pen(service1.cyrcleColor), (int)points1[i].x, (int) points1[i].y, 1, 1);
                
            }

            graphics.FillEllipse(new SolidBrush(Color.White), service1.center.x - service1.radius, service1.center.y - service1.radius, service1.radius * 2, service1.radius * 2);

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
            pictureBox1.Size = new Size(983, 547);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 20;
            timer1.Tick += timerStart_Tick;
            // 
            // Form1
            // 
            BackColor = Color.White;
            ClientSize = new Size(984, 616);
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

        public float timer = 0f;

        public CustomPoint center;
        public CustomPoint moveCenter;
        public int radius;

        public float speed = 4f;
        public float scaleX = 1f;
        public float scaleY = 1f;

        public float radiusCircle = 75f;
        public CustomPoint centerCircle;

        public Brush cyrcleColor = Brushes.Green;
        public int countCrash = 0;

        public CyrcleService(int width, int height)
        {
            this.width = width;
            this.height = height;

            GenerateCyrcle();
        }

        private void ChangeCenter()
        {
            float angleRadians = (float)((timer*4) * Math.PI / 180);
            //r5e3wh
            center.x = moveCenter.x + (int)(radiusCircle * Math.Cos(angleRadians));
            center.y = moveCenter.y + (int)(radiusCircle * Math.Sin(angleRadians));
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

        public void GenerateCyrcle()
        {
            Random random = new Random();
            centerCircle = new CustomPoint(300, 200);
            //radius = random.Next(10, 100);
            radius = 50;
            center = new CustomPoint(centerCircle.x, centerCircle.y);
            moveCenter = new CustomPoint(centerCircle.x, centerCircle.y);
        }

    }
    class CustomPoint
    {
        public float x, y;

        public CustomPoint(float x, float y)
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
