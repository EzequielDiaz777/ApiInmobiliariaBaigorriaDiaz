using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InmobiliariaBaigorriaDiaz.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

namespace InmobiliariaBaigorriaDiaz.Controllers
{
	[Route("[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class PropietariosController : ControllerBase 
	{
		private readonly DataContext contexto;
		private readonly IConfiguration config;

		public PropietariosController(DataContext contexto, IConfiguration config)
		{
			this.contexto = contexto;
			this.config = config; 
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
						expires: DateTime.Now.AddHours(60000),
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

		[HttpPost("olvidecontraseña")]
		[AllowAnonymous]
		public async Task<IActionResult> EnviarEmail([FromForm] string email)
		{
			try
			{
				var propietario = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
				if (propietario == null)
				{
					return NotFound("No se encontró ningún usuario con esta dirección de correo electrónico.");
				}

				// Generar y guardar un token de restablecimiento de contraseña
				var key = new SymmetricSecurityKey(
						System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
				var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, propietario.Email),
					new Claim("FullName", $"{propietario.Nombre} {propietario.Apellido}"),
					new Claim(ClaimTypes.Role, "Usuario"),
                };

				var token = new JwtSecurityToken(
					issuer: config["TokenAuthentication:Issuer"],
					audience: config["TokenAuthentication:Audience"],
					claims: claims,
					expires: DateTime.Now.AddMinutes(5),
					signingCredentials: credenciales
				);

				var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

				var resetLink = Url.Action("CambiarPassword", "Propietarios", new { token = tokenString }, Request.Scheme);
				var message = new MimeMessage();
				message.To.Add(new MailboxAddress(propietario.Nombre, propietario.Email));
				message.From.Add(new MailboxAddress("Sistema", config["SMTPUser"]));
				message.Subject = "Restablecimiento de Contraseña";
				message.Body = new TextPart("html")
				{
					Text = $"<h1>Hola {propietario.Nombre},</h1>" +
						   $"<p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta. " +
						   $"Por favor, haz clic en el siguiente enlace para crear una nueva contraseña:</p>" +
						   $"<a href=\"{resetLink}\">{resetLink}</a>"
				};

				using var client = new SmtpClient();
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;
				await client.ConnectAsync("sandbox.smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(config["SMTPUser"], config["SMTPPass"]);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);

				return Ok("Se ha enviado el enlace de restablecimiento de contraseña correctamente.");
			}
			catch (Exception ex)
			{
				return BadRequest($"Error: {ex.Message}");
			}
		}

		[HttpPost("cambiarcontraseña")]
		public async Task<IActionResult> CambiarPassword([FromForm] LoginView loginView)
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
				if (p == null )
				{
					return BadRequest("Nombre de usuario incorrecto");
				}
				else
				{
					p.Password = hashed;
					contexto.Propietarios.Update(p);
					await contexto.SaveChangesAsync();
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
						expires: DateTime.Now.AddHours(60000),
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

		// PUT <controller>
		[HttpPut]
		public async Task<IActionResult> Put([FromBody] Propietario entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					// Obtener el propietario actual de la base de datos
					var propietario = await contexto.Propietarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email == entidad.Email);

					// Asignar el ID y la contraseña del propietario actual al modelo recibido en la solicitud
					entidad.Id = propietario.Id;
					entidad.Password = propietario.Password;

					// Excluir la propiedad "Password" al actualizar el propietario
					contexto.Entry(entidad).Property(x => x.Password).IsModified = false;

					// Actualizar el propietario en la base de datos
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

	}
}