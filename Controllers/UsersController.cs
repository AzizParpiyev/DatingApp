﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using udemy.Data;
using udemy.Entities;

namespace udemy.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/users
    public class UsersController :ControllerBase
    {
        private readonly DataContext Context;
        public UsersController(DataContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await Context.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await Context.Users.FindAsync(id);
            return user;
        }
    }
}