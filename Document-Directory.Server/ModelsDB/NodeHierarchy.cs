using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class NodeHierarchy
    {
        [Key]
        public int Id { get; set; }

        public int FolderId { get; set; }
        public int NodeId { get; set; }

        public Nodes Folder { get; set; }
        public Nodes Node { get; set; }

        public NodeHierarchy(int FolderId, int NodeId) 
        {
            this.FolderId = FolderId;
            this.NodeId = NodeId;
        }

    }
}
