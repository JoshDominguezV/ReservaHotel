using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using System.Data;

namespace Proyecto_PED.Views.Reservaciones
{
    public partial class ReservationList : Form
    {
        private ConexionBD conexionBD;
        private string usuarioActual;
        private string rolActual;
        private int? reservaSeleccionadaId = null;

        // Controles
        private Guna2DataGridView dgvReservas;
        private Guna2TextBox txtBusqueda;
        private Guna2Button btnActualizar;
        private Guna2Button btnEliminar;
        private Guna2Button btnModificar;
        private Guna2Button btnCheckIn;
        private Guna2ComboBox cmbFiltroEstado;

        public ReservationList(string usuario, string rol)
        {
            usuarioActual = usuario;
            rolActual = rol;
            conexionBD = new ConexionBD();
            InitializeUI();
            CargarReservas();
            ConfigurarPermisos();
        }

        private void ConfigurarPermisos()
        {
            bool esAdministrador = rolActual == "Administrador";
            bool esRecepcionista = rolActual == "Recepcionista";

            btnModificar.Visible = esAdministrador;
            btnEliminar.Visible = esAdministrador;
            btnCheckIn.Visible = esRecepcionista || esAdministrador;
        }

        private void InitializeUI()
        {
            this.Text = "Listado de Reservaciones";
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(240, 240, 245);
            this.FormBorderStyle = FormBorderStyle.None;

            // Panel principal con sombra
            var mainPanel = new Guna2Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderRadius = 20,
                Margin = new Padding(20),
                ShadowDecoration = {
                    Enabled = true,
                    Color = Color.FromArgb(150, 150, 150),
                    Depth = 20
                }
            };

            // Header con título
            var headerPanel = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(50, 50, 80),
                BorderRadius = 20,
                Padding = new Padding(20, 0, 20, 0)
            };

            var lblTitulo = new Label()
            {
                Text = "Listado de Reservaciones",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Height = headerPanel.Height
            };

