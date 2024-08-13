using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Document_Directory.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private AppDBContext _dbContext;

        public UsersController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        async public Task Create(User user)
        {
            Users users = new Users(user.Login, user.Password);
            users.role = (from role in _dbContext.Role where role.Id == user.RoleId select role).First();

            _dbContext.Users.Add(users);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 201;

            await response.WriteAsJsonAsync(users);
        }
        [HttpPatch]
        async public Task Update(User user)
        {
            var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            userToUpdate.role = (from role in _dbContext.Role where role.Id == user.RoleId select role).First();

            _dbContext.Users.Update(userToUpdate);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode =200;
            await response.WriteAsJsonAsync(userToUpdate);
        }

        [HttpDelete]
        async public Task Delete(int id)
        {
            Users userToDelete = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            _dbContext.Users.Remove(userToDelete);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(id);
        }

        [HttpGet("groupsuser")]
        async public Task GetGroupsUser(int idUser)
        {
            List<Groups> groups = Functions.UserGroups(idUser, _dbContext); 
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
        }

        [HttpGet("all")]
        async public Task GetAll()
        {
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Users);
        }
        [HttpGet]
        async public Task Get(int id)
        {
            Users currentUser = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(currentUser);
        }


    }
}
