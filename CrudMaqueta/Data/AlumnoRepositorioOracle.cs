using System;
using System.Collections.Generic;
using CrudMaqueta.Models;
using CrudMaqueta.Utils;
using Oracle.ManagedDataAccess.Client;

namespace CrudMaqueta.Data
{
    /// <summary>
    /// Repositorio con acceso real a Oracle.
    /// Todas las excepciones se reportan al archivo errores.log antes de relanzarse.
    /// </summary>
    public static class AlumnoRepositorioOracle
    {
        private static string _cadenaConexion =
            "User Id=HR_ITT;Password=HR2026;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl19c)))";

        // ============ AGREGAR ============
        public static bool Agregar(Alumno alumno)
        {
            try
            {
                using (OracleConnection conexion = new OracleConnection(_cadenaConexion))
                {
                    conexion.Open();

                    string sqlVerificar = "SELECT COUNT(*) FROM ALUMNOS WHERE NUMERO_CONTROL = :numControl";
                    using (OracleCommand cmdVerificar = new OracleCommand(sqlVerificar, conexion))
                    {
                        cmdVerificar.BindByName = true;
                        cmdVerificar.Parameters.Add(":numControl", alumno.NumeroControl);
                        int count = Convert.ToInt32(cmdVerificar.ExecuteScalar());
                        if (count > 0) return false;
                    }

                    string sqlInsertar = @"INSERT INTO ALUMNOS
                                      (NUMERO_CONTROL, NOMBRE, CARRERA, CORREO, FECHA_NAC, EDAD, DISCAPACIDAD)
                                      VALUES
                                      (:numControl, :nombre, :carrera, :correo, TO_DATE(:fechaNac, 'DD/MM/YYYY'), :edad, :discapacidad)";

                    using (OracleCommand cmdInsertar = new OracleCommand(sqlInsertar, conexion))
                    {
                        cmdInsertar.BindByName = true;
                        cmdInsertar.Parameters.Add(":numControl", alumno.NumeroControl);
                        cmdInsertar.Parameters.Add(":nombre", alumno.Nombre);
                        cmdInsertar.Parameters.Add(":carrera", alumno.Carrera);
                        cmdInsertar.Parameters.Add(":correo", alumno.Correo);
                        cmdInsertar.Parameters.Add(":fechaNac", alumno.FechaNac);
                        cmdInsertar.Parameters.Add(":edad", alumno.Edad);
                        cmdInsertar.Parameters.Add(":discapacidad",
                            string.IsNullOrWhiteSpace(alumno.Discapacidad) ? "Ninguna" : alumno.Discapacidad);
                        cmdInsertar.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorioOracle.Agregar", ex);
                throw;
            }
        }

        // ============ OBTENER TODOS ============
        public static List<Alumno> ObtenerDatos()
        {
            List<Alumno> lista = new List<Alumno>();
            try
            {
                using (OracleConnection conexion = new OracleConnection(_cadenaConexion))
                {
                    conexion.Open();

                    string sql = @"SELECT NUMERO_CONTROL, NOMBRE, CARRERA, CORREO, FECHA_NAC, EDAD, DISCAPACIDAD
                                   FROM ALUMNOS
                                   ORDER BY NUMERO_CONTROL";

                    using (OracleCommand cmd = new OracleCommand(sql, conexion))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Alumno alumno = new Alumno
                            {
                                NumeroControl = reader["NUMERO_CONTROL"].ToString() ?? string.Empty,
                                Nombre = reader["NOMBRE"].ToString() ?? string.Empty,
                                Carrera = reader["CARRERA"].ToString() ?? string.Empty,
                                Correo = reader["CORREO"].ToString() ?? string.Empty,
                                FechaNac = Convert.ToDateTime(reader["FECHA_NAC"]).ToString("dd/MM/yyyy"),
                                Edad = Convert.ToInt32(reader["EDAD"]),
                                Discapacidad = reader["DISCAPACIDAD"] == DBNull.Value
                                    ? "Ninguna"
                                    : reader["DISCAPACIDAD"].ToString() ?? "Ninguna"
                            };
                            lista.Add(alumno);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorioOracle.ObtenerDatos", ex);
                throw;
            }
            return lista;
        }

        // ============ BUSCAR POR CONTROL ============
        public static Alumno? BuscarPorControl(string numeroControl)
        {
            try
            {
                using (OracleConnection conexion = new OracleConnection(_cadenaConexion))
                {
                    conexion.Open();

                    string sql = @"SELECT NUMERO_CONTROL, NOMBRE, CARRERA, CORREO, FECHA_NAC, EDAD, DISCAPACIDAD
                                   FROM ALUMNOS
                                   WHERE NUMERO_CONTROL = :numControl";

                    using (OracleCommand cmd = new OracleCommand(sql, conexion))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":numControl", numeroControl.Trim());

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Alumno
                                {
                                    NumeroControl = reader["NUMERO_CONTROL"].ToString() ?? string.Empty,
                                    Nombre = reader["NOMBRE"].ToString() ?? string.Empty,
                                    Carrera = reader["CARRERA"].ToString() ?? string.Empty,
                                    Correo = reader["CORREO"].ToString() ?? string.Empty,
                                    FechaNac = Convert.ToDateTime(reader["FECHA_NAC"]).ToString("dd/MM/yyyy"),
                                    Edad = Convert.ToInt32(reader["EDAD"]),
                                    Discapacidad = reader["DISCAPACIDAD"] == DBNull.Value
                                        ? "Ninguna"
                                        : reader["DISCAPACIDAD"].ToString() ?? "Ninguna"
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorioOracle.BuscarPorControl", ex);
                throw;
            }
            return null;
        }

        // ============ ACTUALIZAR ============
        public static bool Actualizar(Alumno alumno)
        {
            try
            {
                using (OracleConnection conexion = new OracleConnection(_cadenaConexion))
                {
                    conexion.Open();

                    string sql = @"UPDATE ALUMNOS
                                   SET NOMBRE = :nombre,
                                       CARRERA = :carrera,
                                       CORREO = :correo,
                                       FECHA_NAC = TO_DATE(:fechaNac, 'DD/MM/YYYY'),
                                       EDAD = :edad,
                                       DISCAPACIDAD = :discapacidad
                                   WHERE NUMERO_CONTROL = :numControl";

                    using (OracleCommand cmd = new OracleCommand(sql, conexion))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":nombre", alumno.Nombre);
                        cmd.Parameters.Add(":carrera", alumno.Carrera);
                        cmd.Parameters.Add(":correo", alumno.Correo);
                        cmd.Parameters.Add(":fechaNac", alumno.FechaNac);
                        cmd.Parameters.Add(":edad", alumno.Edad);
                        cmd.Parameters.Add(":discapacidad",
                            string.IsNullOrWhiteSpace(alumno.Discapacidad) ? "Ninguna" : alumno.Discapacidad);
                        cmd.Parameters.Add(":numControl", alumno.NumeroControl.Trim());

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorioOracle.Actualizar", ex);
                throw;
            }
        }

        // ============ ELIMINAR ============
        public static bool Eliminar(string numeroControl)
        {
            try
            {
                using (OracleConnection conexion = new OracleConnection(_cadenaConexion))
                {
                    conexion.Open();

                    string sql = "DELETE FROM ALUMNOS WHERE NUMERO_CONTROL = :numControl";

                    using (OracleCommand cmd = new OracleCommand(sql, conexion))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":numControl", numeroControl.Trim());

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("AlumnoRepositorioOracle.Eliminar", ex);
                throw;
            }
        }
    }
}
