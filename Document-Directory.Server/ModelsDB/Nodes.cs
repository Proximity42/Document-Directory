using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class Nodes
    {
        [Key]
        public int id {  get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string? content { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime activityEnd { get; set; }

        public Nodes(string type, string name, string content, DateTime createdAt, DateTime activityEnd)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.content = content;
            this.createdAt = createdAt;
            this.activityEnd = activityEnd;
        }
    }
}
