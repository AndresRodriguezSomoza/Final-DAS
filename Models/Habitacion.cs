using System.ComponentModel.DataAnnotations;

namespace DAS_Final.Models
{
    public class Habitacion
    {
        public int Id { get; set; }

        [Display(Name = "Imagen")]
        public string Img { get; set; }

        [Required(ErrorMessage = "El número de habitación es obligatorio")]
        [StringLength(10, ErrorMessage = "El número no puede exceder 10 caracteres")]
        [Display(Name = "Número de Habitación")]
        public string NumeroHabitacion { get; set; }

        [Required(ErrorMessage = "El tipo de habitación es obligatorio")]
        [Display(Name = "Tipo de Habitación")]
        public string TipoHabitacion { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio por noche es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio por Noche")]
        public decimal PrecioNoche { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria")]
        [Range(1, 10, ErrorMessage = "La capacidad debe ser entre 1 y 10 personas")]
        [Display(Name = "Capacidad")]
        public int Capacidad { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio")]
        [Display(Name = "Estatus")]
        public string Estatus { get; set; } = "disponible";
    }
}
