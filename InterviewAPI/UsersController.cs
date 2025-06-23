using System;

using Microsoft.AspNetCore.Mvc;
using InterviewAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InterviewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly InterviewDbContext _context;

        public UsersController(InterviewDbContext context)
        {
            _context = context;
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest("Bu e-posta ile kayıtlı bir kullanıcı zaten var.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Kayıt başarılı.");
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginData)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == loginData.Email && u.Password == loginData.Password);

            if (user == null)
                return Unauthorized("Geçersiz e-posta veya şifre.");

            return Ok(new { user.UserId, user.UserName });
        }
    }
}

