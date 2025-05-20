using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Proyecto_PED.Auth
{
    public partial class SplashScreen : Form
    {
        // Constantes para configuración
        private const int AnimationInterval = 15;
        private const double OpacityStep = 0.05;
        private const int ProgressBarStep = 2;
        private const int WaitTime = 2000;

        // Temporizadores
        private System.Windows.Forms.Timer fadeInTimer;
        private System.Windows.Forms.Timer fadeOutTimer;
        private System.Windows.Forms.Timer waitTimer;
        private System.Windows.Forms.Timer progressTimer;

        // Controles UI
        private Guna2Panel mainPanel;
        private Guna2PictureBox logo;
        private Guna2HtmlLabel lblTitle;
        private Guna2ProgressBar progressBar;
        private Guna2HtmlLabel lblLoading;

        public SplashScreen()
        {
            InitializeCustomComponents();
            InitializeTimers();
        }

        private void InitializeCustomComponents()
        {
            // Configuración básica de la ventana
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 550);
            this.BackColor = Color.FromArgb(15, 15, 30);
            this.Opacity = 0;
            this.DoubleBuffered = true;

            // Panel principal
            mainPanel = new Guna2Panel()
            {
                Size = new Size(600, 400),
                Location = new Point((this.Width - 600) / 2, (this.Height - 400) / 2),
                BackColor = Color.FromArgb(25, 25, 45),
                BorderRadius = 20,
                ShadowDecoration = { Enabled = true, Color = Color.Black, Depth = 20 }
            };

            // Logo
            logo = new Guna2PictureBox()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(120, 120),
                Location = new Point(240, 50)
            };

            try { logo.Image = Image.FromFile("Resources/logo_hotel.png"); }
            catch { logo.Image = CreatePlaceholderImage(120, 120); }

            // Título
            lblTitle = new Guna2HtmlLabel()
            {
                Text = "<span style='font-size:24px; font-weight:bold; color:#ffffff'>SISTEMA DE</span><br>" +
                       "<span style='font-size:32px; font-weight:bold; color:#4d8eff'>RESERVAS HOTEL</span>",
                AutoSize = false,
                Size = new Size(400, 100),
                Location = new Point(100, 200),
                TextAlignment = ContentAlignment.MiddleCenter
            };

            // Barra de progreso
            progressBar = new Guna2ProgressBar()
            {
                Size = new Size(400, 10),
                Location = new Point(100, 320),
                ProgressColor = Color.FromArgb(77, 142, 255),
                ProgressColor2 = Color.FromArgb(120, 180, 255),
                FillColor = Color.FromArgb(40, 40, 70),
                Value = 0
            };

            // Texto de carga
            lblLoading = new Guna2HtmlLabel()
            {
                Text = "Cargando...",
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Location = new Point(350, 340),
                Font = new Font("Segoe UI", 10, FontStyle.Italic)
            };

            // Agregar controles
            mainPanel.Controls.Add(logo);
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(progressBar);
            mainPanel.Controls.Add(lblLoading);
            this.Controls.Add(mainPanel);
        }

        private Image CreatePlaceholderImage(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(70, 70, 90));
                g.DrawString("LOGO", new Font("Arial", 16), Brushes.White, new PointF(10, 40));
            }
            return bmp;
        }

        private void InitializeTimers()
        {
            // Temporizador para efecto de aparición gradual
            fadeInTimer = new System.Windows.Forms.Timer { Interval = AnimationInterval };
            fadeInTimer.Tick += (s, e) => FadeInEffect();

            // Temporizador de pausa intermedia
            waitTimer = new System.Windows.Forms.Timer { Interval = WaitTime };
            waitTimer.Tick += (s, e) =>
            {
                waitTimer.Stop();
                fadeOutTimer.Start();
            };

            // Temporizador para efecto de desvanecimiento
            fadeOutTimer = new System.Windows.Forms.Timer { Interval = AnimationInterval };
            fadeOutTimer.Tick += (s, e) => FadeOutEffect();

            // Temporizador para animar la barra de progreso
            progressTimer = new System.Windows.Forms.Timer { Interval = 50 };
            progressTimer.Tick += (s, e) =>
            {
                if (progressBar.Value < 100)
                    progressBar.Value += ProgressBarStep;
                else
                    progressTimer.Stop();
            };

            this.Load += (s, e) =>
            {
                fadeInTimer.Start();
                progressTimer.Start();
            };
        }

        private void FadeInEffect()
        {
            if (this.Opacity < 1)
                this.Opacity += OpacityStep;
            else
            {
                fadeInTimer.Stop();
                waitTimer.Start();
            }
        }

        private void FadeOutEffect()
        {
            if (this.Opacity > 0)
                this.Opacity -= OpacityStep;
            else
            {
                fadeOutTimer.Stop();
                AbrirLogin();
            }
        }

        private void AbrirLogin()
        {
            this.Hide();
            var login = new Login();
            login.Show();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    fadeInTimer?.Stop();
                    fadeOutTimer?.Stop();
                    waitTimer?.Stop();
                    progressTimer?.Stop();

                    fadeInTimer?.Dispose();
                    fadeOutTimer?.Dispose();
                    waitTimer?.Dispose();
                    progressTimer?.Dispose();

                    // Liberar controles UI
                    var controls = new Control[] { mainPanel, logo, progressBar, lblTitle, lblLoading };
                    foreach (var control in controls)
                    {
                        control?.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}