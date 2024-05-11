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
	[Route("[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class PropietariosController : ControllerBase 
	{
		private readonly DataContext contexto;
		private readonly IConfiguration config;
		private readonly IWebHostEnvironment environment;

		public PropietariosController(DataContext contexto, IConfiguration config, IWebHostEnvironment env)
		{
			this.contexto = contexto;
			this.config = config; 
			environment = env;
		}
		// GET: <controller>
		[HttpGet]
		public async Task<ActionResult<Propietario>> Get()
		{
			try
			{
				var usuario = User.Identity.Name;
				return await contexto.Propietarios.SingleOrDefaultAsync(x => x.Email == usuario);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET <controller>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				var entidad = await contexto.Propietarios.SingleOrDefaultAsync(x => x.Id == id);
				return entidad != null ? Ok(entidad) : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET <controller>/token
		[HttpGet("token")]
		public IActionResult Token()
		{
			var perfil = new
			{
				Email = User.Identity.Name,
				Nombre = User.Claims.First(x => x.Type == "FullName").Value,
				Rol = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value
			};

			Random rand = new Random(Environment.TickCount);
			string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
			string nuevaPassword = "";

			for (int i = 0; i < 8; i++)
			{
				nuevaPassword += randomChars[rand.Next(0, randomChars.Length)];
			}

			// Mueve el return fuera del bucle
			return Ok(perfil);
		}



		// GET <controller>/email
		[HttpPost("email")]
		[AllowAnonymous]
		public async Task<IActionResult> GetByEmail([FromForm] string email)
		{
			try
			{ //método sin autenticar, busca el propietario x email
				var entidad = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
				//para hacer: si el propietario existe, mandarle un email con un enlace con el token
				//ese enlace servirá para resetear la contraseña
				//Dominio sirve para armar el enlace, en local será la ip y en producción será el dominio www...
				var dominio = environment.IsDevelopment() ? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() : "www.misitio.com";
				return entidad != null ? Ok(entidad) : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET <controller>/GetAll
		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				return Ok(await contexto.Propietarios.ToListAsync());
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// POST <controller>/login
		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromForm] LoginView loginView)
		{
			try
			{
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: loginView.Clave,
					salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8));
				var p = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == loginView.Usuario);
				if (p == null || p.Password != hashed)
				{
					return BadRequest("Nombre de usuario o Password incorrecta");
				}
				else
				{
					var key = new SymmetricSecurityKey(
						System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
					var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, p.Email),
						new Claim("FullName", p.Nombre + " " + p.Apellido),
						new Claim(ClaimTypes.Role, "Administrador"),
					};

					var token = new JwtSecurityToken(
						issuer: config["TokenAuthentication:Issuer"],
						audience: config["TokenAuthentication:Audience"],
						claims: claims,
						expires: DateTime.Now.AddMinutes(60000),
						signingCredentials: credenciales
					);
					return Ok(new JwtSecurityTokenHandler().WriteToken(token));
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// POST <controller>
		/*[HttpPost]
		public async Task<IActionResult> Post([FromForm] Propietario entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					await contexto.Propietarios.AddAsync(entidad);
					contexto.SaveChanges();
					return CreatedAtAction(nameof(Get), new { id = entidad.Id }, entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}*/

		// PUT <controller>
		[HttpPut]
		public async Task<IActionResult> Put([FromForm] Propietario entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var propietario = await contexto.Propietarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email == entidad.Email);
					entidad.Id = propietario.Id;
					//Propietario original = await contexto.Propietarios.AsNoTracking().FirstAsync(x => x.Id == id);
					if (String.IsNullOrEmpty(entidad.Password))
					{
						entidad.Password = propietario.Password;
					}
					else
					{
						entidad.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
							password: entidad.Password,
							salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
							prf: KeyDerivationPrf.HMACSHA1,
							iterationCount: 1000,
							numBytesRequested: 256 / 8));
					}
					contexto.Propietarios.Update(entidad);
					await contexto.SaveChangesAsync();
					return Ok(entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE <controller>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var p = await contexto.Propietarios.FindAsync(id);
					if (p == null)
						return NotFound();
					contexto.Propietarios.Remove(p);
					await contexto.SaveChangesAsync();
					return Ok(p);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET: Propietarios/test
		[HttpGet("test")]
		[AllowAnonymous]
		public IActionResult Test()
		{
			try
			{
				return Ok("anduvo");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET: Propietarios/test/5
		[HttpGet("test/{codigo}")]
		[AllowAnonymous]
		public IActionResult Code(int codigo)
		{
			try
			{
				//StatusCodes.Status418ImATeapot //constantes con códigos
				return StatusCode(codigo, new { Mensaje = "Anduvo", Error = false });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}