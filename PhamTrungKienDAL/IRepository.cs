using System.Linq.Expressions;

namespace PhamTrungKienDAL
{
    public interface IRepository<T> where T : class
    {
        // Read operations
        IEnumerable<T> GetAll();
        T? GetById(int id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T? FirstOrDefault(Expression<Func<T, bool>> predicate);
        int Count();
        int Count(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);

        // Create operations
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);

        // Update operations
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        // Delete operations
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void RemoveById(int id);

        // Save operations
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}

