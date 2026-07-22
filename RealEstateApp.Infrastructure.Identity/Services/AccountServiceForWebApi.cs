using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Settings;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class AccountServiceForWebApi : BaseAccountService, IAccountServiceForWebApi
    {
        private readonly JwtSettings _jwtSettings;

        public AccountServiceForWebApi(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<JwtSettings> jwtSettings)
            : base(userManager, signInManager)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto, string role)
        {
            return await RegisterUserAsync(dto, role, true, true);
        }

        public async Task<LoginResponseForApiDto> AuthenticateAsync(LoginDto loginDto)
        {
            LoginResponseForApiDto response = new()
            {
                HasError = false,
                Errors = []
            };

            var user = await UserManager.FindByNameAsync(loginDto.UserName)
                ?? await UserManager.FindByEmailAsync(loginDto.UserName);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("Credenciales inválidas, intente de nuevo");
                return response;
            }

            if (IsInactive(user))
            {
                response.HasError = true;
                response.Errors.Add("La cuenta está inactiva");
                return response;
            }

            var result = await SignInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);

            if (!result.Succeeded)
            {
                response.HasError = true;
                if (result.IsLockedOut)
                {
                    response.Errors.Add("La cuenta ha sido bloqueada por múltiples intentos fallidos");
                }
                else
                {
                    response.Errors.Add("Credenciales inválidas, intente de nuevo");
                }
                return response;
            }

            var roles = await UserManager.GetRolesAsync(user);

            if (!roles.Contains(Roles.Admin.ToString()) && !roles.Contains(Roles.Developer.ToString()))
            {
                response.HasError = true;
                response.IsForbidden = true;
                response.Errors.Add("No tiene permisos para acceder al API");
                return response;
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJwtToken(user, roles);

            response.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.UserName = user.UserName ?? "";
            response.Roles = roles.ToList();
            response.Expiration = jwtSecurityToken.ValidTo;

            return response;
        }

        private async Task<JwtSecurityToken> GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var userClaims = await UserManager.GetClaimsAsync(user);

            var rolesClaims = new List<Claim>();
            foreach (var role in roles)
            {
                rolesClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("uid", user.Id)
            }.Union(userClaims).Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }
    }
}
