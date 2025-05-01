using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DotnetCqrs.Infrastructure.Data;

public interface IRepository<T>
{
    IQueryable<T> GetAll();

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate);

    IQueryable<T> Where(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);

    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(Expression<Func<T, bool>> predicate);
    void Removes(IEnumerable<T> entities);
    Task CommitAsync();
}

public class Repository<T> : IRepository<T> where T : Entity, new()
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    protected Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual IQueryable<T> GetAll()
    {
        return _dbSet;
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Remove(T entity)
    {
        _context.Entry(entity).State = EntityState.Deleted;
    }

    public virtual void RemoveRange(Expression<Func<T, bool>> predicate)
    {
        var entities = _dbSet.Where(predicate);
        _dbSet.RemoveRange(entities);
    }

    public virtual void Removes(IEnumerable<T> entities)
    {
        const int batchSize = 1000;
        var batch = new List<T>(batchSize);
        foreach (var entity in entities)
        {
            batch.Add(entity);
            if (batch.Count != batchSize) continue;
            _dbSet.RemoveRange(batch);
            batch.Clear();
        }

        if (batch.Count != 0)
        {
            _dbSet.RemoveRange(batch);
        }
    }

    public virtual async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException;
            throw new Exception(innerException?.Message, innerException);
        }
    }
}