using System.CodeDom.Compiler;
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
        List<CyrcleService> services = [];

        int currentService = 0;

        public Form1()
        {
            InitializeComponent();

            services.Add( new CyrcleService(1, pictureBox1.Width, pictureBox1.Height));
            services.Add( new CyrcleService(2, pictureBox1.Width, pictureBox1.Height));

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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            List<Keys> number = new List<Keys> { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9 };
            if (number.Contains(e.KeyCode))
            {
                currentService *= 10;
                currentService += number.IndexOf(e.KeyCode);
            }
            else {
                if (services.FindIndex(x => x.id == currentService) == -1)
                {
                    currentService = 0;
                    return;
                }
                if (e.KeyCode == Keys.W)
                {
                    services.First(x => x.id == currentService).speed++;
                    Console.WriteLine("UP UP UP");
                }
                if (e.KeyCode == Keys.S)
                {
                    services.First(x => x.id == currentService).speed--;
                }
            }
        }


        private void CircleMove()
        {
            clearForm();

            for (int i = 0; i < services.Count(); i++) {
                points = services[i].ChangeFigurePosition();

                if (!services[i].InForm(points))
                {
                    points = services[i].ChangeFigurePosition();
                }

                for (int j = 0; j < points.Length; j++)
                {
                    graphics.DrawRectangle(new Pen(services[i].cyrcleColor), points[j].x, points[j].y, 1, 1);
                }

                graphics.FillEllipse(new SolidBrush(Color.White), services[i].center.x - services[i].radius, services[i].center.y - services[i].radius, services[i].radius * 2, services[i].radius * 2);
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
            KeyDown += Form1_KeyDown;
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
        public int id;
        private int width;
        private int height;

        private float N;
        private float M;

        public CustomPoint center;
        public int radius;

        public int speed = 3;
        private int vx = 1;
        private int vy = 1;

        private int signX = 1;
        private int signY = 1;

        Random random = new Random();


        public Brush cyrcleColor = Brushes.Green;
        public int countCrash = 0;

        public CyrcleService(int id, int width, int height)
        {
            this.id = id;
            this.width = width;
            this.height = height;

            N = random.Next(100, width - 100);
            M = random.Next(100, height - 100);

            GenerateCyrcle();
        }

        private void ChangeCenter()
        {
            CalculateSpeed();
            center.x += vx;
            center.y += vy;
        }

        public CustomPoint[] ChangeFigurePosition()
        {
            ChangeCenter();
            List<CustomPoint> points = []; // один элемент в списке - центр круга

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

            return [.. points];
        }


        public bool InForm(CustomPoint[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x >= width || points[i].x <= 0 || points[i].y >= height || points[i].y <= 0)
                {
                    center.x -= vx;
                    center.y -= vy;

                    if (points[i].x >= width)
                    {
                        signX *= -1;

                        do{
                            M = random.Next(-1000, 1000);
                        } while(Math.Abs(M) <= radius );

                        N = (float)(center.x / (1f - ((float)center.y / M)));
                    }
                    if (points[i].x <= 0)
                    {
                        signX *= -1;

                        do
                        {
                            N = random.Next(-1500, 1500);
                        } while (Math.Abs(N) <= radius);

                        M = (float)(center.y / (1f - ((float)center.x / N)));
                    }
                    if (points[i].y <= 0)
                    {
                        signY *= -1;

                        do{
                            M = random.Next(-1000, 1000);
                        } while (Math.Abs(M) <= radius);

                        N = (float)(center.x / (1f - ((float)center.y / M)));
                    }
                    if (points[i].y >= height)
                    {
                        signY *= -1;

                        do{
                            N = random.Next(-1500, 1500);
                        } while (Math.Abs(N) <= radius);

                        M = (float)(center.y / (1f - ((float)center.x / N)));
                    }

                    countCrash += 1;
                    return false;
                }
            }
            return true;
        }

        public void GenerateCyrcle()
        {
            radius = random.Next(10, 60);

            int x, y;

            x = random.Next(radius+3, (int)N);
            y = (int)(M - M * x / N);

            CalculateSpeed();
            center = new CustomPoint(x, y);
        }
        
        private void CalculateSpeed()
        {
            double diagonal = Math.Sqrt(N * N + M * M); // Гипотинуза

            double cosinus = Math.Abs(N) / diagonal;

            double sinus = Math.Abs(M) / diagonal;

            vx = (int) Math.Round(signX * speed * cosinus, 0);
            vy = (int) Math.Round(signY * speed * sinus, 0);
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
