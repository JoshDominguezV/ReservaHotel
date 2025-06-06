﻿using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using Proyecto_PED.Views.Checks;
using Proyecto_PED.Views.Clientes;
using Proyecto_PED.Views.Habitaciones;
using Proyecto_PED.Views.Reservaciones;


namespace Proyecto_PED.Views
{
    public partial class Main : Form
    {
        private string usuarioActual;
        private string rolActual;
        private ConexionBD conexionBD;
        private Panel menuPanel;
        private Panel subMenuPanel;
        private Panel contentPanel;
        private const int menuWidth = 220;
        private const int subMenuWidth = 200;

        public Main(string usuario, string rol)
        {
            usuarioActual = usuario;
            rolActual = rol;
            conexionBD = new ConexionBD();

            InitializeComponent();
            ConstruirInterfaz();
            MostrarVistaInicio();
        }

        private void ConstruirInterfaz()
        {
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(800, 600); // Evita que sea demasiado pequeño
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            // Configuración básica del formulario Menu
            this.Text = $"Sistema de Reservaciones - {usuarioActual} ({rolActual})";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.None;

            // Panel del menú principal
            menuPanel = new Panel
            {
                BackColor = Color.FromArgb(51, 51, 76),
                Dock = DockStyle.Left,
                Width = menuWidth
            };

            // Panel del submenú (inicialmente oculto)
            subMenuPanel = new Panel
            {
                BackColor = Color.FromArgb(61, 61, 86),
                Dock = DockStyle.Left,
                Width = 0,
                Visible = false
            };

            // Panel del contenido principal
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Control
            };

            // Construir los menús
            ConstruirMenuPrincipal();
            ConstruirSubMenuReservaciones();

            this.Controls.Add(contentPanel);
            this.Controls.Add(subMenuPanel);
            this.Controls.Add(menuPanel);
        }

