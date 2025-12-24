namespace Backend.Models
{
    public class Admin
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public Admin(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
