using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        async public Task Create(UserToCreate user) //Создание пользователя
        {
            string password = HashFunctions.GenerationHashPassword(user.Password);
            Users users = new Users(user.Login, password);
            users.role = (from role in _dbContext.Role where role.Id == user.RoleId select role).First();

            _dbContext.Users.Add(users);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 201;

            await response.WriteAsJsonAsync(users);
        }
        [HttpPatch]
        async public Task ChangeRole(UserToChangeRole user) //Обновление информации о пользователе
        {
            var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            userToUpdate.role = (from role in _dbContext.Role where role.Id == user.RoleId select role).First();

            _dbContext.Users.Update(userToUpdate);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode =200;
            await response.WriteAsJsonAsync(userToUpdate);
        }

        [HttpDelete("{id}")]
        async public Task Delete(int id) //Удаление пользователя
        {
            Users userToDelete = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            _dbContext.Users.Remove(userToDelete);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(id);
        }

        [HttpGet("groupsuser")]
        async public Task GetGroupsUser(int idUser) //Получение списка групп пользователя по его Id
        {
            List<Groups> groups = UserFunctions.UserGroups(idUser, _dbContext).Item1; 
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
        }

        [Authorize]
        [HttpGet("groupsuserAuthorize")]
        async public Task GetGroupsUser() //Получение списка групп пользователя по его Id
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            List<Groups> groups = UserFunctions.UserGroups(userId, _dbContext).Item1;
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
        }

        [HttpGet("all")]
        async public Task GetAll() //Получение списка всех пользователей 
        {
            var users = _dbContext.Users.Include(u => u.role).ToList();
            List<UserToGet> userToGets = new List<UserToGet>();
            

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(users);
        }
        [HttpGet]
        async public Task Get(int id) //Получение информации пользователя по его идентификатору
        {
            Users currentUser = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(currentUser);
        }

        /*[Authorize]
        [HttpGet]
        async public Task Get() //Получение информации пользователя по его идентификатору
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            Users currentUser = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(currentUser);
        }*/
    }
}
