using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionLibros.Models
{
    public class Partida
    {
        [Key]
        public int PartidaId { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int puntuacion { get; set; } = 0;

        // Clave foránea
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int JugadorId { get; set; }

        // Propiedad de navegación
        [ForeignKey(nameof(JugadorId))]
        public Jugador? Jugador { get; set; }
    }
}