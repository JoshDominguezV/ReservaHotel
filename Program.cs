using Proyecto_PED.Auth;
using Proyecto_PED.Database;
//using Proyecto_PED.Views;
using System;
using System.Windows.Forms;

namespace Proyecto_PED
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                  ConexionBD conexion = new ConexionBD();

                // test conexion --josue
                //conexion.ProbarConexion();

  
                // Inicializar la aplicación
                ApplicationConfiguration.Initialize();
                Application.Run(new SplashScreen());
                //Application.Run(new Reservacion("admin","Administrador")); 
                //Application.Run(new MainDashboard("admin", "admin"));
            }
            catch (Exception ex)
            {
                // Si ocurre un error, mostrar el mensaje de error
                MessageBox.Show("Error en la inicialización de la aplicación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
