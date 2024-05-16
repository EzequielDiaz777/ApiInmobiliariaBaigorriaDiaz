using Microsoft.EntityFrameworkCore;
using InmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_.Net_Core.Api
{
	[Route("[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class InmueblesController : Controller
	{
		private readonly DataContext contexto;

		public InmueblesController(DataContext contexto)
		{
			this.contexto = contexto;
		}

		// GET: api/<controller>
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			try
			{
				var usuario = User.Identity.Name;
				return Ok(contexto.Inmuebles.Include(e => e.Duenio).Where(e => e.Duenio.Email == usuario));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET api/<controller>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				var usuario = User.Identity.Name;
				Inmueble inmueble = contexto.Inmuebles.Include(e => e.Duenio).Where(e => e.Duenio.Email == usuario).Single(e => e.Id == id);
				inmueble.Tipo = contexto.Tipodeinmueble.Single(e => e.Id == inmueble.TipoId);
				inmueble.Uso = contexto.Usodeinmueble.Single(e => e.Id == inmueble.UsoId);
				return Ok(inmueble);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// POST api/<controller>
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Inmueble entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					entidad.PropietarioId = contexto.Propietarios.Single(e => e.Email == User.Identity.Name).Id;
					contexto.Inmuebles.Add(entidad);
					contexto.SaveChanges();
					return CreatedAtAction(nameof(Get), new { id = entidad.Id }, entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/<controller>/5
		[HttpPut("cambiologico/{id}")]
		public async Task<IActionResult> CambioLogico(int id)
		{
			try
			{
				var usuario = User.Identity.Name;
				var entidad = contexto.Inmuebles.Include(e => e.Duenio).FirstOrDefault(e => e.Id == id && e.Duenio.Email == usuario);
				if (entidad != null)
				{
					if(entidad.Estado){
						entidad.Estado = false;
					} else {
						entidad.Estado = true;
					}
					contexto.Inmuebles.Update(entidad);
					contexto.SaveChanges();
					return Ok();
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}