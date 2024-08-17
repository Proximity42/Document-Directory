using Document_Directory.Server.Authorization;
using Document_Directory.Server.ModelsDB;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Document_Directory.Server.Function
{
    public class AuthorizationFunctions
    {
        //Генерация токена
        public static string GenerationToken(Users user, AppDBContext _dbContext)
        {
            var claimsIdentity = GetIdentity(user, _dbContext);
            if (claimsIdentity == null)
            {
                return null;
            }
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claimsIdentity.Claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)), // время действия 2 минуты
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        static private ClaimsIdentity GetIdentity(Users users, AppDBContext _dbContext)
        {

            if (users != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, users.Login),
                    new Claim("Id", users.Id.ToString()),
                    new Claim("Role", _dbContext.Role.FirstOrDefault(r => r.Id == users.roleId).UserRole)
                    //new Claim(ClaimsIdentity.DefaultRoleClaimType, _dbContext.Role.FirstOrDefault(r => r.Id == users.roleId).UserRole)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}
