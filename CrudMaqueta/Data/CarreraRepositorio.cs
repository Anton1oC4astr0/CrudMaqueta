using CrudMaqueta.Models;
using CrudMaqueta.Utils;
using Oracle.ManagedDataAccess.Client;

namespace CrudMaqueta.Data
{
    public static class CarreraRepositorio
    {
        public static List<Carrera> ObtenerCarreras()
        {
            List<Carrera> lista = new List<Carrera>();

            try
            {
                using (OracleConnection conexion = ConexionBD.ObtenerConexion())
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
                                Id = Convert.ToInt32(reader["ID_CARRERA"]),
                                Nombre = reader["NOMBRE"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("CarreraRepositorio.ObtenerCarreras", ex);
                throw;
            }

            return lista;
        }
    }
}