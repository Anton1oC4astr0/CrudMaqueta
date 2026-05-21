using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrudMaqueta.Models;
using CrudMaqueta.Utils;
using Oracle.ManagedDataAccess.Client;

namespace CrudMaqueta.Data
{
    public static class DiscapacidadRepositorioOracle
    {

        public static bool Agregar(Discapacidad discapacidad)
        {
            try
            {
                using (OracleConnection conexion = Conexion.Obtenercon())
                {
                    try
                    {
                        conexion.Open();
                        string sqlInsertar = @"INSERT INTO DISCAPACIDADES_CAT (NOMBRE) VALUES (:nombre)";
                        using (OracleCommand cmdInsertar = new OracleCommand(sqlInsertar, conexion))
                        {
                            cmdInsertar.BindByName = true;
                            cmdInsertar.Parameters.Add(":nombre", discapacidad.nombre);
                            cmdInsertar.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex) 
                    {
                        Logger.RegistrarError("DiscapacidadRepositorioOracle.Agregar - Insertar", ex);
                        throw;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("DiscapacidadRepositorioOracle.Agregar", ex);
                throw;
            }
        }

        public static List<Discapacidad> ObtenerDiscapacidades()
        {
            try
            {
                List<Discapacidad> lista = new List<Discapacidad>();

                using (OracleConnection conexion = Conexion.Obtenercon())
                {
                    conexion.Open();
                    string sql = "SELECT ID_DISCAPACIDAD, NOMBRE FROM DISCAPACIDADES_CAT ORDER BY ID_DISCAPACIDAD";
                    using (OracleCommand cmd = new OracleCommand(sql, conexion))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Discapacidad
                            {
                                id_Discapacidad = Convert.ToInt32(reader["ID_DISCAPACIDAD"]),
                                nombre = reader["NOMBRE"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("DiscapacidadRepositorioOracle.ObtenerDiscapacidades", ex);
                throw;
            }
        }
    }
}
