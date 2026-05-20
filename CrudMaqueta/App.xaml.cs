using System;
using System.Windows;
using System.Windows.Threading;
using CrudMaqueta.Utils;

namespace CrudMaqueta
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Registra el inicio en el log
            Logger.RegistrarInfo("App.OnStartup",
                $"Aplicación iniciada. Log en: {Logger.RutaLog}");

            // Cualquier excepción NO controlada se captura en errores.log
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.RegistrarError("App.DispatcherUnhandledException", e.Exception);
            MessageBox.Show(
                "Ocurrió un error inesperado.\nSe registró en el archivo errores.log.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logger.RegistrarError("AppDomain.UnhandledException", ex);
            }
        }
    }
}
