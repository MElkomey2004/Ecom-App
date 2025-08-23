using Ecom.Core.Entites.Product;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTO
{
	public class ProductDTO 
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	
		public decimal NewpPrice { get; set; }
		public decimal OldPrice { get; set; }


		public List<PhotoDto> Photos { get; set; }

		public string CategoryName { get; set; }
	}

	public record ReturnProductDTO {


		public List<ProductDTO> products { get; set; }
		public int TotalCount { get;set; }

	}



	public record PhotoDto {

		public string ImageName { get; set; }
		public int ProductId { get; set; }


	}

	public record AddProductDTO
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal NewpPrice { get; set; }
		public decimal OldPrice { get; set; }

		public int  CategoryId { get; set; }
		public IFormFileCollection Photo { get; set; }
	}

	public record UpdateProductDTO : AddProductDTO
	{
		public int Id { get; set; }
	}


}
