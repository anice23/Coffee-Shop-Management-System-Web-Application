using CoffeSystem_Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.API.DTOs;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using CoffeSystem_DataAccess.Entities;

namespace CoffeeShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IAntiforgery _antiforgery;

    public UsersController(UserService userService, IAntiforgery antiforgery)
    {
        _userService = userService;
        _antiforgery = antiforgery;
    }


    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    // Endpoint N°1 : Get All Users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        var usersDto = users.Select(user => new UserDto
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            IsSystemUser = user.IsSystemUser
        }).ToList();

        return Ok(usersDto);
    }

    // Endpoint N°2 : Add User
    [HttpPost]
    public async Task<IActionResult> CreateUser( CreateUserRequestDto request)
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);
        // Get the logged-in Admin ID from JWT
        int currentUserId = GetCurrentUserId();

        // Create User Entity
        var newUser = new User
        {
            UserName = request.UserName,
            FullName = request.FullName,
            Role = request.Role,
            IsSystemUser = false
        };

        // Register User through Business Layer
        var result = await _userService.RegisterUserAsync(
            newUser,
            request.Password,
            currentUserId
        );

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }


    // Endpoint N°3 : Update User
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id,UpdateUserRequestDto request)
    {
        // Validate CSRF token
        await _antiforgery.ValidateRequestAsync(HttpContext);

        // Update user through Business Layer
        var result = await _userService.UpdateUserAsync(id,request.UserName,request.FullName,request.Role, request.NewPassword
);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }


    // Endpoint N°4 : Deactivate User

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        var result = await _userService.DeactivateUserAsync(id);

        if (!result.isSuccess)
        {
            return BadRequest(new { message = result.message });
        }

        return Ok(new { message = result.message });
    }

    // Endpoint N°5 : Restore User
    [HttpPatch("{id:int}/restore")]
    public async Task<IActionResult> RestoreUser(int id)
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        var result = await _userService.RestoreUserAsync(id);

        if (!result)
        {
            return BadRequest(new
            {
                message = "Failed to restore user."
            });
        }

        return Ok(new
        {
            message = "User restored successfully."
        });
    }


    // Endpoint N°6 : Get Inactive Users
    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveUsers()
    {
        var users = await _userService.GetInactiveUsersAsync();

        var result = users.Select(user => new UserDto
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            IsSystemUser = user.IsSystemUser
        });

        return Ok(result);
    }
}