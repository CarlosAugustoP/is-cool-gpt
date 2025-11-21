using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsCool.Auth
{
    public interface IJwtService
    {
        string GenerateToken(Models.User u);
    }
}