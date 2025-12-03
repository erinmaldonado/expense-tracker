using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    // You need DTOs for Login/Register input, but for brevity we use simple types.
    // POST: api/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        IdentityUser user = new IdentityUser { UserName = model.Email, Email = model.Email };
        IdentityResult result = await _userManager.CreateAsync(user, model.Password!);

        if (result.Succeeded)
        {
            return Ok(new { Message = "User created successfully" });
        }
        return BadRequest(result.Errors);
    }

    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        IdentityUser? user = await _userManager.FindByEmailAsync(model.Email!);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            string token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }

    // Helper method to generate the JWT Token
    private string GenerateJwtToken(IdentityUser user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        DateTime expires = DateTime.Now.AddDays(7); // Token lasts 7 days

        JwtSecurityToken token = new JwtSecurityToken(
            _config["JwtSettings:Issuer"],
            _config["JwtSettings:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // NOTE: You must define these DTOs somewhere in your project for the controller to compile.
    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
}