using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class NodeHierarchy
    {
        [Key]
        public int id { get; set; }
        public int folderId { get; set; }
        public int nodeId { get; set; }
    }
}
