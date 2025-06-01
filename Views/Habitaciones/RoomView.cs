using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Proyecto_PED.Database;
using Proyecto_PED.Models;
using System.Data;

namespace Proyecto_PED.Views.Habitaciones
{
    public partial class RoomView : Form
    {
        private ConexionBD conexionBD;
        private string usuarioActual;
        private string rolActual;
        private int? habitacionSeleccionadaId = null;
        private Guna2TextBox txtBusqueda;
        private Guna2Button btnNuevaHabitacion;
        private Guna2Button btnEliminarHabitacion;
        private Guna2Button btnModificarHabitacion;
        private Guna2Button btnActualizar;
        private FlowLayoutPanel roomsContainer;

        public RoomView(string usuario, string rol)
        {
            usuarioActual = usuario;
            rolActual = rol;
            conexionBD = new ConexionBD();
            InitializeUI();
            CargarHabitaciones();
            ConfigPermisos();
        }

        private void ConfigPermisos()
        {
            bool esAdmin = rolActual == "Administrador";

            btnNuevaHabitacion.Enabled = esAdmin;
            btnEliminarHabitacion.Enabled = esAdmin;
            btnModificarHabitacion.Enabled = esAdmin;
        }

        private void InitializeUI()
        {
            this.Text = "Gestión de Habitaciones";
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
                Text = "Gestión de Habitaciones",
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
                PlaceholderText = "Buscar habitación...",
                Width = 250,
                Height = 36,
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10),
                BorderColor = Color.FromArgb(200, 200, 200),
                Margin = new Padding(0, 0, 10, 0)
            };

            txtBusqueda.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                    BuscarHabitaciones();
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
            btnActualizar.Click += (s, e) => CargarHabitaciones();

            btnNuevaHabitacion = new Guna2Button()
            {
                Text = "Nueva Habitación",
                Size = new Size(200, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Animated = true
            };
            btnNuevaHabitacion.Click += (s, e) => MostrarFormularioHabitacion();

            btnModificarHabitacion = new Guna2Button()
            {
                Text = "Modificar",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(70, 180, 130),
                ForeColor = Color.White,
                Animated = true,
                Enabled = false
            };
            btnModificarHabitacion.Click += (s, e) =>
            {
                if (habitacionSeleccionadaId.HasValue)
                    MostrarFormularioHabitacion(habitacionSeleccionadaId.Value);
            };

            btnEliminarHabitacion = new Guna2Button()
            {
                Text = "Eliminar",
                Size = new Size(120, 36),
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FillColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Animated = true,
                Enabled = false
            };
            btnEliminarHabitacion.Click += (s, e) => EliminarHabitacion();

            // Contenedor de habitaciones con scroll
            var scrollPanel = new Guna2Panel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };

