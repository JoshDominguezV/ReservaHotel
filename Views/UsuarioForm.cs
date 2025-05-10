using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using Proyecto_PED.Models;

namespace Proyecto_PED.Views
{
    public partial class UsuarioForm : Form
    {
        private ConexionBD conexionBD;
        private string usuarioActual;
        private string rolActual;

        public UsuarioForm(string usuario, string rol)
        {
            InitializeComponent();
            usuarioActual = usuario;
            rolActual = rol;
            conexionBD = new ConexionBD();
            ConstruirInterfaz();
            CargarUsuarios();
            CargarRoles();
        }
        private void ConstruirInterfaz()
        {
            this.Text = $"Gestión de Usuarios - {usuarioActual} ({rolActual})";
            this.Size = new Size(630, 430);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView para mostrar usuarios
            var dgvUsuarios = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(580, 200),
                Name = "dgvUsuarios",
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;

            // Controles para agregar/editar usuarios
            var lblUsuario = new Label { Text = "Usuario:", Location = new Point(20, 240), AutoSize = true };
            var txtUsuario = new TextBox { Location = new Point(110, 240), Width = 150, Name = "txtUsuario", AutoSize = true };

            var lblContrasena = new Label { Text = "Contraseña:", Location = new Point(20, 280), AutoSize = true };
            var txtContrasena = new TextBox { Location = new Point(110, 280), Width = 150, Name = "txtContrasena", PasswordChar = '●', AutoSize = true };

            var lblRol = new Label { Text = "Rol:", Location = new Point(20, 320), AutoSize = true };
            var cmbRoles = new ComboBox { Location = new Point(110, 320), Width = 150, Name = "cmbRoles", DropDownStyle = ComboBoxStyle.DropDownList, AutoSize = true };

            var btnAgregar = new Button { Text = "Agregar", Location = new Point(270, 240), Size = new Size(80, 30), Name = "btnAgregar" };
            btnAgregar.Click += BtnAgregar_Click;

            var btnEditar = new Button { Text = "Editar", Location = new Point(270, 280), Size = new Size(80, 30), Name = "btnEditar" };
            btnEditar.Click += BtnEditar_Click;

            var btnEliminar = new Button { Text = "Eliminar", Location = new Point(270, 320), Size = new Size(80, 30), Name = "btnEliminar" };
            btnEliminar.Click += BtnEliminar_Click;

            this.Controls.AddRange(new Control[] {
                dgvUsuarios, lblUsuario, txtUsuario, lblContrasena, txtContrasena,
                lblRol, cmbRoles, btnAgregar, btnEditar, btnEliminar
            });
        }

        private void DgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            var dgvUsuarios = (DataGridView)this.Controls.Find("dgvUsuarios", true)[0];
            var txtUsuario = (TextBox)this.Controls.Find("txtUsuario", true)[0];
            var txtContrasena = (TextBox)this.Controls.Find("txtContrasena", true)[0];
            var cmbRoles = (ComboBox)this.Controls.Find("cmbRoles", true)[0];

            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvUsuarios.SelectedRows[0];

                txtUsuario.Text = selectedRow.Cells["nombre_usuario"].Value.ToString();
                txtContrasena.Text = ""; // No mostramos la contraseña por seguridad
                txtContrasena.Tag = selectedRow.Cells["id"].Value; // Guardamos el ID para referencia

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
        }

        private void CargarUsuarios()
        {
            var dgvUsuarios = (DataGridView)this.Controls.Find("dgvUsuarios", true)[0];

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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarRoles()
        {
            var cmbRoles = (ComboBox)this.Controls.Find("cmbRoles", true)[0];
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
                MessageBox.Show($"Error al cargar roles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            var txtUsuario = (TextBox)this.Controls.Find("txtUsuario", true)[0];
            var txtContrasena = (TextBox)this.Controls.Find("txtContrasena", true)[0];
            var cmbRoles = (ComboBox)this.Controls.Find("cmbRoles", true)[0];

            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Usuario y contraseña son obligatorios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

                        MessageBox.Show("Usuario creado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarUsuarios();

                        txtUsuario.Text = "";
                        txtContrasena.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            var dgvUsuarios = (DataGridView)this.Controls.Find("dgvUsuarios", true)[0];
            var txtUsuario = (TextBox)this.Controls.Find("txtUsuario", true)[0];
            var cmbRoles = (ComboBox)this.Controls.Find("cmbRoles", true)[0];

            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario para editar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    if (conn != null)
                    {
                        var id = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["id"].Value);
                        var rol = (RolItem)cmbRoles.SelectedItem;

                        var cmd = new MySqlCommand("sp_UpdateUser", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_id", id);
                        cmd.Parameters.AddWithValue("p_nombre_usuario", txtUsuario.Text);
                        cmd.Parameters.AddWithValue("p_rol_id", rol.Id);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Usuario actualizado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarUsuarios();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            var dgvUsuarios = (DataGridView)this.Controls.Find("dgvUsuarios", true)[0];

            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario para eliminar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este usuario?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = conexionBD.ObtenerConexion())
                    {
                        if (conn != null)
                        {
                            var id = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["id"].Value);

                            var cmd = new MySqlCommand("sp_DeleteUser", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("p_id", id);

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Usuario eliminado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarUsuarios();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
