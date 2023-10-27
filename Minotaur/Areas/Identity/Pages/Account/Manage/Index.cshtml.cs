// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using System.ComponentModel.DataAnnotations;

namespace Minotaur.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<MinotaurUser> _userManager;
        private readonly SignInManager<MinotaurUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork,
            UserManager<MinotaurUser> userManager,
            SignInManager<MinotaurUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Телефонный номер")]
            public string PhoneNumber { get; set; }
            [Display(Name = "Имя аккаунта")]
            public string UserName { get; set; }
            [Display(Name = "Имя")]
            public string FirstName { get; set; }
            [Display(Name = "Отчество")]
            public string Surname { get; set; }
            [Display(Name = "Фамилия")]
            public string LastName { get; set; }
            [Display(Name = "Дата рождения")]
            public string DateofBirth { get; set; }
            [Display(Name = "Город")]
            public string City { get; set; }
            [Display(Name = "Улица")]
            public string Street { get; set; }
            [Display(Name = "Номер здания")]
            public string HouseNumber { get; set; }
        }

        private async Task LoadAsync(MinotaurUser user)
        {
            var minotaurUser = await _unitOfWork.MinotaurUsers.GetAsync(u => u.Id == user.Id);

            Username = minotaurUser.UserName;

            Input = new InputModel
            {
                UserName = user.UserName,
                PhoneNumber = minotaurUser.PhoneNumber,
                FirstName = user.FirstName,
                Surname = user.Surname,
                LastName = user.LastName,
                DateofBirth = user.DateofBirth,
                City = user.City,
                Street = user.Street,
                HouseNumber = user.HouseNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Пользователь с ID: '{_userManager.GetUserId(User)}' не найден.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _unitOfWork.MinotaurUsers.GetAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return NotFound($"Пользователь с ID: '{_userManager.GetUserId(User)}' не найден.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.PhoneNumber = Input.PhoneNumber;
            user.FirstName = Input.FirstName;
            user.Surname = Input.Surname;
            user.LastName = Input.LastName;
            if(user.DateofBirth == null)
            {
                user.DateofBirth = Input.DateofBirth;
            }
            user.City = Input.City;
            user.Street = Input.Street;
            user.HouseNumber = Input.HouseNumber;

            _unitOfWork.MinotaurUsers.Update(user);
            _unitOfWork.Save();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Данные профиля были обновлены";
            return RedirectToPage();
        }
    }
}
