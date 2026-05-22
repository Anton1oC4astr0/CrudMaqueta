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

        // ============ AGREGAR ============
        public static bool Agregar(Alumno alumno)
        {
            try
            {
                using (OracleConnection conexion = Conexion.Obtenercon())
                {
                    conexion.Open();
                    string sqlInsertar = @"INSERT INTO ALUMNOS
                                      (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
                                      VALUES
                                      (:numControl, :nombre, :id_carrera, :correo, TO_DATE(:fechaNac, 'DD/MM/YYYY'))";
                    using (OracleCommand cmdInsertar = new OracleCommand(sqlInsertar, conexion))
                    {
                        cmdInsertar.BindByName = true;
                        cmdInsertar.Parameters.Add(":numControl", alumno.NumeroControl);
                        cmdInsertar.Parameters.Add(":nombre", alumno.Nombre);
                        cmdInsertar.Parameters.Add(":id_carrera", alumno.id_Carrera);
                        cmdInsertar.Parameters.Add(":correo", alumno.Correo);
                        cmdInsertar.Parameters.Add(":fechaNac", alumno.FechaNac);
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
        public static List<Alumno> ObtenerDatosTodo()
        {
            List<Alumno> lista = new List<Alumno>();
            try
            {
                using (OracleConnection conexion = Conexion.Obtenercon())
                {
                    conexion.Open();

                    string sql = @"SELECT A.NUMERO_CONTROL,A.NOMBRE,A.CORREO,A.FECHA_NAC,C.NOMBRE AS CARRERA,LISTAGG(D.NOMBRE,', ')
                                    WITHIN GROUP(ORDER BY D.NOMBRE)
                                    AS DISCAPACIDADES
                                    FROM ALUMNOS A 
                                    JOIN CARRERAS_CAT C ON A.ID_CARRERA = C.ID_CARRERA 
                                    LEFT JOIN ALUMNOS_DISCAPACIDADES AD
                                    ON A.NUMERO_CONTROL = AD.NUMERO_CONTROL
                                    LEFT JOIN DISCAPACIDADES_CAT D 
                                    ON AD.ID_DISCAPACIDAD = D.ID_DISCAPACIDAD
                                    GROUP BY A.NUMERO_CONTROL,A.NOMBRE,A.CORREO,A.FECHA_NAC,C.NOMBRE";

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
                                FechaNac = reader["FECHA_NAC"].ToString() ?? string.Empty,
                                Discapacidad = reader["DISCAPACIDADES"].ToString() ?? string.Empty
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
                using (OracleConnection conexion = Conexion.Obtenercon())
                {
                    conexion.Open();

                    string sql = @"SELECT NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC
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
                                    id_Carrera = Convert.ToInt32(reader["ID_CARRERA"]),
                                    Correo = reader["CORREO"].ToString() ?? string.Empty,
                                    FechaNac = Convert.ToDateTime(reader["FECHA_NAC"]).ToString("dd/MM/yyyy"),
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
                using (OracleConnection conexion = Conexion.Obtenercon())
                {
                    conexion.Open();

                    string sql = @"UPDATE ALUMNOS
                                   SET NOMBRE = :nombre,
                                       ID_CARRERA = :id_Carrera,
                                       CORREO = :correo,
                                       FECHA_NAC = TO_DATE(:fechaNac, 'DD/MM/YYYY')
                                   WHERE NUMERO_CONTROL = :numControl";

                    using (OracleCommand cmd = new OracleCommand(sql, conexion))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":numControl", alumno.NumeroControl);
                        cmd.Parameters.Add(":nombre", alumno.Nombre);
                        cmd.Parameters.Add(":id_Carrera", alumno.id_Carrera);
                        cmd.Parameters.Add(":correo", alumno.Correo);
                        cmd.Parameters.Add(":fechaNac", alumno.FechaNac);
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
                using (OracleConnection conexion = Conexion.Obtenercon())
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
        // ============ GUARDAR DISCAPACIDAD ============
        public static void GuardarDiscapacidad(string control, int idDiscapacidad)
        {
            using OracleConnection con = Conexion.Obtenercon();
            con.Open();
            string sql =
            @"INSERT INTO
            ALUMNOS_DISCAPACIDADES
            (NUMERO_CONTROL,ID_DISCAPACIDAD)
            VALUES
            (:NumeroControl,:id_discapacidad)";
            OracleCommand cmd =
                new(sql, con);
            cmd.Parameters.Add(":NumeroControl", control);
            cmd.Parameters.Add(":id_discapacidad", idDiscapacidad);
            cmd.ExecuteNonQuery();
        }
        // ============ ELIMINAR DISCAPACIDADES DE UN ALUMNO (ANTES DE GUARDAR LAS NUEVAS) ============
        public static void EliminarDiscapacidades(string numcontrol)
        {
            using OracleConnection con = Conexion.Obtenercon();
            con.Open();
            string sql =
            @"DELETE FROM
            ALUMNOS_DISCAPACIDADES
            WHERE NUMERO_CONTROL = :NumeroControl";
            OracleCommand cmd =
                new(sql, con);
            cmd.Parameters.Add(":NumeroControl", numcontrol);
            cmd.ExecuteNonQuery();
        }
    }
}