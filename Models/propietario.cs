using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaBaigorriaDiaz.Models
{
    public class Propietario
    {
        [Display(Name = "ID del propietario")]
        public int Id { get; set; }

        public string Nombre { get; set; } = "";

        public string Apellido { get; set; } = "";

		public string DNI { get; set; } = "";

		[Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [EmailAddress]
        public string Email { get; set; } = "";

		[Required(ErrorMessage = "La clave es obligatoria")]
		public string Password { get; set; } = "";

        public override string ToString()
        {
            return $"{Apellido} {Nombre}";
        }
    }
}