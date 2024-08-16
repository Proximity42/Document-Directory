using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Document_Directory.Server.Function;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

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
        async public Task Create(NodeToCreate node) //Создание документа и помещение его во вложенную папку по ее id
        {
            
            var response = this.Response;

            Nodes nodes = Functions.FolderDocumentCheck(node);

            HttpContext context = this.HttpContext;

            //string Id = context.User.FindFirst("Role").Value;

            _dbContext.Nodes.Add(nodes);
            _dbContext.SaveChanges();

            int nodeId = nodes.Id;
            if (node.folderId != 0)
            {
                NodeHierarchy hierarchy = new NodeHierarchy(node.folderId, nodeId);
                _dbContext.NodeHierarchy.Add(hierarchy);
                _dbContext.SaveChanges();
            }
            
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
        [HttpDelete("{id}")]
        async public Task Delete(int id) //Удаление узла по его Id
        {
            var NodeToDelete = _dbContext.Nodes.FirstOrDefault(x => x.Id == id);
            int idToDelete = NodeToDelete.Id;

            Functions.DeleteFolderRecursively(id, _dbContext);

            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(id);
        }
        [HttpGet]
        [Route("all")]
        async public Task GetAll() //Получение списка всех существующих узлов
        {
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Nodes.OrderBy(n => n.Type));
        }

        [HttpGet("access")]
        async public Task GetAccessAll(int idUser) //Получение списка всех доступных узлов пользователю по его Id
        {
            (List<Groups> groupsUser, List<int> idGroups) = Functions.UserGroups(idUser, _dbContext);
            List<Nodes> nodes = Functions.AllNodeAccess(idUser, idGroups, _dbContext);
            

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(nodes.OrderBy(n=>n.Type));
        }

        [HttpGet("{id}")]
        async public Task Get(int? id) //Получение информации о узле по его Id
        {
            var response = this.Response;
            var node = _dbContext.Nodes.FirstOrDefault(n => n.Id == id);
            await response.WriteAsJsonAsync(node);
        }

        [HttpGet("filterByActivityDate")]
        async public Task FilterByActivityDate(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sortDescending = false) //Фильтрация документов по дате активности с сортировкой
        {
            var filteredNodes = _dbContext.Nodes.AsQueryable();

            if (startDate.HasValue)
            {
                var startDateUtc = startDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.ActivityEnd >= startDateUtc);
            }

            if (endDate.HasValue)
            {
                var endDateUtc = endDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.ActivityEnd <= endDateUtc);
            }

            filteredNodes = sortDescending 
                            ? filteredNodes.OrderByDescending(n => n.ActivityEnd)
                            : filteredNodes.OrderBy(n => n.ActivityEnd);

            var result = filteredNodes.ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(result);
        }

        [HttpGet("filterByCreateDate")]
        async public Task FilterByCreateDate(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sortDescending = false) //Фильтрация узлов по дате создания с сортировкой
        {
            var filteredNodes = _dbContext.Nodes.AsQueryable();

            if (startDate.HasValue)
            {
                var startDateUtc = startDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.CreatedAt >= startDateUtc);
            }

            if (endDate.HasValue)
            {
                var endDateUtc = endDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.CreatedAt <= endDateUtc);
            }

            filteredNodes = sortDescending
                            ? filteredNodes.OrderByDescending(n => n.CreatedAt)
                            : filteredNodes.OrderBy(n => n.CreatedAt);

            var result = filteredNodes.ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(result);
        }

        [HttpGet("searchByNodeName")]
        async public Task SearchByNodeName(string? name, bool sortDescending = false) //Поиск узла по названию с сортировкой
        {
            var searchedNodes = _dbContext.Nodes.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                if (name.StartsWith("\"") && name.EndsWith("\""))
                {
                    var exactName = name.Trim('\"');
                    searchedNodes = searchedNodes.Where(n => n.Name == exactName);
                }
                else
                {
                    searchedNodes = searchedNodes.Where(n => n.Name.Contains(name));
                }
            }

            searchedNodes = sortDescending
                            ? searchedNodes.OrderByDescending(n => n.Name)
                            : searchedNodes.OrderBy(n => n.Name);

            var result = await searchedNodes.ToListAsync();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(result);
        }
    }
}
