using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Document_Directory.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeAccessController : ControllerBase
    {
        private AppDBContext _dbContext;

        public NodeAccessController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        async public Task Create(AccessToCreate accessToCreate)
        {
            NodeAccess nodeAccess = new NodeAccess(accessToCreate.nodeId, accessToCreate.groupId, accessToCreate.userId);

            _dbContext.NodeAccess.Add(nodeAccess);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 201;
            await response.WriteAsJsonAsync(nodeAccess);
        }

        [HttpPut]
        async public Task Update(AccessToUpdate accessToUpdate)
        {
            var authorUserId = _dbContext.Nodes.Where(n => n.Id == accessToUpdate.nodeId).Select(n => n.UserId).FirstOrDefault();
            var dbNodes = _dbContext.NodeAccess.Where(n => n.NodeId == accessToUpdate.nodeId).ToList();

            foreach (var node in dbNodes)
            {
                if (node.UserId.HasValue && node.UserId != authorUserId && !accessToUpdate.usersId.Contains(node.UserId.Value) 
                    || node.GroupId.HasValue && !accessToUpdate.groupsId.Contains(node.GroupId.Value))
                {
                    _dbContext.NodeAccess.Remove(node);
                }
                else if (node.UserId.HasValue && accessToUpdate.usersId.Contains(node.UserId.Value))
                {
                    accessToUpdate.usersId.Remove(node.UserId.Value);
                }
                else if (node.GroupId.HasValue && accessToUpdate.groupsId.Contains(node.GroupId.Value))
                {
                    accessToUpdate.groupsId.Remove(node.GroupId.Value);
                }
            }

            foreach (var userId in accessToUpdate.usersId.Where(u => u.HasValue).Select(u => u.Value))
            {
                var newUserAccess = new NodeAccess(accessToUpdate.nodeId, null, userId);
                _dbContext.NodeAccess.Add(newUserAccess);
            }

            foreach (var groupId in accessToUpdate.groupsId.Where(g => g.HasValue).Select(g => g.Value))
            {
                var newGroupAccess = new NodeAccess(accessToUpdate.nodeId, groupId, null);
                _dbContext.NodeAccess.Add(newGroupAccess);
            }

            _dbContext.SaveChanges();

            var updatedNodeAccessList = _dbContext.NodeAccess.Where(n => n.NodeId == accessToUpdate.nodeId).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(updatedNodeAccessList);
        }

        [Authorize]
        [HttpGet("check-access-edit/{nodeId}")]
        async public Task CheckAccessEdit(int nodeId)
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value); ;
            Nodes node = _dbContext.Nodes.Find(nodeId);
            Users user = _dbContext.Users.Find(userId);

            var response = this.Response;
            if (node.UserId == userId || UserFunctions.GetRoleUser(user.roleId, _dbContext) == "Администратор")
            {
                response.StatusCode = 200;
                await response.WriteAsJsonAsync(true);
            }
            else
            {
                response.StatusCode = 403;
                await response.WriteAsJsonAsync(false);
            }
        }
    }
}
