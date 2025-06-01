using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using System.Data;

namespace Proyecto_PED.Views.Checks
{
    public partial class CheckInView : Form
    {
        private ConexionBD conexionBD;
        private int usuarioId;
        private int? reservaSeleccionadaId = null;
        private int? checkInId = null;

        // Controles UI
        private Guna2DataGridView dgvReservaciones;
        private Guna2Panel pnlDatosCliente;
        private Guna2Panel pnlDatosReserva;
        private Guna2Panel pnlCheckIn;
        private Guna2ComboBox cmbMetodoPago;
        private Guna2CheckBox chkDocumentosRecibidos;
        private Guna2NumericUpDown nudDepositoSeguridad;
        private Guna2TextBox txtObservaciones;
        private Guna2Button btnRealizarCheckIn;
        private Guna2Button btnCancelar;

        public CheckInView(int idUsuario)
        {
            usuarioId = idUsuario;
            conexionBD = new ConexionBD();
            InitializeComponent();
            InitializeUI();
            CargarReservacionesPendientes();
        }

        private void InitializeUI()
        {
            this.Text = "Proceso de Check-In";
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
                Text = "PROCESO DE CHECK-IN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 80),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Sección de reservaciones pendientes
            var lblSeccionReservaciones = new Label()
            {
                Text = "1. Seleccionar Reservación Pendiente",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Grid de reservaciones
            dgvReservaciones = new Guna2DataGridView()
            {
                Dock = DockStyle.Top,
                Height = 200,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 30,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Margin = new Padding(0, 10, 0, 20)
            };
            dgvReservaciones.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvReservaciones.SelectionChanged += (s, e) =>
            {
                if (dgvReservaciones.SelectedRows.Count > 0)
                {
                    reservaSeleccionadaId = Convert.ToInt32(dgvReservaciones.SelectedRows[0].Cells["id"].Value);
                    CargarDetallesReserva(reservaSeleccionadaId.Value);
                }
            };

            // Panel de datos del cliente
            pnlDatosCliente = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(240, 240, 245),
                BorderRadius = 10,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                BorderThickness = 1
            };

            var lblDatosCliente = new Label()
            {
                Text = "Datos del Cliente",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlDatosCliente.Controls.Add(lblDatosCliente);

            // Panel de datos de la reserva
            pnlDatosReserva = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.FromArgb(240, 240, 245),
                BorderRadius = 10,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                BorderThickness = 1
            };

            var lblDatosReserva = new Label()
            {
                Text = "Datos de la Reservación",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlDatosReserva.Controls.Add(lblDatosReserva);

            // Sección de check-in
            var lblSeccionCheckIn = new Label()
            {
                Text = "2. Información de Check-In",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Panel de check-in
            pnlCheckIn = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = Color.FromArgb(240, 240, 245),
                BorderRadius = 10,
                Padding = new Padding(10),
                BorderColor = Color.FromArgb(200, 200, 200),
                BorderThickness = 1
            };

            // Método de pago
            var lblMetodoPago = new Label()
            {
                Text = "Método de Pago:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };

            cmbMetodoPago = new Guna2ComboBox()
            {
                DataSource = new string[] { "Efectivo", "Tarjeta", "Transferencia", "Otro" },
                Location = new Point(150, 10),
                Size = new Size(150, 36),
                Font = new Font("Segoe UI", 10)
            };

            // Documentos recibidos
            chkDocumentosRecibidos = new Guna2CheckBox()
            {
                Text = "Documentos Recibidos",
                Checked = false,
                Location = new Point(10, 60),
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };

            // Depósito de seguridad
            var lblDeposito = new Label()
            {
                Text = "Depósito Seguridad:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 105),
                AutoSize = true
            };

            nudDepositoSeguridad = new Guna2NumericUpDown()
            {
                Value = 0,
                Minimum = 0,
                Maximum = 1000,
                Location = new Point(180, 100),
                Size = new Size(100, 36),
                Font = new Font("Segoe UI", 10)
            };

            // Observaciones
            var lblObservaciones = new Label()
            {
                Text = "Observaciones:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(350, 10),
                AutoSize = true
            };

            txtObservaciones = new Guna2TextBox()
            {
                PlaceholderText = "Ingrese observaciones...",
                Location = new Point(350, 40),
                Size = new Size(300, 100),
                Multiline = true,
                Font = new Font("Segoe UI", 10)
            };

            pnlCheckIn.Controls.Add(lblMetodoPago);
            pnlCheckIn.Controls.Add(cmbMetodoPago);
            pnlCheckIn.Controls.Add(chkDocumentosRecibidos);
            pnlCheckIn.Controls.Add(lblDeposito);
            pnlCheckIn.Controls.Add(nudDepositoSeguridad);
            pnlCheckIn.Controls.Add(lblObservaciones);
            pnlCheckIn.Controls.Add(txtObservaciones);

            // Panel de botones
            var pnlBotones = new Guna2Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };

            btnRealizarCheckIn = new Guna2Button()
            {
                Text = "REALIZAR CHECK-IN",
                Size = new Size(180, 40),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true,
                Enabled = false,
                Dock = DockStyle.Right
            };
            btnRealizarCheckIn.Click += (s, e) => RealizarCheckIn();

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

            pnlBotones.Controls.Add(btnRealizarCheckIn);
            pnlBotones.Controls.Add(btnCancelar);

            // Agregar controles al panel principal
            mainPanel.Controls.Add(pnlBotones);
            mainPanel.Controls.Add(pnlCheckIn);
            mainPanel.Controls.Add(lblSeccionCheckIn);
            mainPanel.Controls.Add(pnlDatosReserva);
            mainPanel.Controls.Add(pnlDatosCliente);
            mainPanel.Controls.Add(dgvReservaciones);
            mainPanel.Controls.Add(lblSeccionReservaciones);
            mainPanel.Controls.Add(lblTitulo);

            this.Controls.Add(mainPanel);
        }

        private void CargarReservacionesPendientes()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetPendingReservationsForCheckIn", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    // Verificar si hay datos
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No hay reservaciones pendientes para check-in", "Información",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    dgvReservaciones.DataSource = dt;

                    // Configurar columnas
                    if (dgvReservaciones.Columns.Count > 0)
                    {
                        dgvReservaciones.Columns["id"].Visible = false;
                        dgvReservaciones.Columns["cliente"].HeaderText = "Cliente";
                        dgvReservaciones.Columns["habitacion"].HeaderText = "Habitación";
                        dgvReservaciones.Columns["tipo_habitacion"].HeaderText = "Tipo";
                        dgvReservaciones.Columns["fecha_entrada"].HeaderText = "Entrada";
                        dgvReservaciones.Columns["fecha_salida"].HeaderText = "Salida";
                        dgvReservaciones.Columns["noches"].HeaderText = "Noches";
                        dgvReservaciones.Columns["precio_total"].HeaderText = "Total";
                        dgvReservaciones.Columns["recepcionista"].HeaderText = "Recepción";

                        // Formatear fechas
                        dgvReservaciones.Columns["fecha_entrada"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvReservaciones.Columns["fecha_salida"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar reservaciones: {ex.Message}\n\nDetalles:\n{ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetallesReserva(int reservaId)
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetReservationDetailsForCheckIn", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_reserva_id", reservaId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Limpiar paneles
                            pnlDatosCliente.Controls.Clear();
                            pnlDatosReserva.Controls.Clear();

                            // Agregar título a panel de cliente
                            var lblClienteTitulo = new Label()
                            {
                                Text = "Datos del Cliente",
                                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                Dock = DockStyle.Top,
                                Height = 25,
                                TextAlign = ContentAlignment.MiddleLeft
                            };
                            pnlDatosCliente.Controls.Add(lblClienteTitulo);

                            // Mostrar datos del cliente
                            var lblCliente = new Label()
                            {
                                Text = $"Cliente: {reader["cliente_nombre"]}",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosCliente.Controls.Add(lblCliente);

                            var lblDocumento = new Label()
                            {
                                Text = $"Documento: {reader["tipo_documento"]} {reader["documento_identidad"]}",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosCliente.Controls.Add(lblDocumento);

                            var lblContacto = new Label()
                            {
                                Text = $"Contacto: {reader["telefono"]} | {reader["correo"]}",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosCliente.Controls.Add(lblContacto);

                            // Agregar título a panel de reserva
                            var lblReservaTitulo = new Label()
                            {
                                Text = "Datos de la Reservación",
                                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                Dock = DockStyle.Top,
                                Height = 25,
                                TextAlign = ContentAlignment.MiddleLeft
                            };
                            pnlDatosReserva.Controls.Add(lblReservaTitulo);

                            // Mostrar datos de la reserva
                            var lblHabitacion = new Label()
                            {
                                Text = $"Habitación: {reader["habitacion_numero"]} ({reader["tipo_habitacion"]})",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosReserva.Controls.Add(lblHabitacion);

                            var fechaEntrada = Convert.ToDateTime(reader["fecha_entrada"]);
                            var fechaSalida = Convert.ToDateTime(reader["fecha_salida"]);
                            var noches = (fechaSalida - fechaEntrada).Days;

                            var lblFechas = new Label()
                            {
                                Text = $"Fechas: {fechaEntrada.ToShortDateString()} - " +
                                       $"{fechaSalida.ToShortDateString()} " +
                                       $"({noches} noches)",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosReserva.Controls.Add(lblFechas);

                            var lblOcupantes = new Label()
                            {
                                Text = $"Ocupantes: {reader["adultos"]} adultos, {reader["ninos"]} niños",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosReserva.Controls.Add(lblOcupantes);

                            var lblTotal = new Label()
                            {
                                Text = $"Total: {Convert.ToDecimal(reader["precio_total"]):C2}",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosReserva.Controls.Add(lblTotal);

                            var lblNotas = new Label()
                            {
                                Text = $"Notas: {reader["notas"]}",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosReserva.Controls.Add(lblNotas);
                        }
                    }
                }

                // Habilitar botón de check-in
                btnRealizarCheckIn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar detalles de reserva: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RealizarCheckIn()
        {
            if (!reservaSeleccionadaId.HasValue)
            {
                MessageBox.Show("Seleccione una reservación primero", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_PerformCheckIn", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_reserva_id", reservaSeleccionadaId.Value);
                    cmd.Parameters.AddWithValue("@p_usuario_id", usuarioId);
                    cmd.Parameters.AddWithValue("@p_metodo_pago", cmbMetodoPago.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@p_documentos_recibidos", chkDocumentosRecibidos.Checked);
                    cmd.Parameters.AddWithValue("@p_deposito_seguridad", nudDepositoSeguridad.Value);
                    cmd.Parameters.AddWithValue("@p_observaciones", txtObservaciones.Text);

                    var checkInId = cmd.ExecuteScalar();

                    MessageBox.Show($"Check-In realizado exitosamente. ID: {checkInId}", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar check-in: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}