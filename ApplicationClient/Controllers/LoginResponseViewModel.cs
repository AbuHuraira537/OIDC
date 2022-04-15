namespace ApplicationClient.Controllers
{
    public class LoginResponseViewModel
    {
        public LoginResponseViewModel()
        {
        }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
    }
}