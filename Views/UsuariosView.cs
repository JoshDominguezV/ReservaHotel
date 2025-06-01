using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using Proyecto_PED.Models;
using System.Data;

namespace Proyecto_PED.Views
{
    public partial class UsuariosView : Form
    {
        private ConexionBD conexionBD;
        private string usuarioActual;
        private string rolActual;
        private int? usuarioSeleccionadoId = null;

        public UsuariosView(string usuario, string rol)
        {
            InitializeComponent();
            usuarioActual = usuario;
            rolActual = rol;
            conexionBD = new ConexionBD();

            InitializeForm();
            ConstruirInterfaz();
            CargarUsuarios();
            CargarRoles();
        }

        private void InitializeForm()
        {
            this.Text = $"Gestión de Usuarios - {usuarioActual} ({rolActual})";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 240, 245);
        }

        private void ConstruirInterfaz()
        {
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

            // Header con título centrado
            var headerPanel = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 70, // Altura reducida para mejor proporción
                BackColor = Color.FromArgb(50, 50, 80),
                BorderRadius = 20,
                Padding = new Padding(20, 0, 20, 0)
            };

            var lblTitulo = new Label()
            {
                Text = "Gestión de Usuarios",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Height = headerPanel.Height
            };

            // DataGridView con altura ajustada
            var dgvUsuarios = new Guna2DataGridView()
            {
                Name = "dgvUsuarios",
                Dock = DockStyle.Fill,
                Margin = new Padding(20, 10, 20, 15),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };

