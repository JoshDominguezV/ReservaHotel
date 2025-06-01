using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using Proyecto_PED.Views;

namespace Proyecto_PED.Auth
{
    public partial class Login : Form
    {
        // Constantes para configuración
        private const int AnimationInterval = 15;
        private const double OpacityStep = 0.05;

        // Controles UI
        private Guna2TextBox txtUsuario;
        private Guna2TextBox txtContrasena;
        private Guna2Button btnLogin;
        private Guna2Button btnClose;
        private Guna2CheckBox chkMostrarContrasena;

        public Login()
        {
            this.Opacity = 0;
            InitializeLoginForm();
            this.Load += Login_Load;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = AnimationInterval;
            timer.Tick += (s, ev) =>
            {
                if (this.Opacity < 1)
                    this.Opacity += OpacityStep;
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
                BorderRadius = 20,
                ShadowDecoration = { Enabled = true, Color = Color.Black, Depth = 20 }
            };

            // Botón para cerrar
            btnClose = new Guna2Button()
            {
                Text = "X",
                ForeColor = Color.White,
                FillColor = Color.Transparent,
                Location = new Point(mainPanel.Width - 40, 10),
                Size = new Size(30, 30),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => Application.Exit();
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.Red;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.White;

            // Título del formulario
            var titulo = new Label()
            {
                Text = "INICIO DE SESIÓN",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
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
                Cursor = Cursors.IBeam,
                BorderColor = Color.FromArgb(70, 70, 90)
            };
            txtUsuario.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) txtContrasena.Focus();
            };

            // Campo de contraseña
            txtContrasena = new Guna2TextBox()
            {
                PlaceholderText = "Contraseña",
                Location = new Point(75, 180),
                Size = new Size(300, 40),
                PasswordChar = '●',
                BorderRadius = 10,
                FillColor = Color.FromArgb(40, 40, 70),
                ForeColor = Color.White,
                Cursor = Cursors.IBeam,
                BorderColor = Color.FromArgb(70, 70, 90),
                UseSystemPasswordChar = true
            };
            txtContrasena.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnLogin.PerformClick();
            };



            // Botón de inicio de sesión
            btnLogin = new Guna2Button()
            {
                Text = "INICIAR SESIÓN",
                Location = new Point(75, 280),
                Size = new Size(300, 45),
                FillColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                BorderRadius = 10,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.FillColor = Color.FromArgb(0, 150, 255);
            btnLogin.MouseLeave += (s, e) => btnLogin.FillColor = Color.FromArgb(0, 120, 215);

            // Agregar controles
            mainPanel.Controls.Add(btnClose);
            mainPanel.Controls.Add(titulo);
            mainPanel.Controls.Add(txtUsuario);
            mainPanel.Controls.Add(txtContrasena);
            mainPanel.Controls.Add(chkMostrarContrasena);
            mainPanel.Controls.Add(btnLogin);

            this.Controls.Add(mainPanel);
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
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
                btnLogin.Enabled = false;
                btnLogin.Text = "VERIFICANDO...";

                var (success, rol) = await Task.Run(() => ValidarCredenciales(usuario, contrasena));

                if (success)
                {
                    this.Hide();
                    var menu = new Main(usuario, rol);
                    menu.FormClosed += (s, args) => Application.Exit();
                    menu.Show();
                }
                else
                {
                    MostrarMensaje("Error de autenticación", "Credenciales incorrectas", MessageBoxIcon.Error);
                    txtContrasena.Focus();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error de conexión", $"Error al conectar con la base de datos: {ex.Message}", MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "INICIAR SESIÓN";
            }
        }

        private (bool success, string rol) ValidarCredenciales(string usuario, string contrasena)
        {
            using (var conn = new ConexionBD().ObtenerConexion())
            {
                string query = @"SELECT u.nombre_usuario, r.nombre_rol 
                               FROM usuarios u 
                               JOIN roles r ON u.rol_id = r.id 
                               WHERE u.nombre_usuario = @usuario AND u.contrasena = @contrasena";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (true, reader.GetString("nombre_rol"));
                        }
                    }
                }
            }
            return (false, null);
        }

        private void MostrarMensaje(string titulo, string mensaje, MessageBoxIcon icono)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, icono);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar controles UI
                var controls = new Control[] { txtUsuario, txtContrasena, btnLogin, btnClose, chkMostrarContrasena };
                foreach (var control in controls)
                {
                    control?.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}