using System.ComponentModel.DataAnnotations;
namespace GestionLibros.Models
{
    public class Estudiantes
    {
        [Key]
        public int EstudianteId { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un email válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public DateTime FechaNacimiento { get; set; }
    }
}
