using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Order;
using Ecom.Core.Entites.Product;
using Ecom.Core.interfaces;
using Ecom.Core.services;
using Ecom.infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories.Service
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;

		public OrderService(AppDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
		{
			_context = context;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<Orders> CreateOrdersAsync(orderDTO orderDTO, string BuyerEmail)
		{
			var basket = await _unitOfWork.CustomerBasket.GetBasketAsync(orderDTO.basketId);
			if (basket == null || basket.basketItems == null || !basket.basketItems.Any())
				return null;

			List<OrderItem> orderItems = new List<OrderItem>();

			foreach (var item in basket.basketItems)
			{
				var product = await _unitOfWork.productRepository.GetByIdAsync(item.ProductId);
				if (product == null) continue;

				var orderItem = new OrderItem(product.Id, item.Image, product.Name, item.Price, item.Quantity);
				orderItems.Add(orderItem);
			}

			var deliverMethod = await _context.DeliveryMethods.FirstOrDefaultAsync(m => m.Id == orderDTO.delieveryMethodId);
			var subTotal = orderItems.Sum(m => m.Price * m.Quntity);
			var ship = _mapper.Map<ShippingAddress>(orderDTO.shipAdress);

			var order = new Orders(BuyerEmail, subTotal, ship, orderItems, deliverMethod);

			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();
			await _unitOfWork.CustomerBasket.DeleteBasketAsync(orderDTO.basketId);

			return order;
		}

		public async Task<IReadOnlyList<OrderToReturnDTO>> GetAllOrdersForUserAsync(string BuyerEmail)
		{
			var orders = await _context.Orders
				.Where(m => m.BuyerEmail == BuyerEmail)
				.Include(inc => inc.orderItems)
				.Include(inc => inc.deliveryMethod)
				.ToListAsync();

			return _mapper.Map<IReadOnlyList<OrderToReturnDTO>>(orders);
		}

		public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync() =>
			await _context.DeliveryMethods.AsNoTracking().ToListAsync();

		public async Task<OrderToReturnDTO> GetOrderByIdAsync(int id, string BuyerEmail)
		{
			var order = await _context.Orders
				.Where(m => m.Id == id && m.BuyerEmail == BuyerEmail)
				.Include(inc => inc.orderItems)
				.Include(inc => inc.deliveryMethod)
				.FirstOrDefaultAsync();

			return _mapper.Map<OrderToReturnDTO>(order);
		}
	}
}
