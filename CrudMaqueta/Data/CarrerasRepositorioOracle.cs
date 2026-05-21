using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using CrudMaqueta.Utils;
using CrudMaqueta.Models;
using Oracle.ManagedDataAccess.Client;

namespace CrudMaqueta.Data
{
    public static class CarrerasRepositorioOracle
    {

        public static void AgregarCarrera(Carrera carrera)
        {
            try
            {
                using (OracleConnection conexion = Conexion.Obtenercon())
                {
                    conexion.Open();
                    string sql = "INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (CARRERAS_SEQ.NEXTVAL, :nombre)";
                    using (OracleCommand cmd = new OracleCommand(sql, conexion))
                    {
                        cmd.Parameters.Add(new OracleParameter("nombre", carrera.nombre));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("CarrerasRepositorioOracle.AgregarCarrera", ex);
                throw;
            }
        }

        public static List<Carrera> ObtenerCarreras()
        {
            try
            {
                List<Carrera> lista = new List<Carrera>();

                using (OracleConnection conexion = Conexion.Obtenercon())
                {
                    conexion.Open();
                    string sql = "SELECT ID_CARRERA, NOMBRE FROM CARRERAS_CAT ORDER BY NOMBRE";
                    using (OracleCommand cmd = new OracleCommand(sql, conexion))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Carrera
                            {
                                id_Carrera = Convert.ToInt32(reader["ID_CARRERA"]),
                                nombre = reader["NOMBRE"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("CarrerasRepositorioOracle.ObtenerCarreras", ex);
                throw;
            }
        }
    }
}
