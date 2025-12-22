namespace Backend.Model
{
    public class User
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public float Wallet { get; set; }

        public User(string userName, string password, float wallet = 0f)
        {
            UserName = userName;
            Password = password;
            Wallet = wallet;
        }
    }
}
