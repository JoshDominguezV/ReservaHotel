using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using System.Data;

namespace Proyecto_PED.Views.Reservaciones
{
    public partial class EditReservationForm : Form
    {
        private ConexionBD conexionBD;
        private DataRow reservaData;
        private int usuarioId;

        // Controles
        private Guna2TextBox txtCliente;
        private Guna2TextBox txtHabitacion;
        private Guna2DateTimePicker dtpEntrada;
        private Guna2DateTimePicker dtpSalida;
        private Guna2NumericUpDown numAdultos;
        private Guna2NumericUpDown numNinos;
        private Guna2TextBox txtPrecio;
        private Guna2ComboBox cmbEstado;
        private Guna2TextBox txtNotas;
        private Guna2Button btnGuardar;
        private Guna2Button btnCancelar;

        public EditReservationForm(DataRow reservaData, int usuarioId)
        {
            this.reservaData = reservaData;
            this.usuarioId = usuarioId;
            conexionBD = new ConexionBD();
            InitializeUI();
            CargarDatosReserva();
            CargarEstados();
        }

        private void InitializeUI()
        {
            this.Text = "Editar Reservación";
            this.Size = new Size(500, 650); // Tamaño más compacto
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Panel principal
            var mainPanel = new Guna2Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            // Crear controles con márgenes más ajustados
            txtCliente = new Guna2TextBox()
            {
                PlaceholderText = "Cliente",
                ReadOnly = true,
                Margin = new Padding(0, 0, 0, 10), // Margen reducido
                Dock = DockStyle.Top
            };

            txtHabitacion = new Guna2TextBox()
            {
                PlaceholderText = "Habitación",
                ReadOnly = true,
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top
            };

            dtpEntrada = new Guna2DateTimePicker()
            {
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top
            };

            dtpSalida = new Guna2DateTimePicker()
            {
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top
            };

            numAdultos = new Guna2NumericUpDown()
            {
                Minimum = 1,
                Maximum = 10,
                Value = 1,
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top
            };

            numNinos = new Guna2NumericUpDown()
            {
                Minimum = 0,
                Maximum = 10,
                Value = 0,
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top
            };

            txtPrecio = new Guna2TextBox()
            {
                PlaceholderText = "Precio Total",
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top
            };

            cmbEstado = new Guna2ComboBox()
            {
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top
            };

            txtNotas = new Guna2TextBox()
            {
                PlaceholderText = "Notas adicionales",
                Margin = new Padding(0, 0, 0, 15), // Margen ligeramente mayor para separar de los botones
                Dock = DockStyle.Top,
                Multiline = true,
                Height = 80 // Altura ajustada
            };

            // Panel de botones más compacto
            var panelBotones = new Guna2Panel()
            {
                Height = 45, // Altura reducida
                Dock = DockStyle.Bottom,
                Padding = new Padding(0, 5, 0, 0) // Padding superior para separación
            };

            // FlowLayoutPanel para agrupar los botones
            var flowBotones = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            btnCancelar = new Guna2Button()
            {
                Text = "Cancelar",
                Size = new Size(120, 36), // Tamaño uniforme
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Animated = true,
                Margin = new Padding(5, 0, 0, 0) // Margen entre botones
            };
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            btnGuardar = new Guna2Button()
            {
                Text = "Guardar Cambios",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(60, 179, 113),
                ForeColor = Color.White,
                Animated = true
            };
            btnGuardar.Click += (s, e) => GuardarCambios();

            flowBotones.Controls.Add(btnCancelar);
            flowBotones.Controls.Add(btnGuardar);
            panelBotones.Controls.Add(flowBotones);

            // Panel de contenido para mejor organización
            var contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true // Para contenido extenso
            };

            // Agregar controles al panel de contenido
            contentPanel.Controls.Add(txtNotas);
            contentPanel.Controls.Add(cmbEstado);
            contentPanel.Controls.Add(txtPrecio);
            contentPanel.Controls.Add(numNinos);
            contentPanel.Controls.Add(numAdultos);
            contentPanel.Controls.Add(dtpSalida);
            contentPanel.Controls.Add(dtpEntrada);
            contentPanel.Controls.Add(txtHabitacion);
            contentPanel.Controls.Add(txtCliente);

            // Agregar paneles al panel principal
            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(panelBotones);

            this.Controls.Add(mainPanel);
        }

        private void CargarDatosReserva()
        {
            txtCliente.Text = reservaData["cliente_nombre"].ToString();
            txtHabitacion.Text = reservaData["habitacion_numero"].ToString();
            dtpEntrada.Value = Convert.ToDateTime(reservaData["fecha_entrada"]);
            dtpSalida.Value = Convert.ToDateTime(reservaData["fecha_salida"]);
            numAdultos.Value = Convert.ToInt32(reservaData["adultos"]);
            numNinos.Value = Convert.ToInt32(reservaData["ninos"]);
            txtPrecio.Text = Convert.ToDecimal(reservaData["precio_total"]).ToString("0.00");
            txtNotas.Text = reservaData["notas"].ToString();
        }

        private void CargarEstados()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("SELECT id, nombre FROM estados_reserva", conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    cmbEstado.DataSource = dt;
                    cmbEstado.DisplayMember = "nombre";
                    cmbEstado.ValueMember = "id";

                    // Seleccionar el estado actual
                    cmbEstado.SelectedValue = reservaData["estado_id"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar estados: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GuardarCambios()
        {
            if (dtpEntrada.Value >= dtpSalida.Value)
            {
                MessageBox.Show("La fecha de entrada debe ser anterior a la fecha de salida", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Ingrese un precio válido", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_UpdateReservation", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_reserva_id", reservaData["id"]);
                    cmd.Parameters.AddWithValue("@p_cliente_id", reservaData["cliente_id"]);
                    cmd.Parameters.AddWithValue("@p_habitacion_id", reservaData["habitacion_id"]);
                    cmd.Parameters.AddWithValue("@p_fecha_entrada", dtpEntrada.Value);
                    cmd.Parameters.AddWithValue("@p_fecha_salida", dtpSalida.Value);
                    cmd.Parameters.AddWithValue("@p_adultos", numAdultos.Value);
                    cmd.Parameters.AddWithValue("@p_ninos", numNinos.Value);
                    cmd.Parameters.AddWithValue("@p_precio_total", precio);
                    cmd.Parameters.AddWithValue("@p_notas", txtNotas.Text);
                    cmd.Parameters.AddWithValue("@p_estado_id", cmbEstado.SelectedValue);
                    cmd.Parameters.AddWithValue("@p_usuario_id", usuarioId);

                    cmd.ExecuteNonQuery();

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cambios: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}