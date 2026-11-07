
using CoffeSystem_DataAccess.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseHealthController : ControllerBase
{
    private readonly AppDbContext _context;

    public DatabaseHealthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            bool canConnect = await _context.Database.CanConnectAsync();

            if (!canConnect)
            {
                return StatusCode(503, new
                {
                    status = "Unavailable",
                    message = "Cannot connect to the database."
                });
            }

            return Ok(new
            {
                status = "Connected",
                message = "Database connection successful."
            });
        }
        catch (Exception)
        {
            return StatusCode(503, new
            {
                status = "Unavailable",
                message = "Database connection failed."
            });
        }
    }
}
