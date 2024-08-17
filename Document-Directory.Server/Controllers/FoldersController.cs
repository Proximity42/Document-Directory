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
            DateTimeOffset timestampWithTimezone = new DateTimeOffset(DateTime.UtcNow, TimeSpan.FromHours(0));
            Nodes folders = new Nodes("Folder", folder.Name, timestampWithTimezone); ;


            //int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            /*var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            string userLogin = user.Login;*/

            //Nodes folders = NodeFunctions.FolderDocumentCheck(folder, userId);

            _dbContext.Nodes.Add(folders);
            _dbContext.SaveChanges();

            int nodeId = folders.Id;
            if (folder.folderId != 0)
            {
                NodeHierarchy hierarchy = new NodeHierarchy(folder.folderId, nodeId);
                _dbContext.NodeHierarchy.Add(hierarchy);
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
            Nodes fodlersToDelete = _dbContext.Nodes.Where(n => n.Type == "Folder").FirstOrDefault(n => n.Id == id);

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
            await response.WriteAsJsonAsync(_dbContext.Nodes.OrderBy(n => n.Type).Where(n => n.Type == "Folder"));
        }

        [Authorize]
        [HttpGet("access")]
        async public Task GetAccessAll() //Получение списка всех доступных папок пользователю по его Id
        {
            int userId = Convert.ToInt32(this.HttpContext.User.FindFirst("Id").Value);

            (List<Groups> groupsUser, List<int> idGroups) = UserFunctions.UserGroups(userId, _dbContext);
            List<Nodes> folders = NodeFunctions.AllNodeAccess(userId, idGroups, _dbContext);


            var response = this.Response;
            response.StatusCode = 200;
            await response.WriteAsJsonAsync(folders.OrderBy(n => n.Type).Where(n => n.Type == "Folder"));
        }

        [HttpGet("{id}")]
        async public Task Get(int? id) //Получение информации о папке по его Id
        {
            var response = this.Response;
            var folder = _dbContext.Nodes.Where(n => n.Type == "Folder").FirstOrDefault(n => n.Id == id);
            await response.WriteAsJsonAsync(folder);
        }

        [HttpGet("filterByActivityDate")]
        async public Task FilterByActivityDate(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sortDescending = false) //Фильтрация документов по дате активности с сортировкой
        {
            var filteredNodes = _dbContext.Nodes.AsQueryable();

            if (startDate.HasValue)
            {
                var startDateUtc = startDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.ActivityEnd >= startDateUtc && n.Type == "Folder");
            }

            if (endDate.HasValue)
            {
                var endDateUtc = endDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.ActivityEnd <= endDateUtc && n.Type == "Folder");
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
        async public Task FilterByCreateDate(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sortDescending = false) //Фильтрация папок по дате создания с сортировкой
        {
            var filteredNodes = _dbContext.Nodes.AsQueryable();

            if (startDate.HasValue)
            {
                var startDateUtc = startDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.CreatedAt >= startDateUtc && n.Type == "Folder");
            }

            if (endDate.HasValue)
            {
                var endDateUtc = endDate.Value.UtcDateTime;
                filteredNodes = filteredNodes.Where(n => n.CreatedAt <= endDateUtc && n.Type == "Folder");
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
        async public Task SearchByNodeName(string? name, bool sortDescending = false) //Поиск папки по названию с сортировкой
        {
            var searchedNodes = _dbContext.Nodes.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                if (name.StartsWith("\"") && name.EndsWith("\""))
                {
                    var exactName = name.Trim('\"');
                    searchedNodes = searchedNodes.Where(n => n.Name == exactName && n.Type == "Folder");
                }
                else
                {
                    searchedNodes = searchedNodes.Where(n => n.Name.Contains(name) && n.Type == "Folder");
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
