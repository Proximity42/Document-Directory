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
                _dbContext.SaveChanges();
            }

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

        [HttpPatch]
        async public Task Update(DocumentToUpdate document) //Обновление информации о документе
        {
            var DocumentsToUpdate = _dbContext.Nodes.FirstOrDefault(x => x.Id == document.Id);
            DocumentsToUpdate.Name = document.Name;
            DocumentsToUpdate.Content = document.Content;
            DocumentsToUpdate.ActivityEnd = document.ActivityEnd;

            _dbContext.Nodes.Update(DocumentsToUpdate);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(DocumentsToUpdate);
        }

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

        [HttpGet("filterByActivityDate")]
        async public Task FilterByActivityDate(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sortDescending = false) //Фильтрация документов по дате активности с сортировкой
        {
            var filteredNodes = _dbContext.Nodes.AsQueryable();

            if (startDate.HasValue)
            {
                var startDateUtc = startDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.ActivityEnd >= startDateUtc && n.Type == "Document");
                //filteredNodes = filteredNodes.Where(n => n.CreatedAt >= startDateUtc);
            }

            if (endDate.HasValue)
            {
                var endDateUtc = endDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.ActivityEnd <= endDateUtc && n.Type == "Document");
                //filteredNodes = filteredNodes.Where(n => n.CreatedAt <= endDateUtc);
            }

            filteredNodes = sortDescending 
                            ? filteredNodes.OrderByDescending(n => n.ActivityEnd)
                            : filteredNodes.OrderBy(n => n.ActivityEnd);

            //filteredNodes = sortDescending
            //                ? filteredNodes.OrderByDescending(n => n.CreatedAt)
            //                : filteredNodes.OrderBy(n => n.CreatedAt);

            var result = filteredNodes.ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(result);
        }

        [HttpGet("filterByCreateDate")]
        async public Task FilterByCreateDate(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sortDescending = false) //Фильтрация документов по дате создания с сортировкой
        {
            var filteredNodes = _dbContext.Nodes.AsQueryable();

            if (startDate.HasValue)
            {
                var startDateUtc = startDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.CreatedAt >= startDateUtc && n.Type == "Document");
            }

            if (endDate.HasValue)
            {
                var endDateUtc = endDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.CreatedAt <= endDateUtc && n.Type == "Document");
            }

            filteredNodes = sortDescending
                            ? filteredNodes.OrderByDescending(n => n.CreatedAt)
                            : filteredNodes.OrderBy(n => n.CreatedAt);

            var result = filteredNodes.ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(result);
        }

        [HttpGet("search")]
        async public Task SearchByNodeName(string? name, bool sortDescending = false) //Поиск документа по названию с сортировкой
        {
            var searchedNodes = _dbContext.Nodes.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                if (name.StartsWith("\"") && name.EndsWith("\""))
                {
                    var exactName = name.Trim('\"');
                    searchedNodes = searchedNodes.Where(n => n.Name == exactName && n.Type == "Document");
                }
                else
                {
                    searchedNodes = searchedNodes.Where(n => n.Name.Contains(name) && n.Type == "Document");
                }
            }

            searchedNodes = sortDescending
                            ? searchedNodes.OrderByDescending(n => n.Name)
                            : searchedNodes.OrderBy(n => n.Name);

            var result = searchedNodes.ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(result);
        }
    }
}
