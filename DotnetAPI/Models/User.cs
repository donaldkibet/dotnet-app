namespace DotnetAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string userName { get; set; }
        public byte[] Passwordhash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}