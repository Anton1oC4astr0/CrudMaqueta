using CrudMaqueta.Models;
using CrudMaqueta.Utils;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.IO;

namespace CrudMaqueta.Reports
{
    public static class AlumnoReporte
    {
        public static void Generar(List<Alumno> alumnos)
        {
            try
            {
                // <  Licencia es gratuita   >
                QuestPDF.Settings.License = LicenseType.Community;

                // < donde esta la ruta de mi logo  >
                string rutaLogo = @"D:\CrudMaqueta Proyecto(1)\CrudMaqueta\CrudMaqueta\Resources\ittol-LOGO.jpeg";

                // <  en donde se genera el reporte >
                string rutaPdf = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "ReporteAlumnos.pdf");

                // Definición de colores institucionales ITTol
                var azulReyITTol = "#0B3C5D"; // Azul Rey elegante
                var naranjaITTol = "#D9534F"; // Naranja de acento para destacar

                // <  construccion de mi reporte  >
                Document.Create(contenedor =>
                {
                    contenedor.Page(pagina =>
                    {
                        pagina.Size(PageSizes.A4.Landscape());
                        pagina.Margin(30);
                        pagina.DefaultTextStyle(x => x.FontFamily("Calibri")); // cambio deletra 

                        // <  -----  >
                        pagina.Header().Column(headerCol =>
                        {
                            // Fila principal del encabezado
                            headerCol.Item().Row(row =>
                            {
                                // Logo a la izquierda
                                if (File.Exists(rutaLogo))
                                {
                                    row.ConstantItem(80).Height(60).Image(rutaLogo);
                                }

                                // Título y Fecha al centro/izquierda con desplazamiento
                                row.RelativeItem().PaddingLeft(15).Column(col =>
                                {
                                    col.Item().Text("Reporte de Alumnos")
                                       .FontSize(24)
                                       .Bold()
                                       .FontColor(azulReyITTol);

                                    col.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                                       .FontSize(10)
                                       .FontColor(Colors.Grey.Medium);
                                });

                                // < antidad de alumnos - NUEVA POSICIÓN ESTILO TARJETA >
                                row.ConstantItem(180).Background(naranjaITTol).Padding(8).AlignMiddle().Column(card =>
                                {
                                    card.Item().AlignCenter().Text("TOTAL REGISTRADOS")
                                        .FontSize(9)
                                        .FontColor(Colors.White)
                                        .Bold();

                                    card.Item().AlignCenter().Text($"{alumnos.Count}")
                                        .FontSize(18)
                                        .FontColor(Colors.White)
                                        .Bold();
                                });
                            });

                            // Línea decorativa inferior con el color de la escuela
                            headerCol.Item().PaddingTop(10).Height(2).Background(azulReyITTol);
                        });

                        // <  -----  >
                        pagina.Content().PaddingTop(15).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columnas =>
                            {
                                columnas.RelativeColumn(2); // No. Control
                                columnas.RelativeColumn(3); // Nombre
                                columnas.RelativeColumn(3); // Carrera
                                columnas.RelativeColumn(3); // Correo
                                columnas.RelativeColumn(2); // Fecha Nac
                                columnas.RelativeColumn(1); // Edad
                                columnas.RelativeColumn(2); // Discapacidad
                            });

                            // <  -----  >
                            tabla.Header(encabezado =>
                            {
                                // Encabezados con el Azul Rey de la escuela
                                encabezado.Cell().Background(azulReyITTol).Padding(6).Text("No. Control").FontColor(Colors.White).Bold();
                                encabezado.Cell().Background(azulReyITTol).Padding(6).Text("Nombre").FontColor(Colors.White).Bold();
                                encabezado.Cell().Background(azulReyITTol).Padding(6).Text("Carrera").FontColor(Colors.White).Bold();
                                encabezado.Cell().Background(azulReyITTol).Padding(6).Text("Correo").FontColor(Colors.White).Bold();
                                encabezado.Cell().Background(azulReyITTol).Padding(6).Text("Fecha Nac").FontColor(Colors.White).Bold();
                                encabezado.Cell().Background(azulReyITTol).Padding(6).Text("Edad").FontColor(Colors.White).Bold();
                                encabezado.Cell().Background(azulReyITTol).Padding(6).Text("Discapacidad").FontColor(Colors.White).Bold();
                            });

                            bool filaPar = false;
                            foreach (Alumno alumno in alumnos)
                            {
                                // Alternar colores blanco y gris claro
                                string colorFila = filaPar ? Colors.Grey.Lighten4 : Colors.White;
                                filaPar = !filaPar;

                                tabla.Cell().Background(colorFila).Padding(6).Text(alumno.NumeroControl).FontSize(10);
                                tabla.Cell().Background(colorFila).Padding(6).Text(alumno.Nombre).FontSize(10);
                                tabla.Cell().Background(colorFila).Padding(6).Text(alumno.Carrera).FontSize(10);
                                tabla.Cell().Background(colorFila).Padding(6).Text(alumno.Correo).FontSize(10);
                                tabla.Cell().Background(colorFila).Padding(6).Text(alumno.FechaNac).FontSize(10);
                                tabla.Cell().Background(colorFila).Padding(6).Text(alumno.Edad.ToString()).FontSize(10);

                                tabla.Cell().Background(colorFila).Padding(6)
                                    .Text(alumno.Discapacidad == "Ninguna" ? "Sin discapacidad" : alumno.Discapacidad)
                                    .FontSize(10);
                            }
                        });

                        // < pie de pagina  >
                        pagina.Footer().Column(footerCol =>
                        {
                            footerCol.Item().PaddingBottom(5).Height(1).Background(Colors.Grey.Lighten2);
                            footerCol.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Instituto Tecnológico de Toluca")
                                   .FontSize(9)
                                   .FontColor(Colors.Grey.Medium);

                                row.RelativeItem().AlignRight().Text(texto =>
                                {
                                    texto.Span("Página ").FontSize(9).FontColor(Colors.Grey.Medium);
                                    texto.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                                    texto.Span(" de ").FontSize(9).FontColor(Colors.Grey.Medium);
                                    texto.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                                });
                            });
                        });
                    });
                })
                // sela ruta donde s egenere 
                .GeneratePdf(rutaPdf);

                // <  se abra el reporte de manera automatica  >
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