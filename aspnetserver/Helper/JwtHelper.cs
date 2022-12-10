using aspnetserver.Data.Models;
using aspnetserver.Data.Repos.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aspnetserver.Helper
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
            var signingCredentials = GetSigningCredentials();
            var claims = await usersRepository.GetClaimsAsync(user);
            var token = GenerateToken(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = configuration.GetSection("JwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtConfig["secret"]);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private JwtSecurityToken GenerateToken(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var environment = configuration["Environment"];
            IConfiguration jwtConfig = null;

            switch (environment)
            {
                case "DEV":
                    jwtConfig = configuration.GetSection("DevJwtConfig");
                    break;
                case "PRD":
                    jwtConfig = configuration.GetSection("JwtConfig");
                    break;
                default:
                    break;
            }

            var token = new JwtSecurityToken
            (
            issuer: jwtConfig.GetSection("validIssuers").Get<List<string>>().FirstOrDefault(),
            audience: jwtConfig.GetSection("validAudiences").Get<List<string>>().FirstOrDefault(),
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtConfig["expiresIn"])),
            signingCredentials: signingCredentials
            );

            return token;
        }
    }
}
