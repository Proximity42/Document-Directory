using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
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
            string password = AuthorizationFunctions.GenerationHashPassword(user.Password);
            Users users = _dbContext.Users.FirstOrDefault(x => x.Login == user.Login && x.Password == password);

            string Token = AuthorizationFunctions.GenerationToken(users, _dbContext);
            HttpResponse response = this.Response;
            if (users == null)
            {
                response.StatusCode = 401;
                await response.WriteAsJsonAsync("");
            }
            else
            {
                /*CookieContainer cookieContainer = new CookieContainer();

                // установка кук
                cookieContainer.SetCookies(new Uri("https://localhost:5173/"), "name=Bob");*/
                
                response.Cookies.Append("test", Token);

                int idUser = users.Id;
                //response.Headers.Append("Authorization", Token);
                response.StatusCode = 200;
                await response.WriteAsJsonAsync(_dbContext.Users.FirstOrDefault(u => u.Id == idUser));
            }
            
        }
    }
    /*public record Token
    {
        public string token { get; set; }
        public string identityName { get; set; }
    }*/
}
