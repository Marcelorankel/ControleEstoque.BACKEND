using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Enums;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using ControleEstoque.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ControleEstoque.Application.Middlewares.ErrorHandlingMiddleware;

namespace ControleEstoque.Application.Services
{
    public class UsuarioService : BaseService<Usuario>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
            : base(usuarioRepository)  // passa pro BaseService
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _usuarioRepository.GetByEmailAsync(email);
        }

        public async Task NovoUsuarioAsync(UsuarioRequest request)
        {
            //Usuario(Email) já cadastrado
            var res = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (res != null)
                throw new ValidationException($"Email {request.Email} já cadastrado no sistema.");
            //Valida nome
            if (request.Nome == string.Empty)
                throw new ValidationException($"Nome não informado");
            //Valida email
            if (request.Email == string.Empty)
                throw new ValidationException($"Email não informado");
            //Valida senha
            if (request.Senha == string.Empty)
                throw new ValidationException($"Senha não informada");
            //Valida tamanho senha
            if (request.Senha.Length < 6)
                throw new ValidationException($"Senha deve conter no minimo 6 caracteres");

            var obj = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Email = request.Email,
                TipoUsuario = HelperUtil.ObterValorNumericoEnum<eTipoUsuario>(request.TipoUsuario.ToString()),
                Senha = request.Senha,
                CreatedAt = DateTime.Now
            };

            await _usuarioRepository.CreateAsync(obj);
        }
    }
}