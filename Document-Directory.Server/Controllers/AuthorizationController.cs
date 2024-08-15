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
            (string Token, ClaimsIdentity identity) = Functions.GenerationToken(user, _dbContext);
            HttpResponse response = this.Response;
            Token token = new Token();
            token.token = Token;
            token.identityName = identity.FindFirst("Id").Value;

            await response.WriteAsJsonAsync(token);
        }
    }
    public record Token
    {
        public string token { get; set; }
        public string identityName { get; set; }
    }
}
