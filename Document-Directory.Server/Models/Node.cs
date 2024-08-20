using Document_Directory.Server.ModelsDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Document_Directory.Server.Models
{
    public class DocumentToCreate
    {
        public string Name { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset ActivityEnd { get; set; }
        public int folderId { get; set; }
    }

    public class FolderToCreate
    {
        public string Name { get; set; }
        public int folderId { get; set; }
    }

    public class DocumentToUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset ActivityEnd { get; set; }
    }

    public class AccessToUpdate
    {
        public int Id { get; set; }
        public List<int?> groupsId { get; set; }
        public List<int?> usersId { get; set; }
    }
}
