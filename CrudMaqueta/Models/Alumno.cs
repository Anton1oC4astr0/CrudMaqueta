using System.Collections.ObjectModel;

namespace CrudMaqueta.Models
{
    public class Alumno
    {
        // Se almacena el número de control como una cadena.
        public string NumeroControl { get; set; } = string.Empty;
        // Se almacena el nombre completo del alumno en una sola propiedad.
        public string Nombre { get; set; } = string.Empty;
        // Se almacena el ID de la carrera como un entero. Para poder mostrar el nombre de la carrera, usando un join
        public int id_Carrera { get; set; }
        // Se almacena el correo electrónico del alumno como una cadena.
        public string Correo { get; set; } = string.Empty;
        // Se almacena la fecha de nacimiento como una cadena en formato "dd/MM/yyyy" para simplificar.
        public string FechaNac { get; set; } = string.Empty;
        // Para simplificar, se almacena el nombre de la carrera directamente en la propiedad Carrera.
        public string Carrera { get; set; } = string.Empty;
        // Para simplificar, se almacena una cadena con los nombres de las discapacidades seleccionadas, separados por comas.
        public string Discapacidad { get; set; } = "Ninguna";
        // Para simplificar, se almacena una cadena con los IDs de las discapacidades seleccionadas, separados por comas.
        public string Ids_Discapacidades { get; set; } = string.Empty;
    }
}
