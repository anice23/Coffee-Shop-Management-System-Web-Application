namespace CoffeeShop.API.DTOs;

public class UpdateUserRequestDto
{
    public string UserName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = "Cashier";

    // Optional password change
    public string? NewPassword { get; set; }
}