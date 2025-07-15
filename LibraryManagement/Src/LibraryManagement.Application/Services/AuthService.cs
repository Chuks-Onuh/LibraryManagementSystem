using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Domain.Dtos;

namespace LibraryManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, IConfiguration config, ILogger<AuthService> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task RegisterAsync(UserRegistrationDto userObj)
        {
            var passwordHash = HashPassword(userObj.Password);
            var user = new User { Username = userObj.UserName, FirstName = userObj.Firstname, Lastname = userObj.Lastname, PasswordHash = passwordHash  };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User registered with hash: {Username}", userObj.UserName);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !VerifyPassword(password, user.PasswordHash)) return null;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, user.Username) },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == storedHash;
        }
    }

    public interface IAuthService
    {
        Task<bool> UserExistsAsync(string username);
        Task RegisterAsync(UserRegistrationDto userObj);
        Task<string> LoginAsync(string username, string password);
    }
}
