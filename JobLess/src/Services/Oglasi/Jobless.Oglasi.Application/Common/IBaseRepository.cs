using JobLess.Shared.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace JobLess.Oglasi.Application.Common
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<TEntity> GetByIdOrThrowAsync(int id, CancellationToken cancellationToken);
        Task<TEntity[]> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken);
        Task<TEntity?> SingleByConditionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken);
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> StartQuery();
    }

}
