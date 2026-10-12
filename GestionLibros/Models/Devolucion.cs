using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionLibros.Models;

public class Devolucion
{
    [Key]
    public int DevolucionId { get; set; }

    [Required]
    public int PrestamoId { get; set; }

    [ForeignKey(nameof(PrestamoId))]
    public Prestamos? Prestamo { get; set; }

    public DateTime FechaDevolucion { get; set; } = DateTime.Now;
}
