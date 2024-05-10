using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaBaigorriaDiaz.Models
{
	public class Inmueble
	{
		[Display(Name = "ID del inmueble")]
		public int Id { get; set; }

		[Required]
		public int PropietarioId { get; set; }

		[Required]
		public string Direccion { get; set; } = "";

		[Required]
		public int Ambientes { get; set; }

		public string Tipo { get; set; } = "";

		public string Uso { get; set; } = "";

		public decimal Precio {get; set;}

		public bool Estado {get; set;}
		
		[ForeignKey(nameof(PropietarioId))]
		public Propietario? Duenio { get; set; }
	}
}