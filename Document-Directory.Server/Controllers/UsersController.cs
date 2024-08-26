﻿using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        async public Task Create(UserToCreate user) //Создание пользователя
        {
            string password = AuthorizationFunctions.GenerationHashPassword(user.Password);
            Users users = new Users(user.Login, password);
            users.role = (from role in _dbContext.Role where role.Id == user.RoleId select role).First();

            _dbContext.Users.Add(users);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 201;

            await response.WriteAsJsonAsync(users);
        }

        [Authorize]
        [HttpPatch("RoleChange")]
        async public Task ChangeRole(UserToChangeRole user) //Обновление информации о пользователе
        {
            var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            userToUpdate.role = (from role in _dbContext.Role where role.Id == user.RoleId select role).First();

            _dbContext.Users.Update(userToUpdate);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(userToUpdate);
        }

        [Authorize]
        [HttpPatch("password-change")]
        async public Task ChangePassword(UserToChangePassword user) //Изменение пароля
        {
            var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            string password = AuthorizationFunctions.GenerationHashPassword(user.Password);

            userToUpdate.Password = password;

            _dbContext.Users.Update(userToUpdate);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(userToUpdate);
        }

        [Authorize]
        [HttpPatch("password-change-authorized")]
        async public Task ChangePassword(Password password) //Изменение пароля
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);
            Users user = _dbContext.Users.Find(userId);
            string oldPassword = AuthorizationFunctions.GenerationHashPassword(password.oldPassword);
            if (oldPassword == user.Password)
            {
                user.Password = AuthorizationFunctions.GenerationHashPassword(password.newPassword);
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();

                var response = this.Response;
                response.StatusCode = 200;
                await response.WriteAsJsonAsync(user);
            }
            else
            {
                var response = this.Response;
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(user);
            }
            

            
        }

        [Authorize]
        [HttpDelete("{id}")]
        async public Task Delete(int id) //Удаление пользователя
        {
            Users userToDelete = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            _dbContext.Users.Remove(userToDelete);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(id);
        }

        [Authorize]
        [HttpGet("user-groups")]
        async public Task GetGroupsUser(int idUser) //Получение списка групп пользователя по его Id
        {
            List<Groups> groups = UserFunctions.UserGroups(idUser, _dbContext).Item1;
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
        }

        [Authorize]
        [HttpGet("all-user-groups")]
        async public Task GetGroupsUser() //Получение списка групп пользователя по его Id
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            List<Groups> groups = UserFunctions.UserGroups(userId, _dbContext).Item1;
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
        }

        [Authorize]
        [HttpGet("all")]
        async public Task GetAll() //Получение списка всех пользователей 
        {
            var users = _dbContext.Users.Include(u => u.role).ToList();
            List<UserToGet> userToGets = new List<UserToGet>();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(users);
        }

        [Authorize]
        [HttpGet]
        async public Task Get(int userId) //Получение информации пользователя по его идентификатору
        {
            Users currentUser = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(currentUser);
        }

        [Authorize]
        [HttpGet("with-access-to-node/{nodeId}")]
        async public Task GetUsersWithAccessToNode(int nodeId) //Получение всех пользователей, имеющих доступ к узлу
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            var usersWithDirectAccess = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId && n.UserId.HasValue).Select(n => n.UserId.Value).ToList();
            var groupsWithAccess = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId && n.GroupId.HasValue).Select(n => n.GroupId.Value).ToList();
            var usersInGroupsWithAccess = _dbContext.UserGroups.Where(ug => groupsWithAccess.Contains(ug.GroupId)).Select(ug => ug.UserId).ToList();
            var usersWithAccess = usersWithDirectAccess.Union(usersInGroupsWithAccess).Where(u => u != userId).ToList();
            var users = _dbContext.Users.Where(u => usersWithAccess.Contains(u.Id) && u.roleId != 1 && u.Id != userId).Include(u => u.role).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(users);
        }

        [Authorize]
        [HttpGet("no-access-to-node/{nodeId}")]
        async public Task GetUsersWithoutAccessToNode(int nodeId) //Получение всех пользователей, не имеющих доступ к узлу
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            var allUserIds = _dbContext.Users.Where(u => u.roleId != 1 && u.Id != userId).Select(u => u.Id).ToList();
            var usersWithDirectAccess = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId && n.UserId.HasValue).Select(n => n.UserId.Value).ToList();
            var groupsWithAccess = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId && n.GroupId.HasValue).Select(n => n.GroupId.Value).ToList();
            var usersInGroupsWithAccess = _dbContext.UserGroups.Where(ug => groupsWithAccess.Contains(ug.GroupId)).Select(ug => ug.UserId).ToList();
            var usersWithAccess = usersWithDirectAccess.Union(usersInGroupsWithAccess).ToList();
            var usersWithoutAccess = _dbContext.Users.Where(u => !usersWithAccess.Contains(u.Id) && u.roleId != 1 && u.Id != userId).Include(u => u.role).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(usersWithoutAccess);
        }


        [Authorize]
        [HttpGet("current-user")]
        async public Task GetCurrentUser() //Получение информации пользователя по его идентификатору
        {
            //int userId = 1;
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            Users currentUser = _dbContext.Users.Include(u=> u.role).FirstOrDefault(u => u.Id == userId);
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(currentUser);
        }
    }
}
