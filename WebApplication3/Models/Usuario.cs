using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Contraseña { get; set; }

        [Required]
        public string Rol { get; set; } // "Administrador" o "Cliente"
    }
}
