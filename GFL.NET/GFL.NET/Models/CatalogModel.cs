using System.ComponentModel.DataAnnotations.Schema;

namespace GFL.NET.Models
{
	public class CatalogModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		[ForeignKey("Id")]
		public int? ParentId { get; set; }

        
       

	}
}
