using Proyecto_PED.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_PED.Views
{
    public partial class ClientesView : Form
    {
        private ConexionBD conexionBD;

        public ClientesView(ConexionBD conexionBD)
        {
            InitializeComponent();
            this.conexionBD = conexionBD;

            // Lógica de carga de clientes.
        }
    }

}
