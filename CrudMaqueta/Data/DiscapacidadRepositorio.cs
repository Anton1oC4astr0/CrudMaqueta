using CrudMaqueta.Models;
using CrudMaqueta.Utils;
using Oracle.ManagedDataAccess.Client;

namespace CrudMaqueta.Data
{
    public static class DiscapacidadRepositorio
    {
        
        public static List<Discapacidad> ObtenerDiscapacidades()
        {
            List<Discapacidad> listaDiscapacidades = new List<Discapacidad>();

            try
            {
                using (OracleConnection conexion = ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string sqlVerDatos = "SELECT ID_DISCAPACIDAD, NOMBRE, ESTATUS FROM DISCAPACIDADES_CAT";

                    using (OracleCommand cmd = new OracleCommand(sqlVerDatos, conexion))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Discapacidad discapacidad = new Discapacidad
                            {
                                IdDiscapacidad = Convert.ToInt32(reader["ID_DISCAPACIDAD"]),
                                Nombre = reader["NOMBRE"].ToString() ?? string.Empty,
                                Estatus = reader["ESTATUS"].ToString() ?? string.Empty
                            };
                            listaDiscapacidades.Add(discapacidad);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RegistrarError("DiscapacidadRepositorio.ObtenerDiscapacidades", ex);
                throw;
            }

            return listaDiscapacidades;
        }    
    }
}