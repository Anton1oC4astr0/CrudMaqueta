using System.IO;

namespace CrudMaqueta.Utils
{
    public static class Logger
    {
        // Ruta del archivo de log junto al ejecutable
        private static readonly string _rutaLog =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errores.log");

        /// <summary>
        /// Registra un error en el archivo errores.log
        /// </summary>
        public static void RegistrarError(string origen, Exception ex)
        {
            string linea = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] " +
                           $"[{origen}] " +
                           $"[ERROR: {ex.GetType().Name} - {ex.Message}]";

            // Intenta escribir, si falla no rompe la aplicación principal
            try
            {
                File.AppendAllText(_rutaLog, linea + Environment.NewLine);
            }
            catch
            {
                // Silenciosamente ignora errores de escritura
            }
        }

        /// <summary>
        /// Registra un mensaje informativo (eventos, inicio, operaciones exitosas)
        /// </summary>
        public static void RegistrarInfo(string origen, string mensaje)
        {
            string linea = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] " +
                           $"[INFO] " +
                           $"[{origen}] {mensaje}";

            try
            {
                // Imprime error en el archivo errores.log
                File.AppendAllText(_rutaLog, linea + Environment.NewLine);
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// Propiedad pública para acceder a la ruta del archivo de log
        /// </summary>
        public static string RutaLog => _rutaLog;
    }
}