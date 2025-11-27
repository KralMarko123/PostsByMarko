using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PostsByMarko.Host.Application.Helper
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IUserRepository usersRepository;
        private readonly JwtConfig jwtConfig;

        public JwtHelper(IUserRepository usersRepository, IOptions<JwtConfig> jwtConfig)
        {
            this.usersRepository = usersRepository;
            this.jwtConfig = jwtConfig.Value;
        }

        public async Task<string> CreateTokenAsync(User user)
        {
            var userClaims = await usersRepository.GetClaimsAsync(user);

            return GenerateToken(userClaims);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private string GenerateToken(List<Claim> claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtConfig.ExpiresIn)),
                Issuer = jwtConfig.ValidIssuers.FirstOrDefault(),
                Audience = jwtConfig.ValidAudiences.FirstOrDefault(),
                SigningCredentials = GetSigningCredentials()

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenAsString = tokenHandler.WriteToken(token);

            return tokenAsString;
        }
    }
}
