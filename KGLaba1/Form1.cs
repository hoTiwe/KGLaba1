using System.CodeDom.Compiler;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;

namespace KGLaba1
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        Bitmap bitmap;

        CustomPoint[] points = [];
        CyrcleService service;

        Color backColor;//öâåò ôîíà

        public Form1()
        {
            InitializeComponent();

            service = new CyrcleService(pictureBox1.Width, pictureBox1.Height);

            timer1.Start();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);

            backColor = pictureBox1.BackColor;
            //Draw();
        }

        private void timerStart_Tick(object sender, EventArgs e)
        {
            CircleMove();
            pictureBox1.Image = bitmap;
        }

        private void CircleMove()
        {
            for (int i = 0; i < points.Length; i++)
            {
                graphics.DrawRectangle(new Pen(Brushes.White), points[i].x, points[i].y, 1, 1);
            }

            points = service.ChangeFigurePosition();

            if (!service.InForm(points)) points = service.ChangeFigurePosition();


            for (int i = 0; i < points.Length; i++)
            {
                graphics.DrawRectangle(new Pen(Brushes.Red), points[i].x, points[i].y, 1, 1);
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
            pictureBox1.Location = new Point(1, 6);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(983, 454);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click_1;
            // 
            // timer1
            // 
            timer1.Interval = 25;
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }
    }

    class CyrcleService
    {
        private const int speed = 100;
        private int width;
        private int height;

        private CustomPoint center;
        private int radius;

        private int stepX = 4;

        private float N;
        private float M;

        public CyrcleService(int width, int height)
        {
            N = this.width = width;
            M = this.height = height;

            GenerateCyrcle();
        }

        private void ChangeCenter()
        {
            // X = X0 + Vx*t
            center.x += stepX;

            // y = kx + b
            center.y = (int)(M * (1 - center.x / N));
        }

        public CustomPoint[] ChangeFigurePosition()
        {
            ChangeCenter();
            List<CustomPoint> points = new List<CustomPoint> { this.center }; // один элемент в списке - центр круга

            int x = 0, y = radius, gap = 0, delta = (2 - 2 * radius);
            while (y >= 0)
            {
                points.Add(new CustomPoint( center.x + x, center.y + y));
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
                    if (points[i].x >= width || points[i].x < 0) stepX *= -1;

                    return false;
                }

            }

            return true;
        }

        public void GenerateCyrcle()
        {
            Random random = new Random();
            radius = random.Next(1, 20);

            int x = random.Next(radius, this.width - radius);
            int y = (int)(M * (1f - (float)((float) x / (float) N)));

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
