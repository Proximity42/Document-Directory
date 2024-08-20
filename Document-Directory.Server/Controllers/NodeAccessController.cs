using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

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
        async public Task Create(int nodeId, int? groupId, int? userId)
        {
            NodeAccess nodeAccess = new NodeAccess(nodeId, groupId, userId);

            _dbContext.NodeAccess.Add(nodeAccess);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 201;
            await response.WriteAsJsonAsync(nodeAccess);
        }

        [HttpPatch]
        async public Task Update(int nodeId, List<int?> groupsId, List<int?> usersId)
        {
            var dbNodes = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId).ToList();

            foreach (var node in dbNodes)
            {
                if (node.UserId.HasValue && !usersId.Contains(node.UserId.Value) || node.GroupId.HasValue && !groupsId.Contains(node.GroupId.Value))
                {
                    _dbContext.NodeAccess.Remove(node);
                }
                else if (node.UserId.HasValue && usersId.Contains(node.UserId.Value))
                {
                    usersId.Remove(node.UserId.Value);
                }
                else if (node.GroupId.HasValue && groupsId.Contains(node.GroupId.Value))
                {
                    groupsId.Remove(node.GroupId.Value);
                }
            }

            foreach (var userId in usersId.Where(u => u.HasValue).Select(u => u.Value))
            {
                var newUserAccess = new NodeAccess(nodeId, null, userId);
                _dbContext.NodeAccess.Add(newUserAccess);
            }

            foreach (var groupId in groupsId.Where(g => g.HasValue).Select(g => g.Value))
            {
                var newGroupAccess = new NodeAccess(nodeId, groupId, null);
                _dbContext.NodeAccess.Add(newGroupAccess);
            }
            
            _dbContext.SaveChanges();

            var updatedNodeAccessList = _dbContext.NodeAccess.Where(n => n.NodeId == nodeId).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(updatedNodeAccessList);
        }
    }
}
