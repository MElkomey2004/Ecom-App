using Ecom.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories.Service
{
	public interface IGenerateToken
	{
		string GetAndCreateToken(AppUser user);
	}
}
