using System;
using System.Windows;
using CrudMaqueta.Data;
using CrudMaqueta.Models;
using CrudMaqueta.Utils;

namespace CrudMaqueta.Views
{
    public partial class VentanaBajas : Window
    {
        private readonly Alumno _alumnoBaja;

        public VentanaBajas(Alumno alumno)
        {
            InitializeComponent();
            _alumnoBaja = alumno;
            MostrarDatos();
        }

        private void MostrarDatos()
        {
            lblNumControl.Content = _alumnoBaja.NumeroControl;
            lblNombre.Content = _alumnoBaja.Nombre;
            lblCarrera.Content = _alumnoBaja.Carrera;
            lblCorreo.Content = _alumnoBaja.Correo;
            lblDiscapacidad.Content = string.IsNullOrWhiteSpace(_alumnoBaja.Discapacidad)
                ? "Ninguna" : _alumnoBaja.Discapacidad;
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool respuesta = AlumnoRepositorioOracle.Eliminar(_alumnoBaja.NumeroControl);
                if (respuesta)
                {
                    MessageBox.Show("El alumno fue eliminado correctamente.");
                    Close();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el alumno.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaBajas.btnEliminar_Click", ex);
                MessageBox.Show(
                    "Ocurrió un error al eliminar. Revisa el archivo errores.log.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
