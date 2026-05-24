using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CrudMaqueta.Data;
using CrudMaqueta.Models;
using CrudMaqueta.Utils;

namespace CrudMaqueta.Views
{
    public partial class VentanaAltas : Window
    {
        public VentanaAltas()
        {
            InitializeComponent();

            // Bloquea pegar texto no numérico en la edad
            DataObject.AddPastingHandler(txtEdad, (s, e) =>
            {
                string texto = (e.DataObject.GetData(typeof(string)) as string) ?? "";
                if (!int.TryParse(texto, out _)) e.CancelCommand();
            });
        }

        private int CalcularEdad(DateTime fechaNac)
        {
            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - fechaNac.Year;
            if (fechaNac.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        // ===== Validación en tiempo real: EDAD =====
        private void txtEdad_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Bloquea cualquier tecla que no sea dígito
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void txtEdad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lblErrorEdad == null) return;

            string texto = txtEdad.Text.Trim();

            if (string.IsNullOrEmpty(texto))
            {
                lblErrorEdad.Visibility = Visibility.Collapsed;
                txtEdad.ClearValue(Border.BorderBrushProperty);
                return;
            }

            if (!int.TryParse(texto, out int edad))
            {
                lblErrorEdad.Text = "Debe ser un número";
                lblErrorEdad.Visibility = Visibility.Visible;
                txtEdad.BorderBrush = Brushes.Red;
            }
            else if (edad < 17 || edad > 80)
            {
                lblErrorEdad.Text = "Debe estar entre 17 y 80";
                lblErrorEdad.Visibility = Visibility.Visible;
                txtEdad.BorderBrush = Brushes.Red;
            }
            else
            {
                lblErrorEdad.Visibility = Visibility.Collapsed;
                txtEdad.ClearValue(Border.BorderBrushProperty);
            }
        }

        // ===== Validación en tiempo real: CORREO =====
        private void txtCorreo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lblErrorCorreo == null) return;
            lblErrorCorreo.Visibility = Visibility.Collapsed;
            txtCorreo.ClearValue(Border.BorderBrushProperty);
        }

        private void txtCorreo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (lblErrorCorreo == null) return;

            string texto = txtCorreo.Text.Trim();
            if (string.IsNullOrEmpty(texto))
            {
                lblErrorCorreo.Visibility = Visibility.Collapsed;
                txtCorreo.ClearValue(Border.BorderBrushProperty);
                return;
            }

            string patronCorreo = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            if (!Regex.IsMatch(texto, patronCorreo))
            {
                lblErrorCorreo.Text = "Formato inválido (ej: a@b.com)";
                lblErrorCorreo.Visibility = Visibility.Visible;
                txtCorreo.BorderBrush = Brushes.Red;
            }
            else
            {
                lblErrorCorreo.Visibility = Visibility.Collapsed;
                txtCorreo.ClearValue(Border.BorderBrushProperty);
            }
        }

        // ===== Discapacidad: muestra/oculta TextBox "Otra" =====
        private void cbDiscapacidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblOtraDiscapacidad == null || txtOtraDiscapacidad == null) return;

            if (cbDiscapacidad.SelectedItem is ComboBoxItem item &&
                item.Content?.ToString() == "Otra")
            {
                lblOtraDiscapacidad.Visibility = Visibility.Visible;
                txtOtraDiscapacidad.Visibility = Visibility.Visible;
            }
            else
            {
                lblOtraDiscapacidad.Visibility = Visibility.Collapsed;
                txtOtraDiscapacidad.Visibility = Visibility.Collapsed;
                txtOtraDiscapacidad.Text = string.Empty;
            }
        }

        // ===== Guardar =====
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumControl.Text))
            { MessageBox.Show("Ingresa el número de control."); txtNumControl.Focus(); return; }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Ingresa el nombre."); txtNombre.Focus(); return; }

            if (cbCarrera.SelectedItem == null)
            { MessageBox.Show("Selecciona una carrera."); cbCarrera.Focus(); return; }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
            { MessageBox.Show("Ingresa el correo."); txtCorreo.Focus(); return; }

            string patronCorreo = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            if (!Regex.IsMatch(txtCorreo.Text.Trim(), patronCorreo))
            { MessageBox.Show("El correo no tiene un formato válido.\nEjemplo: nombre@dominio.com"); txtCorreo.Focus(); return; }

            if (dpFechaNac.SelectedDate == null)
            { MessageBox.Show("Selecciona la fecha de nacimiento."); dpFechaNac.Focus(); return; }

            if (string.IsNullOrWhiteSpace(txtEdad.Text))
            { MessageBox.Show("Ingresa la edad."); txtEdad.Focus(); return; }

            if (!int.TryParse(txtEdad.Text, out int edad))
            { MessageBox.Show("La edad debe ser un número entero."); txtEdad.Focus(); return; }

            if (edad <= 0)
            { MessageBox.Show("La edad debe ser mayor a 0."); txtEdad.Focus(); return; }

            int edadCalculada = CalcularEdad(dpFechaNac.SelectedDate.Value);
            if (edad != edadCalculada)
            {
                MessageBox.Show($"La edad no coincide con la fecha de nacimiento.\n" +
                                $"Edad ingresada: {edad}\nEdad correcta según la fecha: {edadCalculada}");
                txtEdad.Focus(); return;
            }

            if (edad < 17 || edad > 80)
            { MessageBox.Show("La edad debe estar entre 17 y 80 años."); dpFechaNac.Focus(); return; }

            if (cbDiscapacidad.SelectedItem == null)
            { MessageBox.Show("Selecciona una discapacidad (o 'Ninguna')."); cbDiscapacidad.Focus(); return; }

            string discapacidad = ((ComboBoxItem)cbDiscapacidad.SelectedItem).Content?.ToString() ?? "Ninguna";

            if (discapacidad == "Otra")
            {
                if (string.IsNullOrWhiteSpace(txtOtraDiscapacidad.Text))
                { MessageBox.Show("Especifica la discapacidad en el campo 'Especifique'."); txtOtraDiscapacidad.Focus(); return; }
                discapacidad = txtOtraDiscapacidad.Text.Trim();
            }

            Alumno alumno = new Alumno
            {
                NumeroControl = txtNumControl.Text.Trim().ToUpper(),
                Nombre = txtNombre.Text.Trim(),
                Carrera = ((ComboBoxItem)cbCarrera.SelectedItem).Content.ToString() ?? string.Empty,
                Correo = txtCorreo.Text.Trim(),
                FechaNac = dpFechaNac.SelectedDate.Value.ToString("dd/MM/yyyy"),
                Edad = edad,
                Discapacidad = discapacidad
            };

            try
            {
                bool guardado = AlumnoRepositorio.Agregar(alumno);
                if (guardado)
                {
                    MessageBox.Show($"Alumno {alumno.Nombre} registrado correctamente.");
                    Close();
                }
                else
                {
                    MessageBox.Show($"El número de control '{alumno.NumeroControl}' ya existe.");
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaAltas.btnGuardar_Click", ex);
                MessageBox.Show("Ocurrió un error al guardar. Revisa el archivo errores.log.",
                    "Error del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => Close();
    }
}