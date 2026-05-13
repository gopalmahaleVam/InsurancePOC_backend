using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance.Domain.Entities;

namespace Insurance.Domain.Interfaces
{

public interface IUserRepository : IGenericRepository<User>
{
        Task<List<User>> SearchByNameAsync(string name);
        Task<User> AddUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
        Task<List<User>> SearchUsersByNameAsync(string firstName, string lastName);
}

}