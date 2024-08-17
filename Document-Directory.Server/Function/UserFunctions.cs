using Document_Directory.Server.ModelsDB;

namespace Document_Directory.Server.Function
{
    public class UserFunctions
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
        public static string GetRoleUser(int idRole, AppDBContext _dbContext) //Получение роли по ее Id
        {
            return (_dbContext.Role.Find(idRole).UserRole);
        }
    }
}
