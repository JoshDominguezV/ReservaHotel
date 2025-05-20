using System;
using System.Drawing;
using System.Windows.Forms;
using Proyecto_PED.Database;

namespace Proyecto_PED.Views
{
    public partial class DashboardForm : Form
    {
        private string usuarioActual;
        private string rolActual;
        private ConexionBD conexionBD;
        private Panel menuPanel;
        private Panel subMenuPanel;
        private Panel contentPanel;
        private const int menuWidth = 220;
        private const int subMenuWidth = 200;

        public DashboardForm(string usuario, string rol)
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
                Width = 0, // Inicialmente colapsado
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
            var btnReservaciones = CrearBotonMenu("Reservaciones", null);
            var btnClientes = CrearBotonMenu("Clientes", null);
            var btnHabitaciones = CrearBotonMenu("Habitaciones", null);
            var btnGestionUsuarios = CrearBotonMenu("Usuarios", null);
            var btnCerrarSesion = CrearBotonMenu("Cerrar Sesión", null);

            // Eventos
            btnInicio.Click += (s, e) => {
                OcultarSubMenu();
                MostrarVistaInicio();
            };

            btnReservaciones.Click += (s, e) => {
                MostrarSubMenuReservaciones();
            };

            btnClientes.Click += (s, e) => {
                OcultarSubMenu();
                MostrarVistaClientes();
            };

            btnHabitaciones.Click += (s, e) => {
                OcultarSubMenu();
                MostrarVistaHabitaciones();
            };

            btnGestionUsuarios.Click += (s, e) => {
                OcultarSubMenu();
                MostrarVistaUsuarios();
            };

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
            var btn = new Button
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
                TextImageRelation = TextImageRelation.ImageBeforeText
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(51, 51, 76);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 100);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 50);

            return btn;
        }

        private Button CrearBotonSubMenu(string text)
        {
            var btn = new Button
            {
                Text = "  " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(20, 0, 0, 0)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(61, 61, 86);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 80, 110);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 60);

            return btn;
        }

        private void MostrarSubMenuReservaciones()
        {
            if (subMenuPanel.Width == subMenuWidth)
            {
                OcultarSubMenu();
                return;
            }

            subMenuPanel.Width = subMenuWidth;
            subMenuPanel.Visible = true;
            MostrarVistaNuevaReserva();
        }

        private void OcultarSubMenu()
        {
            subMenuPanel.Width = 0;
            subMenuPanel.Visible = false;
        }

        private void LimpiarContentPanel()
        {
            contentPanel.Controls.Clear();
        }

        #region Métodos para mostrar vistas
        
        private void MostrarVistaInicio()
        {
            LimpiarContentPanel();
            var vista = new InicioView(usuarioActual, rolActual);
            MostrarVistaEnPanel(vista);
        }

        private void MostrarVistaClientes()
        {
            LimpiarContentPanel();
            var vista = new ClientesView(conexionBD);
            MostrarVistaEnPanel(vista);
        }

        private void MostrarVistaHabitaciones()
        {
            LimpiarContentPanel();
            var vista = new HabitacionesView(conexionBD);
            MostrarVistaEnPanel(vista);
        }

        private void MostrarVistaUsuarios()
        {
            LimpiarContentPanel();
            var vista = new UsuariosView(usuarioActual, rolActual);
            MostrarVistaEnPanel(vista);
        }

        private void MostrarVistaNuevaReserva()
        {
            LimpiarContentPanel();
            var vista = new NuevaReservaView(conexionBD);
            MostrarVistaEnPanel(vista);
        }

        private void MostrarVistaListaReservas()
        {
            LimpiarContentPanel();
            var vista = new ListaReservasView(conexionBD);
            MostrarVistaEnPanel(vista);
        }

        private void MostrarVistaCheckIn()
        {
            LimpiarContentPanel();
            var vista = new CheckInView(conexionBD);
            MostrarVistaEnPanel(vista);
        }

        private void MostrarVistaCheckOut()
        {
            LimpiarContentPanel();
            var vista = new CheckOutView(conexionBD);
            MostrarVistaEnPanel(vista);
        }

        private void MostrarVistaEnPanel(Form vista)
        {
            vista.TopLevel = false;
            vista.FormBorderStyle = FormBorderStyle.None;
            vista.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(vista);
            vista.Show();
        }
        
        #endregion
    }
}