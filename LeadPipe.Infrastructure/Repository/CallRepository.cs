using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Database;
using LeadPipe.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace LeadPipe.Infrastructure.Repository;

internal class CallRepository(PlumbingContext context) : ICallRepository
{
    private readonly PlumbingContext _context = context;
    public async Task<Result<CallEntity>> GetAsync(CallEntity entity)
    {
        Result<CallEntity> result = await GetByIdAsync(entity.Id);
        return result;
    }

    public async Task<Result<CallEntity>> GetByIdAsync(long id)
    {
        CallEntity? found = await _context.CallEntities.FindAsync(id);
        if (found is null)
            return Result.Failure<CallEntity>($"Entity with phone number {id} was not found");
        return found;
    }

    public async Task<Result<List<CallEntity>>> GetAllAsync()
    {
        List<CallEntity>? result = await _context.CallEntities.ToListAsync();
        if (result is null)
            return Result.Failure<List<CallEntity>>("The desired repository is empty");
        return result;
    }

    public async Task<Result> AddAsync(CallEntity entity)
    {
        await _context.CallEntities.AddAsync(entity);
        await _context.SaveChangesAsync();
        return await GetAsync(entity);
    }

    public async Task<Result> AddRangeAsync(List<CallEntity> entities)
    {
        if (entities is null || entities.Count == 0)
            return Result.Failure("No plumbing entities provided.");

        await _context.CallEntities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Verify all entities exist
        List<long> ids = entities.Select(e => e.Id).ToList();
        List<CallEntity> savedEntities = await _context.CallEntities
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();

        return savedEntities.Count == entities.Count
            ? Result.Success()
            : Result.Failure("Not all plumbing entities were saved successfully.");
    }


    public async Task<Result> HardUpdateAsync(CallEntity entity)
    {
        // Check for existence
        CallEntity? exists = await _context.CallEntities
            .FirstOrDefaultAsync(e => e.Id == entity.Id);
        if (exists is null)
            return Result.Failure("The desired entity does not exist");

        // Update
        _context.CallEntities.Update(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateValuesAsync(CallEntity entity)
    {
        // Check for existence
        CallEntity? exists = await _context.CallEntities
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
        CallEntity? entity = await _context.CallEntities.FindAsync(id);
        if (entity is not null)
        {
            _context.CallEntities.Remove(entity);
            await _context.SaveChangesAsync();
            Result<CallEntity> deleted = await GetAsync(entity);
            return deleted.IsSuccess ? Result.Failure("Failed to delete entity") : Result.Success();
        }
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(CallEntity entity)
    {
        return await DeleteAsync(entity.Id);
    }
}