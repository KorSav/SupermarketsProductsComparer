using ApplicationCore;
using ApplicationCore.Entities.Request;
using ApplicationCore.Exceptions;
using Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using static ApplicationCore.Exceptions.ConflictExceptionType;
using static ApplicationCore.Exceptions.NotFoundExceptionType;
using static ApplicationCore.Exceptions.ValidationExceptionType;
using Codes = Npgsql.PostgresErrorCodes;

namespace Infrastructure.Repository;

internal class RequestRepository(AppDbContext dbContext) : IRequestRepository
{
    /// <summary>
    /// Only for integration testing to be able to reproduce race condition. Ignored in app
    /// </summary>
    internal TimeSpan? DelayBeforeCheckingAmount { get; set; }

    public async Task<StoredRequest> AddNewAsync(
        Request request,
        Guid userId,
        int maxCount,
        CancellationToken cancellationToken
    )
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await dbContext.Database.ExecuteSqlAsync(
                $"""
                SELECT 1 FROM "Users"
                WHERE "Id" = {userId}
                FOR UPDATE NOWAIT
                """,
                cancellationToken
            );

            var total = await dbContext
                .Requests.Where(e => e.UserId == userId)
                .CountAsync(cancellationToken);
            if (DelayBeforeCheckingAmount is not null)
                await Task.Delay(DelayBeforeCheckingAmount.Value, cancellationToken);
            if (total >= maxCount)
                throw DomainException.For(StoredRequestsLimitReached);

            EfRequest toAdd = new(new StoredRequest(Guid.Empty, userId, request));
            dbContext.Requests.Add(toAdd);
            await dbContext.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return toAdd.ToStoredRequest(); // ef populates object id after successful save changes
        }
        catch (PostgresException pex) when (pex.SqlState is Codes.LockNotAvailable)
        {
            throw DomainException.For(TooManyRequest, pex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pex)
        {
            if (pex.SqlState is Codes.UniqueViolation)
                throw DomainException.For(StoredRequestNotUnique, ex);
            throw;
        }
    }

    public Task<List<StoredRequest>> FindAllByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    ) =>
        dbContext
            .Requests.Where(e => e.UserId == userId)
            .Select(e => e.ToStoredRequest())
            .ToListAsync(cancellationToken);

    public async Task RemoveByIdAsync(
        Guid storedId,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        await dbContext
            .Requests.Where(e => e.Id == storedId && e.UserId == userId) // user can delete only his data
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task UpdateExistingAsync(
        StoredRequest changedExisting,
        CancellationToken cancellationToken
    )
    {
        int affected;
        try
        {
            affected = await dbContext
                .Requests.Where(e =>
                    e.Id == changedExisting.Id && e.UserId == changedExisting.UserId
                )
                .ExecuteUpdateAsync(
                    setters =>
                        setters
                            .SetProperty(e => e.SearchString, changedExisting.Request.SearchString)
                            .SetProperty(e => e.SortBy, changedExisting.Request.SortBy)
                            .SetProperty(e => e.SortOrder, changedExisting.Request.SortOrder),
                    cancellationToken
                );
        }
        catch (PostgresException pex)
        {
            if (pex.SqlState is Codes.UniqueViolation)
                throw DomainException.For(StoredRequestNotUnique, pex);
            throw;
        }
        if (affected is 0) // if for example delete happened concurrently
            throw DomainException.For(StoredRequestDoesNotExist);
        return;
    }
}
