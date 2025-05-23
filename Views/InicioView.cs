using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_PED.Views
{
    public partial class InicioView : Form
    {
        public InicioView(string usuario, string rol)
        {
            InitializeComponent();
            ConstruirInterfaz(usuario, rol);
        }

        private void ConstruirInterfaz(string usuario, string rol)
        {
            this.BackColor = Color.White;

            var lblBienvenido = new Label
            {
                Text = $"Bienvenido, {usuario} al sistema de reserva",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 50)
            };

            var lblRol = new Label
            {
                Text = $"Rol: {rol}",
                Font = new Font("Segoe UI", 14),
                AutoSize = true,
                Location = new Point(50, 100)
            };

            this.Controls.Add(lblRol);
            this.Controls.Add(lblBienvenido);
        }
    }
}