        private void ConstruirMenuPrincipal()
        {

            // Botón Cerrar con icono
            var btnCerrar = new Button
            {
                Size = new Size(32, 32),
                Location = new Point(this.ClientSize.Width - 40, 8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Image = Properties.Resources.iconX,
                ImageAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 50, 50);
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.Add(btnCerrar);
            btnCerrar.BringToFront();



            // Encabezado
            var lblLogo = new Label
            {
                Text = "Hotel Premium",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Botones principales
            var btnInicio = CrearBotonMenu("Inicio", null);
            var btnClientes = CrearBotonMenu("Clientes", null);
            var btnHabitaciones = CrearBotonMenu("Habitaciones", null);
            var btnReservaciones = CrearBotonMenu("Reservaciones", null);
            var btnGestionUsuarios = CrearBotonMenu("Usuarios", null);
            var btnCerrarSesion = CrearBotonMenu("Cerrar Sesión", null);

            // Eventos
            btnInicio.Click += (s, e) => MostrarVistaInicio();
            btnClientes.Click += (s, e) => MostrarVistaClientes();
            btnHabitaciones.Click += (s, e) => MostrarVistaHabitaciones();
            btnReservaciones.Click += (s, e) => MostrarSubMenuReservaciones();
            btnGestionUsuarios.Click += (s, e) => MostrarVistaUsuarios();
            btnCerrarSesion.Click += (s, e) => this.Close();

            // Ocultar gestión de usuarios si no es administrador
            btnGestionUsuarios.Visible = rolActual == "Administrador";

            // Orden de los controles
            menuPanel.Controls.Add(btnCerrarSesion);
            menuPanel.Controls.Add(btnGestionUsuarios);
            menuPanel.Controls.Add(btnHabitaciones);
            menuPanel.Controls.Add(btnClientes);
            menuPanel.Controls.Add(btnReservaciones);
            menuPanel.Controls.Add(btnInicio);
            menuPanel.Controls.Add(lblLogo);
        }

        private void ConstruirSubMenuReservaciones()
        {
            var btnNuevaReserva = CrearBotonSubMenu("Nueva Reserva");
            var btnListaReservas = CrearBotonSubMenu("Lista de Reservas");
            var btnCheckIn = CrearBotonSubMenu("Check-In");
            var btnCheckOut = CrearBotonSubMenu("Check-Out");

            btnNuevaReserva.Click += (s, e) => MostrarVistaNuevaReserva();
            btnListaReservas.Click += (s, e) => MostrarVistaListaReservas();
            btnCheckIn.Click += (s, e) => MostrarVistaCheckIn();
            btnCheckOut.Click += (s, e) => MostrarVistaCheckOut();

            subMenuPanel.Controls.Add(btnCheckOut);
            subMenuPanel.Controls.Add(btnCheckIn);
            subMenuPanel.Controls.Add(btnListaReservas);
            subMenuPanel.Controls.Add(btnNuevaReserva);
        }

        private Button CrearBotonMenu(string text, Image icon)
        {
            return new Button
            {
                Text = "  " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 11),
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                Image = icon,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                FlatAppearance = {
                    BorderSize = 0,
                    MouseOverBackColor = Color.FromArgb(70, 70, 100),
                    MouseDownBackColor = Color.FromArgb(30, 30, 50)
                },
                BackColor = Color.FromArgb(51, 51, 76)
            };
        }

        private Button CrearBotonSubMenu(string text)
        {
            return new Button
            {
                Text = "  " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(20, 0, 0, 0),
                FlatAppearance = {
                    BorderSize = 0,
                    MouseOverBackColor = Color.FromArgb(80, 80, 110),
                    MouseDownBackColor = Color.FromArgb(40, 40, 60)
                },
                BackColor = Color.FromArgb(61, 61, 86)
            };
        }

        private void MostrarVistaNuevaReserva()
        {
            LimpiarContentPanel();
            OcultarSubMenu();

            var nuevaReservaView = new ReservationView(usuarioActual, rolActual, ObtenerIdUsuarioActual());

            // Configurar callback
            nuevaReservaView.OnReservaGuardada = () =>
            {
                MostrarVistaInicio();
            };

            CargarVistaEnPanel(nuevaReservaView);
        }

        private void MostrarVistaListaReservas()
        {
            LimpiarContentPanel();
            OcultarSubMenu();

            var listaReservasView = new ReservationList(usuarioActual, rolActual);
            CargarVistaEnPanel(listaReservasView);
        }

        private void MostrarVistaCheckIn()
        {
            LimpiarContentPanel();
            OcultarSubMenu();

            int usuarioId = ObtenerIdUsuarioActual();
            if (usuarioId == -1)
            {
                MessageBox.Show("No se pudo obtener el ID del usuario actual", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var checkInView = new CheckInView(usuarioId);
            CargarVistaEnPanel(checkInView);
        }

        private void MostrarVistaCheckOut()
        {
            LimpiarContentPanel();
            OcultarSubMenu();

            int usuarioId = ObtenerIdUsuarioActual();
            if (usuarioId == -1)
            {
                MessageBox.Show("No se pudo obtener el ID del usuario actual", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var checkOutView = new CheckOutView(usuarioId);
            CargarVistaEnPanel(checkOutView);
        }

        private void MostrarSubMenuReservaciones()
        {
            subMenuPanel.Width = subMenuPanel.Width == subMenuWidth ? 0 : subMenuWidth;
            subMenuPanel.Visible = subMenuPanel.Width > 0;
        }

        private void MostrarVistaClientes()
        {
            LimpiarContentPanel();
            OcultarSubMenu();

            var clientesView = new ClientsView(usuarioActual, rolActual);
            CargarVistaEnPanel(clientesView);
        }

        private void MostrarVistaHabitaciones()
        {
            LimpiarContentPanel();
            OcultarSubMenu();

            var habitacionesView = new RoomView(usuarioActual, rolActual);
            CargarVistaEnPanel(habitacionesView);
        }

        private void MostrarVistaInicio()
        {
            LimpiarContentPanel();
            OcultarSubMenu();

            var inicioView = new InicioView(usuarioActual, rolActual);
            CargarVistaEnPanel(inicioView);
        }

        private void MostrarVistaUsuarios()
        {
            LimpiarContentPanel();
            OcultarSubMenu();

            var usuariosView = new UsuariosView(usuarioActual, rolActual);
            CargarVistaEnPanel(usuariosView);
        }

        private void CargarVistaEnPanel(Form vista)
        {
            vista.TopLevel = false;
            vista.FormBorderStyle = FormBorderStyle.None;
            vista.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(vista);
            vista.Show();
        }

        private void LimpiarContentPanel()
        {
            foreach (Control control in contentPanel.Controls)
            {
                if (control is Form form)
                {
                    form.Close();
                    form.Dispose();
                }
            }
            contentPanel.Controls.Clear();
        }

        private void OcultarSubMenu()
        {
            subMenuPanel.Width = 0;
            subMenuPanel.Visible = false;
        }

        private int ObtenerIdUsuarioActual()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        string query = "SELECT id FROM usuarios WHERE nombre_usuario = @usuario";
                        var cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@usuario", usuarioActual);

                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener ID de usuario: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return -1;
        }
    }
}