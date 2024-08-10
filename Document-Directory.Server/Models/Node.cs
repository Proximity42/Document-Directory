namespace Document_Directory.Server.Models
{
    public class Node
    {
        public int Id {  get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ActivityEnd { get; set; }
    }
}
