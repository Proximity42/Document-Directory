using Document_Directory.Server.Function;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Document_Directory.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private AppDBContext _dbContext;

        public FoldersController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpPost]
        async public Task Create(FolderToCreate folder) //Создание папки и помещение ее во вложенную папку по ее id
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            DateTimeOffset timestampWithTimezone = new DateTimeOffset(DateTime.UtcNow, TimeSpan.FromHours(0));
            Nodes folders = new Nodes("Directory", folder.Name, timestampWithTimezone); ;

            _dbContext.Nodes.Add(folders);
            _dbContext.SaveChanges();

            int nodeId = folders.Id;
            if (folder.folderId != 0)
            {
                NodeHierarchy hierarchy = new NodeHierarchy(folder.folderId, nodeId);
                _dbContext.NodeHierarchy.Add(hierarchy);
                NodeAccess nodeAccess = new NodeAccess(nodeId, null, userId);
                _dbContext.NodeAccess.Add(nodeAccess);
                _dbContext.SaveChanges();
            }

            var response = this.Response;
            response.StatusCode = 201;
            await response.WriteAsJsonAsync(folders);
        }

        [HttpPatch]
        async public Task Rename(DocumentToUpdate folder) //Обновление информации о папке
        {
            var FoldersToUpdate = _dbContext.Nodes.FirstOrDefault(x => x.Id == folder.Id);

            FoldersToUpdate.Name = folder.Name;

            _dbContext.Nodes.Update(FoldersToUpdate);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(FoldersToUpdate);
        }
        [HttpDelete("{id}")]
        async public Task Delete(int id) //Удаление папки по его Id
        {
            Nodes fodlersToDelete = _dbContext.Nodes.Where(n => n.Type == "Directory").FirstOrDefault(n => n.Id == id);

            NodeFunctions.DeleteFolderRecursively(id, _dbContext);
            _dbContext.SaveChanges();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(id);
        }

        [HttpGet]
        [Route("all")]
        async public Task GetAll() //Получение списка всех существующих папок
        {
            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(_dbContext.Nodes.OrderBy(n => n.Type).Where(n => n.Type == "Directory"));
        }

        [Authorize]
        [HttpGet("access")]
        async public Task GetAccessAll() //Получение списка всех доступных папок пользователю по его Id
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            (List<Groups> groupsUser, List<int?> idGroups) = UserFunctions.UserGroups(userId, _dbContext);
            List<Nodes> folders = NodeFunctions.AllNodeAccess(userId, idGroups, _dbContext);


            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(folders.OrderBy(n => n.Type).Where(n => n.Type == "Directory"));
        }

        [HttpGet("{id}")]
        async public Task Get(int? id) //Получение информации о папке по его Id
        {
            var response = this.Response;
            var folder = _dbContext.Nodes.Where(n => n.Type == "Directory").FirstOrDefault(n => n.Id == id);
            await response.WriteAsJsonAsync(folder);
        }
        
        [Authorize]        
        [HttpGet("filterBy")]
        async public Task FilterBy(DateTimeOffset? startDate, DateTimeOffset? endDate, string? name, string filterBy = "CreatedDate", string sortBy = "Name", bool sortDescending = false) //Фильтрация папок по дате создания или по имени с сортировкой
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            (List<Groups> groupsUser, List<int?> idGroups) = UserFunctions.UserGroups(userId, _dbContext);
            List<Nodes> documents = NodeFunctions.AllNodeAccess(userId, idGroups, _dbContext);

            var filteredNodes = documents.Where(n => n.Type == "Folder").AsQueryable();

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

            if (!string.IsNullOrEmpty(name))
            {
                var lowerName = name.ToLower();

                if (name.StartsWith("\"") && name.EndsWith("\""))
                {
                    var exactName = lowerName.Trim('\"');
                    filteredNodes = filteredNodes.Where(n => n.Name.ToLower() == exactName);
                }
                else
                {
                    filteredNodes = filteredNodes.Where(n => n.Name.ToLower().Contains(lowerName));
                }
            }

            filteredNodes = NodeFunctions.SortBy(filteredNodes, sortBy, sortDescending);

            var result = filteredNodes.ToList();

            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(result);
        }
    }
}
