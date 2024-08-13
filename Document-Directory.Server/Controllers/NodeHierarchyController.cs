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
        public async Task GetNodeHierarchies(int idUsers) //Отображает внешние папки, т.е. папки и документы, которые не вложены в другие папки
        {
            List<int> nodesId = new List<int>();

            List<Groups> groupsUser = Functions.UserGroups(idUsers, _context);
            List<int> idGroups = new List<int>();
            foreach (var group in groupsUser) { idGroups.Add(group.Id); }

            foreach (NodeHierarchy folder in _context.NodeHierarchy) 
            {
                nodesId.Add(folder.NodeId);
            }

            List<Nodes> exFolders = (from Node in _context.Nodes where !nodesId.Contains(Node.Id) select Node).ToList();
            List<NodeAccess> nodeAccesses = (from Node in _context.NodeAccess where (idGroups.Contains(Node.GroupId)) || (Node.UserId == idUsers) select Node).ToList();
            
            List<Nodes> nodes = new List<Nodes>();

            foreach (var nodeAccess in nodeAccesses)
            {
                Nodes node = _context.Nodes.FirstOrDefault(n => n.Id == nodeAccess.NodeId);
                nodes.Add(node);
            }

            List<Nodes> exFoldersTemp = new List<Nodes>(exFolders);

            foreach (var node in exFolders)
            {
                if (nodes.Contains(node)) { continue; }
                else { exFoldersTemp.Remove(node); }
            }

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(exFoldersTemp);
        }

        [HttpGet("{id}")]
        public async Task GetNodeHierarchy(int id) //Принимает в качестве параметра id папки и отображает все вложенные в эту папку элементы
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
    }
}
