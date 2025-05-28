using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using Proyecto_PED.Views.Clientes;

namespace Proyecto_PED.Views.Reservaciones
{
    public partial class ReservationView : Form
    {
        private ConexionBD conexionBD;
        private string usuarioActual;
        private string rolActual;
        private int usuarioId;
        private int? clienteSeleccionadoId = null;
        private int? habitacionSeleccionadaId = null;

        // Controles UI
        private Guna2TextBox txtBusquedaCliente;
        private Guna2Button btnAgregarCliente;
        private Guna2DataGridView dgvClientes;
        private Guna2DateTimePicker dtpFechaEntrada;
        private Guna2DateTimePicker dtpFechaSalida;
        private Guna2ComboBox cmbTipoHabitacion;
        private Guna2NumericUpDown nudAdultos;
        private Guna2NumericUpDown nudNinos;
        private Guna2TextBox txtNotas;
        private Guna2Button btnBuscarHabitaciones;
        private FlowLayoutPanel pnlHabitaciones;
        private Guna2Button btnConfirmarReserva;
        private Guna2Button btnCancelar;
        private Guna2DataGridView dgvServicios;
        private Guna2ComboBox cmbServicios;
        private Guna2NumericUpDown nudCantidadServicio;
        private Guna2DateTimePicker dtpFechaServicio;
        private Guna2TextBox txtNotasServicio;
        private Guna2Button btnAgregarServicio;
        private Guna2Button btnEliminarServicio;
        private Label lblTotalServicios;
        private Label lblTotalGeneral;
        private decimal precioTotalServicios = 0;

        public ReservationView(string usuario, string rol, int idUsuario)
        {
            usuarioActual = usuario;
            rolActual = rol;
            usuarioId = idUsuario;
            conexionBD = new ConexionBD();

            InitializeUI();
            CargarTiposHabitacion();
            CargarServiciosDisponibles();
        }

        private void InitializeUI()
        {
            this.Text = "Nueva Reservación";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 240, 245);

            // Panel principal
            var mainPanel = new Guna2Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderRadius = 20,
                Padding = new Padding(20),
                ShadowDecoration = {
                    Enabled = true,
                    Color = Color.FromArgb(150, 150, 150),
                    Depth = 20
                }
            };

            // Título
            var lblTitulo = new Label()
            {
                Text = "NUEVA RESERVACIÓN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 80),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Sección de búsqueda de cliente
            var lblSeccionCliente = new Label()
            {
                Text = "1. Seleccionar Cliente",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var pnlBusquedaCliente = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Transparent
            };

            txtBusquedaCliente = new Guna2TextBox()
            {
                PlaceholderText = "Buscar cliente por nombre, apellido o documento...",
                Dock = DockStyle.Left,
                Width = 500,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
            };
            txtBusquedaCliente.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                    BuscarClientes();
            };
            btnAgregarCliente = new Guna2Button()
            {
                Text = "Nuevo Cliente",
                Size = new Size(120, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true,
                Dock = DockStyle.Left,
                Margin = new Padding(10, 0, 0, 0)
            };
            btnAgregarCliente.Click += (s, e) => AgregarNuevoCliente();

            pnlBusquedaCliente.Controls.Add(btnAgregarCliente);
            pnlBusquedaCliente.Controls.Add(txtBusquedaCliente);

            // Grid de clientes
            dgvClientes = new Guna2DataGridView()
            {
                Dock = DockStyle.Top,
                Height = 150,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 30,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Margin = new Padding(0, 10, 0, 10)
            };
            dgvClientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvClientes.SelectionChanged += (s, e) => {
                if (dgvClientes.SelectedRows.Count > 0)
                {
                    clienteSeleccionadoId = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["Id"].Value);

                    // Habilitar botón de confirmar si también hay habitación seleccionada
                    btnConfirmarReserva.Enabled = clienteSeleccionadoId.HasValue && habitacionSeleccionadaId.HasValue;
                }
            };

            // Sección de fechas y tipo de habitación
            var lblSeccionFechas = new Label()
            {
                Text = "2. Seleccionar Fechas y Tipo de Habitación",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var pnlFechas = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Transparent
            };

