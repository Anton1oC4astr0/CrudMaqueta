namespace CrudMaqueta.Models
{
    public class Alumno
    {
        public string NumeroControl { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string FechaNac { get; set; } = string.Empty;
        public int Edad { get; set; }

        // NUEVO: catálogo de discapacidades (o texto libre cuando elige "Otra").
        // Valor por defecto "Ninguna" para mantener compatibilidad con registros previos.
        public string Discapacidad { get; set; } = "Ninguna";
    }
}