            // Configuración de estilos del DataGridView
            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 80);
            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsuarios.RowsDefaultCellStyle.BackColor = Color.White;
            dgvUsuarios.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvUsuarios.RowsDefaultCellStyle.ForeColor = Color.FromArgb(50, 50, 50);
            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvUsuarios.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgvUsuarios.DefaultCellStyle.SelectionForeColor = Color.FromArgb(50, 50, 50);

            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;

            // Panel de controles con mejor distribución
            var controlsPanel = new Guna2Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 220, // Altura aumentada para mejor espaciado
                BackColor = Color.White,
                Padding = new Padding(25) // Padding aumentado
            };

            // Constantes para posicionamiento
            const int labelX = 30;
            const int controlX = 150;
            const int rowHeight = 60;
            const int buttonX = 420;
            const int buttonWidth = 120;
            const int buttonHeight = 35;

            // Campos del formulario con espaciado perfecto
            var lblUsuario = new Label
            {
                Text = "Usuario:",
                Location = new Point(labelX, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            var txtUsuario = new Guna2TextBox
            {
                Location = new Point(controlX, 20),
                Width = 250,
                Name = "txtUsuario",
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                Margin = new Padding(0, 0, 0, 10) // Margen inferior
            };

            var lblContrasena = new Label
            {
                Text = "Contraseña:",
                Location = new Point(labelX, 25 + rowHeight),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            var txtContrasena = new Guna2TextBox
            {
                Location = new Point(controlX, 20 + rowHeight),
                Width = 250,
                Name = "txtContrasena",
                PasswordChar = '●',
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                Margin = new Padding(0, 0, 0, 10) // Margen inferior
            };

            var lblRol = new Label
            {
                Text = "Rol:",
                Location = new Point(labelX, 25 + rowHeight * 2),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            var cmbRoles = new Guna2ComboBox
            {
                Location = new Point(controlX, 20 + rowHeight * 2),
                Width = 250,
                Name = "cmbRoles",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                BorderRadius = 5,
                BorderColor = Color.FromArgb(200, 200, 200)
            };

            // Botones perfectamente alineados
            var btnAgregar = new Guna2Button
            {
                Text = "Agregar",
                Location = new Point(buttonX, 20),
                Size = new Size(buttonWidth, buttonHeight),
                Name = "btnAgregar",
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true
            };
            btnAgregar.Click += BtnAgregar_Click;

            var btnEditar = new Guna2Button
            {
                Text = "Editar",
                Location = new Point(buttonX, 20 + rowHeight),
                Size = new Size(buttonWidth, buttonHeight),
                Name = "btnEditar",
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(60, 179, 113),
                ForeColor = Color.White,
                Animated = true
            };
            btnEditar.Click += BtnEditar_Click;

            var btnEliminar = new Guna2Button
            {
                Text = "Eliminar",
                Location = new Point(buttonX, 20 + rowHeight * 2),
                Size = new Size(buttonWidth, buttonHeight),
                Name = "btnEliminar",
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Animated = true
            };
            btnEliminar.Click += BtnEliminar_Click;

            var btnLimpiar = new Guna2Button
            {
                Text = "Limpiar",
                Location = new Point(buttonX + buttonWidth + 10, 20),
                Size = new Size(buttonWidth, buttonHeight),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                Animated = true
            };
            btnLimpiar.Click += (s, e) => LimpiarCampos();

            // Agregar controles a los paneles
            controlsPanel.Controls.AddRange(new Control[] {
        lblUsuario, txtUsuario, lblContrasena, txtContrasena,
        lblRol, cmbRoles, btnAgregar, btnEditar, btnEliminar, btnLimpiar
    });

            headerPanel.Controls.Add(lblTitulo);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 10, 20, 20)
            };
            contentPanel.Controls.Add(dgvUsuarios);

            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(controlsPanel);
            mainPanel.Controls.Add(headerPanel);

            this.Controls.Add(mainPanel);
        }
        private void DgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            var dgvUsuarios = (Guna2DataGridView)this.Controls.Find("dgvUsuarios", true)[0];
            var txtUsuario = (Guna2TextBox)this.Controls.Find("txtUsuario", true)[0];
            var txtContrasena = (Guna2TextBox)this.Controls.Find("txtContrasena", true)[0];
            var cmbRoles = (Guna2ComboBox)this.Controls.Find("cmbRoles", true)[0];

            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvUsuarios.SelectedRows[0];
                usuarioSeleccionadoId = Convert.ToInt32(selectedRow.Cells["id"].Value);

                txtUsuario.Text = selectedRow.Cells["nombre_usuario"].Value.ToString();
                txtContrasena.Text = "";

                string rolNombre = selectedRow.Cells["nombre_rol"].Value.ToString();
                foreach (RolItem item in cmbRoles.Items)
                {
                    if (item.Nombre == rolNombre)
                    {
                        cmbRoles.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                usuarioSeleccionadoId = null;
            }
        }

        private void CargarUsuarios()
        {
            var dgvUsuarios = (Guna2DataGridView)this.Controls.Find("dgvUsuarios", true)[0];

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        var cmd = new MySqlCommand("sp_GetAllUsersWithRoles", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        var adapter = new MySqlDataAdapter(cmd);
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        dgvUsuarios.DataSource = dt;

                        if (dgvUsuarios.Columns.Contains("id"))
                            dgvUsuarios.Columns["id"].Visible = false;
                        if (dgvUsuarios.Columns.Contains("contrasena"))
                            dgvUsuarios.Columns["contrasena"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError($"Error al cargar usuarios: {ex.Message}");
            }
        }

        private void CargarRoles()
        {
            var cmbRoles = (Guna2ComboBox)this.Controls.Find("cmbRoles", true)[0];
            cmbRoles.Items.Clear();

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        var cmd = new MySqlCommand("sp_GetAllRoles", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cmbRoles.Items.Add(new RolItem
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre_rol")
                            });
                        }

                        if (cmbRoles.Items.Count > 0)
                            cmbRoles.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError($"Error al cargar roles: {ex.Message}");
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            var txtUsuario = (Guna2TextBox)this.Controls.Find("txtUsuario", true)[0];
            var txtContrasena = (Guna2TextBox)this.Controls.Find("txtContrasena", true)[0];
            var cmbRoles = (Guna2ComboBox)this.Controls.Find("cmbRoles", true)[0];

            if (!ValidarCamposUsuario(txtUsuario.Text, txtContrasena.Text))
                return;

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        var rol = (RolItem)cmbRoles.SelectedItem;
                        var cmd = new MySqlCommand("sp_CreateUser", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_nombre_usuario", txtUsuario.Text);
                        cmd.Parameters.AddWithValue("p_contrasena", txtContrasena.Text);
                        cmd.Parameters.AddWithValue("p_rol_id", rol.Id);

                        cmd.ExecuteNonQuery();

                        MostrarMensajeExito("Usuario creado exitosamente");
                        CargarUsuarios();
                        LimpiarCampos();
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MostrarMensajeError("El nombre de usuario ya existe");
                txtUsuario.Focus();
            }
            catch (Exception ex)
            {
                MostrarMensajeError($"Error al crear usuario: {ex.Message}");
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            var dgvUsuarios = (Guna2DataGridView)this.Controls.Find("dgvUsuarios", true)[0];
            var txtUsuario = (Guna2TextBox)this.Controls.Find("txtUsuario", true)[0];
            var cmbRoles = (Guna2ComboBox)this.Controls.Find("cmbRoles", true)[0];

            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MostrarMensajeAdvertencia("Seleccione un usuario para editar");
                return;
            }

            if (!ValidarCamposUsuario(txtUsuario.Text))
                return;

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        var rol = (RolItem)cmbRoles.SelectedItem;

                        var cmd = new MySqlCommand("sp_UpdateUser", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_id", usuarioSeleccionadoId);
                        cmd.Parameters.AddWithValue("p_nombre_usuario", txtUsuario.Text);
                        cmd.Parameters.AddWithValue("p_rol_id", rol.Id);

                        cmd.ExecuteNonQuery();

                        MostrarMensajeExito("Usuario actualizado exitosamente");
                        CargarUsuarios();
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MostrarMensajeError("El nombre de usuario ya existe");
                txtUsuario.Focus();
            }
            catch (Exception ex)
            {
                MostrarMensajeError($"Error al actualizar usuario: {ex.Message}");
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            var dgvUsuarios = (Guna2DataGridView)this.Controls.Find("dgvUsuarios", true)[0];

            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MostrarMensajeAdvertencia("Seleccione un usuario para eliminar");
                return;
            }

            if (dgvUsuarios.SelectedRows[0].Cells["nombre_usuario"].Value.ToString() == usuarioActual)
            {
                MostrarMensajeError("No puede eliminarse a sí mismo");
                return;
            }

            if (MostrarMensajeConfirmacion("¿Está seguro de eliminar este usuario?") == DialogResult.Yes)
            {
                try
                {
                    using (var conn = conexionBD.ObtenerConexion())
                    {
                        if (conn != null)
                        {
                            var cmd = new MySqlCommand("sp_DeleteUser", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("p_id", usuarioSeleccionadoId);

                            cmd.ExecuteNonQuery();

                            MostrarMensajeExito("Usuario eliminado exitosamente");
                            CargarUsuarios();
                            LimpiarCampos();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensajeError($"Error al eliminar usuario: {ex.Message}");
                }
            }
        }

        private bool ValidarCamposUsuario(string usuario, string contrasena = null)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                MostrarMensajeAdvertencia("El nombre de usuario es obligatorio");
                return false;
            }

            if (contrasena != null && string.IsNullOrWhiteSpace(contrasena))
            {
                MostrarMensajeAdvertencia("La contraseña es obligatoria");
                return false;
            }

            if (contrasena != null && contrasena.Length < 6)
            {
                MostrarMensajeAdvertencia("La contraseña debe tener al menos 6 caracteres");
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            var txtUsuario = (Guna2TextBox)this.Controls.Find("txtUsuario", true)[0];
            var txtContrasena = (Guna2TextBox)this.Controls.Find("txtContrasena", true)[0];
            var cmbRoles = (Guna2ComboBox)this.Controls.Find("cmbRoles", true)[0];
            var dgvUsuarios = (Guna2DataGridView)this.Controls.Find("dgvUsuarios", true)[0];

            txtUsuario.Text = "";
            txtContrasena.Text = "";
            if (cmbRoles.Items.Count > 0)
                cmbRoles.SelectedIndex = 0;
            dgvUsuarios.ClearSelection();
            usuarioSeleccionadoId = null;
        }

        private void MostrarMensajeExito(string mensaje)
        {
            MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MostrarMensajeAdvertencia(string mensaje)
        {
            MessageBox.Show(mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void MostrarMensajeError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private DialogResult MostrarMensajeConfirmacion(string mensaje)
        {
            return MessageBox.Show(mensaje, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}