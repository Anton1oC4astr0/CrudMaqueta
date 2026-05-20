using System.Windows;
using CrudMaqueta.Views;

namespace CrudMaqueta
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAltas_Click(object sender, RoutedEventArgs e)
        {
            VentanaAltas ventana = new VentanaAltas();
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        private void btnConsulta_Click(object sender, RoutedEventArgs e)
        {
            VentanaConsulta ventana = new VentanaConsulta();
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        private void btnCambios_Click(object sender, RoutedEventArgs e)
        {
            VentanaCambiosBajas ventana = new VentanaCambiosBajas("Cambios");
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        private void btnBajas_Click(object sender, RoutedEventArgs e)
        {
            VentanaCambiosBajas ventana = new VentanaCambiosBajas("Bajas");
            ventana.Owner = this;
            ventana.ShowDialog();
        }
    }
}
