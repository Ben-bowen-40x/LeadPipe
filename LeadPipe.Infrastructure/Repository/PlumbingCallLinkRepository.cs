using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Database;
using LeadPipe.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace LeadPipe.Infrastructure.Repository;

internal class PlumbingCallLinkRepository(PlumbingContext context)
{
    private readonly PlumbingContext _context = context;
    public async Task<Result<PlumbingCallLink>> GetAsync(PlumbingCallLink entity)
    {
        Result<PlumbingCallLink> result = await GetByIdAsync(entity.PlumbingId, entity.CallId);
        return result;
    }

    public async Task<Result<PlumbingCallLink>> GetByIdAsync(long plumbId, long callId)
    {
        PlumbingCallLink? found = await _context.PlumbingCallLinks.FindAsync(plumbId, callId);
        return found is null
            ? Result.Failure<PlumbingCallLink>($"Entity with compound id was not found\nPlumb Id: {plumbId}\nCall Id: {callId}")
            : Result.Success(found);
    }

    public async Task<Result<List<PlumbingCallLink>>> GetAllAsync()
    {
        List<PlumbingCallLink>? result = await _context.PlumbingCallLinks.ToListAsync();
        return result is null
            ? Result.Failure<List<PlumbingCallLink>>("The desired repository is empty")
            : Result.Success(result);
    }

    public async Task<Result> AddAsync(PlumbingCallLink entity)
    {
        await _context.PlumbingCallLinks.AddAsync(entity);
        await _context.SaveChangesAsync();
        return await GetAsync(entity);
    }

    public async Task<Result> AddRangeAsync(List<PlumbingCallLink> entities)
    {
        if (entities is null || entities.Count == 0)
            return Result.Failure("No link entities provided.");

        await _context.PlumbingCallLinks.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        bool allLinksExist = entities.All(l =>
            _context.PlumbingCallLinks.Any(dbLink =>
                dbLink.PlumbingId == l.PlumbingId && dbLink.CallId == l.CallId));

        return allLinksExist
            ? Result.Success()
            : Result.Failure("Not all link entities were saved successfully.");
    }


    public async Task<Result> HardUpdateAsync(PlumbingCallLink entity)
    {
        // Check for existence
        PlumbingCallLink? exists = await _context.PlumbingCallLinks
            .FirstOrDefaultAsync(e => e.PlumbingId == entity.PlumbingId && e.CallId == entity.CallId);
        if (exists is null)
            return Result.Failure("The desired entity does not exist");

        // Update
        _context.PlumbingCallLinks.Update(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateValuesAsync(PlumbingCallLink entity)
    {
        // Check for existence
        PlumbingCallLink? exists = await _context.PlumbingCallLinks
            .FirstOrDefaultAsync(e => e.PlumbingId == entity.PlumbingId && e.CallId == entity.CallId);
        if (exists is null)
            return Result.Failure("The desired entity does not exist");

        // Update
        _context.Entry(exists).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(long plumId, long callId)
    {
        PlumbingCallLink? entity = await _context.PlumbingCallLinks.FindAsync(plumId, callId);
        if (entity is not null)
        {
            _context.PlumbingCallLinks.Remove(entity);
            await _context.SaveChangesAsync();
            Result<PlumbingCallLink> deleted = await GetAsync(entity);
            return deleted.IsSuccess ? Result.Failure("Failed to delete entity") : Result.Success();
        }
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(PlumbingCallLink entity)
    {
        return await DeleteAsync(entity.PlumbingId, entity.CallId);
    }
}
