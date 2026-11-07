namespace CoffeeShop.API.DTOs;

public class CreateUserRequestDto
{
    public string UserName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "Cashier";
}