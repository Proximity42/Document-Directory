using Document_Directory.Server.Authorization;
using Document_Directory.Server.Models;
using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Document_Directory.Server.Function
{
    public static class Functions
    {
        public static (List<Groups>, List<int>) UserGroups(int idUser, AppDBContext _dbContext) //Получение списка групп, в которых состоит пользователь
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

        public static List<Nodes> AllNodeAccess(int idUser, List<int> idGroups, AppDBContext _dbContext) //Получение всех доступных узлов пользователю
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

        public static Nodes FolderDocumentCheck(NodeToCreate node) //Проверка типа узла (папка/документ)
        {
            DateTimeOffset timestampWithTimezone = new DateTimeOffset(DateTime.UtcNow, TimeSpan.FromHours(0));
            Nodes nodes;

            if (node.Type == "Document")
            {
                nodes = new Nodes(node.Type, node.Name, node.Content, timestampWithTimezone, node.ActivityEnd);
            }
            else
            {
                nodes = new Nodes(node.Type, node.Name, timestampWithTimezone);
            }

            return nodes;
        }

        //Генерация токена
        public static (string, ClaimsIdentity) GenerationToken(UserToToken user, AppDBContext _dbContext)
        {
            var claimsIdentity = GetIdentity(user, _dbContext);
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claimsIdentity.Claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)), // время действия 2 минуты
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return (new JwtSecurityTokenHandler().WriteToken(jwt), claimsIdentity);
        }

        static private ClaimsIdentity GetIdentity(UserToToken user, AppDBContext _dbContext)
        {
            Users users = _dbContext.Users.FirstOrDefault(x => x.Login == user.Login && x.Password == user.Password);
            if (users != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, users.Login),
                    new Claim("Id", users.Id.ToString()),
                    //new Claim(ClaimsIdentity.DefaultRoleClaimType, _dbContext.Role.FirstOrDefault(r => r.Id == users.roleId).UserRole)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
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
