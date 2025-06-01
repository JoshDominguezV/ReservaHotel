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
            string devone = "Tec. Josué Naum Domínguez Velásquez";
            string devtwo = "Tec. Kevin Armando Lemus Alas";

            this.BackColor = Color.White;

            var lblBienvenido = new Label
            {
                Text = $"Bienvenido, {usuario} al sistema de reserva de hotel",
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


            var lbldeveloped = new Label
            {
                Text = "Developed by:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 1040)
            };

            var lblDevs = new Label
            {
                Text = $"{devone} || {devtwo}",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(50, 1060)
            };


            this.Controls.Add(lbldeveloped);
            this.Controls.Add(lblDevs);
        }
    }
}