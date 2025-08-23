using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Entites.Product
{
	public class Product : BaseEntity<int>
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal NewpPrice { get; set; }
		public decimal OldPrice { get; set; }

		public List<Photo> Photos { get;set; } 
		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
		public virtual Category Category { get; set; }
	}
}
