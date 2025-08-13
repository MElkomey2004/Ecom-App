using Ecom.Core.interfaces;
using Ecom.infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;



		public ICategoryRepository categoryRepository { get; }

		public IProductRepository productRepository { get; }

		public IPhotoRepository photoRepository { get; }



		public UnitOfWork(AppDbContext context)
		{
			_context = context;
			categoryRepository = new CategoryRepository(_context);
			productRepository = new ProductRepository(_context);
			photoRepository = new PhotoRepository(_context);
		}
	}
}
