namespace Document_Directory.Server.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] Participants  { get; set; }
    }
}
