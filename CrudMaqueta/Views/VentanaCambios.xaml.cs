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
    public partial class VentanaCambios : Window
    {
       
        public VentanaCambios(Alumno alumno)
        {
            InitializeComponent();
            PreLlenarFormulario(alumno.NumeroControl);
            CargarCarreras();

        }
        private void CargarCarreras()
        {
            try
            {
                List<Carrera> carreas = CarrerasRepositorioOracle.ObtenerCarreras();
                cbCarrera.ItemsSource = carreas;

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                "Error al cargar las carreras. Por favor contacta al administrador.",
                "Error del sistema",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                btnGurdar.IsEnabled = false;

            }
        }
        private void Window_Loaded(
           object sender,
           RoutedEventArgs e)
        {
            CargarDiscapacidades();
        }
        // ===== Cargar dinámicamente las discapacidades desde la base de datos =====
        private void CargarDiscapacidades()
        {
            try
            {
                List<Discapacidad> lista = DiscapacidadRepositorioOracle.ObtenerDiscapacidades();

                panelDiscapacidades.Children.Clear();

                foreach (var d in lista)
                {
                    CheckBox chk =
                        new CheckBox();

                    chk.Content = d.nombre;

                    chk.Tag = d.id_Discapacidad;

                    chk.Margin = new Thickness(5);

                    // EVENTOS
                    chk.Checked += CheckBox_Checked;

                    chk.Unchecked += CheckBox_Unchecked;


                    panelDiscapacidades
                        .Children
                        .Add(chk);
                }

            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaAltas.CargarDiscapacidades", ex);
                MessageBox.Show(
                "Error al cargar las discapacidades. Por favor contacta al administrador.",
                "Error del sistema",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }
        // ===== Lógica para habilitar/deshabilitar opciones según la selección de "Ninguna" =====
        private void CheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            CheckBox actual =
                sender as CheckBox;


            if (actual.Content.ToString()
                .Equals("Ninguna",
                StringComparison
                .OrdinalIgnoreCase))
            {
                btnOtra.IsEnabled = false;
                pnlDiscapacida.Visibility = Visibility.Hidden;
                foreach (CheckBox chk
                    in panelDiscapacidades.Children)
                {
                    if (chk != actual)
                    {
                        chk.IsChecked = false;
                        chk.IsEnabled = false;
                    }
                }
            }
            else
            {
                foreach (CheckBox chk
                    in panelDiscapacidades.Children)
                {
                    if (chk.Content.ToString().Equals("Ninguna", StringComparison.OrdinalIgnoreCase))
                    {
                        chk.IsChecked =
                            false;
                    }
                }
            }
        }
        // Si se desmarca "Ninguna", se habilitan las demás opciones
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox actual = sender as CheckBox;

            if (actual.Content.ToString().Equals("Ninguna", StringComparison.OrdinalIgnoreCase))
            {
                btnOtra.IsEnabled = true;

                foreach (CheckBox chk in panelDiscapacidades.Children)
                {
                    chk.IsEnabled =
                        true;
                }
            }
        }

        // ===== Cálculo de edad a partir de la fecha de nacimiento =====
        private int CalcularEdad(DateTime fechaNac)
        {
            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - fechaNac.Year;
            if (fechaNac.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        private void PreLlenarFormulario(string numeroControl)
        {

            try
            {
                
                Alumno alumno = AlumnoRepositorioOracle.ObtenerALumnoPorNumeroControl(numeroControl);
                if (alumno != null)
                {
                    txtNumControl.Text = alumno.NumeroControl;
                    txtNombre.Text = alumno.Nombre;
                    txtCorreo.Text = alumno.Correo;
                    dpFechaNac.SelectedDate = DateTime.ParseExact(alumno.FechaNac, "dd/MM/yyyy", null);
                    lblEdad.Content = CalcularEdad(dpFechaNac.SelectedDate.Value).ToString();
                    cbCarrera.SelectedValue = alumno.Carrera;
                    // llenar checkboxes de discapacidades
                    string [] discapacidades = alumno.Discapacidad.Split(',');

                    foreach (CheckBox chk in panelDiscapacidades.Children)
                    {
                       int id = (int)chk.Tag;
                        chk.IsChecked = discapacidades.Contains(id.ToString(), StringComparer.OrdinalIgnoreCase);
                    }
                }
                else
                {
                    MessageBox.Show("Alumno no encontrado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaCambios.PreLlenarFormulario", ex);
                MessageBox.Show("Ocurrió un error al cargar los datos del alumno. Revisa el archivo errores.log.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            finally
            {
            }
        }

        // ===== Validación en tiempo real: EDAD =====
        private void txtEdad_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void txtEdad_TextChanged(object sender, TextChangedEventArgs e)
        {
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

        // ===== Actualizar =====
        private void btnCancelar_Click(object sender, RoutedEventArgs e) => Close();

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
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

            int edadCalculada = CalcularEdad(dpFechaNac.SelectedDate.Value);

            if (edadCalculada < 17 || edadCalculada > 88)
            { MessageBox.Show("La edad debe estar entre 17 y 80 años."); dpFechaNac.Focus(); return; }

            Carrera carreraSeleccionada = (Carrera)cbCarrera.SelectedItem;
            Alumno alumnoActualizado = new Alumno
            {
                NumeroControl = txtNumControl.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Correo = txtCorreo.Text.Trim(),
                id_Carrera = carreraSeleccionada.id_Carrera,
                FechaNac = dpFechaNac.SelectedDate.Value.ToString("dd/MM/yyyy"),
            };

            try
            {
                bool exito = AlumnoRepositorioOracle.Actualizar(alumnoActualizado);
                if (exito)
                {
                    MessageBox.Show("Alumno actualizado correctamente.");
                    Close();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el alumno.");
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaCambios.btnActualizar_Click", ex);
                MessageBox.Show("Ocurrió un error al actualizar. Revisa el archivo errores.log.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnOtra_Click(object sender, RoutedEventArgs e)
        {
            panelDiscapacidades.Visibility = Visibility.Visible;
        }
        private void Cacelar_NuevaDisc(object sender, RoutedEventArgs e)
        {
            txtDiscapacidad.Clear();
            pnlDiscapacida.Visibility = Visibility.Hidden;
        }
        // Método para guardar la nueva discapacidad en la base de datos
        private void Guardar_Disc(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDiscapacidad.Text))
                {
                    MessageBox.Show("Ingresa el nombre de la discapacidad.");
                    txtDiscapacidad.Focus();
                    return;
                }
                bool guardado = DiscapacidadRepositorioOracle.Agregar(new Discapacidad { nombre = txtDiscapacidad.Text.Trim() });
                if (guardado)
                {
                    MessageBox.Show($"Discapacidad agregada correctamente.");
                    txtDiscapacidad.Clear();
                    pnlDiscapacida.Visibility = Visibility.Hidden;
                    CargarDiscapacidades(); // Recarga las discapacidades para mostrar la nueva
                }
                else
                {
                    MessageBox.Show($"La discapacidad '{txtDiscapacidad.Text.Trim()}' ya existe.");
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("VentanaAltas.Guardar_Disc", ex);
                MessageBox.Show("Ocurrió un error al guardar la discapacidad. Discapacidad duplicada o existente.",
                    "Error del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Calc_Fecha(object sender, SelectionChangedEventArgs e)
        {
            int edadCalculada = CalcularEdad(dpFechaNac.SelectedDate ?? DateTime.Today);
            lblEdad.Content = $"Edad: {edadCalculada} años";
        }
    }
}