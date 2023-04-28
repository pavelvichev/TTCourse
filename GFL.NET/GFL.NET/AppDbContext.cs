using GFL.NET.Models;
using Microsoft.EntityFrameworkCore;

namespace GFL.NET
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
		{
		}

		public DbSet<CatalogModel> Catalogs { get; set; }
		
	}

}