            roomsContainer = new FlowLayoutPanel()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10)
            };

            // Agregar controles a los paneles
            var controlsFlow = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };
            controlsFlow.Controls.AddRange(new Control[] {
                txtBusqueda,
                btnActualizar,
                btnNuevaHabitacion,
                btnModificarHabitacion,
                btnEliminarHabitacion
            });

            controlsPanel.Controls.Add(controlsFlow);
            headerPanel.Controls.Add(lblTitulo);
            scrollPanel.Controls.Add(roomsContainer);

            mainPanel.Controls.Add(scrollPanel);
            mainPanel.Controls.Add(controlsPanel);
            mainPanel.Controls.Add(headerPanel);

            this.Controls.Add(mainPanel);
        }

        // Modificar el método CargarHabitaciones para que acepte un parámetro opcional
        private void CargarHabitaciones(int? idHabitacion = null)
        {
            try
            {
                if (idHabitacion == null)
                {
                    // Carga completa si no se especifica ID
                    roomsContainer.Controls.Clear();
                }

                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_GetAllRooms", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var habitacion = new HabitacionItem
                            {
                                Id = reader.GetInt32("id"),
                                Numero = reader.GetString("numero"),
                                Tipo = reader.GetString("tipo"),
                                Precio = reader.GetDecimal("precio"),
                                Piso = reader.IsDBNull("piso") ? 0 : reader.GetInt32("piso"),
                                Estado = reader.GetString("estado"),
                                UltimaLimpieza = reader.IsDBNull("ultima_limpieza") ? null : (DateTime?)reader.GetDateTime("ultima_limpieza"),
                                Notas = reader.IsDBNull("notas") ? null : reader.GetString("notas")
                            };

                            // Si se especificó un ID, solo actualizamos esa card
                            if (idHabitacion != null && habitacion.Id == idHabitacion)
                            {
                                ActualizarCard(habitacion);
                                break;
                            }
                            else if (idHabitacion == null)
                            {
                                var roomCard = CrearCardHabitacion(habitacion);
                                roomsContainer.Controls.Add(roomCard);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar habitaciones: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class ClickThroughPanel : Panel
        {
            protected override void WndProc(ref Message m)
            {
                const int WM_NCHITTEST = 0x0084;
                const int HTTRANSPARENT = (-1);

                if (m.Msg == WM_NCHITTEST)
                {
                    m.Result = (IntPtr)HTTRANSPARENT;
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
        }

        // Modifica el método CrearCardHabitacion
        private Guna2Panel CrearCardHabitacion(HabitacionItem habitacion)
        {
            var card = new Guna2Panel()
            {
                Size = new Size(220, 125),
                Margin = new Padding(10),
                BorderRadius = 15,
                BorderThickness = 2,
                BorderColor = Color.FromArgb(200, 200, 200),
                Cursor = Cursors.Hand,
                Tag = habitacion.Id,
                ShadowDecoration = {
                Enabled = true,
                Color = Color.FromArgb(100, 100, 100),
                BorderRadius = 20,
                Depth = 10,
                Shadow = new Padding(5)
                }
            };


            // Evento Click para seleccionar la habitación
            card.Click += (s, e) => SeleccionarHabitacion(card, habitacion.Id);

            // Color según estado
            Color estadoColor = ObtenerColorEstado(habitacion.Estado);
            card.FillColor = estadoColor;

            // Header con número de habitación
            var lblHeader = new Label()
            {
                Text = $"Hab. {habitacion.Numero}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(50, 50, 80),
                ForeColor = Color.White
            };

            // Usar el panel especial que permite clicks
            var contentPanel = new ClickThroughPanel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand // Importante para mantener el cursor
            };

            // Configurar los labels para no interceptar clicks
            var lblTipo = new Label()
            {
                Text = $"{habitacion.Tipo}",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5),
                Cursor = Cursors.Hand
            };

            var lblPrecio = new Label()
            {
                Text = $"${habitacion.Precio}/noche",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 25),
                Cursor = Cursors.Hand
            };

            var lblEstado = new Label()
            {
                Text = $"{habitacion.Estado}",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 45),
                Cursor = Cursors.Hand
            };

            var lblPiso = new Label()
            {
                Text = $"Piso: {habitacion.Piso}",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 65),
                Cursor = Cursors.Hand
            };

            // Configurar eventos para los labels también
            foreach (var lbl in new[] { lblTipo, lblPrecio, lblEstado, lblPiso })
            {
                lbl.Click += (s, e) => SeleccionarHabitacion(card, habitacion.Id);
            }

            contentPanel.Controls.AddRange(new Control[] { lblTipo, lblPrecio, lblEstado, lblPiso });
            card.Controls.Add(contentPanel);
            card.Controls.Add(lblHeader);

            return card;
        }

        // Modificar el método SeleccionarHabitacion para mejor feedback
        private void SeleccionarHabitacion(Guna2Panel cardSeleccionada, int idHabitacion)
        {
            // Deseleccionar todas las cards primero
            foreach (Control control in roomsContainer.Controls)
            {
                if (control is Guna2Panel panel)
                {
                    panel.BorderColor = Color.FromArgb(200, 200, 200);
                    panel.ShadowDecoration.Color = Color.FromArgb(100, 100, 100);
                }
            }

            // Seleccionar la card actual
            cardSeleccionada.BorderColor = Color.FromArgb(70, 130, 180);
            cardSeleccionada.ShadowDecoration.Color = Color.FromArgb(70, 130, 180);
            habitacionSeleccionadaId = idHabitacion;

            // Habilitar botones de acción
            btnModificarHabitacion.Enabled = true;
            btnEliminarHabitacion.Enabled = true;

            // Forzar el repintado para ver los cambios inmediatamente
            cardSeleccionada.Refresh();
        }

        // Método auxiliar para obtener color según estado
        private Color ObtenerColorEstado(string estado)
        {
            if (string.IsNullOrEmpty(estado))
                return Color.LightGray;

            switch (estado.ToUpper())
            {
                case "DISPONIBLE": return Color.FromArgb(144, 238, 144);
                case "OCUPADA": return Color.FromArgb(255, 182, 193);
                case "MANTENIMIENTO": return Color.FromArgb(255, 228, 181);
                case "LIMPIEZA": return Color.FromArgb(173, 216, 230);
                default: return Color.LightGray;
            }
        }

        private void BuscarHabitaciones()
        {
            try
            {
                roomsContainer.Controls.Clear();

                using (var conn = conexionBD.ObtenerConexion())
                {
                    var cmd = new MySqlCommand("sp_SearchRooms", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_search", txtBusqueda.Text);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var habitacion = new HabitacionItem
                            {
                                Id = reader.GetInt32("id"),
                                Numero = reader.GetString("numero"),
                                Tipo = reader.GetString("tipo"),
                                Precio = reader.GetDecimal("precio"),
                                Estado = reader.GetString("estado")
                            };

                            var roomCard = CrearCardHabitacion(habitacion);
                            roomsContainer.Controls.Add(roomCard);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar habitaciones: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Modificar el método EliminarHabitacion
        private void EliminarHabitacion()
        {
            if (!habitacionSeleccionadaId.HasValue)
            {
                MessageBox.Show("Seleccione una habitación para eliminar", "Advertencia",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show("¿Está seguro de eliminar esta habitación?", "Confirmar",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    using (var conn = conexionBD.ObtenerConexion())
                    {
                        var cmd = new MySqlCommand("sp_DeleteRoom", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_id", habitacionSeleccionadaId);
                        cmd.ExecuteNonQuery();
                    }

                    // Eliminar solo la card correspondiente
                    foreach (Control control in roomsContainer.Controls)
                    {
                        if (control is Guna2Panel panel && panel.Tag is int id && id == habitacionSeleccionadaId.Value)
                        {
                            roomsContainer.Controls.Remove(panel);
                            break;
                        }
                    }

                    MessageBox.Show("Habitación eliminada correctamente", "Éxito",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);

                    habitacionSeleccionadaId = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar habitación: {ex.Message}", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ActualizarCard(HabitacionItem habitacion)
        {
            // Buscar la card existente
            for (int i = 0; i < roomsContainer.Controls.Count; i++)
            {
                if (roomsContainer.Controls[i] is Guna2Panel panel && panel.Tag is int id && id == habitacion.Id)
                {
                    // Guardar la posición actual
                    int indice = i;

                    // Crear nueva card con los datos actualizados
                    var nuevaCard = CrearCardHabitacion(habitacion);

                    // Eliminar la card antigua
                    roomsContainer.Controls.RemoveAt(indice);

                    // Insertar la nueva card en la misma posición
                    roomsContainer.Controls.Add(nuevaCard);
                    roomsContainer.Controls.SetChildIndex(nuevaCard, indice);

                    // Si era la seleccionada, mantener la selección
                    if (habitacionSeleccionadaId == habitacion.Id)
                    {
                        SeleccionarHabitacion(nuevaCard, habitacion.Id);
                    }

                    break;
                }
            }
        }

        // Modificar el método MostrarFormularioHabitacion
        private void MostrarFormularioHabitacion(int idHabitacion = 0)
        {
            if (idHabitacion > 0 && !habitacionSeleccionadaId.HasValue)
                return;

            var idAEditar = idHabitacion == 0 ? idHabitacion : habitacionSeleccionadaId.Value;

            var formHabitacion = new RoomForm(idAEditar);
            if (formHabitacion.ShowDialog() == DialogResult.OK)
            {
                if (idAEditar == 0)
                {
                    // Para nueva habitación, recargar todas (o implementar lógica para añadir solo la nueva)
                    CargarHabitaciones();
                }
                else
                {
                    // Para modificación, actualizar solo la card afectada
                    CargarHabitaciones(idAEditar);
                }

                habitacionSeleccionadaId = null;
            }
        }
    }
}