            // Panel de controles superiores
            var controlsPanel = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 78,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };

            txtBusqueda = new Guna2TextBox()
            {
                PlaceholderText = "Buscar reservación...",
                Width = 250,
                Height = 36,
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                Margin = new Padding(0, 0, 10, 0)
            };
            txtBusqueda.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                    BuscarReservas();
            };

            cmbFiltroEstado = new Guna2ComboBox()
            {
                Width = 200,
                Height = 36,
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                Margin = new Padding(0, 0, 10, 0)
            };
            cmbFiltroEstado.Items.AddRange(new object[] { "Todas", "Pendiente", "Check-In", "Check-Out", "Cancelada", "No-Show" });
            cmbFiltroEstado.SelectedIndex = 0;
            cmbFiltroEstado.SelectedIndexChanged += (s, e) => FiltrarPorEstado();

            btnActualizar = new Guna2Button()
            {
                Text = "Actualizar",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(100, 100, 150),
                ForeColor = Color.White,
                Animated = true
            };
            btnActualizar.Click += (s, e) => CargarReservas();

            btnModificar = new Guna2Button()
            {
                Text = "Modificar",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true
            };
            btnModificar.Click += (s, e) => ModificarReserva();

            btnEliminar = new Guna2Button()
            {
                Text = "Eliminar",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Animated = true
            };
            btnEliminar.Click += (s, e) => EliminarReserva();

            btnCheckIn = new Guna2Button()
            {
                Text = "Check-In",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(60, 179, 113),
                ForeColor = Color.White,
                Animated = true
            };
            btnCheckIn.Click += (s, e) => ProcesarCheckIn();

            // DataGridView
            dgvReservas = new Guna2DataGridView()
            {
                Name = "dgvReservas",
                Dock = DockStyle.Fill,
                Margin = new Padding(20, 10, 20, 20),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };

            // Configurar estilos del DataGridView
            dgvReservas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 80);
            dgvReservas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvReservas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReservas.RowsDefaultCellStyle.BackColor = Color.White;
            dgvReservas.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvReservas.RowsDefaultCellStyle.ForeColor = Color.FromArgb(50, 50, 50);
            dgvReservas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvReservas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgvReservas.DefaultCellStyle.SelectionForeColor = Color.FromArgb(50, 50, 50);

            dgvReservas.SelectionChanged += (s, e) =>
            {
                reservaSeleccionadaId = dgvReservas.SelectedRows.Count > 0 ?
                    (int?)Convert.ToInt32(dgvReservas.SelectedRows[0].Cells["id"].Value) :
                    null;
            };

            // Agregar controles a los paneles
            var controlsFlow = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };
            controlsFlow.Controls.AddRange(new Control[] {
                txtBusqueda, cmbFiltroEstado,
                btnActualizar, btnModificar,
                btnEliminar, btnCheckIn
            });

            controlsPanel.Controls.Add(controlsFlow);
            headerPanel.Controls.Add(lblTitulo);

            var contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            contentPanel.Controls.Add(dgvReservas);

            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(controlsPanel);
            mainPanel.Controls.Add(headerPanel);

            this.Controls.Add(mainPanel);
        }

        private void CargarReservas()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetAllReservations", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    dgvReservas.DataSource = dt;

                    if (dgvReservas.Columns.Count > 0)
                    {
                        dgvReservas.Columns["id"].Visible = false;
                        dgvReservas.Columns["cliente"].HeaderText = "Cliente";
                        dgvReservas.Columns["habitacion"].HeaderText = "Habitación";
                        dgvReservas.Columns["fecha_entrada"].HeaderText = "Entrada";
                        dgvReservas.Columns["fecha_salida"].HeaderText = "Salida";
                        dgvReservas.Columns["estado"].HeaderText = "Estado";
                        dgvReservas.Columns["precio_total"].HeaderText = "Total";
                        dgvReservas.Columns["recepcionista"].HeaderText = "Recepcionista";

                        // Formatear columnas
                        dgvReservas.Columns["fecha_entrada"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvReservas.Columns["fecha_salida"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvReservas.Columns["precio_total"].DefaultCellStyle.Format = "C2";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar reservaciones: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuscarReservas()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_SearchReservations", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_busqueda", txtBusqueda.Text);

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    dgvReservas.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar reservaciones: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrarPorEstado()
        {
            try
            {
                string estado = cmbFiltroEstado.SelectedItem.ToString();

                if (estado == "Todas")
                {
                    CargarReservas();
                    return;
                }

                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_FilterReservationsByStatus", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_estado", estado);

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    dgvReservas.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar reservaciones: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModificarReserva()
        {
            if (reservaSeleccionadaId == null)
            {
                MessageBox.Show("Seleccione una reservación para modificar", "Advertencia",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetReservationDetails", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_reserva_id", reservaSeleccionadaId);

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        int usuarioId = ObtenerIdUsuarioActual(); // Método que ya existe en Main.cs
                        var formEdicion = new EditReservationForm(dt.Rows[0], usuarioId);
                        if (formEdicion.ShowDialog() == DialogResult.OK)
                        {
                            CargarReservas();
                            MessageBox.Show("Reservación actualizada correctamente", "Éxito",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar reservación: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ObtenerIdUsuarioActual()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("SELECT id FROM usuarios WHERE nombre_usuario = @usuario", conn);
                    cmd.Parameters.AddWithValue("@usuario", usuarioActual);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                return -1;
            }
        }

        private void EliminarReserva()
        {
            if (reservaSeleccionadaId == null)
            {
                MessageBox.Show("Seleccione una reservación para eliminar", "Advertencia",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show("¿Está seguro de eliminar esta reservación?", "Confirmar",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    using (var conn = conexionBD.ObtenerConexion())
                    {
                        var cmd = new MySqlCommand("sp_DeleteReservation", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", reservaSeleccionadaId);
                        cmd.ExecuteNonQuery();
                    }

                    CargarReservas();
                    MessageBox.Show("Reservación eliminada correctamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar reservación: {ex.Message}", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ProcesarCheckIn()
        {
            if (reservaSeleccionadaId == null)
            {
                MessageBox.Show("Seleccione una reservación para Check-In", "Advertencia",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    // Verificar que la reserva esté en estado Pendiente
                    var cmdVerificar = new MySqlCommand(
                        "SELECT estado_id FROM reservas WHERE id = @id", conn);
                    cmdVerificar.Parameters.AddWithValue("@id", reservaSeleccionadaId);
                    var estado = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                    if (estado != 1) // 1 = Pendiente
                    {
                        MessageBox.Show("Solo se puede hacer Check-In a reservaciones pendientes", "Advertencia",
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Actualizar estado a Check-In
                    var cmdActualizar = new MySqlCommand(
                        "UPDATE reservas SET estado_id = 2 WHERE id = @id", conn); // 2 = Check-In
                    cmdActualizar.Parameters.AddWithValue("@id", reservaSeleccionadaId);
                    cmdActualizar.ExecuteNonQuery();

                    CargarReservas();
                    MessageBox.Show("Check-In realizado correctamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar Check-In: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}