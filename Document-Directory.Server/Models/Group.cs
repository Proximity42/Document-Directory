namespace Document_Directory.Server.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] Participants  { get; set; }
    }

    public class GroupToUpdate
    {
        public int groupId { get; set; }
        public List<int> usersId { get; set; }
    }
}
