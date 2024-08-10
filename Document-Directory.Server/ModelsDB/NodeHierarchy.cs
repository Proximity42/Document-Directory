using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class NodeHierarchy
    {
        [Key]
        public int Id { get; set; }

        // public int Folderid { get; set; }
        // public int NodeId { get; set; }

        public Nodes Folder { get; set; }
        public Nodes Node { get; set; }
    }
}
