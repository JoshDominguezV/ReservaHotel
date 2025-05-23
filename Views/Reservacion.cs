using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using Proyecto_PED.Models;

namespace Proyecto_PED.Views
{
    public partial class Reservacion : Form
    {
        private string usuarioActual;
        private string rolActual;
        private ConexionBD conexionBD;
        private Panel menuPanel;
        private Panel subMenuPanel;
        private Panel contentPanel;
        private const int menuWidth = 220;
        private const int subMenuWidth = 200;

        public Reservacion(string usuario, string rol)
        {
            usuarioActual = usuario;
            rolActual = rol;
            conexionBD = new ConexionBD();

            InitializeComponent();
            ConstruirInterfaz();
            MostrarPanelInicio();
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
            btnInicio.Click += (s, e) =>
            {
                OcultarSubMenu();
                MostrarPanelInicio();
            };

            btnReservaciones.Click += (s, e) =>
            {
                MostrarSubMenuReservaciones();
            };

            btnClientes.Click += (s, e) =>
            {
                OcultarSubMenu();
                MostrarRegistrarCliente();
            };

            btnHabitaciones.Click += (s, e) =>
            {
                OcultarSubMenu();
                MostrarHabitaciones();
            };

            btnGestionUsuarios.Click += (s, e) =>
            {
                OcultarSubMenu();
                MostrarGestionUsuarios();
            };

            btnCerrarSesion.Click += (s, e) => this.Close();

            // Ocultar gestión de usuarios si no es administrador
            btnGestionUsuarios.Visible = rolActual == "Administrador";

            // Orden de los controles (importante para el dock)
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

            btnNuevaReserva.Click += (s, e) => MostrarRegistrarReservacion();
            btnListaReservas.Click += (s, e) => MostrarReservacionesExistentes();
            btnCheckIn.Click += (s, e) => MostrarCheckIn();
            btnCheckOut.Click += (s, e) => MostrarCheckOut();

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
            MostrarRegistrarReservacion();
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

        private void MostrarPanelInicio()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            var lblBienvenido = new Label
            {
                Text = $"Bienvenido, {usuarioActual}",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 50)
            };

            var lblRol = new Label
            {
                Text = $"Rol: {rolActual}",
                Font = new Font("Segoe UI", 14),
                AutoSize = true,
                Location = new Point(50, 100)
            };

            panel.Controls.Add(lblRol);
            panel.Controls.Add(lblBienvenido);
            contentPanel.Controls.Add(panel);
        }

        private void MostrarGestionUsuarios()
        {
            LimpiarContentPanel();
            var usuarioView = new UsuariosView(usuarioActual, rolActual);
            usuarioView.TopLevel = false;
            usuarioView.FormBorderStyle = FormBorderStyle.None;
            usuarioView.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(usuarioView);
            usuarioView.Show();
        }

        private void MostrarHabitaciones()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var dgvHabitaciones = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Cargar datos de habitaciones
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        string query = "SELECT numero, tipo, precio, estado FROM habitaciones ORDER BY numero";
                        var adapter = new MySqlDataAdapter(query, conn);
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        dgvHabitaciones.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar habitaciones: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            panel.Controls.Add(dgvHabitaciones);
            contentPanel.Controls.Add(panel);
        }

        private void MostrarRegistrarCliente()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // [Resto del código para mostrar el formulario de cliente...]
            // Mantener la misma implementación que tenías antes
        }

        private void MostrarRegistrarReservacion()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // [Implementación del formulario de reserva...]
            // Mantener la misma implementación pero mejorada
        }

        private void MostrarReservacionesExistentes()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var dgvReservaciones = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Cargar reservaciones activas
            CargarReservacionesGrid(dgvReservaciones, soloActivas: true);

            panel.Controls.Add(dgvReservaciones);
            contentPanel.Controls.Add(panel);
        }

        private void MostrarCheckIn()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Implementar interfaz para check-in
            // Mostrar reservaciones pendientes de check-in
            // Permitir seleccionar una y hacer check-in
        }

        private void MostrarCheckOut()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Implementar interfaz para check-out
            // Mostrar reservaciones activas (check-in realizado)
            // Permitir seleccionar una y hacer check-out
        }

        private void CargarReservacionesGrid(DataGridView dgv, bool soloActivas = true)
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        string query = @"SELECT r.id, c.nombre AS cliente, h.numero AS habitacion, 
                                       h.tipo, r.fecha_entrada, r.fecha_salida, r.estado
                                       FROM reservas r
                                       JOIN clientes c ON r.cliente_id = c.id
                                       JOIN habitaciones h ON r.habitacion_id = h.id
                                       WHERE (@soloActivas = 0 OR r.estado IN ('Pendiente', 'Check-In'))
                                       ORDER BY r.fecha_entrada DESC";

                        var cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@soloActivas", soloActivas ? 1 : 0);

                        var adapter = new MySqlDataAdapter(cmd);
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        dgv.DataSource = dt;

                        // Formatear columnas
                        if (dgv.Columns.Contains("fecha_entrada") && dgv.Columns.Contains("fecha_salida"))
                        {
                            dgv.Columns["fecha_entrada"].DefaultCellStyle.Format = "dd/MM/yyyy";
                            dgv.Columns["fecha_salida"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar reservaciones: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // [Mantener los demás métodos como CargarClientesCombo, CargarHabitacionesCombo, etc.]


        private void CargarClientesCombo(ComboBox cmbClientes)
        {
            cmbClientes.Items.Clear();

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        string query = "SELECT id, nombre FROM clientes ORDER BY nombre";
                        var cmd = new MySqlCommand(query, conn);
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            cmbClientes.Items.Add(new ClienteItem
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre")
                            });
                        }

                        if (cmbClientes.Items.Count > 0)
                            cmbClientes.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarHabitacionesCombo(ComboBox cmbHabitaciones)
        {
            cmbHabitaciones.Items.Clear();

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        string query = "SELECT id, numero, tipo, precio FROM habitaciones WHERE disponible = TRUE ORDER BY numero";
                        var cmd = new MySqlCommand(query, conn);
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            cmbHabitaciones.Items.Add(new HabitacionItem
                            {
                                Id = reader.GetInt32("id"),
                                Numero = reader.GetString("numero"),
                                Tipo = reader.GetString("tipo"),
                                Precio = reader.GetDecimal("precio")
                            });
                        }

                        if (cmbHabitaciones.Items.Count > 0)
                            cmbHabitaciones.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar habitaciones: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CargarReservacionesGrid(DataGridView dgv)
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        string query = @"SELECT r.id, c.nombre AS cliente, h.numero AS habitacion, 
                                       h.tipo, r.fecha_entrada, r.fecha_salida
                                       FROM reservas r
                                       JOIN clientes c ON r.cliente_id = c.id
                                       JOIN habitaciones h ON r.habitacion_id = h.id
                                       ORDER BY r.fecha_entrada DESC";

                        var adapter = new MySqlDataAdapter(query, conn);
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        dgv.DataSource = dt;

                        // Formatear columnas
                        if (dgv.Columns.Contains("fecha_entrada") && dgv.Columns.Contains("fecha_salida"))
                        {
                            dgv.Columns["fecha_entrada"].DefaultCellStyle.Format = "dd/MM/yyyy";
                            dgv.Columns["fecha_salida"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar reservaciones: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}