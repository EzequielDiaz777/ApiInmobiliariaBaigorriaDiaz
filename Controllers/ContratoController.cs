using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
					.Include(c => c.Inmueble) // Incluye la propiedad de navegación Inmueble
						.ThenInclude(i => i.Duenio) // Incluye la propiedad de navegación Duenio dentro de Inmueble
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