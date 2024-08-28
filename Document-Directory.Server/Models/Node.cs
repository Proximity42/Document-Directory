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
        public string Name { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset ActivityEnd { get; set; }
    }

    public class AccessToCreate
    {
        public int nodeId { get; set; }
        public int? groupId { get; set; }
        public int? userId { get; set; }
    }

    public class AccessToUpdate
    {
        public int nodeId { get; set; }
        public List<int?> groupsId { get; set; }
        public List<int?> usersId { get; set; }
    }

    public class FiltersParameters
    {
        public DateTimeOffset? startDate { get; set; }
        public DateTimeOffset? endDate { get; set; }
        public string? name { get; set; }
        public string filterBy { get; set;}
        public string sortBy { get; set; }
        public bool sortDescending { get; set; } 

        public FiltersParameters (DateTimeOffset? startDate, DateTimeOffset? endDate, string? name, string filterBy = "CreatedDate", string sortBy = "Name", bool sortDescending = false)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.name = name;
            this.filterBy = filterBy;
            this.sortBy = sortBy;
            this.sortDescending = sortDescending;
        }
    }
}
