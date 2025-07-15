using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto userDto)
    {
        if (await _authService.UserExistsAsync(userDto.UserName))
            return BadRequest(new BaseResponse<bool> { Data = false, Success = false, Message = "Username already exists." });

        await _authService.RegisterAsync(userDto);
        _logger.LogInformation("User registered: {Username}", userDto.UserName);

        return Ok(new BaseResponse<bool> { Data = true, Success = true, Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
    {
        var token = await _authService.LoginAsync(userDto.Username, userDto.Password);

        if (token == null)
            return Unauthorized(new BaseResponse<string> { Data = null, Success = false, Message = "Invalid credentials." });

        _logger.LogInformation("User logged in: {Username}", userDto.Username);
        return Ok(new BaseResponse<string> { Data = token, Success = true, Message = "Login successful." });
    }
}
