using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaBaigorriaDiaz.Models
{
    public class Propietario
    {
        [Display(Name = "ID del propietario")]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

		[Required]
		public string DNI { get; set; } = "";

		[Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

		[Required(ErrorMessage = "La clave es obligatoria")]
		public string Password { get; set; } = "";

        public override string ToString()
        {
            return $"{Apellido} {Nombre}";
        }
    }
}