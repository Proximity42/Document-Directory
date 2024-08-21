using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.EntityFrameworkCore;

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
        async public Task Rename(int id, string newName) //Переименование группы по его Id
        {
            Groups groupToRename = _dbContext.Groups.FirstOrDefault(g => g.Id == id);
            groupToRename.Name = newName;

            _dbContext.Groups.Update(groupToRename);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(groupToRename);
        }

        [HttpGet]
        public async Task GetGroupParticipants(int groupId) //Получение списка участников группы по его Id
        {
            List<int> participantsId = new List<int>();
            List<Users> users = new List<Users>();

            foreach(UserGroups user in _dbContext.UserGroups)
            {
                participantsId.Add(user.UserId);
            }

            foreach(int userId in participantsId)
            {
                Users user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
                users.Add(user);
            }

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(users);
        }

        [HttpGet("inNode")]
        async public Task GetInNode(int nodeId) //Получение всех групп, имеющих доступ к узлу
        {
            var groupsId = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId && n.GroupId.HasValue).Select(n => n.GroupId.Value).ToList();
            var groups = _dbContext.Groups.Where(g => groupsId.Contains(g.Id)).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(groups);
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
