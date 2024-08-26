using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.EntityFrameworkCore;

namespace Document_Directory.Server.Function
{
    public class NodeFunctions
    {
        public static List<Nodes> AllNodes(AppDBContext _dbContext) //Вывод всех документов с не истекшим сроком действия и папок
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            List<Nodes> nodes = new List<Nodes>(_dbContext.Nodes.OrderBy(n => n.Type).Where(n => n.ActivityEnd == null || n.ActivityEnd > utcNow));
            return nodes;
        }

        public static List<Nodes> AllNodeAccess(int? idUser, List<int?> idGroups, AppDBContext _dbContext) //Получение всех доступных узлов пользователю
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            List<NodeAccess> nodeAccesses = (from Node in _dbContext.NodeAccess where idGroups.Contains(Node.GroupId) || (Node.UserId == idUser) select Node).ToList();
            List<Nodes> nodes = new List<Nodes>();
            foreach (var nodeAccess in nodeAccesses)
            {
                Nodes node = _dbContext.Nodes.FirstOrDefault(n => n.Id == nodeAccess.NodeId && (n.ActivityEnd == null || n.ActivityEnd > utcNow));
                if (node != null) nodes.Add(node);
                //nodes.Add(node);
            }

            return nodes;
        }

        public static List<Nodes> NodeAccessFolder(List<Nodes> nodesInFolder, List<Nodes> allAccessNode) //Получение доступных узлов во вложенной папке
        {
            List<Nodes> inNodesTemp = new List<Nodes>(nodesInFolder);
            foreach (var node in nodesInFolder)
            {
                if (allAccessNode.Contains(node)) { continue; }
                else { inNodesTemp.Remove(node); }
            }
            return inNodesTemp;
        }

        public static void DeleteFolderRecursively(int folderId, AppDBContext _dbContext) // Рекурсивное удаление папок и их содержимых 
        {
            var childNodes = _dbContext.NodeHierarchy.Where(x => x.FolderId == folderId).ToList();

            foreach (var childNode in childNodes)
            {
                var node = _dbContext.Nodes.Find(childNode.NodeId);
                if (node.Type == "Document")
                {
                    _dbContext.NodeHierarchy.Remove(childNode);
                    _dbContext.Nodes.Remove(node);
                }
                else
                {
                    DeleteFolderRecursively(node.Id, _dbContext);
                }
            }

            var folderToDelete = _dbContext.Nodes.Find(folderId);
            _dbContext.Nodes.Remove(folderToDelete);
        }

        public static IQueryable<Nodes> SortBy(IQueryable<Nodes> sortedNodes, string sortBy = "Name", bool sortDescending = false) //Сортировка узлов
        {
            if (sortBy == "ActivityDate")
            {
                sortedNodes = sortDescending
                        ? sortedNodes.OrderByDescending(n => n.ActivityEnd)
                        : sortedNodes.OrderBy(n => n.ActivityEnd);
            }
            else if (sortBy == "CreatedDate")
            {
                sortedNodes = sortDescending
                        ? sortedNodes.OrderByDescending(n => n.CreatedAt)
                        : sortedNodes.OrderBy(n => n.CreatedAt);
            }
            else
            {
                sortedNodes = sortDescending
                        ? sortedNodes.OrderByDescending(n => n.Name)
                        : sortedNodes.OrderBy(n => n.Name);
            }
            return sortedNodes;
        }
    }
}
