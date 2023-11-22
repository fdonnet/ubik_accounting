namespace Ubik.Accounting.WebApp.Security
{
    public class UserInfo
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public required string AccessToken {  get; set; }
        public required string RefreshToken {  get; set; }
    }
}
