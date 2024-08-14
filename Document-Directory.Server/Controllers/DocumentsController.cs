using Document_Directory.Server.Models;
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
        async public Task Create(NodeToCreate node, int folderId) //Создание документа и помещение его во вложенную папку по ее id
        {
            Nodes nodes = new Nodes(node.Type, node.Name, node.Content, DateTime.Now, node.ActivityEnd);

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
        async public Task Update(NodeToUpdate node) //Обновление информации о узле
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
        async public Task Delete(int id) //Удаление узла по его Id
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
        async public Task GetAll() //Получение списка всех существующих узлов
        {
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Nodes);
        }

        [HttpGet("access")]
        async public Task GetAccessAll(int idUser) //Получение списка всех доступных узлов пользователю по его Id
        {
            (List<Groups> groupsUser, List<int> idGroups) = Functions.UserGroups(idUser, _dbContext);
            List<Nodes> nodes = Functions.AllNodeAccess(idUser, idGroups, _dbContext);

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(nodes);
        }

        [HttpGet("{id}")]
        async public Task Get(int? id) //Получение информации о узле по его Id
        {
            var response = this.Response;
            var node = _dbContext.Nodes.FirstOrDefault(n => n.Id == id);
            await response.WriteAsJsonAsync(node);
        }
    }
}