            var lblFechaEntrada = new Label()
            {
                Text = "Fecha Entrada:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 110,
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpFechaEntrada = new Guna2DateTimePicker()
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                MinDate = DateTime.Today,
                Width = 150,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Margin = new Padding(5, 0, 20, 0)
            };

            var lblFechaSalida = new Label()
            {
                Text = "Fecha Salida:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 110,
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpFechaSalida = new Guna2DateTimePicker()
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(1),
                MinDate = DateTime.Today.AddDays(1),
                Width = 150,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Margin = new Padding(5, 0, 20, 0)
            };

            var lblTipoHabitacion = new Label()
            {
                Text = "Tipo Habitación:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbTipoHabitacion = new Guna2ComboBox()
            {
                Width = 150,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            btnBuscarHabitaciones = new Guna2Button()
            {
                Text = "Buscar Habitaciones",
                Size = new Size(150, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true,
                Dock = DockStyle.Right
            };
            btnBuscarHabitaciones.Click += (s, e) => BuscarHabitacionesDisponibles();

            pnlFechas.Controls.Add(btnBuscarHabitaciones);
            pnlFechas.Controls.Add(cmbTipoHabitacion);
            pnlFechas.Controls.Add(lblTipoHabitacion);
            pnlFechas.Controls.Add(dtpFechaSalida);
            pnlFechas.Controls.Add(lblFechaSalida);
            pnlFechas.Controls.Add(dtpFechaEntrada);
            pnlFechas.Controls.Add(lblFechaEntrada);


            // Sección de ocupantes
            var lblSeccionOcupantes = new Label()
            {
                Text = "3. Detalles de Ocupación",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var pnlOcupantes = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Transparent
            };

            var lblAdultos = new Label()
            {
                Text = "Adultos:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 80,
                TextAlign = ContentAlignment.MiddleLeft
            };

            nudAdultos = new Guna2NumericUpDown()
            {
                Value = 1,
                Minimum = 1,
                Maximum = 10,
                Width = 60,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Margin = new Padding(5, 0, 20, 0)
            };

            var lblNinos = new Label()
            {
                Text = "Niños:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 80,
                TextAlign = ContentAlignment.MiddleLeft
            };

            nudNinos = new Guna2NumericUpDown()
            {
                Value = 0,
                Minimum = 0,
                Maximum = 10,
                Width = 60,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Margin = new Padding(5, 0, 20, 0)
            };

            var lblNotas = new Label()
            {
                Text = "Notas:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 80,
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtNotas = new Guna2TextBox()
            {
                PlaceholderText = "Notas adicionales...",
                Dock = DockStyle.Fill,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
            };

            pnlOcupantes.Controls.Add(txtNotas);
            pnlOcupantes.Controls.Add(lblNotas);
            pnlOcupantes.Controls.Add(nudNinos);
            pnlOcupantes.Controls.Add(lblNinos);
            pnlOcupantes.Controls.Add(nudAdultos);
            pnlOcupantes.Controls.Add(lblAdultos);


            // Sección de habitaciones disponibles

            var lblSeccionHabitaciones = new Label()
            {
                Text = "4. Habitaciones Disponibles",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlHabitaciones = new FlowLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = 150,
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 10, 0, 10)
            };

            // Sección de Servicios Adicionales
            var lblSeccionServicios = new Label()
            {
                Text = "5. Servicios Adicionales",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Panel para agregar nuevos servicios
            var pnlAgregarServicio = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Transparent
            };

            var lblServicio = new Label()
            {
                Text = "Servicio:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 80,
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbServicios = new Guna2ComboBox()
            {
                Width = 150,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5, 0, 20, 0)
            };

            var lblCantidadServicio = new Label()
            {
                Text = "Cantidad:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 100,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(10, 0, 0, 0)
            };

            nudCantidadServicio = new Guna2NumericUpDown()
            {
                Value = 1,
                Minimum = 1,
                Maximum = 10,
                Width = 60,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left
            };

            var lblFechaServicio = new Label()
            {
                Text = "Fecha:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 80,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(10, 0, 0, 0)
            };

            dtpFechaServicio = new Guna2DateTimePicker()
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Width = 150,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left
            };

            var lblNotasServicio = new Label()
            {
                Text = "Notas:",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left,
                Width = 80,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(10, 0, 0, 0)
            };

            txtNotasServicio = new Guna2TextBox()
            {
                PlaceholderText = "Notas del servicio...",
                Width = 200,
                Height = 36,
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Left
            };

            btnAgregarServicio = new Guna2Button()
            {
                Text = "Agregar",
                Size = new Size(100, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 180, 130),
                ForeColor = Color.White,
                Animated = true,
                Dock = DockStyle.Right
            };
            btnAgregarServicio.Click += (s, e) => AgregarServicio();

            pnlAgregarServicio.Controls.Add(btnAgregarServicio);
            pnlAgregarServicio.Controls.Add(dtpFechaServicio);
            pnlAgregarServicio.Controls.Add(lblFechaServicio);
            pnlAgregarServicio.Controls.Add(nudCantidadServicio);
            pnlAgregarServicio.Controls.Add(lblCantidadServicio);
            pnlAgregarServicio.Controls.Add(cmbServicios);
            pnlAgregarServicio.Controls.Add(lblServicio);
            pnlAgregarServicio.Controls.Add(txtNotasServicio);
            pnlAgregarServicio.Controls.Add(lblNotasServicio);

            // Grid de servicios
            dgvServicios = new Guna2DataGridView()
            {
                Dock = DockStyle.Top,
                Height = 150,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 30,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Margin = new Padding(0, 0, 0, 10)
            };
            dgvServicios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvServicios.SelectionChanged += (s, e) => {
                btnEliminarServicio.Enabled = dgvServicios.SelectedRows.Count > 0;
            };

            // Panel para eliminar servicios
            var pnlEliminarServicio = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Transparent
            };

            btnEliminarServicio = new Guna2Button()
            {
                Text = "Eliminar Servicio",
                Size = new Size(150, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Animated = true,
                Dock = DockStyle.Left,
                Enabled = false
            };
            btnEliminarServicio.Click += (s, e) => EliminarServicio();

            pnlEliminarServicio.Controls.Add(btnEliminarServicio);

            // Panel de totales
            var pnlTotales = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.Transparent
            };

            lblTotalServicios = new Label()
            {
                Text = "Total Servicios: $0.00",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Right,
                Width = 150,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(0, 0, 20, 0)
            };
            lblTotalGeneral = new Label()
            {
                Text = "Total General: $0.00",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Right,
                Width = 150,
                TextAlign = ContentAlignment.MiddleRight
            };

            pnlTotales.Controls.Add(lblTotalGeneral);
            pnlTotales.Controls.Add(lblTotalServicios);

            // Panel de botones
            var pnlBotones = new Guna2Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };

