using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;

namespace Proyecto_PED.Views.Habitaciones
{
    public partial class RoomForm : Form
    {
        private ConexionBD conexionBD;
        private int habitacionId;
        private bool esNuevo;
        private Guna2ComboBox cmbTipoHabitacion;
        private Guna2TextBox txtNumero;
        private Guna2TextBox txtPiso;
        private Guna2ComboBox cmbEstado;
        private Guna2TextBox txtNotas;
        private Guna2Button btnGuardar;
        private Guna2Button btnCancelar;

        public RoomForm(int idHabitacion = 0)
        {
            habitacionId = idHabitacion;
            esNuevo = (idHabitacion == 0);
            conexionBD = new ConexionBD();
            InitializeUI();
            CargarTiposHabitacion();
            if (!esNuevo) CargarDatosHabitacion();
        }

        private void InitializeUI()
        {
            this.Text = esNuevo ? "Nueva Habitación" : "Editar Habitación";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 240, 245);

            // Panel principal con sombra
            var mainPanel = new Guna2Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderRadius = 20,
                Padding = new Padding(30),
                ShadowDecoration = {
                    Enabled = true,
                    Color = Color.FromArgb(150, 150, 150),
                    Depth = 20
                }
            };

            // Título del formulario
            var lblTitulo = new Label()
            {
                Text = esNuevo ? "NUEVA HABITACIÓN" : "EDITAR HABITACIÓN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 80),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Constantes para posicionamiento
            const int labelWidth = 150;
            const int controlX = 180;
            const int rowHeight = 45;

            // Campos del formulario
            var lblNumero = new Label
            {
                Text = "Número:",
                Location = new Point(30, 70),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            txtNumero = new Guna2TextBox()
            {
                Location = new Point(controlX, 70),
                Size = new Size(120, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
            };

            var lblTipo = new Label
            {
                Text = "Tipo Habitación:",
                Location = new Point(30, 70 + rowHeight),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            cmbTipoHabitacion = new Guna2ComboBox()
            {
                Location = new Point(controlX, 70 + rowHeight),
                Size = new Size(250, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var lblPiso = new Label
            {
                Text = "Piso:",
                Location = new Point(30, 70 + rowHeight * 2),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            txtPiso = new Guna2TextBox()
            {
                Location = new Point(controlX, 70 + rowHeight * 2),
                Size = new Size(80, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
            };

            var lblEstado = new Label
            {
                Text = "Estado:",
                Location = new Point(30, 70 + rowHeight * 3),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            cmbEstado = new Guna2ComboBox()
            {
                Location = new Point(controlX, 70 + rowHeight * 3),
                Size = new Size(180, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEstado.Items.AddRange(new object[] { "Disponible", "Ocupada", "Mantenimiento", "Limpieza" });
            cmbEstado.SelectedIndex = 0;

            var lblNotas = new Label
            {
                Text = "Notas:",
                Location = new Point(30, 70 + rowHeight * 4),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            txtNotas = new Guna2TextBox()
            {
                Location = new Point(controlX, 70 + rowHeight * 4),
                Size = new Size(250, 80),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Panel de botones
            var buttonsPanel = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.Transparent
            };

            btnGuardar = new Guna2Button()
            {
                Text = "GUARDAR",
                Size = new Size(120, 40),
                Location = new Point(100, 15),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true
            };
            btnGuardar.Click += (s, e) => GuardarHabitacion();

            btnCancelar = new Guna2Button()
            {
                Text = "CANCELAR",
                Size = new Size(120, 40),
                Location = new Point(250, 15),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Animated = true
            };
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonsPanel.Controls.AddRange(new Control[] { btnGuardar, btnCancelar });

            // Agregar controles al panel principal
            mainPanel.Controls.AddRange(new Control[] {
                lblTitulo,
                lblNumero, txtNumero,
                lblTipo, cmbTipoHabitacion,
                lblPiso, txtPiso,
                lblEstado, cmbEstado,
                lblNotas, txtNotas,
                buttonsPanel
            });

            this.Controls.Add(mainPanel);
        }

        private void CargarTiposHabitacion()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetRoomTypes", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbTipoHabitacion.Items.Add(new
                            {
                                Id = reader.GetInt32("id"),
                                Display = $"{reader.GetString("nombre")} (${reader.GetDecimal("precio_base")})"
                            });
                        }
                    }
                }

                cmbTipoHabitacion.DisplayMember = "Display";
                cmbTipoHabitacion.ValueMember = "Id";
                if (cmbTipoHabitacion.Items.Count > 0)
                    cmbTipoHabitacion.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tipos de habitación: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatosHabitacion()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("SELECT * FROM habitaciones WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", habitacionId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNumero.Text = reader["numero"].ToString();
                            txtPiso.Text = reader["piso"].ToString();
                            cmbEstado.SelectedItem = reader["estado"].ToString();
                            txtNotas.Text = reader["notas"].ToString();

                            // Buscar el tipo de habitación
                            foreach (var item in cmbTipoHabitacion.Items)
                            {
                                dynamic tipo = item;
                                if (tipo.Id == Convert.ToInt32(reader["tipo_id"]))
                                {
                                    cmbTipoHabitacion.SelectedItem = item;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos de la habitación: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void GuardarHabitacion()
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text))
            {
                MessageBox.Show("El número de habitación es obligatorio", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedTipo = cmbTipoHabitacion.SelectedItem;
                int tipoId = selectedTipo.Id;

                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (esNuevo)
                    {
                        var cmd = new MySqlCommand("sp_CreateRoom", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_numero", txtNumero.Text);
                        cmd.Parameters.AddWithValue("@p_tipo_id", tipoId);
                        cmd.Parameters.AddWithValue("@p_piso", Convert.ToInt32(txtPiso.Text));
                        cmd.Parameters.AddWithValue("@p_estado", cmbEstado.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@p_notas", txtNotas.Text);

                        var nuevoId = cmd.ExecuteScalar();
                        MessageBox.Show("Habitación creada exitosamente", "Éxito",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var cmd = new MySqlCommand("sp_UpdateRoom", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", habitacionId);
                        cmd.Parameters.AddWithValue("@p_numero", txtNumero.Text);
                        cmd.Parameters.AddWithValue("@p_tipo_id", tipoId);
                        cmd.Parameters.AddWithValue("@p_piso", Convert.ToInt32(txtPiso.Text));
                        cmd.Parameters.AddWithValue("@p_estado", cmbEstado.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@p_notas", txtNotas.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Habitación actualizada exitosamente", "Éxito",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar habitación: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}