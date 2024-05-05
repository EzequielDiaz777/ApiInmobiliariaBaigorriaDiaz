using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaBaigorriaDiaz.Models
{
    [Table("Propietarios")]
    public class Propietario
    {
        [Display(Name = "ID del propietario")]
        public int IdPropietario { get; set; }

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
		public string Clave { get; set; } = "";

        [Required]
        public bool Estado { get; set; }

        public override string ToString()
        {
            return $"{Apellido} {Nombre}";
        }
    }
}