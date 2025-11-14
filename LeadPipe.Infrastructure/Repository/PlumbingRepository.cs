using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Database;
using LeadPipe.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace LeadPipe.Infrastructure.Repository;

internal class PlumbingRepository(PlumbingContext context)
{
    private readonly PlumbingContext _context = context;
    public async Task<Result<PlumbingEntity>> GetAsync(PlumbingEntity entity)
    {
        var result = await GetByPhoneNumberAsync(entity.PhoneNumber);
        return result;
    }

    public async Task<Result<PlumbingEntity>> GetByPhoneNumberAsync(long phoneNumber)
    {
        PlumbingEntity? found = await _context.PlumbingEntities.FindAsync(phoneNumber);
        if (found is null)
            return Result.Failure<PlumbingEntity>($"Entity with phone number {phoneNumber} was not found");
        return found;
    }

    public async Task<Result<List<PlumbingEntity>>> GetAllAsync()
    {
        List<PlumbingEntity>? result = await _context.PlumbingEntities.ToListAsync();
        if (result is null)
            return Result.Failure<List<PlumbingEntity>>("The desired repository is empty");
        return result;
    }

    public async Task<Result> AddAsync(PlumbingEntity entity)
    {
        await _context.PlumbingEntities.AddAsync(entity);
        await _context.SaveChangesAsync();
        return await GetAsync(entity);
    }

    public async Task<Result> UpdateAsync(PlumbingEntity entity)
    {
        // Check for existence
        PlumbingEntity? exists = await _context.PlumbingEntities
            .FirstOrDefaultAsync(e => e.PhoneNumber == entity.PhoneNumber);
        if (exists is null)
            return Result.Failure("The desired entity does not exist");

        // Update
        _context.PlumbingEntities.Update(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(long phoneNumber)
    {
        var entity = await _context.PlumbingEntities.FindAsync(phoneNumber);
        if (entity != null)
        {
            _context.PlumbingEntities.Remove(entity);
            await _context.SaveChangesAsync();
            var deleted = await GetAsync(entity);
            return deleted.IsSuccess ? Result.Failure("Failed to delete entity") : Result.Success();
        }
        return Result.Success();
    }
    public async Task<Result> DeleteAsync(PlumbingEntity entity)
    {
        return await DeleteAsync(entity.PhoneNumber);
    }
}