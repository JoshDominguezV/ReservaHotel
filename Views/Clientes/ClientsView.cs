using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;

namespace Proyecto_PED.Views.Clientes
{
    public partial class ClientsView : Form
    {
        private ConexionBD conexionBD;
        private string usuarioActual;
        private string rolActual;
        private int? clienteSeleccionadoId = null;
        private Guna2DataGridView dgvClientes;
        private Guna2TextBox txtBusqueda;
        private Guna2Button btnNuevoCliente;
        private Guna2Button btnEliminarCliente;
        private Guna2Button btnActualizar;

        public ClientsView(string usuario, string rol)
        {
            usuarioActual = usuario;
            rolActual = rol;
            conexionBD = new ConexionBD();
            InitializeUI();
            CargarClientes();
            ConfigPermisos();
        }

        private void ConfigPermisos()
        {
            bool tienePermiso = rolActual == "Administrador" || rolActual == "Recepcionista";

            btnNuevoCliente.Enabled = tienePermiso;
            btnEliminarCliente.Visible = rolActual == "Administrador"; 
        }
        private void InitializeUI()
        {
            this.Text = "Gestión de Clientes";
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(240, 240, 245);
            this.FormBorderStyle = FormBorderStyle.None;

            // Panel principal con sombra
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
                Text = "Gestión de Clientes",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Height = headerPanel.Height
            };

            // Panel de controles superiores
            var controlsPanel = new Guna2Panel()
            {
                Dock = DockStyle.Top,
                Height = 78,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };

            txtBusqueda = new Guna2TextBox()
            {
                PlaceholderText = "Buscar cliente...",
                Width = 250,
                Height = 36, 
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                Margin = new Padding(0, 0, 10, 0)
            };

            txtBusqueda.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                    BuscarClientes();
            };

            btnActualizar = new Guna2Button()
            {
                Text = "Actualizar",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(100, 100, 150),
                ForeColor = Color.White,
                Animated = true
            };
            btnActualizar.Click += (s, e) => CargarClientes();

            btnNuevoCliente = new Guna2Button()
            {
                Text = "Nuevo Cliente",
                Size = new Size(150, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true
            };
            btnNuevoCliente.Click += (s, e) => MostrarFormularioCliente();

            // Botón Eliminar Cliente
            btnEliminarCliente = new Guna2Button()
            {
                Text = "Eliminar",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Animated = true,
            };
            btnEliminarCliente.Click += (s, e) => EliminarCliente(); 

            // DataGridView con estilo Guna2
            dgvClientes = new Guna2DataGridView()
            {
                Name = "dgvClientes",
                Dock = DockStyle.Fill,
                Margin = new Padding(20, 10, 20, 20),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };

            // Configuración de estilos del DataGridView
            dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 80);
            dgvClientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvClientes.RowsDefaultCellStyle.BackColor = Color.White;
            dgvClientes.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvClientes.RowsDefaultCellStyle.ForeColor = Color.FromArgb(50, 50, 50);
            dgvClientes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvClientes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgvClientes.DefaultCellStyle.SelectionForeColor = Color.FromArgb(50, 50, 50);

            dgvClientes.CellDoubleClick += (s, e) => EditarClienteSeleccionado();

            // Agregar controles a los paneles
            var controlsFlow = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };
            controlsFlow.Controls.AddRange(new Control[] { txtBusqueda, btnActualizar, btnNuevoCliente, btnEliminarCliente });

            controlsPanel.Controls.Add(controlsFlow);
            headerPanel.Controls.Add(lblTitulo);

            var contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            contentPanel.Controls.Add(dgvClientes);

            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(controlsPanel);
            mainPanel.Controls.Add(headerPanel);

            this.Controls.Add(mainPanel);
        }

        private void CargarClientes()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetClients", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    dgvClientes.DataSource = dt;

                    if (dgvClientes.Columns.Count > 0)
                    {
                        dgvClientes.Columns["id"].Visible = false;
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
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuscarClientes()
        {
            try
            {
                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_SearchClients", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_busqueda", txtBusqueda.Text);

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    dgvClientes.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar clientes: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarCliente()
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un cliente para eliminar", "Advertencia",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show("¿Está seguro de eliminar este cliente?", "Confirmar",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    int idCliente = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["id"].Value);

                    using (var conn = conexionBD.ObtenerConexion())
                    {
                        var cmd = new MySqlCommand("sp_DeleteClient", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", idCliente);
                        cmd.ExecuteNonQuery();
                    }

                    CargarClientes();
                    MessageBox.Show("Cliente eliminado correctamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar cliente: {ex.Message}", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditarClienteSeleccionado()
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                int idCliente = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["id"].Value);
                MostrarFormularioCliente(idCliente);
            }
        }

        private void MostrarFormularioCliente(int idCliente = 0)
        {
            var formCliente = new ClientForm(idCliente);
            if (formCliente.ShowDialog() == DialogResult.OK)
            {
                CargarClientes();
            }
        }
    }
}