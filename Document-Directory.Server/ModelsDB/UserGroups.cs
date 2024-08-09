using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class UserGroups
    {
        [Key]
        public int id { get; set; }
        public int userId {  get; set; }
        public int groupId { get; set; }
    }
}
