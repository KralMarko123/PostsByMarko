using Microsoft.IdentityModel.Tokens;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Repos.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PostsByMarko.Host.Helper
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IUsersRepository usersRepository;
        private readonly IConfiguration configuration;

        public JwtHelper(IUsersRepository usersRepository, IConfiguration configuration)
        {
            this.usersRepository = usersRepository;
            this.configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(User user)
        {
            var userClaims = await usersRepository.GetClaimsAsync(user);

            return GenerateToken(userClaims);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = configuration.GetSection("JwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtConfig["secret"]!);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private string GenerateToken(List<Claim> claims)
        {
            var jwtConfig = configuration.GetSection("JwtConfig");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtConfig["expiresIn"])),
                Issuer = jwtConfig.GetSection("validIssuers")!.Get<List<string>>()!.FirstOrDefault(),
                Audience = jwtConfig.GetSection("validAudiences")!.Get<List<string>>()!.FirstOrDefault(),
                SigningCredentials = GetSigningCredentials()

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenAsString = tokenHandler.WriteToken(token);

            return tokenAsString;
        }
    }
}
