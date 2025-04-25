using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.DTOs;
using WebApplication3.Models;

namespace WebApplication3.Services
{
    // constructor inicializa el programa
    
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly IConfiguration _config;

        public AuthService(UserManager<IdentityUser> userMgr, IConfiguration config)
        {
            _userMgr = userMgr;
            _config = config;
        }

        // registra el usuario 

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userMgr.CreateAsync(user, dto.Contraseña);

            if (!result.Succeeded)

                if (!result.Succeeded)
                {
                    var errores = string.Join(", ", result.Errors.Select(e => e.Description)); // email o contraseña
                    throw new ApplicationException($"Registro fallido: {errores}"); 
                }

            // asigna un rol

            await _userMgr.AddToRoleAsync(user, dto.Rol);

            return await BuildAuthResponse(user);
        }


        // devuelve el resultado si esta bueno o no
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userMgr.FindByEmailAsync(dto.Email);
            if (user == null || !await _userMgr.CheckPasswordAsync(user, dto.Contraseña))
                throw new ApplicationException("Credenciales inválidas.");

            return await BuildAuthResponse(user);
        }



        // JW
        private async Task<AuthResponseDto> BuildAuthResponse(IdentityUser user)
        {
            var roles = await _userMgr.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, roles[0])
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); // Crea la clave de sguridad
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);    // crea credenciales 
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"])); // tiempo de expiración conf



            // creación del toquen para enviarlo la respuesta al cliente

            var token = new JwtSecurityToken(
                                _config["Jwt:Issuer"],
                                _config["Jwt:Audience"],
                                claims,
                                expires: expires,
                                signingCredentials: creds);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expires.ToString("O"),
                Email = user.Email,
                Nombre = user.UserName,
                Rol = roles[0]
            };
        }
    }
}
