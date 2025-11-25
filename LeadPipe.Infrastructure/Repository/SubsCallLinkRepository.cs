using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Database;
using LeadPipe.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace LeadPipe.Infrastructure.Repository;

internal class SubsCallLinkRepository(PlumbingContext context) : ISubsCallLinkRepository
{
    private readonly PlumbingContext _context = context;
    public async Task<Result<SubsCallLink>> GetAsync(SubsCallLink entity)
    {
        Result<SubsCallLink> result = await GetByIdAsync(entity.Id);
        return result;
    }

    public async Task<Result<SubsCallLink>> GetByIdAsync(long id)
    {
        SubsCallLink? found = await _context.SubsCallLinks.FindAsync(id);
        return found is null
            ? Result.Failure<SubsCallLink>($"Entity with compound id was not found\nId: {id}")
            : Result.Success(found);
    }

    public async Task<Result<List<SubsCallLink>>> GetAllAsync()
    {
        List<SubsCallLink>? result = await _context.SubsCallLinks.ToListAsync();
        return result is null
            ? Result.Failure<List<SubsCallLink>>("The desired repository is empty")
            : Result.Success(result);
    }

    public async Task<Result> AddAsync(SubsCallLink entity)
    {
        await _context.SubsCallLinks.AddAsync(entity);
        await _context.SaveChangesAsync();
        return await GetAsync(entity);
    }

    public async Task<Result> AddRangeAsync(List<SubsCallLink> entities)
    {
        if (entities is null || entities.Count == 0)
            return Result.Failure("No link entities provided.");

        await _context.SubsCallLinks.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        bool allLinksExist = entities.All(l =>
            _context.SubsCallLinks.Any(dbLink =>
                dbLink.Id == l.Id));

        return allLinksExist
            ? Result.Success()
            : Result.Failure("Not all link entities were saved successfully.");
    }


    public async Task<Result> HardUpdateAsync(SubsCallLink entity)
    {
        // Check for existence
        SubsCallLink? exists = await _context.SubsCallLinks
            .FirstOrDefaultAsync(e => e.Id == entity.Id);
        if (exists is null)
            return Result.Failure("The desired entity does not exist");

        // Update
        _context.SubsCallLinks.Update(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateValuesAsync(SubsCallLink entity)
    {
        // Check for existence
        SubsCallLink? exists = await _context.SubsCallLinks
            .FirstOrDefaultAsync(e => e.Id == entity.Id);
        if (exists is null)
            return Result.Failure("The desired entity does not exist");

        // Update
        _context.Entry(exists).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(long id)
    {
        SubsCallLink? entity = await _context.SubsCallLinks.FindAsync(id);
        if (entity is not null)
        {
            _context.SubsCallLinks.Remove(entity);
            await _context.SaveChangesAsync();
            Result<SubsCallLink> deleted = await GetAsync(entity);
            return deleted.IsSuccess ? Result.Failure("Failed to delete entity") : Result.Success();
        }
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(SubsCallLink entity)
    {
        return await DeleteAsync(entity.Id);
    }
}
