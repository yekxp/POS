namespace JwtManagerHandler.Models
{
    public class AuthenticationResponse
    {
        public required string UserName { get; set; }

        public required string JwtToken { get; set; }

        public required int ExpiresIn { get; set; }
    }
}
