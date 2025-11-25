using MySql.Data.MySqlClient;
using DAS_Final.Models;

namespace DAS_Final.Models
{
    public class OpUsuario
    {
        private readonly conexion _conexionBD;

        public OpUsuario()
        {
            _conexionBD = new conexion();
        }

        public List<Usuario> ObtenerTodosUsuarios()
        {
            var listaUsuarios = new List<Usuario>();

            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand("SELECT * FROM Usuarios", conexion))
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            listaUsuarios.Add(new Usuario
                            {
                                Id = lector.GetInt32("Id"),
                                Nombre = lector.GetString("Nombre"),
                                Password = lector.GetString("Contrasena"),
                                Email = lector.GetString("Email"),
                                TipoUsuario = lector.GetString("TipoUsuario")
                            });
                        }
                    }
                }
            }
            return listaUsuarios;
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand("SELECT * FROM Usuarios WHERE Id = @Id", conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
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

        public bool CrearUsuario(Usuario usuario)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand(
                    "INSERT INTO Usuarios (Nombre, Contrasena, Email, TipoUsuario) VALUES (@Nombre, @Contrasena, @Email, @TipoUsuario)", // CAMBIADO
                    conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    comando.Parameters.AddWithValue("@Contrasena", usuario.Password); // CAMBIADO
                    comando.Parameters.AddWithValue("@Email", usuario.Email);
                    comando.Parameters.AddWithValue("@TipoUsuario", usuario.TipoUsuario);

                    return comando.ExecuteNonQuery() > 0; // CORREGIDO: ExecuteNonQuery
                }
            }
        }

        public bool EditarUsuario(Usuario usuario)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand(
                    "UPDATE Usuarios SET Nombre = @Nombre, Contrasena = @Contrasena, Email = @Email, TipoUsuario = @TipoUsuario WHERE Id = @Id", // CAMBIADO
                    conexion))
                {
                    comando.Parameters.AddWithValue("@Id", usuario.Id);
                    comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    comando.Parameters.AddWithValue("@Contrasena", usuario.Password); // CAMBIADO
                    comando.Parameters.AddWithValue("@Email", usuario.Email);
                    comando.Parameters.AddWithValue("@TipoUsuario", usuario.TipoUsuario);

                    return comando.ExecuteNonQuery() > 0; // CORREGIDO: ExecuteNonQuery
                }
            }
        }

        public bool EliminarUsuario(int id)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand("DELETE FROM Usuarios WHERE Id = @Id", conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EmailExiste(string email, int? excluirId = null)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                var consulta = "SELECT COUNT(*) FROM Usuarios WHERE Email = @Email";
                if (excluirId.HasValue)
                    consulta += " AND Id != @ExcluirId";

                using (var comando = new MySqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Email", email);
                    if (excluirId.HasValue)
                        comando.Parameters.AddWithValue("@ExcluirId", excluirId.Value);

                    var cantidad = Convert.ToInt32(comando.ExecuteScalar());
                    return cantidad > 0;
                }
            }
        }
    }
}
