using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
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
					salt: Encoding.ASCII.GetBytes(config["Salt"]),
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
						Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
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


		private string GetLocalIpAddress()
		{
			string localIp = null;

			// Obtiene todas las direcciones IP del host local
			var host = Dns.GetHostEntry(Dns.GetHostName());

			foreach (var ip in host.AddressList)
			{
				// Selecciona la dirección IPv4 no loopback
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIp = ip.ToString();
					break;
				}
			}

			return localIp;
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
				var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
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
					expires: DateTime.Now.AddHours(5),
					signingCredentials: credenciales
				);

				var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
				var dominio = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
				var resetLink = Url.Action("CambiarPassword", "Propietarios");
				var rutaCompleta = Request.Scheme + "://" + GetLocalIpAddress() + ":" + Request.Host.Port + resetLink;
				var message = new MimeMessage();
				message.To.Add(new MailboxAddress(propietario.Nombre, propietario.Email));
				message.From.Add(new MailboxAddress("Sistema", config["SMTPUser"]));
				message.Subject = "Restablecimiento de Contraseña";
				message.Body = new TextPart("html")
				{
					Text = $@"<h1>Hola {propietario.Nombre},</h1>
						   <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.
							<p>Por favor, haz clic en el siguiente enlace para crear una nueva contraseña:</p>
						   <a href='{rutaCompleta}?access_token={tokenString}'>{rutaCompleta}?access_token={tokenString}</a>"
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

		[HttpGet("cambiarpassword")]
		public async Task<IActionResult> CambiarPassword()
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]);
				var symmetricKey = new SymmetricSecurityKey(key);
				Random rand = new Random(Environment.TickCount);
				string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
				string nuevaClave = "";
				for (int i = 0; i < 8; i++)
				{
					nuevaClave += randomChars[rand.Next(0, randomChars.Length)];
				}
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: nuevaClave,
					salt: Encoding.ASCII.GetBytes(config["Salt"]),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8));
				var p = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
				if (p == null)
				{
					return BadRequest("Nombre de usuario incorrecto");
				}
				else
				{
					p.Password = hashed;
					contexto.Propietarios.Update(p);
					await contexto.SaveChangesAsync();
					var message = new MimeMessage();
					message.To.Add(new MailboxAddress(p.Nombre, p.Email));
					message.From.Add(new MailboxAddress("Sistema", config["SMTPUser"]));
					message.Subject = "Restablecimiento de Contraseña";
					message.Body = new TextPart("html")
					{
						Text = $"<h1>Hola {p.Nombre},</h1>" +
							   $"<p>Has cambiado tu contraseña de forma correcta. " +
							   $"Tu nueva contraseña es la siguiente: {nuevaClave}</p>"
					};

					using var client = new SmtpClient();
					client.ServerCertificateValidationCallback = (s, c, h, e) => true;
					await client.ConnectAsync("sandbox.smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
					await client.AuthenticateAsync(config["SMTPUser"], config["SMTPPass"]);
					await client.SendAsync(message);
					await client.DisconnectAsync(true);

					return Ok("Se ha enviado el restablecido la contraseña correctamente.");
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
			{ //hacer validación 
				if (ModelState.IsValid)
				{
					var propietario = await contexto.Propietarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email == entidad.Email);
					entidad.Id = propietario.Id;
					if (String.IsNullOrEmpty(entidad.Password))
					{
						entidad.Password = propietario.Password;
					}
					else
					{
						string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: entidad.Password,
						salt: Encoding.ASCII.GetBytes(config["Salt"]),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8));
						entidad.Password = hashed;
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
	}
}