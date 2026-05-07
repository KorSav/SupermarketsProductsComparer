using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using Infrastructure.Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using static ApplicationCore.Exceptions.ConflictExceptionType;
using static ApplicationCore.Exceptions.NotFoundExceptionType;
using Codes = Npgsql.PostgresErrorCodes;

namespace Infrastructure.Repository;

internal class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindByNameAndEmailAsync(string name, string email)
    {
        var existing = await dbContext.Users.FirstOrDefaultAsync(e =>
            e.Name == name && e.Email == email
        );
        return existing?.ToUser();
    }

    public async Task<bool> IsPasswordValidAsync(Guid userId, string password)
    {
        var efUser =
            await dbContext.Users.Where(e => e.Id == userId).FirstOrDefaultAsync()
            ?? throw DomainException.For(UserDoesNotExist);
        var hasher = new PasswordHasher<User>();
        return hasher.VerifyHashedPassword(efUser.ToUser(), efUser.PasswordHash, password) switch
        {
            PasswordVerificationResult.Success => true,
            PasswordVerificationResult.SuccessRehashNeeded => true, // won't never match
            _ => false,
        };
    }

    public async Task<User> RegisterNewAsync(string name, string email, string password)
    {
        try
        {
            var hasher = new PasswordHasher<string>();
            var passwordHash = hasher.HashPassword(user: "", password);
            EfUser registered = new(name, email, passwordHash);
            dbContext.Add(registered);
            await dbContext.SaveChangesAsync();
            return registered.ToUser();
        }
        catch (DbUpdateException ex) when (ex.InnerException is NpgsqlException pex)
        {
            if (pex.SqlState is Codes.UniqueViolation)
                throw DomainException.For(UserDuplicateRegister, ex);
            throw;
        }
    }
}
