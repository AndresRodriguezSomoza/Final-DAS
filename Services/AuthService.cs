using DAS_Final.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Claims;
namespace DAS_Final.Services
{
    public class AuthService : IAuthService
    {
        private readonly conexion _conexionBD;

        public AuthService()
        {
            _conexionBD = new conexion();
        }

        public async Task<Usuario?> Authenticate(string email, string password)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                await conexion.OpenAsync();
                using (var comando = new MySqlCommand("SELECT * FROM Usuarios WHERE Email = @Email AND Contrasena = @Password", conexion))
                {
                    comando.Parameters.AddWithValue("@Email", email);
                    comando.Parameters.AddWithValue("@Password", password);

                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        if (await lector.ReadAsync())
                        {
                            return new Usuario
                            {
                                Id = lector.GetInt32("Id"),
                                Nombre = lector.GetString("Nombre"),
                                Password = lector.GetString("Contrasena"),
                                Email = lector.GetString("Email"),
                                TipoUsuario = lector.GetString("TipoUsuario")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<bool> UserExists(string email)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                await conexion.OpenAsync();
                using (var comando = new MySqlCommand("SELECT COUNT(*) FROM Usuarios WHERE Email = @Email", conexion))
                {
                    comando.Parameters.AddWithValue("@Email", email);
                    var count = Convert.ToInt32(await comando.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }
    }
}
