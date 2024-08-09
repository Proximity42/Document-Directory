using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class Groups
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public Groups(string Name)
        {
            this.Name = Name;
        }
    }
}
