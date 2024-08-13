﻿using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Document_Directory.Server.Function;
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
        async public Task Create(Node node, int folderId) //Создание документа и помещение его во вложенную папку по ее id
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
            
            var NodeToDelete = _dbContext.Nodes.FirstOrDefault(x => x.Id == id);
            int idToDelete = NodeToDelete.Id;
            _dbContext.Nodes.Remove(NodeToDelete);
            _dbContext.SaveChanges();

            var response = this.Response;
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

        [HttpGet("access")]
        async public Task GetAccessAll(int idUser)
        {
            List<Groups> groupsUser = Functions.UserGroups(idUser, _dbContext);
            List<int> idGroups = new List<int>();
            foreach (var group in groupsUser) { idGroups.Add(group.Id); }
            List<NodeAccess> nodeAccesses = (from Node in _dbContext.NodeAccess where idGroups.Contains(Node.GroupId) || (Node.UserId == idUser) select Node).ToList();
            List<Nodes> nodes = new List<Nodes>();
            foreach (var nodeAccess in nodeAccesses) 
            {
                Nodes node = _dbContext.Nodes.FirstOrDefault(n => n.Id == nodeAccess.NodeId);
                nodes.Add(node);
            }

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(nodes);
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