            btnConfirmarReserva = new Guna2Button()
            {
                Text = "CONFIRMAR RESERVA",
                Size = new Size(180, 40),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true,
                Enabled = false,
                Dock = DockStyle.Right
            };
            btnConfirmarReserva.Click += (s, e) => ConfirmarReserva();
            btnCancelar = new Guna2Button()
            {
                Text = "CANCELAR",
                Size = new Size(120, 40),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Animated = true,
                Dock = DockStyle.Left
            };
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            pnlBotones.Controls.Add(btnConfirmarReserva);
            pnlBotones.Controls.Add(btnCancelar);

            // Agregar controles al panel principal
            mainPanel.Controls.Add(pnlBotones);
            mainPanel.Controls.Add(pnlTotales);
            mainPanel.Controls.Add(pnlEliminarServicio);
            mainPanel.Controls.Add(dgvServicios);
            mainPanel.Controls.Add(pnlAgregarServicio);
            mainPanel.Controls.Add(lblSeccionServicios);
            mainPanel.Controls.Add(pnlHabitaciones);
            mainPanel.Controls.Add(lblSeccionHabitaciones);
            mainPanel.Controls.Add(pnlOcupantes);
            mainPanel.Controls.Add(lblSeccionOcupantes);
            mainPanel.Controls.Add(pnlFechas);
            mainPanel.Controls.Add(lblSeccionFechas);
            mainPanel.Controls.Add(dgvClientes);
            mainPanel.Controls.Add(lblSeccionCliente);
            mainPanel.Controls.Add(pnlBusquedaCliente);
            mainPanel.Controls.Add(lblTitulo);

