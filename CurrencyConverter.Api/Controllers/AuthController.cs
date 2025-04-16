using CurrencyConverter.Api.Helpers;
using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CurrencyConverter.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtOptions _jwtOptions;

    public AuthController(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    /// <summary>
    /// Generates a JWT token for testing.
    /// </summary>
    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] AuthRequest request)
    {
        var token = JwtTokenGenerator.GenerateToken(request.UserId, request.Role, _jwtOptions);

        return Ok(new
        {
            access_token = token
        });
    }
}
