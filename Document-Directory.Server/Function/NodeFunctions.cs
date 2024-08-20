using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;

namespace Document_Directory.Server.Function
{
    public class NodeFunctions
    {
        public static List<Nodes> AllNodeAccess(int idUser, List<int?> idGroups, AppDBContext _dbContext) //Получение всех доступных узлов пользователю
        {
            List<NodeAccess> nodeAccesses = (from Node in _dbContext.NodeAccess where idGroups.Contains(Node.GroupId) || (Node.UserId == idUser) select Node).ToList();
            List<Nodes> nodes = new List<Nodes>();
            foreach (var nodeAccess in nodeAccesses)
            {
                Nodes node = _dbContext.Nodes.FirstOrDefault(n => n.Id == nodeAccess.NodeId);
                nodes.Add(node);
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
    }
}
