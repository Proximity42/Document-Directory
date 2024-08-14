using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Document_Directory.Server.Function
{
    public static class Functions
    {
        public static (List<Groups>, List<int>) UserGroups(int idUser, AppDBContext _dbContext)
        {
            List<UserGroups> usergroups = (from User in _dbContext.UserGroups where User.UserId == idUser select User).ToList();
            List<Groups> groups = new List<Groups>();
            foreach (var group in usergroups)
            {
                groups.Add(_dbContext.Groups.FirstOrDefault(g => g.Id == group.GroupId));
            }
            List<int> idGroups = new List<int>();
            foreach (var group in groups) { idGroups.Add(group.Id); }

            return (groups, idGroups);
        }

        public static List<Nodes> AllNodeAccess(int idUser, List<int> idGroups, AppDBContext _dbContext)
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
        public static List<Nodes> NodeAccessFolder(List<Nodes> nodesInFolder, List<Nodes> allAccessNode)
        {
            List<Nodes> inNodesTemp = new List<Nodes>(nodesInFolder);
            foreach (var node in nodesInFolder)
            {
                if (allAccessNode.Contains(node)) { continue; }
                else { inNodesTemp.Remove(node); }
            }
            return inNodesTemp;
        }
    }
}
