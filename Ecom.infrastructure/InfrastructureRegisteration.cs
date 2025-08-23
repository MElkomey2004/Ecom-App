using Ecom.Core.Entites;
using Ecom.Core.interfaces;
using Ecom.Core.services;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositories;
using Ecom.infrastructure.Repositories.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure
{
	public static class InfrastructureRegisteration
	{
		public static IServiceCollection infrastructureConfiguration(this IServiceCollection services , IConfiguration configuration)
		{
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			services.AddScoped<IEmailService, EmailService>();

			//register token
			services.AddScoped<IGenerateToken, GenerateToken>();	

			//apply redis connection
			services.AddSingleton<IConnectionMultiplexer>(i =>
			{
				var config = ConfigurationOptions.Parse(configuration.GetConnectionString("redis"));
				return ConnectionMultiplexer.Connect(config);
			});

			services.AddSingleton<IImageManagementService, ImageManagementService>();
			services.AddSingleton<IFileProvider>(
				new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));



			services.AddDbContext<AppDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("EcomDatabase"));
			});

			services.AddIdentity<AppUser, IdentityRole>()
			.AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders();
			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
	 .AddCookie(x =>
	 {
		 x.Cookie.Name = "token";
		 x.Events.OnRedirectToLogin = context =>
		 {
			 context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			 return Task.CompletedTask;
		 };
	 })
	 .AddJwtBearer(x =>
	 {
		 x.RequireHttpsMetadata = false;
		 x.SaveToken = true;
		 x.TokenValidationParameters = new TokenValidationParameters
		 {
			 ValidateIssuerSigningKey = true,

			 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Secret"])),
			 ValidateIssuer = true,
			 ValidIssuer = configuration["Token:Issuer"],
			 ValidateAudience = false,
			 ClockSkew = TimeSpan.Zero
		 };
		 x.Events = new JwtBearerEvents
		 {

			 OnMessageReceived = context =>
			 {
				 var token = context.Request.Cookies["token"];
				 context.Token = token;
				 return Task.CompletedTask;
			 }
		 };
	 });



			return services;
		}
	}
}
