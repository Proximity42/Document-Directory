using Document_Directory.Server.ModelsDB;
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
        async public Task Create(int nodeId, int groupId, int userId)
        {
            NodeAccess nodeAccess = new NodeAccess(nodeId, groupId, userId);

            _dbContext.NodeAccess.Add(nodeAccess);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 201;
            await response.WriteAsJsonAsync(nodeAccess);
        }


    }
}
