namespace Document_Directory.Server.Models
{
    public class UserToCreate
    {
        public int RoleId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        
    }
    public class UserToChangeRole
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
    }

    public class UserToChangePassword
    {
        public int Id { get; set; }
        public string Password { get; set; }
    }

    public class UserToGet
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Login { get; set; }
    }

    public class UserToToken
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class Password
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
