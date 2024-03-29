﻿using Microsoft.IdentityModel.Tokens;
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
            var signingCredentials = GetSigningCredentials();
            var claims = await usersRepository.GetClaimsAsync(user);
            var token = GenerateToken(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = configuration.GetSection("JwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtConfig["secret"]!);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private JwtSecurityToken GenerateToken(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtConfig = configuration.GetSection("JwtConfig");
            var token = new JwtSecurityToken
            (
            issuer: jwtConfig.GetSection("validIssuers")!.Get<List<string>>()!.FirstOrDefault(),
            audience: jwtConfig.GetSection("validAudiences")!.Get<List<string>>()!.FirstOrDefault(),
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtConfig["expiresIn"])),
            signingCredentials: signingCredentials
            );

            return token;
        }
    }
}
