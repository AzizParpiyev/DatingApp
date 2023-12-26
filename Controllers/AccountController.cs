using DatingApp.Data;
using DatingApp.DTO;
using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext Context;
        private readonly ITokenService TokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            TokenService = tokenService;
            Context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto register)
        {
            if (await UserExists(register.Username))
                return BadRequest("Username exists!");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = register.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
                PasswordSalt = hmac.Key
            };

            Context.Users.Add(user);
            await Context.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = TokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto login)
        {
            var user = await Context.Users.SingleOrDefaultAsync(x => x.UserName == login.Username.ToLower());
            if (user == null)
                return Unauthorized("Invalid username!");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var password = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));
            for (var i = 0; i < password.Length; i++)
            {
                if (user.PasswordHash[i] != password[i]) return Unauthorized("Invalid password!");
            }
            return new UserDto 
            {
                Username = user.UserName,
                Token = TokenService.CreateToken(user)
            };
        }



        private async Task<bool> UserExists(string username)
        {
            return await Context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
