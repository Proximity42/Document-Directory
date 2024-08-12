namespace Document_Directory.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
