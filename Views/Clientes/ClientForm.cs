using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;

namespace Proyecto_PED.Views.Clientes
{
    public partial class ClientForm : Form
    {
        private ConexionBD conexionBD;
        private int clienteId;
        private bool esNuevo;

        private Guna2TextBox txtNombre;
        private Guna2TextBox txtApellido;
        private Guna2ComboBox cmbTipoDocumento;
        private Guna2TextBox txtDocumento;
        private Guna2TextBox txtTelefono;
        private Guna2TextBox txtCorreo;
        private Guna2Button btnGuardar;
        private Guna2Button btnCancelar;

        public ClientForm(ConexionBD conexion, int idCliente = 0)
        {
            conexionBD = conexion;
            clienteId = idCliente;
            esNuevo = (idCliente == 0);

            InitializeUI();
            if (!esNuevo) CargarDatosCliente();
        }

        private void InitializeUI()
        {
            this.Text = esNuevo ? "Nuevo Cliente" : "Editar Cliente";
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
                Text = esNuevo ? "NUEVO CLIENTE" : "EDITAR CLIENTE",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 80),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Constantes para posicionamiento
            const int labelWidth = 120;
            const int controlX = 155;
            const int rowHeight = 45;

            // Campos del formulario
            var lblNombre = new Label
            {
                Text = "Nombre:",
                Location = new Point(30, 90),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            txtNombre = new Guna2TextBox()
            {
                Location = new Point(controlX, 90),
                Size = new Size(300, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
            };

            var lblApellido = new Label
            {
                Text = "Apellido:",
                Location = new Point(30, 90 + rowHeight),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            txtApellido = new Guna2TextBox()
            {
                Location = new Point(controlX, 90 + rowHeight),
                Size = new Size(300, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
            };

            var lblTipoDoc = new Label
            {
                Text = "Tipo Documento:",
                Location = new Point(30, 90 + rowHeight * 2),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            cmbTipoDocumento = new Guna2ComboBox()
            {
                Location = new Point(controlX, 90 + rowHeight * 2),
                Size = new Size(190, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTipoDocumento.Items.AddRange(new object[] { "DNI", "Pasaporte", "Carnet Extranjería", "Otro" });
            cmbTipoDocumento.SelectedIndex = 0;

            var lblDocumento = new Label
            {
                Text = "N° Documento:",
                Location = new Point(30, 90 + rowHeight * 3),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            txtDocumento = new Guna2TextBox()
            {
                Location = new Point(controlX, 90 + rowHeight * 3),
                Size = new Size(200, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
            };

            var lblTelefono = new Label
            {
                Text = "Teléfono:",
                Location = new Point(30, 90 + rowHeight * 4),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            txtTelefono = new Guna2TextBox()
            {
                Location = new Point(controlX, 90 + rowHeight * 4),
                Size = new Size(200, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
            };

            var lblCorreo = new Label
            {
                Text = "Correo:",
                Location = new Point(30, 90 + rowHeight * 5),
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false
            };

            txtCorreo = new Guna2TextBox()
            {
                Location = new Point(controlX, 90 + rowHeight * 5),
                Size = new Size(300, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200)
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
            btnGuardar.Click += (s, e) => GuardarCliente();

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
                lblNombre, txtNombre,
                lblApellido, txtApellido,
                lblTipoDoc, cmbTipoDocumento,
                lblDocumento, txtDocumento,
                lblTelefono, txtTelefono,
                lblCorreo, txtCorreo,
                buttonsPanel
            });

            this.Controls.Add(mainPanel);
        }

        private void CargarDatosCliente()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetClientById", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_id", clienteId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNombre.Text = reader["nombre"].ToString();
                            txtApellido.Text = reader["apellido"].ToString();
                            cmbTipoDocumento.SelectedItem = reader["tipo_documento"].ToString();
                            txtDocumento.Text = reader["documento_identidad"].ToString();
                            txtTelefono.Text = reader["telefono"].ToString();
                            txtCorreo.Text = reader["correo"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del cliente: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void GuardarCliente()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (esNuevo)
                    {
                        var cmd = new MySqlCommand("sp_CreateClient", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@p_apellido", txtApellido.Text);
                        cmd.Parameters.AddWithValue("@p_tipo_documento", cmbTipoDocumento.SelectedItem);
                        cmd.Parameters.AddWithValue("@p_documento_identidad", txtDocumento.Text);
                        cmd.Parameters.AddWithValue("@p_telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@p_correo", txtCorreo.Text);

                        var nuevoId = cmd.ExecuteScalar();
                        MessageBox.Show("Cliente creado exitosamente", "Éxito",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var cmd = new MySqlCommand("sp_UpdateClient", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", clienteId);
                        cmd.Parameters.AddWithValue("@p_nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@p_apellido", txtApellido.Text);
                        cmd.Parameters.AddWithValue("@p_tipo_documento", cmbTipoDocumento.SelectedItem);
                        cmd.Parameters.AddWithValue("@p_documento_identidad", txtDocumento.Text);
                        cmd.Parameters.AddWithValue("@p_telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@p_correo", txtCorreo.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cliente actualizado exitosamente", "Éxito",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Ya existe un cliente con este documento de identidad", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cliente: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}