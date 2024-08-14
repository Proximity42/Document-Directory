namespace Document_Directory.Server.Models
{
    public class NodeToCreate
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset ActivityEnd { get; set; }
        public int folderId { get; set; }
    }

    public class NodeToUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset ActivityEnd { get; set; }
    }
}
