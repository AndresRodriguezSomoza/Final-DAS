using MySql.Data.MySqlClient;

namespace DAS_Final.Models
{
    public class OpReservacion
    {
        private readonly conexion _conexionBD;

        public OpReservacion()
        {
            _conexionBD = new conexion();
        }

        public List<Reservacion> ObtenerTodasReservaciones()
        {
            var lista = new List<Reservacion>();

            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand(@"SELECT r.*, u.Nombre as UsuarioNombre, h.numero_habitacion, h.precio_noche
                    FROM Reservaciones r
                    INNER JOIN Usuarios u ON r.usuario_id = u.Id
                    INNER JOIN Habitaciones h ON r.habitacion_id = h.id", conexion))
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Reservacion
                            {
                                Id = lector.GetInt32("id"),
                                UsuarioId = lector.GetInt32("usuario_id"),
                                HabitacionId = lector.GetInt32("habitacion_id"),
                                FechaEntrada = lector.GetDateTime("fecha_entrada"),
                                FechaSalida = lector.GetDateTime("fecha_salida"),
                                TotalHabitacion = lector.GetDecimal("total_habitacion"),
                                Estatus = lector.GetString("estatus"),
                                Usuario = new Usuario { Nombre = lector.GetString("UsuarioNombre") },
                                Habitacion = new Habitacion
                                {
                                    NumeroHabitacion = lector.GetString("numero_habitacion"),
                                    PrecioNoche = lector.GetDecimal("precio_noche")
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Reservacion ObtenerReservacionPorId(int id)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand(@"SELECT r.*, u.Nombre as UsuarioNombre, h.numero_habitacion, h.precio_noche
                    FROM Reservaciones r
                    INNER JOIN Usuarios u ON r.usuario_id = u.Id
                    INNER JOIN Habitaciones h ON r.habitacion_id = h.id
                    WHERE r.id = @Id", conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Reservacion
                            {
                                Id = lector.GetInt32("id"),
                                UsuarioId = lector.GetInt32("usuario_id"),
                                HabitacionId = lector.GetInt32("habitacion_id"),
                                FechaEntrada = lector.GetDateTime("fecha_entrada"),
                                FechaSalida = lector.GetDateTime("fecha_salida"),
                                TotalHabitacion = lector.GetDecimal("total_habitacion"),
                                Estatus = lector.GetString("estatus"),
                                Usuario = new Usuario { Nombre = lector.GetString("UsuarioNombre") },
                                Habitacion = new Habitacion
                                {
                                    NumeroHabitacion = lector.GetString("numero_habitacion"),
                                    PrecioNoche = lector.GetDecimal("precio_noche")
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool CrearReservacion(Reservacion reservacion)
        {
            try
            {
                using (var conexion = _conexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    // SOLUCIÓN: Query mejorado con manejo de valores
                    string query = @"INSERT INTO Reservaciones 
                           (usuario_id, habitacion_id, fecha_entrada, fecha_salida, total_habitacion, estatus) 
                           VALUES (@UsuarioId, @HabitacionId, @FechaEntrada, @FechaSalida, @TotalHabitacion, @Estatus)";

                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@UsuarioId", reservacion.UsuarioId);
                        comando.Parameters.AddWithValue("@HabitacionId", reservacion.HabitacionId);
                        comando.Parameters.AddWithValue("@FechaEntrada", reservacion.FechaEntrada.Date);
                        comando.Parameters.AddWithValue("@FechaSalida", reservacion.FechaSalida.Date);
                        comando.Parameters.AddWithValue("@TotalHabitacion", reservacion.TotalHabitacion);
                        comando.Parameters.AddWithValue("@Estatus", reservacion.Estatus ?? "pendiente");

                        int resultado = comando.ExecuteNonQuery();
                        return resultado > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.Debug.WriteLine($"ERROR CrearReservacion: {ex.Message}");
                return false;
            }
        }

        public bool EditarReservacion(Reservacion reservacion)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand(
                    "UPDATE Reservaciones SET usuario_id = @UsuarioId, habitacion_id = @HabitacionId, fecha_entrada = @FechaEntrada, fecha_salida = @FechaSalida, total_habitacion = @TotalHabitacion, estatus = @Estatus WHERE id = @Id",
                    conexion))
                {
                    comando.Parameters.AddWithValue("@Id", reservacion.Id);
                    comando.Parameters.AddWithValue("@UsuarioId", reservacion.UsuarioId);
                    comando.Parameters.AddWithValue("@HabitacionId", reservacion.HabitacionId);
                    comando.Parameters.AddWithValue("@FechaEntrada", reservacion.FechaEntrada);
                    comando.Parameters.AddWithValue("@FechaSalida", reservacion.FechaSalida);
                    comando.Parameters.AddWithValue("@TotalHabitacion", reservacion.TotalHabitacion);
                    comando.Parameters.AddWithValue("@Estatus", reservacion.Estatus);

                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EliminarReservacion(int id)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new MySqlCommand("DELETE FROM Reservaciones WHERE id = @Id", conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool HabitacionDisponible(int habitacionId, DateTime fechaEntrada, DateTime fechaSalida, int? excluirId = null)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();
                var consulta = @"
                    SELECT COUNT(*) FROM Reservaciones 
                    WHERE habitacion_id = @HabitacionId 
                    AND estatus IN ('confirmada', 'activa', 'pendiente')
                    AND ((fecha_entrada BETWEEN @FechaEntrada AND @FechaSalida) 
                         OR (fecha_salida BETWEEN @FechaEntrada AND @FechaSalida)
                         OR (fecha_entrada <= @FechaEntrada AND fecha_salida >= @FechaSalida))";

                if (excluirId.HasValue)
                    consulta += " AND id != @ExcluirId";

                using (var comando = new MySqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@HabitacionId", habitacionId);
                    comando.Parameters.AddWithValue("@FechaEntrada", fechaEntrada);
                    comando.Parameters.AddWithValue("@FechaSalida", fechaSalida);

                    if (excluirId.HasValue)
                        comando.Parameters.AddWithValue("@ExcluirId", excluirId.Value);

                    var cantidad = Convert.ToInt32(comando.ExecuteScalar());
                    return cantidad == 0;
                }
            }
        }

        public bool HayConflictoDeFechas(int habitacionId, DateTime entrada, DateTime salida, int? excluirId = null)
        {
            using (var conexion = _conexionBD.ObtenerConexion())
            {
                conexion.Open();

                // Construimos la consulta base
                string query = "SELECT COUNT(*) FROM Reservaciones WHERE habitacion_id = @HabitacionId AND estatus != 'cancelada' AND fecha_salida > @FechaEntrada AND fecha_entrada < @FechaSalida";

                // Si estamos editando (excluirId tiene valor), agregamos la condición para ignorar la reserva actual
                if (excluirId.HasValue)
                {
                    query += " AND id != @ExcluirId";
                }

                using (var comando = new MySqlCommand(query, conexion))
                {
                    // Agregamos los parámetros obligatorios
                    comando.Parameters.AddWithValue("@HabitacionId", habitacionId);
                    comando.Parameters.AddWithValue("@FechaEntrada", entrada);
                    comando.Parameters.AddWithValue("@FechaSalida", salida);

                    // Agregamos el parámetro opcional solo si es necesario
                    if (excluirId.HasValue)
                    {
                        comando.Parameters.AddWithValue("@ExcluirId", excluirId.Value);
                    }

                    // Ejecutamos y verificamos si hay coincidencias
                    var count = Convert.ToInt32(comando.ExecuteScalar());
                    return count > 0;
                }
            }
        }


    }
}
