using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Windows.Forms;

namespace Proyecto_PED.Database
{
    public class ConexionBD
    {
        // stringbase
        private string connectionString = "server=localhost;database=ped_hotel_reservas;uid=root;pwd=;";


        public MySqlConnection ObtenerConexion()
        {
            MySqlConnection conexion = new MySqlConnection(connectionString);
            try
            {
                conexion.Open(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al conectar a la base de datos: " + ex.Message, "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; 
            }

            return conexion; 
        }

/*
        public void ProbarConexion()
        {
            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("✅ Conexión exitosa.\nUsuarios registrados:\n");

                        string query = "SELECT u.id, u.nombre_usuario, r.nombre_rol FROM usuarios u JOIN roles r ON u.rol_id = r.id";
                        MySqlCommand cmd = new MySqlCommand(query, conexion);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string usuario = reader.GetString("nombre_usuario");
                            string rol = reader.GetString("nombre_rol");

                            sb.AppendLine($"🧑 ID: {id}, Usuario: {usuario}, Rol: {rol}");
                        }

                        MessageBox.Show(sb.ToString(), "Prueba de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al realizar la consulta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
   */
        // Método para realizar una consulta general (solo ejemplo)
        public void EjecutarConsulta(string query)
        {
            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion != null)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conexion);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("✅ Consulta ejecutada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al ejecutar la consulta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
