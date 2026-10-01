using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionLibros.Models;

public class Prestamos
{
    [Key]
    public int PrestamoId { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public int EstudianteId { get; set; }

    [ForeignKey(nameof(EstudianteId))]
    public Estudiantes? Estudiante { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public int LibroId { get; set; }

    [ForeignKey(nameof(LibroId))]
    public Libros? Libro { get; set; }

    public DateTime FechaPrestamo { get; set; } = DateTime.Now;

    public bool Activo { get; set; } = true;
}
