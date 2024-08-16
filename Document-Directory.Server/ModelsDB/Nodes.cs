using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Directory.Server.ModelsDB
{
    public class Nodes
    {
        [Key]
        
        public int Id {  get; set; }
        public int? UserId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ActivityEnd { get; set; }
        
        public Users? User { get; set; }

        public Nodes(int UserId, string Type, string Name, string? Content, DateTimeOffset CreatedAt, DateTimeOffset? ActivityEnd)
        {
            this.UserId = UserId;
            this.Type = Type;
            this.Name = Name;
            this.Content = Content;
            this.CreatedAt = CreatedAt;
            this.ActivityEnd = ActivityEnd;
        }
        public Nodes(string Type, string Name, string? Content, DateTimeOffset CreatedAt)
        {
            this.Type = Type;
            this.Name = Name;
            this.Content = Content;
            this.CreatedAt = CreatedAt;
            this.ActivityEnd = ActivityEnd;
        }

        public Nodes(string Type, string Name, DateTimeOffset CreatedAt) 
        {
            this.Type = Type;
            this.Name = Name;
            this.CreatedAt = CreatedAt;
        }
    }
}
