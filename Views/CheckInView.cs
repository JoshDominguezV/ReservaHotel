using Proyecto_PED.Database;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Guna.UI2.WinForms;

namespace Proyecto_PED.Views
{
    public partial class CheckInView : Form
    {
        private ConexionBD conexionBD;
        private int? reservaSeleccionadaId = null;
        private int usuarioId;

        public CheckInView(ConexionBD conexionBD, int usuarioId)
        {
            InitializeComponent();
            this.conexionBD = conexionBD;
            this.usuarioId = usuarioId;
            InitializeUI();
            CargarReservasPendientes();
            CargarMetodosPago();
        }

        private void InitializeUI()
        {
            // Configuración básica del formulario
            this.Text = "Gestión de Check-In";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 240, 245);

            // Panel principal
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
                Text = "Gestión de Check-In",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Height = headerPanel.Height
            };

            // Panel de contenido dividido en dos partes
            var contentPanel = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400,
                BorderStyle = BorderStyle.None
            };

            // Panel izquierdo - Lista de reservas
            var reservasPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            var lblReservas = new Label()
            {
                Text = "Reservas Pendientes de Check-In",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            var dgvReservas = new Guna2DataGridView()
            {
                Name = "dgvReservas",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };

            // Configuración de estilos del DataGridView
            dgvReservas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 80);
            dgvReservas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvReservas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReservas.RowsDefaultCellStyle.BackColor = Color.White;
            dgvReservas.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvReservas.RowsDefaultCellStyle.ForeColor = Color.FromArgb(50, 50, 50);
            dgvReservas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvReservas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgvReservas.DefaultCellStyle.SelectionForeColor = Color.FromArgb(50, 50, 50);
            dgvReservas.SelectionChanged += DgvReservas_SelectionChanged;

            reservasPanel.Controls.Add(dgvReservas);
            reservasPanel.Controls.Add(lblReservas);
            contentPanel.Panel1.Controls.Add(reservasPanel);

            // Panel derecho - Detalles y check-in
            var detallesPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            var lblDetalles = new Label()
            {
                Text = "Detalles de la Reserva",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            var pnlDetalles = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 250,
                BorderRadius = 10,
                BorderColor = Color.FromArgb(200, 200, 200),
                BorderThickness = 1,
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 0, 15)
            };

            // Controles para detalles de la reserva
            var lblCliente = new Label() { Text = "Cliente:", AutoSize = true, Location = new Point(20, 20) };
            var txtCliente = new Guna2TextBox() { ReadOnly = true, Location = new Point(120, 15), Width = 250 };

            var lblHabitacion = new Label() { Text = "Habitación:", AutoSize = true, Location = new Point(20, 60) };
            var txtHabitacion = new Guna2TextBox() { ReadOnly = true, Location = new Point(120, 55), Width = 100 };

            var lblFechas = new Label() { Text = "Fechas:", AutoSize = true, Location = new Point(20, 100) };
            var txtFechas = new Guna2TextBox() { ReadOnly = true, Location = new Point(120, 95), Width = 250 };

            var lblTotal = new Label() { Text = "Total:", AutoSize = true, Location = new Point(20, 140) };
            var txtTotal = new Guna2TextBox() { ReadOnly = true, Location = new Point(120, 135), Width = 100 };

            pnlDetalles.Controls.AddRange(new Control[] { lblCliente, txtCliente, lblHabitacion, txtHabitacion, lblFechas, txtFechas, lblTotal, txtTotal });

            // Panel de servicios
            var lblServicios = new Label()
            {
                Text = "Servicios Adicionales",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            var pnlServicios = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 150,
                BorderRadius = 10,
                BorderColor = Color.FromArgb(200, 200, 200),
                BorderThickness = 1,
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 0, 15)
            };

            var cmbServicios = new Guna2ComboBox()
            {
                Location = new Point(20, 20),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var numCantidad = new Guna2NumericUpDown()
            {
                Location = new Point(240, 20),
                Width = 80,
                Minimum = 1,
                Maximum = 10,
                Value = 1
            };

            var btnAgregarServicio = new Guna2Button()
            {
                Text = "Agregar Servicio",
                Location = new Point(340, 20),
                Size = new Size(150, 30),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White
            };
            btnAgregarServicio.Click += BtnAgregarServicio_Click;

            var dgvServicios = new Guna2DataGridView()
            {
                Location = new Point(20, 60),
                Width = pnlServicios.Width - 40,
                Height = 70,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None
            };

            pnlServicios.Controls.AddRange(new Control[] { cmbServicios, numCantidad, btnAgregarServicio, dgvServicios });

            // Panel de check-in
            var pnlCheckIn = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 120,
                BorderRadius = 10,
                BorderColor = Color.FromArgb(200, 200, 200),
                BorderThickness = 1,
                Padding = new Padding(15)
            };

            var lblMetodoPago = new Label() { Text = "Método de Pago:", AutoSize = true, Location = new Point(20, 20) };
            var cmbMetodoPago = new Guna2ComboBox()
            {
                Location = new Point(150, 15),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var lblDeposito = new Label() { Text = "Depósito Seguridad:", AutoSize = true, Location = new Point(20, 60) };
            var txtDeposito = new Guna2TextBox()
            {
                Location = new Point(150, 55),
                Width = 150,
                Text = "0.00"
            };

            var btnCheckIn = new Guna2Button()
            {
                Text = "Realizar Check-In",
                Location = new Point(320, 55),
                Size = new Size(150, 30),
                FillColor = Color.FromArgb(60, 179, 113),
                ForeColor = Color.White
            };
            btnCheckIn.Click += BtnCheckIn_Click;

            pnlCheckIn.Controls.AddRange(new Control[] { lblMetodoPago, cmbMetodoPago, lblDeposito, txtDeposito, btnCheckIn });

            detallesPanel.Controls.Add(pnlCheckIn);
            detallesPanel.Controls.Add(lblServicios);
            detallesPanel.Controls.Add(pnlServicios);
            detallesPanel.Controls.Add(lblDetalles);
            detallesPanel.Controls.Add(pnlDetalles);
            contentPanel.Panel2.Controls.Add(detallesPanel);

            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(headerPanel);
            this.Controls.Add(mainPanel);

            // Asignar nombres a los controles para poder acceder a ellos
            txtCliente.Name = "txtCliente";
            txtHabitacion.Name = "txtHabitacion";
            txtFechas.Name = "txtFechas";
            txtTotal.Name = "txtTotal";
            cmbServicios.Name = "cmbServicios";
            dgvServicios.Name = "dgvServicios";
            cmbMetodoPago.Name = "cmbMetodoPago";
            txtDeposito.Name = "txtDeposito";
        }

        private void CargarReservasPendientes()
        {
            var dgvReservas = (Guna2DataGridView)this.Controls.Find("dgvReservas", true)[0];

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        var cmd = new MySqlCommand("sp_GetPendingCheckIns", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        var adapter = new MySqlDataAdapter(cmd);
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        dgvReservas.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar reservas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarMetodosPago()
        {
            var cmbMetodoPago = (Guna2ComboBox)this.Controls.Find("cmbMetodoPago", true)[0];

            cmbMetodoPago.Items.Add("Efectivo");
            cmbMetodoPago.Items.Add("Tarjeta");
            cmbMetodoPago.Items.Add("Transferencia");
            cmbMetodoPago.Items.Add("Otro");

            if (cmbMetodoPago.Items.Count > 0)
                cmbMetodoPago.SelectedIndex = 0;
        }

        private void DgvReservas_SelectionChanged(object sender, EventArgs e)
        {
            var dgvReservas = (Guna2DataGridView)this.Controls.Find("dgvReservas", true)[0];
            var txtCliente = (Guna2TextBox)this.Controls.Find("txtCliente", true)[0];
            var txtHabitacion = (Guna2TextBox)this.Controls.Find("txtHabitacion", true)[0];
            var txtFechas = (Guna2TextBox)this.Controls.Find("txtFechas", true)[0];
            var txtTotal = (Guna2TextBox)this.Controls.Find("txtTotal", true)[0];
            var cmbServicios = (Guna2ComboBox)this.Controls.Find("cmbServicios", true)[0];
            var dgvServicios = (Guna2DataGridView)this.Controls.Find("dgvServicios", true)[0];

            if (dgvReservas.SelectedRows.Count > 0)
            {
                reservaSeleccionadaId = Convert.ToInt32(dgvReservas.SelectedRows[0].Cells["reserva_id"].Value);

                // Cargar detalles de la reserva
                try
                {
                    using (var conn = conexionBD.ObtenerConexion())
                    {
                        if (conn != null)
                        {
                            var cmd = new MySqlCommand("sp_GetReservaDetails", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("p_reserva_id", reservaSeleccionadaId);

                            var reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                txtCliente.Text = reader["cliente"].ToString();
                                txtHabitacion.Text = reader["habitacion"].ToString();
                                txtFechas.Text = $"{reader["fecha_entrada"]:d} - {reader["fecha_salida"]:d}";
                                txtTotal.Text = $"{reader["precio_total"]:C}";
                            }
                            reader.Close();

                            // Cargar servicios de la reserva
                            var cmdServicios = new MySqlCommand("sp_GetReservaServices", conn);
                            cmdServicios.CommandType = CommandType.StoredProcedure;
                            cmdServicios.Parameters.AddWithValue("p_reserva_id", reservaSeleccionadaId);

                            var adapter = new MySqlDataAdapter(cmdServicios);
                            var dt = new DataTable();
                            adapter.Fill(dt);

                            dgvServicios.DataSource = dt;

                            // Cargar servicios disponibles
                            var cmdServiciosDisponibles = new MySqlCommand("sp_GetAvailableServices", conn);
                            cmdServiciosDisponibles.CommandType = CommandType.StoredProcedure;

                            cmbServicios.Items.Clear();
                            var readerServicios = cmdServiciosDisponibles.ExecuteReader();
                            while (readerServicios.Read())
                            {
                                cmbServicios.Items.Add(new
                                {
                                    Id = readerServicios.GetInt32("id"),
                                    Nombre = readerServicios.GetString("nombre"),
                                    Precio = readerServicios.GetDecimal("precio")
                                });
                            }
                            readerServicios.Close();

                            if (cmbServicios.Items.Count > 0)
                                cmbServicios.DisplayMember = "Nombre";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar detalles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                reservaSeleccionadaId = null;
                txtCliente.Text = string.Empty;
                txtHabitacion.Text = string.Empty;
                txtFechas.Text = string.Empty;
                txtTotal.Text = string.Empty;
                dgvServicios.DataSource = null;
                cmbServicios.Items.Clear();
            }
        }

        private void BtnAgregarServicio_Click(object sender, EventArgs e)
        {
            var cmbServicios = (Guna2ComboBox)this.Controls.Find("cmbServicios", true)[0];
            var numCantidad = (Guna2NumericUpDown)this.Controls.Find("numCantidad", true)[0];
            var dgvServicios = (Guna2DataGridView)this.Controls.Find("dgvServicios", true)[0];

            if (reservaSeleccionadaId == null)
            {
                MessageBox.Show("Seleccione una reserva primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbServicios.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un servicio", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic servicio = cmbServicios.SelectedItem;
                int cantidad = (int)numCantidad.Value;

                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        var cmd = new MySqlCommand("sp_AddServiceToReserva", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_reserva_id", reservaSeleccionadaId);
                        cmd.Parameters.AddWithValue("p_servicio_id", servicio.Id);
                        cmd.Parameters.AddWithValue("p_cantidad", cantidad);
                        cmd.Parameters.AddWithValue("p_fecha", DateTime.Today);

                        cmd.ExecuteNonQuery();

                        // Actualizar la lista de servicios
                        DgvReservas_SelectionChanged(null, null);
                        MessageBox.Show("Servicio agregado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar servicio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCheckIn_Click(object sender, EventArgs e)
        {
            var cmbMetodoPago = (Guna2ComboBox)this.Controls.Find("cmbMetodoPago", true)[0];
            var txtDeposito = (Guna2TextBox)this.Controls.Find("txtDeposito", true)[0];
            var dgvReservas = (Guna2DataGridView)this.Controls.Find("dgvReservas", true)[0];

            if (reservaSeleccionadaId == null)
            {
                MessageBox.Show("Seleccione una reserva primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbMetodoPago.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un método de pago", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtDeposito.Text, out decimal deposito))
            {
                MessageBox.Show("Ingrese un valor válido para el depósito", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        var cmd = new MySqlCommand("sp_CheckInReserva", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_reserva_id", reservaSeleccionadaId);
                        cmd.Parameters.AddWithValue("p_usuario_id", usuarioId);
                        cmd.Parameters.AddWithValue("p_metodo_pago", cmbMetodoPago.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("p_deposito_seguridad", deposito);
                        cmd.Parameters.AddWithValue("p_observaciones", "");

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Check-In realizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Actualizar la lista de reservas
                        CargarReservasPendientes();

                        // Limpiar detalles
                        DgvReservas_SelectionChanged(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar check-in: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}