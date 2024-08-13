using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Document_Directory.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private AppDBContext _dbContext;

        public DocumentsController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        async public Task Create(Node node, int folderId)
        {
            Nodes nodes = new Nodes(node.Type, node.Name, node.Content, node.CreatedAt, node.ActivityEnd);

            _dbContext.Nodes.Add(nodes);

            _dbContext.SaveChanges();

            int nodeId = nodes.Id;

            if (folderId != 0)
            {
                NodeHierarchy hierarchy = new NodeHierarchy(folderId, nodeId);
                _dbContext.NodeHierarchy.Add(hierarchy);
                _dbContext.SaveChanges();
            }

            var response = this.Response;
            response.StatusCode = 201;
            
            await response.WriteAsJsonAsync(nodes);
        }

        [HttpPatch]
        async public Task Update(Node node)
        {
            var NodesToUpdate = _dbContext.Nodes.FirstOrDefault(x => x.Id == node.Id);
            NodesToUpdate.Name = node.Name;
            NodesToUpdate.Content = node.Content;
            NodesToUpdate.ActivityEnd = node.ActivityEnd;

            _dbContext.Nodes.Update(NodesToUpdate);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(NodesToUpdate);
        }
        [HttpDelete]
        async public Task Delete(int id)
        {
            var response = this.Response;
            var NodeToDelete = _dbContext.Nodes.FirstOrDefault(x => x.Id == id);
            int idToDelete = NodeToDelete.Id;
            _dbContext.Nodes.Remove(NodeToDelete);
            _dbContext.SaveChanges();
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(idToDelete);
        }
        [HttpGet]
        [Route("all")]
        async public Task GetAll()
        {
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Nodes);
        }
        [HttpGet("{id}")]
        async public Task Get(int? id)
        {
            var response = this.Response;
            var node = _dbContext.Nodes.FirstOrDefault(n => n.Id == id);
            await response.WriteAsJsonAsync(node);
        }
    }
}
