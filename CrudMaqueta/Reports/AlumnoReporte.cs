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
 
        public static void Generar(List<Alumno> alumnos)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            string colorEncabezado = Colors.Green.Darken3;
            byte[]? logoBytes = CargarLogoEmbebido();
 
            string rutaPdf = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "ReporteAlumnos.pdf");
 
            try
            {
                Document.Create(contenedor =>
                {
                    contenedor.Page(pagina =>
                    {
                        pagina.Size(PageSizes.A4.Landscape());
                        pagina.Margin(25);
                        pagina.DefaultTextStyle(x => x.FontFamily("Verdana"));
 
                        pagina.Header().Row(row =>
                        {
                            if (logoBytes != null && logoBytes.Length > 0)
                                row.ConstantItem(70).Image(logoBytes);
 
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().PaddingLeft(10).Text("Reporte de Alumnos").FontSize(20).Bold();
                                col.Item().PaddingLeft(10)
                                    .Text("Tecnológico Nacional de México - Instituto Tecnológico de Toluca")
                                    .FontSize(10).FontColor(Colors.Grey.Darken1);
                                col.Item().PaddingLeft(10)
                                    .Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(10).FontColor(Colors.Grey.Medium);
                                col.Item().PaddingLeft(10)
                                    .Text($"Total de alumnos registrados: {alumnos.Count}")
                                    .FontSize(11).Bold().FontColor(Colors.Green.Darken3);
                            });
                        });
 
                        pagina.Content().PaddingTop(15).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columnas =>
                            {
                                columnas.RelativeColumn(2);
                                columnas.RelativeColumn(3);
                                columnas.RelativeColumn(3);
                                columnas.RelativeColumn(3);
                                columnas.RelativeColumn(2);
                                columnas.RelativeColumn(1);
                                columnas.RelativeColumn(2);
                            });
 
                            tabla.Header(encabezado =>
                            {
                                void Celda(string texto)
                                {
                                    encabezado.Cell().Background(colorEncabezado).Padding(5)
                                        .Text(texto).FontColor(Colors.White).Bold();
                                }
                                Celda("No. Control"); Celda("Nombre"); Celda("Carrera");
                                Celda("Correo"); Celda("Fecha Nac"); Celda("Edad"); Celda("Discapacidad");
                            });
 
                            bool filaPar = false;
                            foreach (Alumno alumno in alumnos)
                            {
                                string colorFila = filaPar ? Colors.Grey.Lighten3 : Colors.White;
                                filaPar = !filaPar;
                                tabla.Cell().Background(colorFila).Padding(5).Text(alumno.NumeroControl);
                                tabla.Cell().Background(colorFila).Padding(5).Text(alumno.Nombre);
                                tabla.Cell().Background(colorFila).Padding(5).Text(alumno.id_Carrera);
                                tabla.Cell().Background(colorFila).Padding(5).Text(alumno.Correo);
                                tabla.Cell().Background(colorFila).Padding(5).Text(alumno.FechaNac);
                            }
                        });
 
                        pagina.Footer().AlignRight().Text(texto =>
                        {
                            texto.Span("Página ").FontSize(10);
                            texto.CurrentPageNumber().FontSize(10);
                            texto.Span(" de ").FontSize(10);
                            texto.TotalPages().FontSize(10);
                        });
                    });
                }).GeneratePdf(rutaPdf);
 
                Logger.RegistrarInfo("AlumnoReporte.Generar", $"Reporte PDF generado en: {rutaPdf}");
 
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