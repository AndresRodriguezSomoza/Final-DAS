using DAS_Final.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

public class PaginaClientesController : Controller
{
    private string connectionString = "Server=mysql-amigosdedonbosco.alwaysdata.net;" +
                                     "Database=amigosdedonbosco_dasfinal;" +
                                     "Uid=441605;" +
                                     "Pwd=ezkbJ2FqC7;" +
                                     "Charset=utf8mb3;";

    public IActionResult Index()
    {
        var habitaciones = ObtenerHabitaciones();
        return View(habitaciones);
    }

    public IActionResult Rooms()
    {
        var habitaciones = ObtenerHabitaciones();
        return View(habitaciones);
    }

    private List<Habitacion> ObtenerHabitaciones()
    {
        var habitaciones = new List<Habitacion>();

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT id, img, numero_habitacion, tipo_habitacion, descripcion, estatus, precio_noche, capacidad FROM Habitaciones WHERE estatus = 'Disponible'";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        habitaciones.Add(new Habitacion
                        {
                            Id = reader.GetInt32("id"),
                            Img = reader.GetString("img"),
                            NumeroHabitacion = reader.GetString("numero_habitacion"),
                            TipoHabitacion = reader.GetString("tipo_habitacion"),
                            Descripcion = reader.GetString("descripcion"),
                            Estatus = reader.GetString("estatus"),
                            PrecioNoche = reader.GetDecimal("precio_noche"),
                            Capacidad = reader.GetInt32("capacidad")
                        });
                    }
                }
            }
        }

        return habitaciones;
    }
}