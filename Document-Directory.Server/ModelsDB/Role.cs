using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string UserRole { get; set; }

        public Role(string UserRole)
        {
            this.UserRole = UserRole;
        }
    }
}
