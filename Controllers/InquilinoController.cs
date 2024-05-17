using InmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaBaigorriaDiaz.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class InquilinosController : ControllerBase
	{
		private readonly DataContext contexto;

		public InquilinosController(DataContext contexto)
		{
			this.contexto = contexto;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Inquilino>> Get(int id)
		{
			try
			{
				var entidad = await contexto.Inquilinos.SingleOrDefaultAsync(x => x.Id == id);
				return entidad != null ? Ok(entidad) : NotFound();
			}catch(Exception ex){
				return BadRequest(ex.Message);
			}
		}
	}
}