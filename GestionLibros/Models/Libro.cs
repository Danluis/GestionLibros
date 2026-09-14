using System;
using System.ComponentModel.DataAnnotations;

namespace GestionLibros.Models
{
    public class Libro
    {
        [Key]
        public int LibroId { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Range(1, 9999, ErrorMessage = "Ingrese un año válido")]
        public int AnoPublicacion { get; set; }
    }
}
