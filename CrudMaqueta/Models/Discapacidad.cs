using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudMaqueta.Models
{
    public class Discapacidad
    {
        public int id_Discapacidad { get; set; }
        public string nombre { get; set; } = "Ninguna";
        public bool Seleccionada { get; set; }

    }
}
