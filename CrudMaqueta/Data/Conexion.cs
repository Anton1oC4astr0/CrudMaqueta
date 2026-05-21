using System;
using System.Collections.Generic;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace CrudMaqueta.Data
{
    public static class Conexion
    {
        // Cadena de conexion
        private static string _cadenaConexion =
         "User Id=HR_ITT;Password=HR2026;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl21c)))";


        public static OracleConnection Obtenercon()
        {
            return new OracleConnection(_cadenaConexion);
        }
    }
}