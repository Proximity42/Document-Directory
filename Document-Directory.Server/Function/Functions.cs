using Document_Directory.Server.ModelsDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Document_Directory.Server.Function
{
    public static class Functions
    {
        public static List<Groups> UserGroups(int idUser, AppDBContext _dbContext)
        {
            List<UserGroups> usergroups = (from User in _dbContext.UserGroups where User.UserId == idUser select User).ToList();
            List<Groups> groups = new List<Groups>();
            foreach (var group in usergroups)
            {
                groups.Add(_dbContext.Groups.FirstOrDefault(g => g.Id == group.GroupId));
            }

            return groups;
        }

        async public static Task ReturnRespons(int statusCode, StreamContent content, Controller controller)
        {
            var response = controller.Response;
            response.StatusCode = statusCode;
            await response.WriteAsJsonAsync(content);
        }
    }
}
