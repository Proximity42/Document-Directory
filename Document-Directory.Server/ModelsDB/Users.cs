using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Directory.Server.ModelsDB
{
    public class Users
    {
        [Key]
        public int id { get; set; }
        public int idRole { get; set; }
        public string login { get; set; }
        public string password { get; set; }

        public Users(string login, string password)
        {
            this.login = login;
            this.password = password;
        }
    }
}
