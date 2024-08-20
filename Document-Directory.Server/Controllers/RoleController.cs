using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Document_Directory.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private AppDBContext _dbContext;
        public RoleController(AppDBContext dbContext) 
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        async public Task Get(int id) //Получение роли пользователя по его Id
        {
            Role role = _dbContext.Role.FirstOrDefault(x => x.Id == id);
            HttpResponse response = this.Response;
            await response.WriteAsJsonAsync(role);
        }
        [HttpGet("all")]
        async public Task GetAll()
        {
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Role);
        }
    }
}
