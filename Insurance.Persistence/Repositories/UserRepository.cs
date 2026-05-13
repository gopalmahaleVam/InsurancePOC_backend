
using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Persistence.Repositories
{
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(InsuranceDbContext context) : base(context)
    {
    }

    public async Task<User> AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<List<User>> SearchUsersByNameAsync(string firstName, string lastName)
    {
        return await _context.Users
            .Where(u => u.FirstName.Contains(firstName) && u.LastName.Contains(lastName))
            .ToListAsync();
    }

        public Task<List<User>> SearchByNameAsync(string name)
        {
            return _context.Users
                .Where(u => u.FirstName.Contains(name) || u.LastName.Contains(name))
                .ToListAsync();
        }
    }

}
