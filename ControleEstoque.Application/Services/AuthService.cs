using ControleEstoque.Application.Middlewares;
using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Enums;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControleEstoque.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<string> Login(LoginRequest loginRequest)
        {

            var res = await _usuarioRepository.GetByEmailAsync(loginRequest.Email);
            if (res == null)
            {
                throw new ErrorHandlingMiddleware.NotFoundException($"Usuario não existe");
            }
            //Validação Usuario
            if (res != null)
            {
                
                if (res.Email != loginRequest?.Email && res.Senha == loginRequest?.Senha)
                {
                    throw new ErrorHandlingMiddleware.ValidationException($"Email invalido.");
                }
                if (res.Email == loginRequest?.Email && res.Senha != loginRequest.Senha)
                {
                    throw new ErrorHandlingMiddleware.ValidationException("Senha do usuário inválida.");
                }
            }
            return GeraToken(res);
        }

        private string GeraToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string? apiKey = _configuration["AppSettings:TokenKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("A chave do token não foi configurada.");
            }
            var key = Encoding.ASCII.GetBytes(apiKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim(ClaimTypes.Role, Enum.GetName(typeof(eTipoUsuario), usuario.TipoUsuario)!) //0 - Admministrador, 1 - Vendedor
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}