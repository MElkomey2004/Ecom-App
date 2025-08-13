using Ecom.Core.Entites.Product;
using Ecom.Core.interfaces;
using Ecom.infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories
{
	public class ProductRepository : GenericRepository<Product> , IProductRepository
	{
		public ProductRepository(AppDbContext context) : base(context)
		{
		}
	}
}
