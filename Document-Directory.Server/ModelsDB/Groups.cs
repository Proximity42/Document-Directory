using System.ComponentModel.DataAnnotations;

namespace Document_Directory.Server.ModelsDB
{
    public class Groups
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }

        public Groups(string name)
        {
            this.name = name;
        }
    }
}
