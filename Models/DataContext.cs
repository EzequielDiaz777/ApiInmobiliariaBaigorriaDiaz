using Microsoft.EntityFrameworkCore;

namespace InmobiliariaBaigorriaDiaz.Models
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		public DbSet<Propietario> Propietarios { get; set; }
	}
}