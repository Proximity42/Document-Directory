using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class UserGroups
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId {  get; set; }
        public int GroupId { get; set; }

        public Groups Group { get; set; }
        public Users User { get; set; }

        public UserGroups() { }

        public UserGroups(int GroupId, int UserId) 
        {
            this.GroupId = GroupId;
            this.UserId = UserId;
        }
    }
}
