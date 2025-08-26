using Ecom.API.Middleware;
using Ecom.Core.Entites;
using Ecom.infrastructure;
using Ecom.infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;   // ✅ أضفت مكتبة Redis

namespace Ecom.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddMemoryCache();

			var redisConnection = builder.Configuration.GetConnectionString("redis");
			var muxer = ConnectionMultiplexer.Connect(redisConnection);
			builder.Services.AddSingleton<IConnectionMultiplexer>(muxer);

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy.AllowAnyOrigin()
						  .AllowAnyMethod()
						  .AllowAnyHeader();
				});
			});

			builder.Services.AddControllers();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.infrastructureConfiguration(builder.Configuration);

			// Fix for CS1503: Use a lambda to configure AutoMapper
			builder.Services.AddAutoMapper(cfg =>
			{
				cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			//if (app.Environment.IsDevelopment())
			//{
			app.UseSwagger();
			app.UseSwaggerUI();
			//}

			app.UseCors("AllowAll");
			app.UseMiddleware<ExceptionsMiddleware>();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseStaticFiles();
			app.UseStatusCodePagesWithReExecute("/errors/{0}");

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
