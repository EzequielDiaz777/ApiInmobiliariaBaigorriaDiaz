using InmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaBaigorriaDiaz.Controllers
{
	[Route("[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class UsoController : ControllerBase
	{
		private readonly DataContext contexto;
		private readonly IConfiguration config;

		public UsoController(DataContext contexto, IConfiguration config)
		{
			this.contexto = contexto;
			this.config = config;
		}
		// GET: <controller>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Uso>>> Get()
		{
			try
			{
				var usuario = User.Identity.Name;
				var usos = await contexto.Usodeinmueble.ToListAsync();
				return Ok(usos);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}