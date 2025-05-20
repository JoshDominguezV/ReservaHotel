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
    public partial class UsuariosView : Form
    {
        private string usuario;
        private string rol;
        public UsuariosView(string usuario, string rol)
        {
            InitializeComponent();
            this.usuario = usuario;
            this.rol = rol;
        }
    }
}
