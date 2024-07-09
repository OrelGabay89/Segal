using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class JwtHelper
    {
        public static bool IsJwtExpired(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return true;

            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            if (expClaim == null)
                return true;

            var expDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

            return expDateTime < DateTime.UtcNow;
        }
    }
}
