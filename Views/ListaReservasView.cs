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
    public partial class ListaReservasView : Form
    {
        private ConexionBD conexionBD;
        public ListaReservasView(ConexionBD conexionBD)
        {
            InitializeComponent();
            this.conexionBD = conexionBD;
        }
    }
}
