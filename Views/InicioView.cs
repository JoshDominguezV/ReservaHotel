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

    public partial class InicioView : Form
    {
        private string usuario;
        private string rol;

        public InicioView(string usuario, string rol)
        {
            InitializeComponent();
            this.usuario = usuario;
            this.rol = rol;

            // Lógica para cargar datos según el usuario y rol si es necesario.
        }
    }

}
