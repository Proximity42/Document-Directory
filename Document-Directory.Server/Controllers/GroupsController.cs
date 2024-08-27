using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Document_Directory.Server.Function;

namespace Document_Directory.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private AppDBContext _dbContext;

        public GroupsController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        async public Task Create(Group group) //Создание группы
        {
            Groups groups = new Groups(group.Name);
            _dbContext.Groups.Add(groups);
            foreach (var item in group.Participants) 
            {
                UserGroups users = new UserGroups();
                users.Group = groups;
                users.User = (from User in _dbContext.Users where User.Id == item select User).First();
                _dbContext.UserGroups.Add(users);
            }
            _dbContext.SaveChanges();
            
            var response = this.Response;
            response.StatusCode = 201;
            await response.WriteAsJsonAsync(groups);
        }
        [HttpPatch]
        async public Task Rename(GroupToRename group) //Переименование группы по его Id
        {
            Groups groupToRename = _dbContext.Groups.FirstOrDefault(g => g.Id == group.Id);
            groupToRename.Name = group.Name;

            _dbContext.Groups.Update(groupToRename);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(groupToRename);
        }

        [HttpGet("all")]
        async public Task GetAllGroups()
        {
            var response = this.Response;   
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Groups);
        }

        //[Authorize]
        //[HttpGet("user")]
        //async public Task GetGroupsUser() //Получение списка групп пользователя по его Id
        //{

        //    List<Groups> groups = UserFunctions.UserGroups(idUser, _dbContext).Item1;
        //    var response = this.Response;
        //    response.StatusCode = 200;
        //    await response.WriteAsJsonAsync(groups);
        //}

        [Authorize]
        [HttpGet("user")]
        async public Task GetUserGroups() //Получение списка групп пользователя по его Id
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            List<Groups> groups = UserFunctions.UserGroups(userId, _dbContext).Item1;
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
        }

        [HttpGet("all-participants")]
        public async Task GetAllParticipants()
        {
            var groups = _dbContext.UserGroups.Include(ug => ug.User).Include(ug => ug.Group).GroupBy(ug => ug.GroupId).ToList();
            List<int> participantsId = new List<int>();

            foreach (var group in groups)
            {
                participantsId.AddRange(group.Select(g => g.UserId));
                participantsId.Add(0);
            }

            if (participantsId.Count > 0 && participantsId.Last() == 0)
            {
                participantsId.RemoveAt(participantsId.Count - 1);
            }

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(participantsId);
        }

        [HttpGet("{groupId}")]
        public async Task GetGroupParticipants(int groupId) //Получение списка участников группы по его Id
        {
            List<int> participantsId = new List<int>();
            List<Users> users = new List<Users>();
            List<Users> allUsers = _dbContext.Users.ToList();
            List<UserGroups> groups = _dbContext.UserGroups.ToList();

            foreach (UserGroups user in groups)
            {
                if (user.GroupId == groupId)
                    participantsId.Add(user.UserId);
            }

            foreach (Users user in allUsers)
            {
                if (participantsId.Contains(user.Id))
                    users.Add(user);
            }

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(users);
        }

        [Authorize]
        [HttpGet("with-access-to-node/{nodeId}")]
        async public Task GetGroupsWithAccessToNode(int nodeId) //Получение всех групп, имеющих доступ к узлу
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);
            var groupsId = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId && n.GroupId.HasValue).Select(n => n.GroupId.Value).ToList();
            var groups = _dbContext.Groups.Where(g => groupsId.Contains(g.Id)).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
        }

        [HttpGet("no-access-to-node/{nodeId}")]
        async public Task GetGroupsWithoutAccessToNode(int nodeId) //Получение всех групп, не имеющих доступ к узлу
        {
            var groupsId = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId && n.GroupId.HasValue).Select(n => n.GroupId.Value).ToList();
            var groups = _dbContext.Groups.Where(g => !groupsId.Contains(g.Id)).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
        }

        [HttpDelete("{id}")]
        async public Task DeleteGroup(int id)
        {
            Groups group = _dbContext.Groups.FirstOrDefault((group) => group.Id == id);
            _dbContext.Groups.Remove(group);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsync("");
        }

        [HttpPost("composition")]
        async public Task AddParticipants(int id, List<int> participants) //Добавление удастника(-ов) в группу по его Id
        {
            Groups group = _dbContext.Groups.FirstOrDefault(g => g.Id == id);
            foreach (int participantId in participants)
            {
                UserGroups users = new UserGroups();
                users.GroupId = id;
                users.UserId = participantId;
                _dbContext.UserGroups.Add(users);
            }
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 201;
            await response.WriteAsJsonAsync(group);
        }

        [HttpDelete("composition")]
        async public Task DeleteParticipants(int id, List<int> participants) //Удаление участника(-ов) группы по его Id
        {
            Groups group = _dbContext.Groups.FirstOrDefault(g => g.Id == id);
            foreach (int participantId in participants)
            {
                UserGroups users = _dbContext.UserGroups.FirstOrDefault(u => (u.GroupId == id && u.UserId == participantId));
                _dbContext.UserGroups.Remove(users);
            }
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(group);
        }

        [HttpPatch("UserGroup")]
        async public Task Update(GroupToUpdate groupToUpdate)
        {
            var dbGroups = _dbContext.UserGroups.Where(n => n.GroupId == groupToUpdate.groupId).ToList();

            foreach (var group in dbGroups)
            {
                if (!groupToUpdate.usersId.Contains(group.UserId))
                {
                    _dbContext.UserGroups.Remove(group);
                }
                else
                {
                    groupToUpdate.usersId.Remove(group.UserId);
                }
            }

            foreach (var userId in groupToUpdate.usersId)
            {
                var newUserGroup = new UserGroups(groupToUpdate.groupId, userId);
                _dbContext.UserGroups.Add(newUserGroup);
            }

            _dbContext.SaveChanges();

            var updatedUserGroupList = _dbContext.UserGroups.Where(n => n.GroupId == groupToUpdate.groupId).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(updatedUserGroupList);
        }
    }
}
