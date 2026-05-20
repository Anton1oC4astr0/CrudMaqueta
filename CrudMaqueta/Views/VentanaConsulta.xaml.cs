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
                _todosLosAlumnos = AlumnoRepositorioOracle.ObtenerDatos();
                dgAlumnos.ItemsSource = _todosLosAlumnos;
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaConsulta.CargarAlumnos", ex);
                MessageBox.Show("Ocurrió un error al consultar alumnos. Revisa el archivo errores.log.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AplicarFiltros()
        {
            if (_todosLosAlumnos == null) return;

            string carrera = (cbFiltroCarrera.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todas";
            string discapacidad = (cbFiltroDiscapacidad.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todas";

            IEnumerable<Alumno> consulta = _todosLosAlumnos;

            if (carrera != "Todas")
                consulta = consulta.Where(a => a.Carrera == carrera);

            if (discapacidad != "Todas")
            {
                if (discapacidad == "Otra")
                {
                    string[] catalogo = { "Ninguna", "Visual", "Auditiva", "Motriz",
                                          "Cognitiva", "Psicosocial", "Lenguaje" };
                    consulta = consulta.Where(a => !catalogo.Contains(a.Discapacidad));
                }
                else
                {
                    consulta = consulta.Where(a => a.Discapacidad == discapacidad);
                }
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

                AlumnoReporte.Generar(alumnosVisibles);
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