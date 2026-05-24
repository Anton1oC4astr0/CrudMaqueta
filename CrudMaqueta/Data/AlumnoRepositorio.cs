using CrudMaqueta.Models;
using CrudMaqueta.Utils;
using Oracle.ManagedDataAccess.Client;

namespace CrudMaqueta.Data
{
    public static class AlumnoRepositorio
    {
        // -- AGREGAR --
        public static bool Agregar(Alumno alumno)
        {
            try
            {
                using (OracleConnection conexion = ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string sqlVerificar = "SELECT COUNT(*) FROM ALUMNOS WHERE NUMERO_CONTROL = :numControl";

                    using (OracleCommand cmdVerificar = new OracleCommand(sqlVerificar, conexion))
                    {
                        cmdVerificar.Parameters.Add(":numControl", alumno.NumeroControl);
                        int count = Convert.ToInt32(cmdVerificar.ExecuteScalar());
                        if (count > 0)
                            return false;
                    }

                    string sqlInsertar = @"INSERT INTO ALUMNOS 
                                          (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
                                          VALUES 
                                        (:numControl, :nombre, :idCarrera, :correo, 
                       TO_DATE(:fechaNac, 'DD/MM/YYYY'), :edad, :idDiscapacidad)";

                    using (OracleCommand cmdInsertar = new OracleCommand(sqlInsertar, conexion))
                    {
                        cmdInsertar.BindByName = true;
                        cmdInsertar.Parameters.Add(":numControl", alumno.NumeroControl);
                        cmdInsertar.Parameters.Add(":nombre", alumno.Nombre);
                        cmdInsertar.Parameters.Add(":idCarrera", alumno.IdCarrera);
                        cmdInsertar.Parameters.Add(":correo", alumno.Correo);
                        cmdInsertar.Parameters.Add(":fechaNac", alumno.FechaNac);
                        cmdInsertar.Parameters.Add(":edad", alumno.Edad);
                        cmdInsertar.Parameters.Add(":idDiscapacidad", alumno.IdDiscapacidad);
                        cmdInsertar.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorio.Agregar", ex);
                throw;
            }
        }


        // -- CONSULTA --
        public static List<Alumno> ObtenerDatos()
        {
            List<Alumno> lista = new List<Alumno>();

            try
            {
                using (OracleConnection conexion = ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string sqlVerDatos = @" SELECT a.NUMERO_CONTROL,
                                                    a.NOMBRE,
                                                    a.ID_CARRERA,
                                                    c.NOMBRE AS CARRERA,
                                                    a.CORREO,
                                                    a.FECHA_NAC,
                                                    a.EDAD,
                                                    d.NOMBRE AS DISCAPACIDAD
                                            FROM ALUMNOS a
                                            LEFT JOIN CARRERAS_CAT c       
                                                ON c.ID_CARRERA = a.ID_CARRERA
                                            LEFT JOIN DISCAPACIDADES_CAT d 
                                                ON d.ID_DISCAPACIDAD = a.ID_DISCAPACIDAD";

                    using (OracleCommand cmd = new OracleCommand(sqlVerDatos, conexion))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Forma de inicializar el objeto
                            // {}; toma cualquier propiedad de Alumno y dales el siguiente valor
                            Alumno alumno = new Alumno
                            {
                                NumeroControl = reader["NUMERO_CONTROL"].ToString() ?? string.Empty,
                                Nombre = reader["NOMBRE"].ToString() ?? string.Empty,
                                IdCarrera = Convert.ToInt32(reader["ID_CARRERA"]),
                                Carrera = reader["CARRERA"].ToString() ?? string.Empty,
                                Correo = reader["CORREO"].ToString() ?? string.Empty,
                                FechaNac = Convert.ToDateTime(reader["FECHA_NAC"]).ToString("dd/MM/yyyy"),
                                Edad = Convert.ToInt32(reader["EDAD"]),
                                Discapacidad = reader["DISCAPACIDAD"]?.ToString() ?? "Ninguna"
                            };
                            lista.Add(alumno);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorio.ObtenerDatos", ex);
                throw;
            }

            return lista;
        }


        // -- BUSCAR --
        public static Alumno? BuscarPorControl(string numeroControl)
        {
            Alumno? alumnoEncontrado = null;

            try
            {
                using (OracleConnection conexion = ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string sqlBuscar = @"SELECT 
                            a.NUMERO_CONTROL, 
                            a.NOMBRE, 
                            a.ID_CARRERA, 
                            c.NOMBRE AS NOMBRE_CARRERA, 
                            a.CORREO, 
                            a.FECHA_NAC, 
                            a.EDAD,
                            a.ID_DISCAPACIDAD,
                            d.NOMBRE AS DISCAPACIDAD
                        FROM ALUMNOS a
                        INNER JOIN CARRERAS_CAT c ON a.ID_CARRERA = c.ID_CARRERA
                        LEFT JOIN DISCAPACIDADES_CAT d ON a.ID_DISCAPACIDAD = d.ID_DISCAPACIDAD WHERE a.NUMERO_CONTROL = :numControl";

                    using (OracleCommand cmd = new OracleCommand(sqlBuscar, conexion))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":numControl", numeroControl.Trim());

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                alumnoEncontrado = new Alumno
                                {
                                    NumeroControl = reader["NUMERO_CONTROL"].ToString() ?? string.Empty,
                                    Nombre = reader["NOMBRE"].ToString() ?? string.Empty,
                                    IdCarrera = Convert.ToInt32(reader["ID_CARRERA"]),
                                    Carrera = reader["NOMBRE_CARRERA"].ToString() ?? string.Empty,
                                    Correo = reader["CORREO"].ToString() ?? string.Empty,
                                    FechaNac = Convert.ToDateTime(reader["FECHA_NAC"]).ToString("dd/MM/yyyy"),
                                    Edad = Convert.ToInt32(reader["EDAD"]),
                                    Discapacidad = reader["DISCAPACIDAD"]?.ToString() ?? "Ninguna"
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorio.BuscarPorControl", ex);
                throw;
            }

            return alumnoEncontrado;
        }


        // -- ACTUALIZAR --
        public static bool Actualizar(Alumno alumno)
        {
            try
            {
                using (OracleConnection conexion = ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string sqlActualizar = @"UPDATE ALUMNOS SET 
                                             NOMBRE          = :nombre, 
                                             ID_CARRERA      = :idCarrera, 
                                             CORREO          = :correo, 
                                             FECHA_NAC       = TO_DATE(:fechaNac, 'DD/MM/YYYY'), 
                                             EDAD            = :edad,
                                             ID_DISCAPACIDAD = :idDiscapacidad
                                             WHERE NUMERO_CONTROL = :numControl";

                    using (OracleCommand cmdActualizar = new OracleCommand(sqlActualizar, conexion))
                    {
                        cmdActualizar.BindByName = true;
                        cmdActualizar.Parameters.Add(":nombre", alumno.Nombre);
                        cmdActualizar.Parameters.Add(":idCarrera", alumno.IdCarrera);
                        cmdActualizar.Parameters.Add(":correo", alumno.Correo);
                        cmdActualizar.Parameters.Add(":fechaNac", alumno.FechaNac);
                        cmdActualizar.Parameters.Add(":edad", alumno.Edad);
                        cmdActualizar.Parameters.Add(":idDiscapacidad", alumno.IdDiscapacidad);
                        cmdActualizar.Parameters.Add(":numControl", alumno.NumeroControl);

                        int filasAfectadas = cmdActualizar.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorio.Actualizar", ex);
                throw;
            }
        }


        // -- ELIMINAR --
        public static bool Eliminar(string numeroControl)
        {
            try
            {
                using (OracleConnection conexion = ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string sqlEliminar = "DELETE FROM ALUMNOS WHERE NUMERO_CONTROL = :numControl";

                    using (OracleCommand cmdEliminar = new OracleCommand(sqlEliminar, conexion))
                    {
                        cmdEliminar.BindByName = true;
                        cmdEliminar.Parameters.Add(":numControl", numeroControl.Trim());

                        int filasAfectadas = cmdEliminar.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorio.Eliminar", ex);
                throw;
            }
        }
    }
}