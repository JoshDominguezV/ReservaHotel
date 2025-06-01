using Proyecto_PED.Database;
using Proyecto_PED.Views;
using Proyecto_PED.Auth;



//using Proyecto_PED.Views;


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
                //Application.Run(new SplashScreen());

                Application.Run(new Main("admin","Administrador")); 
                //Application.Run(new Main("recepcion", "Recepcionista"));
            }
            catch (Exception ex)
            {
                // Si ocurre un error, mostrar el mensaje de error
                MessageBox.Show("Error en la inicialización de la aplicación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
