using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Security.Cryptography;

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

        [Authorize]
        [HttpGet]
        public async Task GetNodeHierarchies() //Отображает внешние папки, т.е. папки и документы, которые не вложены в другие папки
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            (List<Groups> groupsUser, List<int?> idGroups) = UserFunctions.UserGroups(userId, _context);
            List<Nodes> nodes = NodeFunctions.AllNodeAccess(userId, idGroups, _context);

            List<int> nodesId = new List<int>();
            foreach (NodeHierarchy folder in _context.NodeHierarchy) 
            {
                nodesId.Add(folder.NodeId);
            }
            List<Nodes> exFolders = (from Node in _context.Nodes where !nodesId.Contains(Node.Id) select Node).ToList();
           
            List<Nodes> exFoldersTemp = NodeFunctions.NodeAccessFolder(exFolders, nodes);

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(exFoldersTemp);
        }

        [Authorize]
        [HttpGet("internal/{idFolder}")]
        public async Task GetNodeHierarchy(int idFolder) //Принимает в качестве параметра id папки и отображает все вложенные в эту папку элементы
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            (List<Groups> groupsUser, List<int?> idGroups) = UserFunctions.UserGroups(userId, _context);
            List<Nodes> nodes = NodeFunctions.AllNodeAccess(userId, idGroups, _context);

            List<NodeHierarchy> exFolder = (from Folder in _context.NodeHierarchy where Folder.FolderId == idFolder select Folder).ToList();
            List<Nodes> inNodes = new List<Nodes>();
            foreach (NodeHierarchy folder in exFolder)
            {
                Nodes node = _context.Nodes.FirstOrDefault(n => n.Id == folder.NodeId); 
                inNodes.Add(node);
            }

            List<Nodes> inNodesTemp = NodeFunctions.NodeAccessFolder(inNodes, nodes);

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(inNodesTemp);
        }
    }
}