            this.Controls.Add(mainPanel);
        }

        private void CargarTiposHabitacion()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetRoomTypesForReservation", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    cmbTipoHabitacion.DataSource = dt;
                    cmbTipoHabitacion.DisplayMember = "nombre";
                    cmbTipoHabitacion.ValueMember = "id";

                    // Agregar opción "Todos los tipos"
                    var row = dt.NewRow();
                    row["id"] = 0;
                    row["nombre"] = "Todos los tipos";
                    dt.Rows.InsertAt(row, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tipos de habitación: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuscarClientes()
        {
            if (string.IsNullOrWhiteSpace(txtBusquedaCliente.Text))
            {
                MessageBox.Show("Ingrese un término de búsqueda", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_SearchClientsForReservation", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_search", txtBusquedaCliente.Text);

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    dgvClientes.DataSource = dt;

                    // Configurar columnas
                    if (dgvClientes.Columns.Count > 0)
                    {
                        dgvClientes.Columns["Id"].Visible = false;
                        dgvClientes.Columns["nombre_completo"].HeaderText = "Nombre Completo";
                        dgvClientes.Columns["tipo_documento"].HeaderText = "Tipo Doc.";
                        dgvClientes.Columns["documento_identidad"].HeaderText = "N° Documento";
                        dgvClientes.Columns["telefono"].HeaderText = "Teléfono";
                        dgvClientes.Columns["correo"].HeaderText = "Correo";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar clientes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarNuevoCliente()
        {
            var clienteForm = new ClientForm();
            if (clienteForm.ShowDialog() == DialogResult.OK)
            {
                // Actualizar búsqueda para mostrar el nuevo cliente
                txtBusquedaCliente.Text = "";
                BuscarClientes();
            }
        }

        private void BuscarHabitacionesDisponibles()
        {
            if (!clienteSeleccionadoId.HasValue)
            {
                MessageBox.Show("Seleccione un cliente primero", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpFechaSalida.Value <= dtpFechaEntrada.Value)
            {
                MessageBox.Show("La fecha de salida debe ser posterior a la de entrada", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                pnlHabitaciones.Controls.Clear();

                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetAvailableRooms", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_fecha_entrada", dtpFechaEntrada.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@p_fecha_salida", dtpFechaSalida.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@p_tipo_habitacion_id", cmbTipoHabitacion.SelectedValue);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var habitacion = new
                            {
                                Id = reader.GetInt32("id"),
                                Numero = reader.GetString("numero"),
                                Tipo = reader.GetString("tipo"),
                                Capacidad = reader.GetInt32("capacidad"),
                                Precio = reader.GetDecimal("precio"),
                                Piso = reader.GetInt32("piso")
                            };

                            var card = CrearCardHabitacion(habitacion);
                            pnlHabitaciones.Controls.Add(card);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar habitaciones: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Guna2Panel CrearCardHabitacion(dynamic habitacion)
        {
            var card = new Guna2Panel()
            {
                Size = new Size(200, 120),
                Margin = new Padding(10),
                BorderRadius = 15,
                BorderThickness = 2,
                BorderColor = Color.FromArgb(200, 200, 200),
                Cursor = Cursors.Hand,
                Tag = habitacion.Id,
                FillColor = Color.White
            };

            // Header con número de habitación
            var lblHeader = new Label()
            {
                Text = $"Hab. {habitacion.Numero}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White
            };

            // Contenido
            var contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblTipo = new Label()
            {
                Text = $"{habitacion.Tipo}",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5),
                Cursor = Cursors.Hand
            };

            var lblPrecio = new Label()
            {
                Text = $"${habitacion.Precio}/noche",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 25),
                Cursor = Cursors.Hand
            };

            var lblCapacidad = new Label()
            {
                Text = $"Capacidad: {habitacion.Capacidad}",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 45),
                Cursor = Cursors.Hand
            };

            var lblPiso = new Label()
            {
                Text = $"Piso: {habitacion.Piso}",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 65),
                Cursor = Cursors.Hand
            };

            // Configurar eventos para los labels también
            foreach (var lbl in new[] { lblTipo, lblPrecio, lblCapacidad, lblPiso })
            {
                lbl.Click += (s, e) => SeleccionarHabitacion(card, habitacion.Id);
            }

            contentPanel.Controls.Add(lblPiso);
            contentPanel.Controls.Add(lblCapacidad);
            contentPanel.Controls.Add(lblPrecio);
            contentPanel.Controls.Add(lblTipo);

            card.Controls.Add(contentPanel);
            card.Controls.Add(lblHeader);

            // Evento Click para seleccionar la habitación
            card.Click += (s, e) => SeleccionarHabitacion(card, habitacion.Id);

            return card;
        }

        private void SeleccionarHabitacion(Guna2Panel cardSeleccionada, int idHabitacion)
        {
            // Deseleccionar todas las cards primero
            foreach (Control control in pnlHabitaciones.Controls)
            {
                if (control is Guna2Panel panel)
                {
                    panel.BorderColor = Color.FromArgb(200, 200, 200);
                    panel.FillColor = Color.White;
                    panel.ShadowDecoration.Enabled = false;
                }
            }

            // Seleccionar la card actual
            cardSeleccionada.BorderColor = Color.FromArgb(70, 130, 180);
            cardSeleccionada.FillColor = Color.FromArgb(240, 240, 245);
            cardSeleccionada.ShadowDecoration.Enabled = true;
            cardSeleccionada.ShadowDecoration.Color = Color.FromArgb(70, 130, 180);
            habitacionSeleccionadaId = idHabitacion;

            // Habilitar botón de confirmar si también hay cliente seleccionado
            btnConfirmarReserva.Enabled = clienteSeleccionadoId.HasValue && habitacionSeleccionadaId.HasValue;

            // Forzar el repintado para ver los cambios inmediatamente
            cardSeleccionada.Refresh();
        }

        private void ConfirmarReserva()
        {
            if (!clienteSeleccionadoId.HasValue)
            {
                MessageBox.Show("Seleccione un cliente primero", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!habitacionSeleccionadaId.HasValue)
            {
                MessageBox.Show("Seleccione una habitación primero", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpFechaSalida.Value <= dtpFechaEntrada.Value)
            {
                MessageBox.Show("La fecha de salida debe ser posterior a la de entrada", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calcular precio total
            int noches = (dtpFechaSalida.Value - dtpFechaEntrada.Value).Days;
            decimal precioNoche = 0;

            // Obtener precio de la habitación seleccionada
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("SELECT precio_base FROM tipos_habitacion th " +
                                             "JOIN habitaciones h ON th.id = h.tipo_id " +
                                             "WHERE h.id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", habitacionSeleccionadaId.Value);
                    precioNoche = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener precio de habitación: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal precioTotal = noches * precioNoche + precioTotalServicios;

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    // Crear la reserva
                    var cmd = new MySqlCommand("sp_CreateReservation", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_cliente_id", clienteSeleccionadoId.Value);
                    cmd.Parameters.AddWithValue("@p_habitacion_id", habitacionSeleccionadaId.Value);
                    cmd.Parameters.AddWithValue("@p_usuario_id", usuarioId);
                    cmd.Parameters.AddWithValue("@p_fecha_entrada", dtpFechaEntrada.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@p_fecha_salida", dtpFechaSalida.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@p_adultos", nudAdultos.Value);
                    cmd.Parameters.AddWithValue("@p_ninos", nudNinos.Value);
                    cmd.Parameters.AddWithValue("@p_precio_total", precioTotal);
                    cmd.Parameters.AddWithValue("@p_notas", txtNotas.Text);

                    var reservaId = Convert.ToInt32(cmd.ExecuteScalar());

                    // Agregar servicios si hay alguno
                    if (dgvServicios.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow row in dgvServicios.Rows)
                        {
                            var cmdServicio = new MySqlCommand("sp_AddServiceToReservation", conn);
                            cmdServicio.CommandType = CommandType.StoredProcedure;
                            cmdServicio.Parameters.AddWithValue("@p_reserva_id", reservaId);
                            cmdServicio.Parameters.AddWithValue("@p_servicio_id", row.Cells["id"].Value);
                            cmdServicio.Parameters.AddWithValue("@p_cantidad", row.Cells["cantidad"].Value);
                            cmdServicio.Parameters.AddWithValue("@p_fecha", ((DateTime)row.Cells["fecha"].Value).ToString("yyyy-MM-dd"));
                            cmdServicio.Parameters.AddWithValue("@p_precio_unitario", row.Cells["precio_unitario"].Value);
                            cmdServicio.Parameters.AddWithValue("@p_notas", row.Cells["notas"].Value?.ToString() ?? "");
                            cmdServicio.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Reserva creada exitosamente. N° {reservaId}", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear reserva: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarServiciosDisponibles()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetAvailableServices", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    cmbServicios.DataSource = dt;
                    cmbServicios.DisplayMember = "nombre";
                    cmbServicios.ValueMember = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar servicios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarServicio()
        {
            if (cmbServicios.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un servicio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Obtener datos del servicio seleccionado
                var servicio = (DataRowView)cmbServicios.SelectedItem;
                int servicioId = Convert.ToInt32(servicio["id"]);
                string nombreServicio = servicio["nombre"].ToString();
                int cantidad = (int)nudCantidadServicio.Value;
                DateTime fecha = dtpFechaServicio.Value;
                string notas = txtNotasServicio.Text; // Ahora está inicializado
                decimal precio = Convert.ToDecimal(servicio["precio"]);

                // Obtener o crear DataTable
                DataTable dt;
                if (dgvServicios.DataSource == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("id", typeof(int));
                    dt.Columns.Add("servicio", typeof(string));
                    dt.Columns.Add("cantidad", typeof(int));
                    dt.Columns.Add("fecha", typeof(DateTime));
                    dt.Columns.Add("precio_unitario", typeof(decimal));
                    dt.Columns.Add("total", typeof(decimal));
                    dt.Columns.Add("notas", typeof(string));
                    dgvServicios.DataSource = dt;
                }
                else
                {
                    dt = (DataTable)dgvServicios.DataSource;
                }

                // Agregar nueva fila
                DataRow newRow = dt.NewRow();
                newRow["id"] = servicioId;
                newRow["servicio"] = nombreServicio;
                newRow["cantidad"] = cantidad;
                newRow["fecha"] = fecha;
                newRow["precio_unitario"] = precio;
                newRow["total"] = cantidad * precio;
                newRow["notas"] = notas;
                dt.Rows.Add(newRow);

                // Calcular totales
                precioTotalServicios = dt.AsEnumerable()
                    .Sum(row => Convert.ToDecimal(row["total"]));
                ActualizarTotales();

                // Limpiar controles
                nudCantidadServicio.Value = 1;
                txtNotasServicio.Text = "";
                cmbServicios.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar servicio: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarServicio()
        {
            if (dgvServicios.SelectedRows.Count == 0) return;

            var confirmacion = MessageBox.Show("¿Eliminar este servicio?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                var dt = dgvServicios.DataSource as DataTable;
                if (dt != null)
                {
                    dt.Rows.RemoveAt(dgvServicios.SelectedRows[0].Index);

                    // Recalcular totales
                    precioTotalServicios = dt.AsEnumerable()
                        .Sum(row => row.Field<decimal>("total"));
                    ActualizarTotales();
                }
            }
        }

        private void ActualizarTotales()
        {
            lblTotalServicios.Text = $"Total Servicios: {precioTotalServicios:C2}";

            // Calcular total general si hay habitación seleccionada
            if (habitacionSeleccionadaId.HasValue)
            {
                int noches = (dtpFechaSalida.Value - dtpFechaEntrada.Value).Days;
                decimal precioNoche = 0;

                try
                {
                    using (var conn = conexionBD.ObtenerConexion())
                    {
                        var cmd = new MySqlCommand("SELECT precio_base FROM tipos_habitacion th " +
                                                 "JOIN habitaciones h ON th.id = h.tipo_id " +
                                                 "WHERE h.id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", habitacionSeleccionadaId.Value);
                        precioNoche = Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                }
                catch { /* Ignorar errores en este cálculo */ }

                decimal totalHabitacion = noches * precioNoche;
                decimal totalGeneral = totalHabitacion + precioTotalServicios;

                lblTotalGeneral.Text = $"Total General: {totalGeneral:C2}";
            }
        }
    }
}