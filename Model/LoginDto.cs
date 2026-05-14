namespace WebApiNew.Model
{
    public class LoginDto
    {
        public string Role { get; set; }
        public string EmailOrMobile { get; set; }
        public string Password { get; set; }
        public string? Mobile { get; set; }
    }

}
