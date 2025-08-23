using Ecom.Core.Entites.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Data.Config
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.Property(x => x.Name).IsRequired();
			builder.Property(x => x.Description).IsRequired();
			builder.Property(x => x.NewpPrice).HasColumnType("decimal(18,2)");
			builder.Property(x => x.OldPrice).HasColumnType("decimal(18,2)");


			builder.HasData(

				new Product { Id = 1 , Name="Test" , Description="Test" , CategoryId = 1, NewpPrice = 12 }


				);
		}
	}
}
