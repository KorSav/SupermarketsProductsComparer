using ApplicationCore;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

internal class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<User?> FindByNameAndSurnameAsync(string name, string surname)
    {
        throw new NotImplementedException();
    }

    // public async Task<User?> GetUserByEmailAsync(string email)
    // {
    //     return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    // }

    // public async Task<User?> GetUserByNameAsync(string userName)
    // {
    //     return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name == userName);
    // }

    public Task<bool> IsPasswordValidAsync(Guid id, string password)
    {
        throw new NotImplementedException();
    }

    public Task<User> RegisterNewAsync(string name, string surname, string password)
    {
        throw new NotImplementedException();
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
