namespace Proyecto_PED.Models
{
    internal class HabitacionItem
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public decimal Precio { get; set; }
        public int Piso { get; set; }
        public string Estado { get; set; }
        public DateTime? UltimaLimpieza { get; set; }
        public string Notas { get; set; }

        public override string ToString()
        {
            return $"{Numero} - {Tipo} (${Precio}/noche)";
        }
    }
}
