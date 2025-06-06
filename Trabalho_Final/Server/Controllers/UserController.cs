﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.DTO;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;  // Adicione esta linha
        private readonly UserService _userService;

        public UserController(ApplicationDbContext context, UserManager<User> userManager, UserService userService)  // Adicione o UserManager no construtor
        {
            _context = context;
            _userManager = userManager;  // Atribua a variável _userManager
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)  // Garante que só traga o usuário com o id específico
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Mapear para o DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                UserName = user.UserName,
                Email = user.Email,
                Password = user.PasswordHash
            };

            return Ok(userDto);
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    PhoneNumber = u.PhoneNumber,
                    UserType = u.UserType,
                    UserName = u.UserName,
                    Email = u.Email, 
                    Password = u.PasswordHash
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (_context.Users.Any(u => u.Email == userDto.Email))
            {
                return BadRequest("Email já está em uso.");
            }

            var user = new User
            {
                Name = userDto.Name,
                PhoneNumber = userDto.PhoneNumber,
                UserType = userDto.UserType,
                UserName = userDto.UserName,
                Email = userDto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);  // Usando _userManager para criar o usuário

            if (result.Succeeded)
            {
                return Ok(new 
                { 
                    message = "Utilizador criado com sucesso!", 
                    userId = user.Id 
                });
            }
        
            return BadRequest(result.Errors);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (id != userDto.Id)
                return BadRequest("ID mismatch.");

            var result = await _userService.UpdateUserProfile(userDto);
            if (result != "User updated successfully.")
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("participants")]
        public async Task<IActionResult> GetParticipants()
        {
            var participants = await _context.Users
                .Where(u => u.UserType == "Participante")
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    PhoneNumber = u.PhoneNumber,
                    UserType = u.UserType,
                    UserName = u.UserName,
                    Email = u.Email,
                    Password = u.PasswordHash
                })
                .ToListAsync();

            return Ok(participants);
        }

    }
}
