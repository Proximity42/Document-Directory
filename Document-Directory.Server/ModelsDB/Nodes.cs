using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Directory.Server.ModelsDB
{
    public class Nodes
    {
        [Key]
        
        public int Id {  get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ActivityEnd { get; set; }

        public Nodes(string Type, string Name, string Content, DateTime CreatedAt, DateTime ActivityEnd)
        {
            this.Type = Type;
            this.Name = Name;
            this.Content = Content;
            this.CreatedAt = CreatedAt;
            this.ActivityEnd = ActivityEnd;
        }
    }
}
