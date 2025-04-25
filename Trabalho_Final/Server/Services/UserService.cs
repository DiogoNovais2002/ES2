using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTO;
using Server.Models;

namespace Server.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> UpdateUserProfile(UserUpdateDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user == null)
                return "Utilizador não encontrado.";

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id);
            if (dbUser == null)
                return "Utilizador não encontrado na base de dados.";

            if (userDto.Email != user.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(userDto.Email);
                if (emailExists != null && emailExists.Id != user.Id)
                    return "E-mail já em uso.";
            }

            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, userDto.Password);
                if (!result.Succeeded)
                    return "Erro ao atualizar a senha: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                return "Erro ao atualizar o utilizador: " +
                       string.Join(", ", identityResult.Errors.Select(e => e.Description));

            dbUser.Name = userDto.Name;
            dbUser.PhoneNumber = userDto.PhoneNumber;
            dbUser.Email = userDto.Email;
            dbUser.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return "Erro de concorrência ao atualizar o utilizador. Tente novamente.";
            }

            return "Utilizador atualizado com sucesso.";
        }
    }
}