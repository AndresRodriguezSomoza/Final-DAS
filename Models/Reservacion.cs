using System.ComponentModel.DataAnnotations;

namespace DAS_Final.Models
{
    public class Reservacion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La habitación es obligatoria")]
        [Display(Name = "Habitación")]
        public int HabitacionId { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        [Display(Name = "Fecha de Entrada")]
        [DataType(DataType.Date)]
        public DateTime FechaEntrada { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        [Display(Name = "Fecha de Salida")]
        [DataType(DataType.Date)]
        public DateTime FechaSalida { get; set; }

        [Display(Name = "Total")]
        public decimal TotalHabitacion { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio")]
        [Display(Name = "Estatus")]
        public string Estatus { get; set; } = "pendiente";

        // Propiedades de navegación
        public Usuario Usuario { get; set; }
        public Habitacion Habitacion { get; set; }
    }
}
