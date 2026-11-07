using CoffeSystem_DataAccess.DBContext;
using CoffeSystem_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeSystem_DataAccess.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methode N°1. Get All Active Users
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                                 .Where(u => u.IsActive)
                                 .OrderBy(u => u.FullName)
                                 .ToListAsync();
        }

        // Methode N°2. Get User By Id
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                                 .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // Methode N°3. Get User By UserName
        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            string normalizedUserName = userName.Trim();

            return await _context.Users
                                 .FirstOrDefaultAsync(
                                     u => u.UserName == normalizedUserName
                                 );
        }

        // Methode N°4. Add New User
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Methode N°5. Update User
        public async Task<bool> UpdateUserAsync(int userId,string userName,  string fullName,string passwordHash,string role)
        {
            var user = await _context.Users
                                     .FirstOrDefaultAsync(
                                         u => u.UserId == userId
                                     );

            if (user == null)
                return false;

            user.UserName = userName;
            user.FullName = fullName;
            user.Role = role;

            if (!string.IsNullOrWhiteSpace(passwordHash))
            {
                user.PasswordHash = passwordHash;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        // Methode N°6. Deactivate User
        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user = await _context.Users
                                     .FirstOrDefaultAsync(
                                         u => u.UserId == userId
                                     );

            if (user == null)
                return false;

            user.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // Methode N°7. Restore User
        public async Task<bool> RestoreUserAsync(int userId)
        {
            var user = await _context.Users
                                     .FirstOrDefaultAsync(
                                         u => u.UserId == userId
                                     );

            if (user == null)
                return false;

            user.IsActive = true;

            await _context.SaveChangesAsync();

            return true;
        }

        // Methode N°8. Get All Inactive Users
        public async Task<List<User>> GetInactiveUsersAsync()
        {
            return await _context.Users
                                 .Where(u => !u.IsActive)
                                 .OrderBy(u => u.FullName)
                                 .ToListAsync();
        }

        //Methode N° 9. Search Active Users
        public async Task<List<User>> SearchUsersAsync(string searchText)
        {
            return await _context.Users
                                 .Where(u => u.IsActive &&
                                            (u.UserName.Contains(searchText) ||
                                             u.FullName.Contains(searchText)))
                                 .OrderBy(u => u.FullName)
                                 .ToListAsync();
        }

        // Methode N°10. Search Inactive Users
        public async Task<List<User>> SearchInactiveUsersAsync(string searchText)
        {
            return await _context.Users
                                 .Where(u => !u.IsActive &&
                                            (u.UserName.Contains(searchText) ||
                                             u.FullName.Contains(searchText)))
                                 .OrderBy(u => u.FullName)
                                 .ToListAsync();
        }

        //Methode N° 11. Check Username Exists
        public async Task<bool> UserNameExistsAsync( string userName,int? excludedUserId = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            string normalizedUserName = userName.Trim();

            return await _context.Users
                                 .AnyAsync(u =>
                                     u.UserName == normalizedUserName &&
                                     (!excludedUserId.HasValue ||
                                      u.UserId != excludedUserId.Value));
        }

        // Methode N°12 : count system users
        public async Task<int> GetActiveSystemAdminCountAsync()
        {
            return await _context.Users
                                 .CountAsync(u =>
                                     u.IsSystemUser &&
                                     u.IsActive &&
                                     u.Role == "Admin");
        }
    }
}