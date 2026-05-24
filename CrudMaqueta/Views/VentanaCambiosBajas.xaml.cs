using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CrudMaqueta.Data;
using CrudMaqueta.Models;
using CrudMaqueta.Utils;

namespace CrudMaqueta.Views
{
    public partial class VentanaCambiosBajas : Window
    {
        // Modo de operación: "Cambios" o "Bajas"
        private readonly string _modo;

        public VentanaCambiosBajas(string modo)
        {
            InitializeComponent();
            _modo = modo;

            // Configuración dinámica de la UI según el modo
            // Esto evita que se "repitan" los botones Modificar/Eliminar:
            // sólo aparece el que corresponde a la acción que se va a realizar.
            if (_modo == "Cambios")
            {
                Title = "Modificar Alumnos";
                lblTitulo.Text = "Modificar Alumnos";
                btnAccion.Content = "Modificar";
            }
            else // "Bajas"
            {
                Title = "Eliminar Alumnos";
                lblTitulo.Text = "Eliminar Alumnos";
                btnAccion.Content = "Eliminar";
                btnAccion.Background = System.Windows.Media.Brushes.IndianRed;
                btnAccion.Foreground = System.Windows.Media.Brushes.White;
            }

            // IMPORTANTE: La tabla inicia VACÍA.
            // El usuario debe buscar primero por número de control o carrera.
            dgAlumnos.ItemsSource = null;
        }

        // === Búsqueda combinada (número de control y/o carrera) ===
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string numControl = txtBuscarControl.Text.Trim();
            ComboBoxItem? itemCarrera = cbFiltroCarrera.SelectedItem as ComboBoxItem;
            string carrera = itemCarrera?.Content?.ToString() ?? "Todas";

            // Si no se ingresó ningún criterio, avisar
            if (string.IsNullOrEmpty(numControl) && carrera == "Todas")
            {
                MessageBox.Show(
                    "Ingresa un número de control o selecciona una carrera para buscar.",
                    "Búsqueda",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            try
            {
                List<Alumno> resultados = AlumnoRepositorio.ObtenerDatos();

                // Filtro por número de control (coincidencia parcial, case-insensitive)
                if (!string.IsNullOrEmpty(numControl))
                {
                    resultados = resultados
                        .Where(a => a.NumeroControl
                            .StartsWith(numControl, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Filtro por carrera
                if (carrera != "Todas")
                {
                    resultados = resultados
                        .Where(a => string.Equals(a.Carrera, carrera, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                dgAlumnos.ItemsSource = resultados;

                lblEstado.Text = resultados.Count == 0
                    ? "No se encontraron alumnos con los criterios indicados."
                    : $"Se encontraron {resultados.Count} alumno(s).";
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaCambiosBajas.btnBuscar_Click", ex);
                MessageBox.Show(
                    "Ocurrió un error al consultar la base de datos.\nRevisa el archivo errores.log.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscarControl.Clear();
            cbFiltroCarrera.SelectedIndex = 0;
            dgAlumnos.ItemsSource = null;
            lblEstado.Text = "Realiza una búsqueda para mostrar resultados.";
        }

        // === Acción única según el modo ===
        private void btnAccion_Click(object sender, RoutedEventArgs e)
        {
            if (dgAlumnos.SelectedItem is not Alumno alumnoSeleccionado)
            {
                MessageBox.Show(
                    "Selecciona un alumno de la tabla.",
                    "Atención",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (_modo == "Cambios")
            {
                VentanaCambios ventana = new VentanaCambios(alumnoSeleccionado);
                ventana.Owner = this;
                ventana.ShowDialog();
            }
            else // "Bajas"
            {
                VentanaBajas ventana = new VentanaBajas(alumnoSeleccionado);
                ventana.Owner = this;
                ventana.ShowDialog();
            }

            // Refrescar resultados con los mismos filtros
            btnBuscar_Click(sender, e);
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
