using ControleEstoque.Application.Services;
using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Enums;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Models;
using ControleEstoque.Infrastructure.Persistence;
using ControleEstoque.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Testes
{
    public class UsuarioServiceTests
    {
        [Fact]
        public async Task CriarUsuario_RealDB_ComRollback()
        {
            var options = new DbContextOptionsBuilder<ControleEstoqueDbContext>()
                .UseMySql(
                    "Server=localhost;Port=3306;Database=controleestoque;User=root;Password=admin",
                    new MySqlServerVersion(new Version(8, 0, 33))
                )
                .Options;

            using var context = new ControleEstoqueDbContext(options);

            var usuarioRepository = new UsuarioRepository(context);
            var service = new UsuarioService(usuarioRepository);

            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var request = new UsuarioRequest
                {
                    Nome = "João Teste Real",
                    Email = "joao.real@teste.com",
                    Senha = "123456",
                    TipoUsuario = eTipoUsuario.ADMIN
                };

                await service.NovoUsuarioAsync(request);

                var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
                Assert.NotNull(usuario);
                Assert.Equal(request.Nome, usuario!.Nome);
                Console.WriteLine("Usuário salvo com sucesso na base real (rollback no final do teste)");

                // Confirma na base real
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}