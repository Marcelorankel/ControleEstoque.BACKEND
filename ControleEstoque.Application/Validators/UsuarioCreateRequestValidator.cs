using ControleEstoque.Application.Services;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Application.Validators
{
    public class UsuarioCreateRequestValidator : AbstractValidator<UsuarioCreateRequest>
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioCreateRequestValidator(IUsuarioService usuarioService)
        {
            //_usuarioService = usuarioService;

            //RuleFor(u => u.Email)
            //    .NotEmpty().WithMessage("Email é obrigatório")
            //    .EmailAddress().WithMessage("Email inválido")
            //    .MustAsync(async (email, cancellationToken) =>
            //    {
            //        var usuarioExistente = await _usuarioService.GetB(email);
            //        return usuarioExistente == null;
            //    }).WithMessage("Usuário já existe");

            //RuleFor(u => u.Nome)
            //    .NotEmpty().WithMessage("Nome é obrigatório")
            //    .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");
        }
    }
}
