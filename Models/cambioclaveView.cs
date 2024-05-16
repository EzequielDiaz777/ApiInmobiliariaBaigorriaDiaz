using System.ComponentModel.DataAnnotations;
namespace InmobiliariaBaigorriaDiaz.Models
{
	public class CambioClaveView
	{
		[Required(ErrorMessage = "La nueva contraseña es requerida")]
		[StringLength(50, ErrorMessage = "La clave debe tener entre 8 y 50 caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string ClaveNueva { get; set; }

		[Required(ErrorMessage = "Debe repetir la contraseña nueva")]
		[StringLength(50, ErrorMessage = "La clave debe tener entre 8 y 50 caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		[Compare("ClaveNueva")]
		public string ClaveRepeticion { get; set; }
	}
}