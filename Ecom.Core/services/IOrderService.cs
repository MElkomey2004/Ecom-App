using Ecom.Core.DTO;
using Ecom.Core.Entites.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.services
{
	public interface IOrderService
	{
		Task<Orders> CreateOrdersAsync(orderDTO orderDTO , string BuyerEmail);
		Task<IReadOnlyList<OrderToReturnDTO>> GetAllOrdersForUserAsync(string BuyerEmail);	
		Task<OrderToReturnDTO> GetOrderByIdAsync(int id , string BuyerEmail);
		Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync(); 
	}
}
