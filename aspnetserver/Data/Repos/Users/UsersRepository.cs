using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aspnetserver.Data.Repos.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private User? user;

        public UsersRepository(UserManager<User> userManager, IMapper mapper, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public async Task<object> GetUserDetailsForUsernameAsync(string userName)
        {
            user = await userManager.FindByNameAsync(userName);
            return new { user.UserName, user.Email, user.FirstName, user.LastName };
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration)
        {
            var user = mapper.Map<User>(userRegistration);
            var result = await userManager.CreateAsync(user, userRegistration.Password);

            return result;
        }

        public async Task<bool> ValidateUserAsync(UserLoginDto userLogin)
        {
            user = await userManager.FindByNameAsync(userLogin.UserName);
            var result = user != null && await userManager.CheckPasswordAsync(user, userLogin.Password);

            return result;
        }

        public async Task<string> CreateTokenAsync()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaimsAsync();
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

        private async Task<List<Claim>> GetClaimsAsync()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
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
