using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.interfaces
{
	public interface IUnitOfWork
	{
		public ICategoryRepository categoryRepository { get; }
		public IProductRepository productRepository { get; }
		public IPhotoRepository photoRepository { get; }

		public ICustomerBasketRepository CustomerBasket { get; }

		public IAuth Auth { get; }

	}
}
