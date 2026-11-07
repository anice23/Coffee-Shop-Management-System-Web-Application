
using CoffeeShop.API.DTOs;
using CoffeSystem_Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly CafeSettingsService _settingsService;
    private readonly IAntiforgery _antiforgery;

    public SettingsController(CafeSettingsService settingsService,IAntiforgery antiforgery)
    {
        _settingsService = settingsService;
        _antiforgery = antiforgery;
    }


    // Endpoint N°1 : Get Settings Info
    [HttpGet]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> GetSettings()
    {
        var settings = await _settingsService.GetSettingsAsync();

        if (settings == null)
            return NotFound();

        return Ok(CafeSettingsDto.FromEntity(settings));
    }



    // Endpoint N°2 : Update Settings
    [HttpPut]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> UpdateSettings([FromBody] UpdateCafeSettingsRequestDto request)
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

        try
        {
            var settings = await _settingsService.GetSettingsAsync();

            if (settings == null ||settings.CafeSettingId != request.CafeSettingId)
            {
                return NotFound(new
                {
                    message = "Settings not found."
                });
            }

            await _settingsService.UpdateSettingsAsync(
                request.CafeSettingId,
                request.CafeName,
                request.Address,
                request.PhoneNumber,
                request.ReceiptFooter,
                request.LowStockThreshold
            );

            return Ok(new
            {
                message = "Settings updated successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // Endpoint N°3 : Get Receipt Info
    [HttpGet("receipt")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> GetReceiptSettings()
    {
        var settings = await _settingsService.GetSettingsAsync();

        if (settings == null)
            return NotFound();

        return Ok(new
        {
            cafeName = settings.CafeName,
            address = settings.Address,
            phoneNumber = settings.PhoneNumber,
            receiptFooter = settings.ReceiptFooter
        });
    }
}
