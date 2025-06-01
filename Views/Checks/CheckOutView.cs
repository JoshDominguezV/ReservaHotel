using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using System.Data;

namespace Proyecto_PED.Views.Checks
{
    public partial class CheckOutView : Form
    {
        private ConexionBD conexionBD;
        private int usuarioId;
        private int? checkInSeleccionadoId = null;
        private int? reservaSeleccionadaId = null;

        // Controles UI
        private Guna2DataGridView dgvCheckIns;
        private Guna2Panel pnlDatosCliente;
        private Guna2Panel pnlDatosReserva;
        private Guna2Panel pnlCheckOut;
        private Guna2ComboBox cmbEstadoHabitacion;
        private Guna2NumericUpDown nudCobrosAdicionales;
        private Guna2NumericUpDown nudDevolucionDeposito;
        private Guna2TextBox txtObservaciones;
        private Guna2Button btnRealizarCheckOut;
        private Guna2Button btnCancelar;

        public CheckOutView(int idUsuario)
        {
            usuarioId = idUsuario;
            conexionBD = new ConexionBD();
            InitializeUI();
            CargarCheckIns();
        }

        private void InitializeUI()
        {
            this.Text = "Proceso de Check-Out";
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
                Text = "PROCESO DE CHECK-OUT",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 80),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Sección de check-ins activos
            var lblSeccionCheckIns = new Label()
            {
                Text = "1. Seleccionar Check-In Activo",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Grid de check-ins
            dgvCheckIns = new Guna2DataGridView()
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
            dgvCheckIns.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvCheckIns.SelectionChanged += (s, e) =>
            {
                if (dgvCheckIns.SelectedRows.Count > 0)
                {
                    checkInSeleccionadoId = Convert.ToInt32(dgvCheckIns.SelectedRows[0].Cells["checkin_id"].Value);
                    reservaSeleccionadaId = Convert.ToInt32(dgvCheckIns.SelectedRows[0].Cells["reserva_id"].Value);
                    CargarDetallesCheckIn(reservaSeleccionadaId.Value);
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

            // Sección de check-out
            var lblSeccionCheckOut = new Label()
            {
                Text = "2. Información de Check-Out",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Panel de check-out
            pnlCheckOut = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = Color.FromArgb(240, 240, 245),
                BorderRadius = 10,
                Padding = new Padding(10),
                BorderColor = Color.FromArgb(200, 200, 200),
                BorderThickness = 1
            };

            // Estado de la habitación
            var lblEstadoHabitacion = new Label()
            {
                Text = "Estado Habitación:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };

            cmbEstadoHabitacion = new Guna2ComboBox()
            {
                DataSource = new string[] { "Excelente", "Bueno", "Daños menores", "Daños graves" },
                Location = new Point(180, 10),
                Size = new Size(170, 36),
                Font = new Font("Segoe UI", 10)
            };

            // Cobros adicionales
            var lblCobrosAdicionales = new Label()
            {
                Text = "Cobros Adicionales:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 60),
                AutoSize = true
            };

            nudCobrosAdicionales = new Guna2NumericUpDown()
            {
                Value = 0,
                Minimum = 0,
                Maximum = 10000,
                Location = new Point(180, 60),
                Size = new Size(100, 36),
                Font = new Font("Segoe UI", 10)
            };

            // Devolución depósito
            var lblDevolucionDeposito = new Label()
            {
                Text = "Devolución Depósito:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 110),
                AutoSize = true
            };

            nudDevolucionDeposito = new Guna2NumericUpDown()
            {
                Value = 0,
                Minimum = 0,
                Maximum = 10000,
                Location = new Point(180, 110),
                Size = new Size(100, 36),
                Font = new Font("Segoe UI", 10)
            };

            // Observaciones
            var lblObservaciones = new Label()
            {
                Text = "Observaciones:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(360, 10),
                AutoSize = true
            };

            txtObservaciones = new Guna2TextBox()
            {
                PlaceholderText = "Ingrese observaciones...",
                Location = new Point(360, 40),
                Size = new Size(300, 100),
                Multiline = true,
                Font = new Font("Segoe UI", 10)
            };

            pnlCheckOut.Controls.Add(lblEstadoHabitacion);
            pnlCheckOut.Controls.Add(cmbEstadoHabitacion);
            pnlCheckOut.Controls.Add(lblCobrosAdicionales);
            pnlCheckOut.Controls.Add(nudCobrosAdicionales);
            pnlCheckOut.Controls.Add(lblDevolucionDeposito);
            pnlCheckOut.Controls.Add(nudDevolucionDeposito);
            pnlCheckOut.Controls.Add(lblObservaciones);
            pnlCheckOut.Controls.Add(txtObservaciones);

            // Panel de botones
            var pnlBotones = new Guna2Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };

            btnRealizarCheckOut = new Guna2Button()
            {
                Text = "REALIZAR CHECK-OUT",
                Size = new Size(180, 40),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true,
                Enabled = false,
                Dock = DockStyle.Right
            };
            btnRealizarCheckOut.Click += (s, e) => RealizarCheckOut();

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

            pnlBotones.Controls.Add(btnRealizarCheckOut);
            pnlBotones.Controls.Add(btnCancelar);

            // Agregar controles al panel principal
            mainPanel.Controls.Add(pnlBotones);
            mainPanel.Controls.Add(pnlCheckOut);
            mainPanel.Controls.Add(lblSeccionCheckOut);
            mainPanel.Controls.Add(pnlDatosReserva);
            mainPanel.Controls.Add(pnlDatosCliente);
            mainPanel.Controls.Add(dgvCheckIns);
            mainPanel.Controls.Add(lblSeccionCheckIns);
            mainPanel.Controls.Add(lblTitulo);

            this.Controls.Add(mainPanel);
        }

        private void CargarCheckIns()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetAllCheckInsForCheckOut", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    dgvCheckIns.DataSource = dt;

                    // Configurar columnas
                    if (dgvCheckIns.Columns.Count > 0)
                    {
                        dgvCheckIns.Columns["checkin_id"].Visible = false;
                        dgvCheckIns.Columns["reserva_id"].Visible = false;
                        dgvCheckIns.Columns["cliente"].HeaderText = "Cliente";
                        dgvCheckIns.Columns["habitacion"].HeaderText = "Habitación";
                        dgvCheckIns.Columns["tipo_habitacion"].HeaderText = "Tipo";
                        dgvCheckIns.Columns["fecha_entrada"].HeaderText = "Entrada";
                        dgvCheckIns.Columns["fecha_salida"].HeaderText = "Salida";
                        dgvCheckIns.Columns["noches"].HeaderText = "Noches";
                        dgvCheckIns.Columns["precio_total"].HeaderText = "Total";
                        dgvCheckIns.Columns["fecha_checkin"].HeaderText = "Fecha Check-In";
                        dgvCheckIns.Columns["recepcionista"].HeaderText = "Recepción";

                        // Formatear columnas
                        dgvCheckIns.Columns["fecha_entrada"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvCheckIns.Columns["fecha_salida"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvCheckIns.Columns["fecha_checkin"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                        dgvCheckIns.Columns["precio_total"].DefaultCellStyle.Format = "C2";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar check-ins: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetallesCheckIn(int reservaId)
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
                                       $"{fechaSalida.ToShortDateString()} ({noches} noches)",
                                Dock = DockStyle.Top,
                                Font = new Font("Segoe UI", 10),
                                Height = 20
                            };
                            pnlDatosReserva.Controls.Add(lblFechas);

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

                // Habilitar botón de check-out
                btnRealizarCheckOut.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar detalles de check-in: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RealizarCheckOut()
        {
            if (!checkInSeleccionadoId.HasValue || !reservaSeleccionadaId.HasValue)
            {
                MessageBox.Show("Seleccione un check-in primero", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_PerformCheckOut", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_checkin_id", checkInSeleccionadoId.Value);
                    cmd.Parameters.AddWithValue("@p_reserva_id", reservaSeleccionadaId.Value);
                    cmd.Parameters.AddWithValue("@p_usuario_id", usuarioId);
                    cmd.Parameters.AddWithValue("@p_estado_habitacion", cmbEstadoHabitacion.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@p_cobros_adicionales", nudCobrosAdicionales.Value);
                    cmd.Parameters.AddWithValue("@p_devolucion_deposito", nudDevolucionDeposito.Value);
                    cmd.Parameters.AddWithValue("@p_observaciones", txtObservaciones.Text);

                    var checkoutId = cmd.ExecuteScalar();

                    MessageBox.Show($"Check-Out realizado exitosamente. ID: {checkoutId}", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar check-out: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}