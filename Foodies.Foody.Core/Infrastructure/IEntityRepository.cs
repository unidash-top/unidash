﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Foodies.Foody.Core.Domain;

namespace Foodies.Foody.Core.Infrastructure
{
    public interface IEntityRepository<T> where T : Entity
    {
        Task<T> AddAsync(T entity);

        Task<T> FindByIdAsync(string entityId);

        Task RemoveAsync(T entity);

        Task<T> GetOrCreateAsync(string id, T entity);

        Task<IEnumerable<T>> FindAllAsync();

        Task<IEnumerable<T>> FindAllByAsync(Expression<Func<T, bool>> match);
    }
}