namespace Document_Directory.Server.Models
{
    public class UserToCreate
    {
        public int RoleId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        
    }
    public class UserToUpdate
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Password { get; set; }
    }
    public class UserToToken
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
