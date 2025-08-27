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

        public async Task<object> CreateAsync(T entity)
        {
            try
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
            catch (Exception ex)
            {

                throw new Exception($"{ex.InnerException?.Message}");
            }
        }
        public async Task UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception($"{ex.InnerException?.Message}");
            }
        }

        public async Task DeleteAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception($"{ex.InnerException?.Message}");
            }
        }
    }
}