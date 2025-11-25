using MySql.Data.MySqlClient;
using DAS_Final.Models;

namespace DAS_Final.Models
{
    public class OpHabitacion
    {
        private readonly conexion _conexionBD;

        public OpHabitacion()
        {
            _conexionBD = new conexion();
        }

        public List<Habitacion> ObtenerTodasHabitaciones()
        {
            var listaHabitaciones = new List<Habitacion>();

            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand("SELECT * FROM Habitaciones", conexion))
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            listaHabitaciones.Add(new Habitacion
                            {
                                Id = lector.GetInt32("id"),
                                Img = lector.IsDBNull(lector.GetOrdinal("img")) ? null : lector.GetString("img"),
                                NumeroHabitacion = lector.GetString("numero_habitacion"),
                                TipoHabitacion = lector.GetString("tipo_habitacion"),
                                Descripcion = lector.IsDBNull(lector.GetOrdinal("descripcion")) ? null : lector.GetString("descripcion"),
                                PrecioNoche = lector.GetDecimal("precio_noche"),
                                Capacidad = lector.GetInt32("capacidad"),
                                Estatus = lector.GetString("estatus")
                            });
                        }
                    }
                }
            }
            return listaHabitaciones;
        }

        public Habitacion ObtenerHabitacionPorId(int id)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand("SELECT * FROM Habitaciones WHERE id = @Id", conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Habitacion
                            {
                                Id = lector.GetInt32("id"),
                                Img = lector.IsDBNull(lector.GetOrdinal("img")) ? null : lector.GetString("img"),
                                NumeroHabitacion = lector.GetString("numero_habitacion"),
                                TipoHabitacion = lector.GetString("tipo_habitacion"),
                                Descripcion = lector.IsDBNull(lector.GetOrdinal("descripcion")) ? null : lector.GetString("descripcion"),
                                PrecioNoche = lector.GetDecimal("precio_noche"),
                                Capacidad = lector.GetInt32("capacidad"),
                                Estatus = lector.GetString("estatus")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool CrearHabitacion(Habitacion habitacion)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand(
                    "INSERT INTO Habitaciones (img, numero_habitacion, tipo_habitacion, descripcion, precio_noche, capacidad, estatus) VALUES (@Img, @NumeroHabitacion, @TipoHabitacion, @Descripcion, @PrecioNoche, @Capacidad, @Estatus)",
                    conexion))
                {
                    comando.Parameters.AddWithValue("@Img", string.IsNullOrEmpty(habitacion.Img) ? (object)DBNull.Value : habitacion.Img);
                    comando.Parameters.AddWithValue("@NumeroHabitacion", habitacion.NumeroHabitacion);
                    comando.Parameters.AddWithValue("@TipoHabitacion", habitacion.TipoHabitacion);
                    comando.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(habitacion.Descripcion) ? (object)DBNull.Value : habitacion.Descripcion);
                    comando.Parameters.AddWithValue("@PrecioNoche", habitacion.PrecioNoche);
                    comando.Parameters.AddWithValue("@Capacidad", habitacion.Capacidad);
                    comando.Parameters.AddWithValue("@Estatus", habitacion.Estatus);

                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EditarHabitacion(Habitacion habitacion)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand(
                    "UPDATE Habitaciones SET img = @Img, numero_habitacion = @NumeroHabitacion, tipo_habitacion = @TipoHabitacion, descripcion = @Descripcion, precio_noche = @PrecioNoche, capacidad = @Capacidad, estatus = @Estatus WHERE id = @Id",
                    conexion))
                {
                    comando.Parameters.AddWithValue("@Id", habitacion.Id);
                    comando.Parameters.AddWithValue("@Img", string.IsNullOrEmpty(habitacion.Img) ? (object)DBNull.Value : habitacion.Img);
                    comando.Parameters.AddWithValue("@NumeroHabitacion", habitacion.NumeroHabitacion);
                    comando.Parameters.AddWithValue("@TipoHabitacion", habitacion.TipoHabitacion);
                    comando.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(habitacion.Descripcion) ? (object)DBNull.Value : habitacion.Descripcion);
                    comando.Parameters.AddWithValue("@PrecioNoche", habitacion.PrecioNoche);
                    comando.Parameters.AddWithValue("@Capacidad", habitacion.Capacidad);
                    comando.Parameters.AddWithValue("@Estatus", habitacion.Estatus);

                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EliminarHabitacion(int id)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand("DELETE FROM Habitaciones WHERE id = @Id", conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool NumeroHabitacionExiste(string numeroHabitacion, int? excluirId = null)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                var consulta = "SELECT COUNT(*) FROM Habitaciones WHERE numero_habitacion = @NumeroHabitacion";
                if (excluirId.HasValue)
                    consulta += " AND id != @ExcluirId";

                using (var comando = new MySqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@NumeroHabitacion", numeroHabitacion);
                    if (excluirId.HasValue)
                        comando.Parameters.AddWithValue("@ExcluirId", excluirId.Value);

                    var cantidad = Convert.ToInt32(comando.ExecuteScalar());
                    return cantidad > 0;
                }
            }
        }
    }
}
