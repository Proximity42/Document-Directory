using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Directory.Server.ModelsDB
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        
        public string Login { get; set; }
        public string Password { get; set; }
        public int roleId { get; set; }
        public Role role { get; set; }
        public Users(string Login, string Password)
        {
            this.Login = Login;
            this.Password = Password;
        }
    }
}
