using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CrudMaqueta.Data;
using CrudMaqueta.Models;
using CrudMaqueta.Reports;
using CrudMaqueta.Utils;

namespace CrudMaqueta.Views
{
    public partial class VentanaConsulta : Window
    {
        private List<Alumno> _todosLosAlumnos = new List<Alumno>();

        public VentanaConsulta()
        {
            InitializeComponent();
            CargarAlumnos();
        }

        private void CargarAlumnos()
        {
            try
            {
                _todosLosAlumnos = AlumnoRepositorioOracle.ObtenerDatosTodo();
                dgAlumnos.ItemsSource = _todosLosAlumnos;
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaConsulta.CargarAlumnos", ex);
                MessageBox.Show("Ocurrió un error al consultar alumnos. Revisa el archivo errores.log.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Devuelve el valor seleccionado en un ComboBox.
        /// Si no hay selección válida regresa "Todas".
        /// </summary>
        private static string ObtenerFiltro(ComboBox cb)
            => (cb.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todas";

        private void AplicarFiltros()
        {
            if (_todosLosAlumnos == null) return;

            string carrera      = ObtenerFiltro(cbFiltroCarrera);
            string discapacidad = ObtenerFiltro(cbFiltroDiscapacidad);

            IEnumerable<Alumno> consulta = _todosLosAlumnos;

            // Filtro por carrera
            if (carrera != "Todas")
                consulta = consulta.Where(a => a.Carrera == carrera);

            // Filtro por discapacidad
            // "Ninguna" busca alumnos sin ninguna discapacidad registrada;
            // cualquier otro valor busca alumnos cuya lista de discapacidades lo contenga.
            if (discapacidad != "Todas")
            {
                if (discapacidad == "Ninguna")
                    consulta = consulta.Where(a =>
                        string.IsNullOrWhiteSpace(a.Discapacidad) || a.Discapacidad == "Ninguna");
                else
                    consulta = consulta.Where(a =>
                        !string.IsNullOrWhiteSpace(a.Discapacidad) &&
                        a.Discapacidad.Split(',')
                            .Select(d => d.Trim())
                            .Contains(discapacidad, StringComparer.OrdinalIgnoreCase));
            }

            dgAlumnos.ItemsSource = consulta.ToList();
        }

        private void cbFiltroCarrera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAlumnos == null) return;
            AplicarFiltros();
        }

        private void cbFiltroDiscapacidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAlumnos == null) return;
            AplicarFiltros();
        }

        private void btnGenerarReporte_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Alumno> alumnosVisibles = (dgAlumnos.ItemsSource as IEnumerable<Alumno>)?.ToList()
                    ?? new List<Alumno>();

                if (alumnosVisibles.Count == 0)
                {
                    MessageBox.Show("No hay alumnos para incluir en el reporte.",
                        "Reporte PDF", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Pasar los filtros activos al reporte para el nombre del archivo y el encabezado
                string filtroCarrera      = ObtenerFiltro(cbFiltroCarrera);
                string filtroDiscapacidad = ObtenerFiltro(cbFiltroDiscapacidad);

                AlumnoReporte.Generar(alumnosVisibles, filtroCarrera, filtroDiscapacidad);
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaConsulta.btnGenerarReporte_Click", ex);
                MessageBox.Show("Ocurrió un error al generar el reporte.\nRevisa el archivo errores.log.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
