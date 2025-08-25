using AutoMapper;
using Ecom.Core.Entites;
using Ecom.Core.interfaces;
using Ecom.Core.services;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositories.Service;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
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
		private readonly IMapper _mapper;
		private readonly IConnectionMultiplexer redis;
		private readonly IImageManagementService _imageManagementService;
		private readonly UserManager<AppUser> _userManager;
		private readonly IEmailService _emailService;
		private readonly SignInManager<AppUser> _signInManager; // Corrected type name
		private readonly IGenerateToken _generateToken;

		public ICategoryRepository categoryRepository { get; }
		public IProductRepository productRepository { get; }
		public IPhotoRepository photoRepository { get; }
		public ICustomerBasketRepository CustomerBasket { get; }
		public IAuth Auth { get; }

		public UnitOfWork(AppDbContext context, IImageManagementService imageManagementService, IMapper mapper, IConnectionMultiplexer redis, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IGenerateToken generateToken)
		{
			_context = context;
			_imageManagementService = imageManagementService;
			_mapper = mapper;
			this.redis = redis;
			_userManager = userManager;
			_signInManager = signInManager; // Assign the SignInManager instance
			_generateToken = generateToken;
			_emailService = emailService;
			categoryRepository = new CategoryRepository(_context);
			productRepository = new ProductRepositry(_context, _mapper, _imageManagementService);
			photoRepository = new PhotoRepository(_context);
			CustomerBasket = new CustomerBasketRepository(redis);
			Auth = new AuthRepository(_userManager, _emailService, _signInManager , _generateToken , context);
		}
	}
}
