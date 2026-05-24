using Oracle.ManagedDataAccess.Client;

namespace CrudMaqueta.Data
{
    public static class ConexionBD
    {
        private static string _cadenaConexion =

        // Fabiola:
        "User Id=HR_ITT;Password=HR2026;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orclFabiola12c)))";

        // Laboratorio: 
        // "User Id=HR_ITT;Password=HR2026;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1522))(CONNECT_DATA=(SERVICE_NAME=ORCL19C)))";

        public static OracleConnection ObtenerConexion()
        {
            return new OracleConnection(_cadenaConexion);
        }
    }
}