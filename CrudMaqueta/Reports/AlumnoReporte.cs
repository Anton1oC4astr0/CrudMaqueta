using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using CrudMaqueta.Models;
using CrudMaqueta.Utils;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CrudMaqueta.Reports
{
    public static class AlumnoReporte
    {
        private static byte[]? CargarLogoEmbebido()
        {
            try
            {
                Uri uri = new Uri("pack://application:,,,/Resources/mi_logo.png", UriKind.Absolute);
                var streamInfo = Application.GetResourceStream(uri);
                if (streamInfo == null) return null;

                using (var ms = new MemoryStream())
                {
                    streamInfo.Stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch
            {
                try
                {
                    string ruta = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, "Resources", "mi_logo.png");
                    return File.Exists(ruta) ? File.ReadAllBytes(ruta) : null;
                }
                catch { return null; }
            }
        }

        private static int CalcularEdad(string fechaNac)
        {
            if (DateTime.TryParseExact(fechaNac, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime fecha))
            {
                var hoy = DateTime.Today;
                int edad = hoy.Year - fecha.Year;
                if (fecha.Date > hoy.AddYears(-edad)) edad--;
                return edad;
            }
            return 0;
        }

        private static string SanitizarNombreArchivo(string nombre)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                nombre = nombre.Replace(c, '_');
            return nombre.Replace(' ', '_');
        }

        /// <summary>
        /// Genera el reporte PDF respetando los filtros activos.
        /// </summary>
        /// <param name="alumnos">Lista de alumnos a incluir (ya filtrada).</param>
        /// <param name="filtroCarrera">Valor del filtro de carrera ("Todas" si no aplica).</param>
        /// <param name="filtroDiscapacidad">Valor del filtro de discapacidad ("Todas" si no aplica).</param>
        public static void Generar(
            List<Alumno> alumnos,
            string filtroCarrera = "Todas",
            string filtroDiscapacidad = "Todas")
        {
            QuestPDF.Settings.License = LicenseType.Community;
            string colorEncabezado = Colors.Green.Darken3;
            byte[]? logoBytes = CargarLogoEmbebido();

            // --- Construir nombre de archivo según filtros activos ---
            bool hayFiltroCarrera      = filtroCarrera      != "Todas";
            bool hayFiltroDiscapacidad = filtroDiscapacidad != "Todas";

            string sufijo = "";
            if (hayFiltroCarrera && hayFiltroDiscapacidad)
                sufijo = $"_Carrera-{SanitizarNombreArchivo(filtroCarrera)}_Disc-{SanitizarNombreArchivo(filtroDiscapacidad)}";
            else if (hayFiltroCarrera)
                sufijo = $"_Carrera-{SanitizarNombreArchivo(filtroCarrera)}";
            else if (hayFiltroDiscapacidad)
                sufijo = $"_Disc-{SanitizarNombreArchivo(filtroDiscapacidad)}";

            string nombreArchivo = $"ReporteAlumnos{sufijo}.pdf";
            string rutaPdf = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                nombreArchivo);

            // --- Texto descriptivo de filtros para el encabezado ---
            string descripcionFiltros = "";
            if (hayFiltroCarrera && hayFiltroDiscapacidad)
                descripcionFiltros = $"Filtros activos — Carrera: {filtroCarrera}  |  Discapacidad: {filtroDiscapacidad}";
            else if (hayFiltroCarrera)
                descripcionFiltros = $"Filtro activo — Carrera: {filtroCarrera}";
            else if (hayFiltroDiscapacidad)
                descripcionFiltros = $"Filtro activo — Discapacidad: {filtroDiscapacidad}";
            else
                descripcionFiltros = "Sin filtros aplicados — todos los alumnos";

            try
            {
                Document.Create(contenedor =>
                {
                    contenedor.Page(pagina =>
                    {
                        pagina.Size(PageSizes.A4.Landscape());
                        pagina.Margin(25);
                        pagina.DefaultTextStyle(x => x.FontFamily("Verdana"));

                        // ── Encabezado ──────────────────────────────────────────
                        pagina.Header().Row(row =>
                        {
                            if (logoBytes != null && logoBytes.Length > 0)
                                row.ConstantItem(70).Image(logoBytes);

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().PaddingLeft(10)
                                    .Text("Reporte de Alumnos")
                                    .FontSize(20).Bold();
                                col.Item().PaddingLeft(10)
                                    .Text("Tecnológico Nacional de México - Instituto Tecnológico de Toluca")
                                    .FontSize(10).FontColor(Colors.Grey.Darken1);
                                col.Item().PaddingLeft(10)
                                    .Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(10).FontColor(Colors.Grey.Medium);
                                col.Item().PaddingLeft(10)
                                    .Text($"Total de alumnos: {alumnos.Count}  —  {descripcionFiltros}")
                                    .FontSize(10).Bold().FontColor(Colors.Green.Darken3);
                            });
                        });

                        // ── Tabla de datos ──────────────────────────────────────
                        pagina.Content().PaddingTop(15).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columnas =>
                            {
                                columnas.RelativeColumn(2.0f); // No. Control
                                columnas.RelativeColumn(3.0f); // Nombre
                                columnas.RelativeColumn(3.0f); // Carrera
                                columnas.RelativeColumn(3.0f); // Correo
                                columnas.RelativeColumn(2.0f); // Fecha Nac
                                columnas.RelativeColumn(1.0f); // Edad
                                columnas.RelativeColumn(2.5f); // Discapacidades
                            });

                            // Encabezado de columnas
                            tabla.Header(encabezado =>
                            {
                                void Celda(string texto) =>
                                    encabezado.Cell()
                                        .Background(colorEncabezado)
                                        .Padding(5)
                                        .Text(texto)
                                        .FontColor(Colors.White)
                                        .Bold()
                                        .FontSize(9);

                                Celda("No. Control");
                                Celda("Nombre");
                                Celda("Carrera");
                                Celda("Correo");
                                Celda("Fecha Nac.");
                                Celda("Edad");
                                Celda("Discapacidades");
                            });

                            // Filas de datos
                            bool filaPar = false;
                            foreach (Alumno alumno in alumnos)
                            {
                                string colorFila = filaPar ? Colors.Grey.Lighten3 : Colors.White;
                                filaPar = !filaPar;

                                int edad = CalcularEdad(alumno.FechaNac);
                                string discapacidad = string.IsNullOrWhiteSpace(alumno.Discapacidad)
                                    ? "Ninguna"
                                    : alumno.Discapacidad;

                                void Dato(string texto) =>
                                    tabla.Cell()
                                        .Background(colorFila)
                                        .Padding(5)
                                        .Text(texto)
                                        .FontSize(9);

                                Dato(alumno.NumeroControl);
                                Dato(alumno.Nombre);
                                Dato(alumno.Carrera);          // nombre de carrera (no el ID)
                                Dato(alumno.Correo);
                                Dato(alumno.FechaNac);
                                Dato(edad > 0 ? edad.ToString() : "—");
                                Dato(discapacidad);
                            }
                        });

                        // ── Pie de página ───────────────────────────────────────
                        pagina.Footer().AlignRight().Text(texto =>
                        {
                            texto.Span("Página ").FontSize(10);
                            texto.CurrentPageNumber().FontSize(10);
                            texto.Span(" de ").FontSize(10);
                            texto.TotalPages().FontSize(10);
                        });
                    });
                }).GeneratePdf(rutaPdf);

                Logger.RegistrarInfo("AlumnoReporte.Generar",
                    $"Reporte PDF generado en: {rutaPdf} | Filtros: carrera={filtroCarrera}, discapacidad={filtroDiscapacidad}");

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = rutaPdf,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoReporte.Generar", ex);
                throw;
            }
        }
    }
}
