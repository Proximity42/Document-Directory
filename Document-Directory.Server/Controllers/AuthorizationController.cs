using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Document_Directory.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private AppDBContext _dbContext;

        public AuthorizationController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        async public Task GetToken(UserToToken user)
        {
            string Token = Functions.GenerationToken(user, _dbContext);
            HttpResponse response = this.Response;
            if (response == null)
            {
                response.StatusCode = 401;
                await response.WriteAsJsonAsync(user);
            }
            
            int idUser = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);
            response.Headers.Append("Token", Token);
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Users.FirstOrDefault(u => u.Id == idUser));
        }
    }
    /*public record Token
    {
        public string token { get; set; }
        public string identityName { get; set; }
    }*/
}
