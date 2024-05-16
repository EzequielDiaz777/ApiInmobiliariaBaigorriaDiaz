using InmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaBaigorriaDiaz.Controllers
{
	[Route("[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class TipoController : ControllerBase
	{
		private readonly DataContext contexto;
		private readonly IConfiguration config;

		public TipoController(DataContext contexto, IConfiguration config)
		{
			this.contexto = contexto;
			this.config = config;
		}
		// GET: <controller>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Tipo>>> Get()
		{
			try
			{
				var usuario = User.Identity.Name;
				var tipos = await contexto.Tipodeinmueble.ToListAsync();
				return Ok(tipos);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}