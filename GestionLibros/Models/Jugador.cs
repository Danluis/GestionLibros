using System.ComponentModel.DataAnnotations;

namespace GestionLibros.Models
{
    public class Jugador
    {
        [Key]
        public int JugadorId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo es obligatorio")]

        public int pin { get; set; } = 0;

    }
}
