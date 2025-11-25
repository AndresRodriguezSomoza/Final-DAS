using MySql.Data.MySqlClient;

namespace DAS_Final.Models
{
    public class conexion
    {
        private readonly string conn;

        public conexion()
        {
            conn = "Server=mysql-amigosdedonbosco.alwaysdata.net;" +
                               "Database=amigosdedonbosco_dasfinal;" +
                               "Uid=441605;" +
                               "Pwd=ezkbJ2FqC7;" +
                               "Charset=utf8mb3;";
        }

        public MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(conn);
        }
    }
}
