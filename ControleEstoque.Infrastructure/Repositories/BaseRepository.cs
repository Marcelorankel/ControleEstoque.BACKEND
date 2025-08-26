using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ControleEstoqueDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(ControleEstoqueDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Cria a entidade e retorna o valor da chave gerada
        /// </summary>
        public async Task<object> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Pega a chave primária da entidade dinamicamente
            var keyName = _context.Model
                .FindEntityType(typeof(T))
                .FindPrimaryKey()
                .Properties
                .Select(p => p.Name)
                .FirstOrDefault();

            if (keyName == null)
                throw new Exception("Não foi possível determinar a chave primária da entidade.");

            var keyValue = entity.GetType().GetProperty(keyName)?.GetValue(entity);
            return keyValue!;
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}