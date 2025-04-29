using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MR_power.Repositories.Interfaces;
using MR_power.Services.Interfaces;

namespace MR_power.Services
{
    public abstract class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;

        protected BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task DeleteAsync(T entity)
        {
            await _repository.DeleteAsync(entity);
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
    }
} 