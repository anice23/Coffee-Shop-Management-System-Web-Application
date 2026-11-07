
using CoffeeShop.API.DTOs;
using CoffeeShop.API.Services;
using CoffeSystem_Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CoffeeShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IAntiforgery _antiforgery;

    public AuthController(
      UserService userService,
      JwtTokenService jwtTokenService,
      IAntiforgery antiforgery)
    {
        _userService = userService;
        _jwtTokenService = jwtTokenService;
        _antiforgery = antiforgery;
    }

    //
    [HttpGet("csrf")]
    public IActionResult GetCsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new
        {
            requestToken = tokens.RequestToken,
            cookieTokenGenerated = tokens.CookieToken != null,
            isHttps = Request.IsHttps
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {

        try
        {
            await _antiforgery.ValidateRequestAsync(HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return BadRequest(new
            {
                message = "CSRF validation failed."
            });
        }

        var result = await _userService.LoginAsync(
            request.UserName,
            request.Password);

        if (!result.IsSuccess || result.user == null)
        {
            return Unauthorized(new
            {
                message = result.Message
            });
        }

        var token = _jwtTokenService.GenerateToken(
            result.user);

        Response.Cookies.Append("access_token", token,
          new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.AddMinutes(15),
        Path = "/"
    });

        return Ok(new
        {
            message = result.Message,
            expiresIn = 900,
            user = new
            {
                userId = result.user.UserId,
                userName = result.user.UserName,
                fullName = result.user.FullName,
                role = result.user.Role
            }
        });
    }


    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            userName = User.FindFirstValue(ClaimTypes.Name),
            fullName = User.FindFirstValue("FullName"),
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }


    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _antiforgery.ValidateRequestAsync(HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return BadRequest(new { message = "CSRF validation failed." });
        }

        Response.Cookies.Delete("access_token",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

        return Ok(new { message = "Logged out successfully." });
    }
}
