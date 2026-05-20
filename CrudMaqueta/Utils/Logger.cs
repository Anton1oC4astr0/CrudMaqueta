using System;
using System.IO;

namespace CrudMaqueta.Utils
{
    /// <summary>
    /// Logger sencillo en archivo de texto. Cada llamada agrega una línea
    /// al archivo errores.log dentro del directorio de la aplicación,
    /// generando evidencia en tiempo real de las excepciones capturadas.
    /// </summary>
    public static class Logger
    {
        // El archivo se crea junto al ejecutable (bin/Debug/.../errores.log)
        private static readonly string _rutaLog =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errores.log");

        public static void RegistrarError(string origen, Exception ex)
        {
            string linea = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}]" +
                           $"[{origen}]" +
                           $"[{ex.GetType().Name}: {ex.Message}]";

            try
            {
                File.AppendAllText(_rutaLog, linea + Environment.NewLine);
            }
            catch
            {
                // Si por alguna razón no se puede escribir el log,
                // no propagamos la excepción para no romper el flujo principal.
            }
        }

        /// <summary>
        /// Registra un mensaje informativo (eventos exitosos, inicio de app, etc.).
        /// </summary>
        public static void RegistrarInfo(string origen, string mensaje)
        {
            string linea = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}]" +
                           $"[INFO][{origen}]{mensaje}";

            try
            {
                File.AppendAllText(_rutaLog, linea + Environment.NewLine);
            }
            catch { }
        }

        public static string RutaLog => _rutaLog;
    }
}
