using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_PED.Models
{
    internal class HabitacionItem
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public decimal Precio { get; set; }

        public override string ToString()
        {
            return $"{Numero} - {Tipo} (${Precio}/noche)";
        }
    }
}
