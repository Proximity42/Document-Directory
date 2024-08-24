using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Document_Directory.Server.Function;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Linq;

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

        [Authorize]
        [HttpPost]
        async public Task Create(DocumentToCreate document) //Создание документа и помещение его во вложенную папку по ее id
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            DateTimeOffset timestampWithTimezone = new DateTimeOffset(DateTime.UtcNow, TimeSpan.FromHours(0));
            Nodes documents = new Nodes(userId, "Document", document.Name, document.Content, timestampWithTimezone, document.ActivityEnd); ; 

            _dbContext.Nodes.Add(documents);
            _dbContext.SaveChanges();

            int nodeId = documents.Id;
            if (document.folderId != 0)
            {
                NodeHierarchy hierarchy = new NodeHierarchy(document.folderId, nodeId);
                _dbContext.NodeHierarchy.Add(hierarchy);
            }

            NodeAccess nodeAccess = new NodeAccess(nodeId, null, userId);
            _dbContext.NodeAccess.Add(nodeAccess);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 201;
            await response.WriteAsJsonAsync(documents);
        }

        [Authorize]
        [HttpGet("checkaccessedit")]
        async public Task CheckAccessEdit(int id)
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value); ;
            Nodes node = _dbContext.Nodes.Find(id);
            Users user = _dbContext.Users.Find(userId);

            var response = this.Response;
            if (node.UserId == userId || UserFunctions.GetRoleUser(user.Id, _dbContext) == "Администратор")
            {
                response.StatusCode = 200;
                await response.WriteAsJsonAsync(true);
            }
            else
            {
                response.StatusCode = 401;
                await response.WriteAsJsonAsync(false);
            }

        }

        [Authorize]
        [HttpPatch("{id}")]
        async public Task Update(int id, DocumentToUpdate document) //Обновление информации о документе
        {
            var DocumentsToUpdate = _dbContext.Nodes.FirstOrDefault(x => x.Id == id);
            DocumentsToUpdate.Name = document.Name;
            DocumentsToUpdate.Content = document.Content;
            DocumentsToUpdate.ActivityEnd = document.ActivityEnd;

            _dbContext.Nodes.Update(DocumentsToUpdate);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(DocumentsToUpdate);
        }

        [Authorize]
        [HttpDelete("{id}")]
        async public Task Delete(int id) //Удаление узла по его Id
        {
            Nodes documentsToDelete = _dbContext.Nodes.Where(n => n.Type == "Document").FirstOrDefault(n => n.Id == id);
            
            _dbContext.Nodes.Remove(documentsToDelete);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(id);
        }
        
        [HttpGet]
        [Route("all")]
        async public Task GetAll() //Получение списка всех существующих документов
        {
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Nodes.OrderBy(n => n.Type).Where(n => n.Type == "Document"));
        }

        [Authorize]
        [HttpGet("access")]
        async public Task GetAccessAll() //Получение списка всех доступных документов пользователю по его Id
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            (List<Groups> groupsUser, List<int?> idGroups) = UserFunctions.UserGroups(userId, _dbContext);
            List<Nodes> documents = NodeFunctions.AllNodeAccess(userId, idGroups, _dbContext);
            

            var response = this.Response;
            response.StatusCode=200;
            await response.WriteAsJsonAsync(documents.OrderBy(n=>n.Type).Where(n => n.Type == "Document"));
        }

        [HttpGet("{id}")]
        async public Task Get(int? id) //Получение информации о документе по его Id
        {
            var response = this.Response;
            var document = _dbContext.Nodes.Where(n => n.Type == "Document").FirstOrDefault(n => n.Id == id);
            await response.WriteAsJsonAsync(document);
        }

        [Authorize]
        [HttpGet("nodes-author")]
        async public Task GetNodesAuthor() //Получение всех документов, автором которых является текущий пользователь
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);
            var nodes = (from Node in _dbContext.Nodes where (Node.UserId == userId && Node.Type == "Document") select Node).ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(nodes);
        }

        [Authorize]
        [HttpPost("filterBy")]
        async public Task FilterBy(FiltersParameters filtersParameters) //Фильтрация документов по дате активности, дате создания или по имени с сортировкой
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            (List<Groups> groupsUser, List<int?> idGroups) = UserFunctions.UserGroups(userId, _dbContext);
            List<Nodes> documents = NodeFunctions.AllNodeAccess(userId, idGroups, _dbContext);

            var filteredNodes = documents.Where(n => n.Type == "Document").AsQueryable();

            if (filtersParameters.startDate.HasValue)
            {
                var startDateUtc = filtersParameters.startDate.Value.UtcDateTime;
                if (filtersParameters.filterBy == "ActivityDate")
                {
                    filteredNodes = filteredNodes.Where(n => n.ActivityEnd >= startDateUtc);
                }
                else
                {
                    filteredNodes = filteredNodes.Where(n => n.CreatedAt >= startDateUtc);
                }
            }

            if (filtersParameters.endDate.HasValue)
            {
                var endDateUtc = filtersParameters.endDate.Value.UtcDateTime;
                if (filtersParameters.filterBy == "ActivityDate")
                {
                    filteredNodes = filteredNodes.Where(n => n.ActivityEnd <= endDateUtc);
                }
                else
                {
                    filteredNodes = filteredNodes.Where(n => n.CreatedAt <= endDateUtc);
                }
            }
            
            if (!string.IsNullOrEmpty(filtersParameters.name))
            {
                var lowerName = filtersParameters.name.ToLower();

                if (filtersParameters.name.StartsWith("\"") && filtersParameters.name.EndsWith("\""))
                {
                    var exactName = lowerName.Trim('\"');
                    filteredNodes = filteredNodes.Where(n => n.Name.ToLower() == exactName);
                }
                else
                {
                    filteredNodes = filteredNodes.Where(n => n.Name.ToLower().Contains(lowerName));
                }
            }

            filteredNodes = NodeFunctions.SortBy(filteredNodes, filtersParameters.sortBy, filtersParameters.sortDescending);

            var result = filteredNodes.ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(result);
        }
    }
}
