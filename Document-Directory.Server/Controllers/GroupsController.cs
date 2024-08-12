﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;

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
        async public Task Create(Group group)
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
        async public Task Rename(int id, string newName)
        {
            Groups groupToRename = _dbContext.Groups.FirstOrDefault(g => g.Id == id);
            groupToRename.Name = newName;

            _dbContext.Groups.Update(groupToRename);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(groupToRename);
        }
        /*[HttpGet]
        async public Task listParticipants(int id)
        {
            Groups groups 
        }*/
    }
}
