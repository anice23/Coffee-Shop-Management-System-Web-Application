using CoffeSystem_DataAccess.Constants;
using CoffeSystem_DataAccess.Entities;
using CoffeSystem_DataAccess.Repositories;

namespace CoffeSystem_Business
{
    public class UserService
    {
        private readonly UserRepository _repository;

        public UserService(UserRepository repository)
        {
            _repository = repository;
        }

        // =========================================================
        // GET ALL ACTIVE USERS
        // =========================================================

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _repository.GetAllUsersAsync();
        }

        // =========================================================
        // GET USER BY ID
        // =========================================================

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _repository.GetUserByIdAsync(userId);
        }

        // =========================================================
        // GET USER BY USERNAME
        // =========================================================

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _repository.GetUserByUserNameAsync(userName);
        }

        // =========================================================
        // REGISTER USER
        // =========================================================


        public async Task<(bool IsSuccess, string Message)> RegisterUserAsync( User newUser,string rawPassword,int requestedByUserId)
        {
            // Validation
            if (newUser == null)
            {
                return (
                    false,
                    "بيانات المستخدم غير صحيحة."
                );
            }

            if (string.IsNullOrWhiteSpace(newUser.UserName) ||
                string.IsNullOrWhiteSpace(rawPassword))
            {
                return (
                    false,
                    "اسم المستخدم وكلمة المرور مطلوبة."
                );
            }

            if (string.IsNullOrWhiteSpace(newUser.FullName))
            {
                return (
                    false,
                    "الاسم الكامل مطلوب."
                );
            }

            // Normalize user data
            newUser.UserName = newUser.UserName.Trim();
            newUser.FullName = newUser.FullName.Trim();

            // Verify the requester from the database
            var requester = await _repository.GetUserByIdAsync(
                requestedByUserId
            );

            if (requester == null ||
                !requester.IsActive ||
                !string.Equals(
                    requester.Role,
                    UserRoles.Admin,
                    StringComparison.OrdinalIgnoreCase))
            {
                return (
                    false,
                    "غير مسموح لك بإضافة مستخدمين."
                );
            }

            // Default Role
            if (string.IsNullOrWhiteSpace(newUser.Role))
            {
                newUser.Role = UserRoles.Cashier;
            }
            else
            {
                newUser.Role = newUser.Role.Trim();
            }

            // Validate Role
            bool isAdminRole = string.Equals(
                newUser.Role,
                UserRoles.Admin,
                StringComparison.OrdinalIgnoreCase
            );

            bool isCashierRole = string.Equals(
                newUser.Role,
                UserRoles.Cashier,
                StringComparison.OrdinalIgnoreCase
            );

            if (!isAdminRole && !isCashierRole)
            {
                return (
                    false,
                    $"الصلاحية '{newUser.Role}' غير مجازة."
                );
            }

            // Store the canonical role value
            newUser.Role = isAdminRole
                ? UserRoles.Admin
                : UserRoles.Cashier;

            // Check Username
            bool isExists = await _repository.UserNameExistsAsync(
                newUser.UserName
            );

            if (isExists)
            {
                return (
                    false,
                    "اسم المستخدم مستعمل!"
                );
            }

            // Hash Password using BCrypt
            newUser.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(rawPassword);

            // New users are Active
            newUser.IsActive = true;

            await _repository.AddUserAsync(newUser);

            return (
                true,
                "تم تسجيل المستخدم بنجاح."
            );
        }

        // =========================================================
        // LOGIN
        // =========================================================

        public async Task<(bool IsSuccess, User? user, string Message)> LoginAsync( string userName,string rawPassword)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(rawPassword))
            {
                return (
                    false,
                    null,
                    "يرجى إدخال اسم المستخدم وكلمة المرور"
                );
            }

            // Get User
            var user = await _repository.GetUserByUserNameAsync(userName);
            // User doesn't exist OR inactive
            if (user == null || !user.IsActive)
            {
                return (
                    false,
                    null,
                    "اسم المستخدم أو كلمة المرور غير صحيحة"
                );
            }

            // Verify Password
            bool isValidPassword =BCrypt.Net.BCrypt.Verify(rawPassword,user.PasswordHash);

            if (!isValidPassword)
            {
                return (
                    false,
                    null,
                    "اسم المستخدم أو كلمة المرور غير صحيحة"
                );
            }

            return (
                true,
                user,
                "تم تسجيل الدخول بنجاح."
            );
        }

        // =========================================================
        // ADD USER
        // =========================================================

        public async Task AddUserAsync(User user)
        {
            await _repository.AddUserAsync(user);
        }

        // =========================================================
        // UPDATE USER
        // =========================================================
        public async Task<(bool IsSuccess, string Message)> UpdateUserAsync(int userId,string userName,string fullName,string role, string? newPassword)
        {
            // 1. Validate required fields
            if (string.IsNullOrWhiteSpace(userName))
                return (false, "Username is required.");

            if (string.IsNullOrWhiteSpace(fullName))
                return (false, "Full name is required.");

            // 2. Normalize input
            userName = userName.Trim();
            fullName = fullName.Trim();

            // 3. Check if user exists
            User? existingUser = await _repository.GetUserByIdAsync(userId);

            if (existingUser == null)
                return (false, "User not found.");

            // 4. Validate role
            role = string.IsNullOrWhiteSpace(role)
                ? UserRoles.Cashier
                : role.Trim();

            bool isAdminRole = string.Equals(
                role,
                UserRoles.Admin,
                StringComparison.OrdinalIgnoreCase);

            bool isCashierRole = string.Equals(
                role,
                UserRoles.Cashier,
                StringComparison.OrdinalIgnoreCase);

            if (!isAdminRole && !isCashierRole)
                return (false, "Invalid user role.");

            role = isAdminRole
                ? UserRoles.Admin
                : UserRoles.Cashier;

            // 5. Protect System User
            if (existingUser.IsSystemUser)
            {
                if (!string.Equals(
                    existingUser.UserName,
                    userName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return (false, "Cannot update username of system user.");
                }

                if (!string.Equals(
                    existingUser.Role,
                    role,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return (false, "Cannot update role of system user.");
                }
            }

            // 6. Check username uniqueness
            bool userNameExists = await _repository.UserNameExistsAsync(
                userName,
                userId);

            if (userNameExists)
                return (false, "Username is already in use.");

            // 7. Validate and hash new password if provided
            string passwordHash = string.Empty;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                newPassword = newPassword.Trim();

                if (newPassword.Length < 6)
                    return (false, "Password must be at least 6 characters.");

                passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            // 8. Update user without changing password
            bool result = await _repository.UpdateUserAsync(
                userId,
                userName,
                fullName,
               passwordHash,
                role);

            if (!result)
                return (false, "Failed to update user.");

            return (true, "User updated successfully.");
        }

        // =========================================================
        // DEACTIVATE USER
        // =========================================================

        public async Task<(bool isSuccess,string message)> DeactivateUserAsync(int userId)
        {
            User? user = await _repository.GetUserByIdAsync(userId);

            // Case 1 : if user not found
            if (user == null)
            {
                return (false, "User not found.");
            }

            // Case 2 :  if  user is system user
            if(user.IsSystemUser)
            {
                return (false, "System User cannot be desactivated ");
            }

            // case 3 : if no active system user
            // The system must always have at least one active System Admin.
            int activeSystemAdminCount =
                await _repository.GetActiveSystemAdminCountAsync();

            if (activeSystemAdminCount <= 0)
            {
                return (
                    false,
                    "The system must have at least one active system administrator."
                );
            }


            // case 3 : desactivate
            bool result = await _repository.DeactivateUserAsync(userId);
            
            // check if done or no
            if(!result)
            {
                return(false, "failed to desactivate user");
            }
            return (true, "User desactivated successfully");




        }

        // =========================================================
        // RESTORE USER
        // =========================================================

        public async Task<bool> RestoreUserAsync(int userId)
        {
            return await _repository.RestoreUserAsync(userId);
        }

        // =========================================================
        // GET INACTIVE USERS
        // =========================================================

        public async Task<List<User>> GetInactiveUsersAsync()
        {
            return await _repository.GetInactiveUsersAsync();
        }

        // =========================================================
        // SEARCH ACTIVE USERS
        // =========================================================

        public async Task<List<User>> SearchUsersAsync(
            string searchText)
        {
            return await _repository.SearchUsersAsync(searchText);
        }

        // =========================================================
        // SEARCH INACTIVE USERS
        // =========================================================

        public async Task<List<User>> SearchInactiveUsersAsync(
            string searchText)
        {
            return await _repository.SearchInactiveUsersAsync(
                searchText
            );
        }

        // =========================================================
        // CHECK USERNAME
        // =========================================================

        public async Task<bool> UserNameExistsAsync(
            string userName,
            int? excludedUserId = null)
        {
            return await _repository.UserNameExistsAsync(
                userName,
                excludedUserId
            );
        }

        // ============================================================
        // COUNT SYSTEM USERS
        // ===========================================================
        public async Task<int> GetActiveSystemAdminCountAsync()
        {
            return await _repository.GetActiveSystemAdminCountAsync();
        }
    }
}