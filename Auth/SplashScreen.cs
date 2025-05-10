using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Proyecto_PED.Auth
{
    /// <summary>
    /// Formulario de presentación inicial con animaciones de fade in/out
    /// </summary>
    public partial class SplashScreen : Form
    {
        // Temporizadores para controlar las transiciones
        private System.Windows.Forms.Timer fadeInTimer;
        private System.Windows.Forms.Timer fadeOutTimer;
        private System.Windows.Forms.Timer waitTimer;

        /// <summary>
        /// Inicializa una nueva instancia del formulario SplashScreen
        /// </summary>
        public SplashScreen()
        {
            InitializeSplashScreen();
            InitializeTimers();
        }

        /// <summary>
        /// Configura los elementos visuales del formulario
        /// </summary>
        private void InitializeSplashScreen()
        {
            // Configuración básica de la ventana
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 550);
            this.BackColor = Color.FromArgb(15, 15, 30);
            this.Opacity = 0;
            this.DoubleBuffered = true;

            // Panel principal con efectos visuales
            var mainPanel = new Guna2Panel()
            {
                Size = new Size(600, 400),  // Ajustado para mejor proporción con la ventana
                Location = new Point((this.Width - 600) / 2, (this.Height - 400) / 2),  // Centrado correctamente
                BackColor = Color.FromArgb(25, 25, 45),
                BorderRadius = 20,
                ShadowDecoration = { Enabled = true, Color = Color.Black, Depth = 20 }
            };

            // Control para mostrar el logo de la aplicación
            var logo = new Guna2PictureBox()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(120, 120),
                Location = new Point(240, 50)  // Ajustada posición vertical
            };

            // Carga la imagen del logo si existe
            try
            {
                logo.Image = Image.FromFile("Resources/logo_hotel.png");
            }
            catch
            {
                logo.Image = CreatePlaceholderImage(120, 120);
            }

            // Etiqueta principal con el título de la aplicación
            var lblTitle = new Guna2HtmlLabel()
            {
                Text = "<span style='font-size:24px; font-weight:bold; color:#ffffff'>SISTEMA DE</span><br>" +
                       "<span style='font-size:32px; font-weight:bold; color:#4d8eff'>RESERVAS HOTEL</span>",
                AutoSize = false,
                Size = new Size(400, 100),
                Location = new Point(100, 200),  // Ajustada posición vertical
                TextAlignment = ContentAlignment.MiddleCenter
            };

            // Barra de progreso para indicar actividad
            var progressBar = new Guna2ProgressBar()
            {
                Size = new Size(400, 10),
                Location = new Point(100, 320),  // Ajustada posición vertical
                ProgressColor = Color.FromArgb(77, 142, 255),
                ProgressColor2 = Color.FromArgb(120, 180, 255),
                FillColor = Color.FromArgb(40, 40, 70)
            };

            // Temporizador para animar la barra de progreso
            var progressTimer = new System.Windows.Forms.Timer() { Interval = 50 };
            progressTimer.Tick += (s, e) => {
                if (progressBar.Value < 100)
                    progressBar.Value += 2;
                else
                    progressTimer.Stop();
            };
            progressTimer.Start();

            // Etiqueta secundaria con texto de estado
            var lblLoading = new Guna2HtmlLabel()
            {
                Text = "Cargando...",
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Location = new Point(350, 340),  // Ajustada posición vertical
                Font = new Font("Segoe UI", 10, FontStyle.Italic)
            };

            // Agregar controles al contenedor principal
            mainPanel.Controls.Add(logo);
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(progressBar);
            mainPanel.Controls.Add(lblLoading);

            this.Controls.Add(mainPanel);
        }

        /// <summary>
        /// Genera una imagen de relleno cuando no existe el logo
        /// </summary>
        /// <param name="width">Ancho de la imagen</param>
        /// <param name="height">Alto de la imagen</param>
        /// <returns>Objeto Image con el placeholder</returns>
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

        /// <summary>
        /// Configura los temporizadores para las animaciones
        /// </summary>
        private void InitializeTimers()
        {
            // Temporizador para efecto de aparición gradual
            fadeInTimer = new System.Windows.Forms.Timer { Interval = 15 };
            fadeInTimer.Tick += (s, e) =>
            {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                {
                    fadeInTimer.Stop();
                    waitTimer.Start();
                }
            };

            // Temporizador de pausa intermedia
            waitTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            waitTimer.Tick += (s, e) =>
            {
                waitTimer.Stop();
                fadeOutTimer.Start();
            };

            // Temporizador para efecto de desvanecimiento
            fadeOutTimer = new System.Windows.Forms.Timer { Interval = 15 };
            fadeOutTimer.Tick += (s, e) =>
            {
                if (this.Opacity > 0)
                    this.Opacity -= 0.05;
                else
                {
                    fadeOutTimer.Stop();
                    AbrirLogin();
                }
            };

            this.Load += (s, e) => fadeInTimer.Start();
        }

        /// <summary>
        /// Cierra este formulario y muestra la pantalla de login
        /// </summary>
        private void AbrirLogin()
        {
            this.Hide();
            var login = new Login();
            login.Show();
        }
    }
}