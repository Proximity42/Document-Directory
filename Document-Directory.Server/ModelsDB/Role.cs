using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class Role
    {
        [Key]
        public int id { get; set; }
        public string role { get; set; }

        public Role(string role)
        {
            this.role = role;
        }
    }
}
