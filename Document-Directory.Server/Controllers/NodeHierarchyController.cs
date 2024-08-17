using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
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

        [HttpGet]
        public async Task GetNodeHierarchies() //Отображает внешние папки, т.е. папки и документы, которые не вложены в другие папки
        {
            int idUsers = 3;
            (List<Groups> groupsUser, List<int> idGroups) = Functions.UserGroups(idUsers, _context);
            List<Nodes> nodes = Functions.AllNodeAccess(idUsers, idGroups, _context);

            List<int> nodesId = new List<int>();
            foreach (NodeHierarchy folder in _context.NodeHierarchy) 
            {
                nodesId.Add(folder.NodeId);
            }
            List<Nodes> exFolders = (from Node in _context.Nodes where !nodesId.Contains(Node.Id) select Node).ToList();
           
            List<Nodes> exFoldersTemp = Functions.NodeAccessFolder(exFolders, nodes);

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(exFoldersTemp);
        }

        [HttpGet("{idFolder}")]
        public async Task GetNodeHierarchy(int idFolder) //Принимает в качестве параметра id папки и отображает все вложенные в эту папку элементы
        {
            int idUser = 3;
            (List<Groups> groupsUser, List<int> idGroups) = Functions.UserGroups(idUser, _context);
            List<Nodes> nodes = Functions.AllNodeAccess(idUser, idGroups, _context);

            List<NodeHierarchy> exFolder = (from Folder in _context.NodeHierarchy where Folder.FolderId == idFolder select Folder).ToList();
            List<Nodes> inNodes = new List<Nodes>();
            foreach (NodeHierarchy folder in exFolder)
            {
                Nodes node = _context.Nodes.FirstOrDefault(n => n.Id == folder.NodeId); 
                inNodes.Add(node);
            }

            List<Nodes> inNodesTemp = Functions.NodeAccessFolder(inNodes, nodes);

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(inNodesTemp);
        }
    }
}
