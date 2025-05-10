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
        private Panel contentPanel;
        private const int menuWidth = 200;

        public Reservacion(string usuario, string rol)
        {
            usuarioActual = usuario;
            rolActual = rol;
            conexionBD = new ConexionBD();

            InitializeComponent();
            ConstruirInterfaz();
            MostrarRegistrarCliente();
        }

        private void ConstruirInterfaz()
        {
            this.Text = $"Sistema de Reservaciones - Usuario: {usuarioActual} ({rolActual})";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Panel del menú (estático)
            menuPanel = new Panel
            {
                BackColor = Color.FromArgb(51, 51, 76),
                Dock = DockStyle.Left,
                Width = menuWidth
            };

            // Panel del contenido
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Control
            };

            // Construir el menú
            ConstruirMenu();

            this.Controls.Add(contentPanel);
            this.Controls.Add(menuPanel);
        }

        private void ConstruirMenu()
        {
            var lblLogo = new Label
            {
                Text = "Reserva de Habitaciones - PED",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnGestionUsuarios = CrearBotonMenu("Gestionar Usuarios", null);
            var btnRegistrarCliente = CrearBotonMenu("Registrar Cliente", null);
            var btnRegistrarReservacion = CrearBotonMenu("Registrar Reservación", null);
            var btnReservacionesExistentes = CrearBotonMenu("Reservaciones Existentes", null);

            // Asignar eventos
            btnGestionUsuarios.Click += (sender, e) => MostrarGestionUsuarios();
            btnRegistrarCliente.Click += (sender, e) => MostrarRegistrarCliente();
            btnRegistrarReservacion.Click += (sender, e) => MostrarRegistrarReservacion();
            btnReservacionesExistentes.Click += (sender, e) => MostrarReservacionesExistentes();

            // Ocultar gestión de usuarios si no es administrador
            btnGestionUsuarios.Visible = rolActual == "Administrador";

            menuPanel.Controls.Add(btnReservacionesExistentes);
            menuPanel.Controls.Add(btnRegistrarReservacion);
            menuPanel.Controls.Add(btnRegistrarCliente);
            menuPanel.Controls.Add(btnGestionUsuarios);
            menuPanel.Controls.Add(lblLogo);
        }

        private Button CrearBotonMenu(string text, Image icon)
        {
            var btn = new Button
            {
                Text = "  " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 60,
                FlatStyle = FlatStyle.Flat,
                Image = icon,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Name = "btn" + text.Replace(" ", "")
            };

            if (icon != null)
            {
                btn.Image = icon;
                btn.ImageAlign = ContentAlignment.MiddleLeft;
                btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            }

            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(51, 51, 76);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 100);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 50);

            return btn;
        }

        private void LimpiarContentPanel()
        {
            contentPanel.Controls.Clear();
        }

        private void MostrarGestionUsuarios()
        {
            LimpiarContentPanel();
            var usuarioForm = new UsuarioForm(usuarioActual, rolActual);
            usuarioForm.TopLevel = false;
            usuarioForm.FormBorderStyle = FormBorderStyle.None;
            usuarioForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(usuarioForm);
            usuarioForm.Show();
        }

        private void MostrarRegistrarCliente()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var lblTitulo = new Label
            {
                Text = "Nuevo Cliente",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var tableLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 5,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(20, 60),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 170));

            // Controles para el formulario de cliente
            var lblNombre = new Label { Text = "Nombre:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtNombre = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 10), Name = "txtNombre", MinimumSize = new Size(170, 20) };

            var lblDocumento = new Label { Text = "Documento:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtDocumento = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 10), Name = "txtDocumento" };

            var lblTelefono = new Label { Text = "Teléfono:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtTelefono = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 10), Name = "txtTelefono" };

            var lblCorreo = new Label { Text = "Correo:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtCorreo = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 10), Name = "txtCorreo" };

            var btnRegistrar = new Button
            {
                Text = "Registrar Cliente",
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnRegistrar.Click += BtnRegistrarCliente_Click;

            // Agregar controles al TableLayout
            tableLayout.Controls.Add(lblNombre, 0, 0);
            tableLayout.Controls.Add(txtNombre, 1, 0);
            tableLayout.Controls.Add(lblDocumento, 0, 1);
            tableLayout.Controls.Add(txtDocumento, 1, 1);
            tableLayout.Controls.Add(lblTelefono, 0, 2);
            tableLayout.Controls.Add(txtTelefono, 1, 2);
            tableLayout.Controls.Add(lblCorreo, 0, 3);
            tableLayout.Controls.Add(txtCorreo, 1, 3);

            // Agregar controles al panel
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(tableLayout);
            panel.Controls.Add(btnRegistrar);

            // Posicionar el botón
            btnRegistrar.Location = new Point(
                panel.Width - btnRegistrar.Width - 25,
                tableLayout.Bottom + 20
            );

            contentPanel.Controls.Add(panel);
        }

        private void MostrarRegistrarReservacion()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var lblTitulo = new Label
            {
                Text = "Nueva Reservación",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var tableLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 5,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(20, 60),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Mismos estilos de columnas que en Registrar Cliente
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120)); // Ancho columna labels
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 170));  // Ancho columna controles

            // Controles con el mismo estilo que Registrar Cliente
            var lblCliente = new Label
            {
                Text = "Cliente:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var cmbClientes = new ComboBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "cmbClientes",
                MinimumSize = new Size(170, 25) // Mismo tamaño que los TextBox
            };

            var lblHabitacion = new Label
            {
                Text = "Habitación:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var cmbHabitaciones = new ComboBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "cmbHabitaciones",
                MinimumSize = new Size(170, 25)
            };

            var lblEntrada = new Label
            {
                Text = "Fecha Entrada:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var dtpEntrada = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 10),
                Format = DateTimePickerFormat.Short,
                Name = "dtpEntrada",
                MinimumSize = new Size(170, 25)
            };

            var lblSalida = new Label
            {
                Text = "Fecha Salida:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var dtpSalida = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 10),
                Format = DateTimePickerFormat.Short,
                Name = "dtpSalida",
                MinimumSize = new Size(170, 25)
            };

            var btnReservar = new Button
            {
                Text = "Crear Reservación",
                Size = new Size(170, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnReservar.Click += BtnReservar_Click;

            // Cargar datos
            CargarClientesCombo(cmbClientes);
            CargarHabitacionesCombo(cmbHabitaciones);

            // Agregar controles al TableLayout
            tableLayout.Controls.Add(lblCliente, 0, 0);
            tableLayout.Controls.Add(cmbClientes, 1, 0);
            tableLayout.Controls.Add(lblHabitacion, 0, 1);
            tableLayout.Controls.Add(cmbHabitaciones, 1, 1);
            tableLayout.Controls.Add(lblEntrada, 0, 2);
            tableLayout.Controls.Add(dtpEntrada, 1, 2);
            tableLayout.Controls.Add(lblSalida, 0, 3);
            tableLayout.Controls.Add(dtpSalida, 1, 3);

            // Agregar controles al panel
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(tableLayout);
            panel.Controls.Add(btnReservar);

            // Posicionar el botón igual que en Registrar Cliente
            btnReservar.Location = new Point(
                panel.Width - btnReservar.Width - 5,
                tableLayout.Bottom + 20
            );

            contentPanel.Controls.Add(panel);
        }

        private void MostrarReservacionesExistentes()
        {
            LimpiarContentPanel();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                RowStyles = {
            new RowStyle(SizeType.Absolute, 40), // Para el título
            new RowStyle(SizeType.Percent, 100)  // Para el DataGridView
        }
            };

            var lblTitulo = new Label
            {
                Text = "Reservaciones Existentes",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var dgvReservaciones = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.None,
                Name = "dgvReservaciones"
            };

            // Agregar controles al TableLayout
            tableLayout.Controls.Add(lblTitulo, 0, 0);
            tableLayout.Controls.Add(dgvReservaciones, 0, 1);

            // Agregar al panel principal
            panel.Controls.Add(tableLayout);
            contentPanel.Controls.Add(panel);

            // Cargar datos
            CargarReservacionesGrid(dgvReservaciones);
        }

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

        private void BtnRegistrarCliente_Click(object sender, EventArgs e)
        {
            var txtNombre = (TextBox)contentPanel.Controls.Find("txtNombre", true)[0];
            var txtDocumento = (TextBox)contentPanel.Controls.Find("txtDocumento", true)[0];
            var txtTelefono = (TextBox)contentPanel.Controls.Find("txtTelefono", true)[0];
            var txtCorreo = (TextBox)contentPanel.Controls.Find("txtCorreo", true)[0];

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                MessageBox.Show("Nombre y documento son campos obligatorios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        string query = @"INSERT INTO clientes (nombre, documento_identidad, telefono, correo)
                                       VALUES (@nombre, @documento, @telefono, @correo)";

                        var cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@documento", txtDocumento.Text);
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@correo", txtCorreo.Text);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Cliente registrado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiar campos
                            txtNombre.Text = "";
                            txtDocumento.Text = "";
                            txtTelefono.Text = "";
                            txtCorreo.Text = "";

                            // Actualizar combos en otras pantallas si es necesario
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReservar_Click(object sender, EventArgs e)
        {
            var cmbClientes = (ComboBox)contentPanel.Controls.Find("cmbClientes", true)[0];
            var cmbHabitaciones = (ComboBox)contentPanel.Controls.Find("cmbHabitaciones", true)[0];
            var dtpEntrada = (DateTimePicker)contentPanel.Controls.Find("dtpEntrada", true)[0];
            var dtpSalida = (DateTimePicker)contentPanel.Controls.Find("dtpSalida", true)[0];

            if (cmbClientes.SelectedItem == null || cmbHabitaciones.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente y una habitación", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpEntrada.Value >= dtpSalida.Value)
            {
                MessageBox.Show("La fecha de entrada debe ser anterior a la fecha de salida", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cliente = (ClienteItem)cmbClientes.SelectedItem;
            var habitacion = (HabitacionItem)cmbHabitaciones.SelectedItem;

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        // Verificar disponibilidad de habitación
                        string verificarQuery = @"SELECT COUNT(*) FROM reservas 
                                                WHERE habitacion_id = @habitacion_id
                                                AND (
                                                    (fecha_entrada BETWEEN @entrada AND @salida)
                                                    OR (fecha_salida BETWEEN @entrada AND @salida)
                                                    OR (@entrada BETWEEN fecha_entrada AND fecha_salida)
                                                )";

                        var verificarCmd = new MySqlCommand(verificarQuery, conn);
                        verificarCmd.Parameters.AddWithValue("@habitacion_id", habitacion.Id);
                        verificarCmd.Parameters.AddWithValue("@entrada", dtpEntrada.Value);
                        verificarCmd.Parameters.AddWithValue("@salida", dtpSalida.Value);

                        int reservasExistentes = Convert.ToInt32(verificarCmd.ExecuteScalar());

                        if (reservasExistentes > 0)
                        {
                            MessageBox.Show("La habitación no está disponible para las fechas seleccionadas", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string insertQuery = @"INSERT INTO reservas (cliente_id, habitacion_id, fecha_entrada, fecha_salida)
                                             VALUES (@cliente_id, @habitacion_id, @fecha_entrada, @fecha_salida)";

                        var insertCmd = new MySqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@cliente_id", cliente.Id);
                        insertCmd.Parameters.AddWithValue("@habitacion_id", habitacion.Id);
                        insertCmd.Parameters.AddWithValue("@fecha_entrada", dtpEntrada.Value);
                        insertCmd.Parameters.AddWithValue("@fecha_salida", dtpSalida.Value);

                        int result = insertCmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Reservación creada exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Actualizar la vista de reservaciones si está visible
                            var dgv = contentPanel.Controls.Find("dgvReservaciones", true);
                            if (dgv.Length > 0)
                            {
                                CargarReservacionesGrid((DataGridView)dgv[0]);
                            }

                            // Actualizar combobox de habitaciones
                            var cmbHabit = contentPanel.Controls.Find("cmbHabitaciones", true);
                            if (cmbHabit.Length > 0)
                            {
                                CargarHabitacionesCombo((ComboBox)cmbHabit[0]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear reservación: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}