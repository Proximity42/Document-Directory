using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Document_Directory.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NodeHierarchyController : ControllerBase
    {
        private readonly AppDBContext _context;

        public NodeHierarchyController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task GetNodeHierarchies()
        {
            List<int> nodesId = new List<int>();

            foreach (NodeHierarchy folder in _context.NodeHierarchy) 
            {
                nodesId.Add(folder.NodeId);
            }

            List<Nodes> exFolders = (from Node in _context.Nodes where !nodesId.Contains(Node.Id) select Node).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(exFolders);
        }

        [HttpGet("{id}")]
        public async Task GetNodeHierarchy(int id)
        {
            List<NodeHierarchy> exFolders = (from Folder in _context.NodeHierarchy where Folder.FolderId == id select Folder).ToList();

            List<Nodes> inNodes = new List<Nodes>();

            foreach (NodeHierarchy folder in exFolders)
            {
                Nodes node;
                node = _context.Nodes.FirstOrDefault(n => n.Id == folder.NodeId);
                inNodes.Add(node);
            }

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(inNodes);
        }

        /*[HttpPost]
        public async Task<ActionResult<NodeHierarchy>> PostNodeHierarchy(NodeHierarchy nodeHierarchy)
        {
            _context.Set<NodeHierarchy>().Add(nodeHierarchy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNodeHierarchy", new { id = nodeHierarchy.Id }, nodeHierarchy);
        }*/

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNodeHierarchy(int id)
        {
            var nodeHierarchy = await _context.Set<NodeHierarchy>().FindAsync(id);
            if (nodeHierarchy == null)
            {
                return NotFound();
            }

            _context.Set<NodeHierarchy>().Remove(nodeHierarchy);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/
    }
}
