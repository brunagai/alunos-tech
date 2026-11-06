using AlunosTech.Domain.Entities;
using AlunosTech.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AlunosTech.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity>
    where TEntity : BaseEntity
{
    private readonly AlunosTechDbContext _context;

    protected RepositoryBase(AlunosTechDbContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
