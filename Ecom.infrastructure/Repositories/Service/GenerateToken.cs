using Ecom.Core.Entites;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories.Service
{
	public class GenerateToken : IGenerateToken
	{
		private readonly IConfiguration configuration; //To can acssess to app settings.

		public GenerateToken(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public string GetAndCreateToken(AppUser user)
		{

			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Name, user.UserName),
			};

			string secret = configuration["Token:Secret"];
			if (string.IsNullOrEmpty(secret))
			{
				throw new ArgumentNullException("Token secret is not configured");
			}

			byte[] key = Encoding.ASCII.GetBytes(secret);
			SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

			SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(7), 
				Issuer = configuration["Token:Issuer"],
				SigningCredentials = credentials,
				NotBefore = DateTime.UtcNow
			};

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

			string tokenString = tokenHandler.WriteToken(token);

			// Log the token to ensure it looks correct
			Console.WriteLine("Generated JWT: " + tokenString);

			return tokenString;
		}
		
		

	
	}
}
