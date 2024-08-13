using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class NodeAccess
    {
        [Key]
        public int Id {  get; set; }
        
        public int NodeId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }

        public Nodes Node { get; set; }
        public Groups? Group { get; set; }
        public Users? User { get; set; }
    }
}
