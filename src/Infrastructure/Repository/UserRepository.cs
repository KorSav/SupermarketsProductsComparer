using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using static ApplicationCore.Exceptions.ConflictExceptionType;
using Codes = Npgsql.PostgresErrorCodes;

namespace Infrastructure.Repository;

internal class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindByNameAndSurnameAsync(string name, string surname)
    {
        var existing = await dbContext.Users.FirstOrDefaultAsync(e =>
            e.Name == name || e.Surname == surname
        );
        return existing?.ToUser();
    }

    public Task<bool> IsPasswordValidAsync(Guid id, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<User> RegisterNewAsync(string name, string surname, string password)
    {
        try
        {
            EfUser toRegister = new(name, surname, password);
            dbContext.Add(toRegister);
            await dbContext.SaveChangesAsync();
            return toRegister.ToUser();
        }
        catch (DbUpdateException ex) when (ex.InnerException is NpgsqlException pex)
        {
            if (pex.SqlState is Codes.UniqueViolation)
                throw DomainException.For(UserDuplicateRegister, ex);
            throw;
        }
    }

    // public async Task<User> SaveAsync(User user)
    // {
    //     User? existingUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
    //         u.Name == user.Name || u.Email == user.Email
    //     );
    //     if (existingUser is null)
    //     {
    //         _dbContext.Users.Add(user);
    //     }
    //     else
    //     {
    //         existingUser.Name = user.Name;
    //         existingUser.Email = user.Email;
    //         existingUser.PasswordHash = user.PasswordHash;
    //     }
    //     await _dbContext.SaveChangesAsync();
    //     _dbContext.Entry(user).State = EntityState.Detached;
    //     return user;
    // }
}
