using InmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaBaigorriaDiaz.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class ContratosController : ControllerBase
	{
		private readonly DataContext contexto;

		public ContratosController(DataContext contexto)
		{
			this.contexto = contexto;
		}

		[HttpGet]
		public async Task<ActionResult<Contrato>> Get()
		{
			try
			{
				var usuario = User.Identity.Name;
				var contratos = await contexto.Contratos
					.Include(c => c.Inmueble)
						.ThenInclude(i => i.Duenio)
					.Include(c => c.Inmueble)
						.ThenInclude(i => i.Uso)
					.Include(c => c.Inmueble)
						.ThenInclude(i => i.Tipo)
					.Include(i => i.Inquilino)
					.Where(c => c.Inmueble.Duenio.Email == usuario)
					.ToListAsync();
				return Ok(contratos);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}