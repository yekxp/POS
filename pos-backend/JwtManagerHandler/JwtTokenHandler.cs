using JwtManagerHandler.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtManagerHandler
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURE_KEY = "3zoeI4ENA4nhDP9s5oklAE5KTNppzsGI";
        private const int JWT_TOKEN_VALIDITY_MINS = 60;

        public JwtTokenHandler()
        {
            
        }

        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrWhiteSpace(authenticationRequest.Password) || string.IsNullOrWhiteSpace(authenticationRequest.Username))
            {
                return null;
            }

            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURE_KEY);

            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Name, authenticationRequest.Username),
                    new Claim(ClaimTypes.Role, authenticationRequest.Role.ToString()),
                }
            );

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature
            );

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {   JwtToken = token,
                UserName = authenticationRequest.Username,
                ExpiresIn = (int) tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds
            };

        }

        public string? GetRoleFromJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return null;
            }

            var t = jwtToken.Claims.FirstOrDefault();
            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role");
            return roleClaim?.Value;
        }
    }
}
