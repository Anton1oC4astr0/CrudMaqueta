using System.Windows;
using CrudMaqueta.Views;
using CrudMaqueta.Data;
using CrudMaqueta.Models;

namespace CrudMaqueta.Views
{
    public partial class VentanaBuscar : Window
    {
        private readonly string _modo;

        public VentanaBuscar(string modo)
        {
            InitializeComponent();
            _modo = modo;

            lblTitulo.Content = _modo == "Cambios" ? "Buscar para Editar"
                                                   : "Buscar para Eliminar";
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                MessageBox.Show("Ingresa un numero de control.");
                return;
            }

            Alumno? alumno = AlumnoRepositorio.BuscarPorControl(txtBuscar.Text.Trim());

            if (alumno == null)
            {
                MessageBox.Show("El numero de control no existe.");
                return;
            }

            if (_modo == "Cambios")
            {
                VentanaCambios ventana = new VentanaCambios(alumno);
                ventana.Owner = this;
                ventana.ShowDialog();
            }
            else
            {
                VentanaBajas ventana = new VentanaBajas(alumno);
                ventana.Owner = this;
                ventana.ShowDialog();
            }

            Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}