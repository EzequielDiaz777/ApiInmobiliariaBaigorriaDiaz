using InmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaBaigorriaDiaz.Controllers
{
	[Route("[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class UsosController : ControllerBase
	{
		private readonly DataContext contexto;

		public UsosController(DataContext contexto)
		{
			this.contexto = contexto;
		}
		
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

		[HttpGet("{id}")]
		public async Task<ActionResult<Uso>> Get(int id)
		{
			try
			{
				var entidad = await contexto.Usodeinmueble.SingleOrDefaultAsync(u => u.Id == id);
				return entidad != null ? Ok(entidad) : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}