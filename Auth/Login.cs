using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using Proyecto_PED.Views;

namespace Proyecto_PED.Auth
{
    public partial class Login : Form
    {
        private Guna2TextBox txtUsuario;
        private Guna2TextBox txtContrasena;
        private Guna2Button btnLogin;
        private Guna2Button btnClose;

        public Login()
        {
            this.Opacity = 0;
            InitializeLoginForm();
            this.Load += Login_Load;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 15;
            timer.Tick += (s, ev) =>
            {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                {
                    timer.Stop();
                    txtUsuario.Focus();
                }
            };
            timer.Start();
        }

        private void InitializeLoginForm()
        {
            // Configuración básica del formulario
            this.Text = "Login - Hotel Premium";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(15, 15, 30);
            this.DoubleBuffered = true;

            // Panel principal
            var mainPanel = new Guna2Panel()
            {
                Size = new Size(450, 400),
                Location = new Point(25, 25),
                BackColor = Color.FromArgb(25, 25, 45),
                BorderRadius = 20
            };

            try
            {
                mainPanel.ShadowDecoration.Enabled = true;
                mainPanel.ShadowDecoration.Color = Color.Black;
                mainPanel.ShadowDecoration.Depth = 20;
            }
            catch { }

            // Botón para cerrar
            btnClose = new Guna2Button()
            {
                Text = "X",
                ForeColor = Color.White,
                FillColor = Color.Transparent,
                Location = new Point(mainPanel.Width - 40, 10),
                Size = new Size(30, 30),
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => Application.Exit();

            // Título del formulario
            var titulo = new Label()
            {
                Text = "LOGIN",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(300, 50),
                Location = new Point(75, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Campo de usuario
            txtUsuario = new Guna2TextBox()
            {
                PlaceholderText = "Usuario",
                Location = new Point(75, 120),
                Size = new Size(300, 40),
                BorderRadius = 10,
                FillColor = Color.FromArgb(40, 40, 70),
                ForeColor = Color.White,
                Cursor = Cursors.IBeam
            };
            txtUsuario.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) txtContrasena.Focus();
            };

            // Campo de contraseña
            txtContrasena = new Guna2TextBox()
            {
                PlaceholderText = "Contraseña",
                Location = new Point(75, 190),
                Size = new Size(300, 40),
                PasswordChar = '●',
                BorderRadius = 10,
                FillColor = Color.FromArgb(40, 40, 70),
                ForeColor = Color.White,
                Cursor = Cursors.IBeam
            };
            txtContrasena.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) btnLogin.PerformClick();
            };

            try
            {
                typeof(Guna2TextBox).GetProperty("BorderColor")?.SetValue(txtUsuario, Color.FromArgb(70, 70, 90));
                typeof(Guna2TextBox).GetProperty("BorderColor")?.SetValue(txtContrasena, Color.FromArgb(70, 70, 90));
            }
            catch { }

            // Botón de inicio de sesión solo con flecha
            btnLogin = new Guna2Button()
            {
                Text = "→",
                Location = new Point(75, 280),
                Size = new Size(60, 45),
                FillColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                BorderRadius = 10,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.Click += BtnLogin_Click;

            // Agregar controles
            mainPanel.Controls.Add(btnClose);
            mainPanel.Controls.Add(titulo);
            mainPanel.Controls.Add(txtUsuario);
            mainPanel.Controls.Add(txtContrasena);
            mainPanel.Controls.Add(btnLogin);

            this.Controls.Add(mainPanel);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                MostrarMensaje("Campos vacíos", "Por favor, complete todos los campos", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ConexionBD conexionBD = new ConexionBD();
                using (MySqlConnection conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        string query = @"SELECT u.nombre_usuario, r.nombre_rol 
                                       FROM usuarios u 
                                       JOIN roles r ON u.rol_id = r.id 
                                       WHERE u.nombre_usuario = @usuario AND u.contrasena = @contrasena";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string rol = reader.GetString("nombre_rol");
                                this.Hide();
                                var menu = new Reservacion(usuario, rol);
                                menu.FormClosed += (s, args) => Application.Exit();
                                menu.Show();
                            }
                            else
                            {
                                MostrarMensaje("Error de autenticación", "Credenciales incorrectas", MessageBoxIcon.Error);
                                txtContrasena.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error de conexión", $"Error al conectar con la base de datos: {ex.Message}", MessageBoxIcon.Error);
            }
        }

        private void MostrarMensaje(string titulo, string mensaje, MessageBoxIcon icono)
        {
            // Versión simplificada que usa MessageBox estándar con estilo personalizado
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, icono);
        }
    }
}