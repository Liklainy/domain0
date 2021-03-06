﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain0.Repository
{
    public interface IRepository<TKey, TEntity> 
        where TEntity : new()
    {
        Task<decimal> Insert(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(TKey id);

        Task<TEntity> FindById(TKey id);

        Task<TEntity[]> FindByIds(IEnumerable<TKey> ids);
    }
}