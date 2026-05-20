using System.Collections.Generic;
using System.Linq;
using CrudMaqueta.Models;

namespace CrudMaqueta.Data
{
    /// <summary>
    /// Repositorio en memoria (fallback / pruebas).
    /// Mantiene la misma firma que AlumnoRepositorioOracle, incluido el campo Discapacidad.
    /// </summary>
    public static class AlumnoRepositorio
    {
        private static List<Alumno> _alumnos = new List<Alumno>();

        public static bool Agregar(Alumno alumno)
        {
            if (_alumnos.Any(a => a.NumeroControl == alumno.NumeroControl))
                return false;

            _alumnos.Add(alumno);
            return true;
        }

        public static List<Alumno> ObtenerDatos()
        {
            return new List<Alumno>(_alumnos);
        }

        public static Alumno? BuscarPorControl(string numeroControl)
        {
            return _alumnos.FirstOrDefault(a => a.NumeroControl == numeroControl);
        }

        public static bool Eliminar(string numeroControl)
        {
            Alumno? alumno = BuscarPorControl(numeroControl);
            if (alumno == null)
                return false;

            _alumnos.Remove(alumno);
            return true;
        }

        public static bool Actualizar(Alumno alumnoActualizado)
        {
            Alumno? existente = BuscarPorControl(alumnoActualizado.NumeroControl);
            if (existente == null) return false;

            existente.Nombre = alumnoActualizado.Nombre;
            existente.Carrera = alumnoActualizado.Carrera;
            existente.Correo = alumnoActualizado.Correo;
            existente.FechaNac = alumnoActualizado.FechaNac;
            existente.Edad = alumnoActualizado.Edad;
            existente.Discapacidad = alumnoActualizado.Discapacidad;
            return true;
        }
    }
}
