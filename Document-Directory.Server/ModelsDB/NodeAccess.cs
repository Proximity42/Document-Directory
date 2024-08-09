using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class NodeAccess
    {
        [Key]
        public int id {  get; set; }
        public int nodeId { get; set; }
        public int groupId { get; set; }
        public int userId { get; set; }
    }
}
