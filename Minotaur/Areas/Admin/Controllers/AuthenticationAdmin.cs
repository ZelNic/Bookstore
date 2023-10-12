using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.Models;
using Minotaur.Models.SD;

namespace Minotaur.Areas.Admin.AuthenticationAdmin
{
    [Area("Admin"), Authorize(Roles = Roles.Role_Admin)]
    public class AuthenticationAdminController : Controller
    {
        private readonly UserManager<MinotaurUser> _userManager;
        private readonly SignInManager<MinotaurUser> _signInManager;
        private readonly ILogger<AuthenticationAdminController> _logger;
        public AuthenticationAdminController(SignInManager<MinotaurUser> signInManager, ILogger<AuthenticationAdminController> logger, UserManager<MinotaurUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> ConfirmAction(string? password = null, string? operationName = null, string? userId = null)
        {
            if (password == null) { return BadRequest(); }

            MinotaurUser? user = await _userManager.GetUserAsync(User);

            var emailUser = _userManager.GetEmailAsync(user);

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Администратор {user.Id} подтвердил действие: {operationName} в отношение пользователя {userId}"); ;
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
