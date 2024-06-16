using Mafia.Application.Services.Interfaces;
using Mafia.Application.Paggination;
using Mafia.Domain.Data.Adapters;
using Mafia.Domain.Dto;
using Mafia.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mafia.Application.Services.AccountAndUser
{
    public class UserService : IUserService
    {
        private readonly MafiaDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UserService(ICurrentUserService currentUserService, MafiaDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserService> logger, IHttpClientFactory clientFactory)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _clientFactory = clientFactory;
            _currentUserService = currentUserService;
        }

        public async Task<Pagination<ApplicationUserRoles>> GetAsync(int page, int size)
        {
            //var query = _userManager.Users.ToList();
            var query = await _context.ApplicationUsers
                .ToListAsync();
         
            try
            {
                var list = await PaginationService.GetPagination(query, page, size);
                var applicationUserRoles = new List<ApplicationUserRoles>();
                foreach (var temp in list.Result)
                {
                    var roles = await _userManager.GetRolesAsync(temp);
                    var role = await _roleManager.FindByNameAsync(roles.First());

                    applicationUserRoles.Add(new ApplicationUserRoles()
                    {
                        Email = temp.Email,
                        EmailConfirmed = temp.EmailConfirmed,
                        LockoutEnabled = temp.LockoutEnabled,
                        LockoutEnd = temp.LockoutEnd,
                        NormalizedEmail = temp.NormalizedEmail,
                        TwoFactorEnabled = temp.TwoFactorEnabled,
                        AccessFailedCount = temp.AccessFailedCount,
                        ConcurrencyStamp = temp.ConcurrencyStamp,
                        FIO = temp.FIO,
                        Id = temp.Id,
                        NormalizedUserName = temp.NormalizedUserName,
                        PasswordHash = temp.PasswordHash,
                        Block = temp.LockoutEnd == null ? false : true,
                        Phone = temp.Phone,
                        PhoneNumber = temp.Phone,
                        PhoneNumberConfirmed = temp.PhoneNumberConfirmed,
                        Pin = temp.Pin,
                        SecurityStamp = temp.SecurityStamp,
                        UserName = temp.UserName,
                        IdentityRoles = roles.First(),
                        IdentityRoleId = role.Id
                    });
                }
                var pagination = new Pagination<ApplicationUserRoles>()
                {
                    CurrentPage = list.CurrentPage,
                    PageSize = size,
                    Result = applicationUserRoles,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages
                };
                return pagination;
            }
            catch
            {
                return null;
                //return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task<UserCreate> PostAsync(UserCreatePost user)
        {
            ApplicationUser appUser = new ApplicationUser
            {
                UserName = user.Email,
                Email = user.Email,
                FIO = user.FIO,
                Phone = user.Phone,
                EmailConfirmed = true,
            };
            //_logger.LogInformation("AccountController method: Create Post. Time: " + DateTime.Now.ToString() + ". username: " + user.UserName + ", password: " + user.Password + ", LPU: " + user.LpuId);
            try
            {
                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    IdentityRole role = _roleManager.FindByIdAsync(user.IdentityRoleId).Result;
                    _userManager.AddToRoleAsync(appUser, role.Name).Wait();

                    Console.WriteLine("user create sucess");
                    return new UserCreate()
                    {
                        Email = user.Email,
                        Id = appUser.Id,
                        FIO = user.FIO,
                        Phone = user.Phone,
                        Password= user.Password,
                        IdentityRoleId = user.IdentityRoleId
                    };
                }
                else
                {
                    return new UserCreate();
                    //return JsonData("С таким ПИН уже существует пользователь", 450);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
                //return BadRequest(e.Message);
            }
        }

        public async Task<ApplicationUser> PutAsync(String id, UserCreate user)
        {
            try
            {
                //ApplicationUser appUser = await _userManager.FindByIdAsync(user.Id);
                ApplicationUser appUser = await _userManager.FindByIdAsync(id);
                //ApplicationUser appUser = await _context.Users.Where(x=>x.Id==user.Id).FirstOrDefaultAsync();
                if (user == null)
                {
                    //return NotFound();
                    return null;
                }

                appUser.Email = user.Email;
                appUser.FIO = user.FIO;
                appUser.Phone = user.Phone;
                appUser.EmailConfirmed = true;
                if (user.Password.Length > 0)
                {
                    await _userManager.RemovePasswordAsync(appUser);
                    await _userManager.AddPasswordAsync(appUser, "Test123!");
                    IdentityResult resultPass = await _userManager.ChangePasswordAsync(appUser, "Test123!", user.Password);
                    if (!resultPass.Succeeded)
                    {
                        //return BadRequest("Ошибка при изменении пароля!");
                        return null;
                    }
                }
                IdentityResult result = await _userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    IdentityRole role = _roleManager.FindByIdAsync(user.IdentityRoleId).Result;

                    var roles = await _userManager.GetRolesAsync(appUser);
                    await _userManager.RemoveFromRolesAsync(appUser, roles);

                    _userManager.AddToRoleAsync(appUser, role.Name).Wait();
                    await _context.SaveChangesAsync();
                }
                return appUser;
            }
            catch 
            {
                //return BadRequest(e.Message);
                return null;
            }
        }

        public async Task<UserCreate> GetAsync(string id)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    //return NotFound();
                    return null;
                }
                UserCreate userCreate = new UserCreate();
                userCreate.FIO = user.FIO;
                userCreate.Email = user.UserName;
                userCreate.Phone = user.Phone;
                return userCreate;
            }
            catch
            {
                //return BadRequest(e.Message);
                return null;
            }
        }

        public async Task<IActionResult> Block(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                //return NotFound();
                return null;
            }
            //IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(100));
            IdentityResult result = await _userManager.SetLockoutEnabledAsync(user, true);
            if (result.Succeeded)
            {
                return new OkResult();
            }
            else
            {
                //return BadRequest();
                return null;
            }
        }

        public async Task<IActionResult> UnBlock(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                //return NotFound();
                return null;
            }
            IdentityResult result = await _userManager.SetLockoutEnabledAsync(user, false);
            if (result.Succeeded)
            {
                return new OkResult();
            }
            else
            {
                //return BadRequest();
                return null;
            }
        }

        public async Task<ActionResult> Reset(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                //return NotFound();
                return null;
            }
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, "Test123!");
            return new OkResult();
        }

        public async Task<ActionResult> ChangePassword(string id, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                //return NotFound();
                return null;
            }
            var checkPassword = _userManager.CheckPasswordAsync(user, oldPassword);
            if (checkPassword.Result)
            {
                foreach (IPasswordValidator<ApplicationUser> passwordValidator in _userManager.PasswordValidators)
                {
                    var result = await passwordValidator.ValidateAsync(_userManager, user, newPassword);

                    if (!result.Succeeded)
                    {
                        //новый пароль не соответствует валидации
                        return null;
                    }
                }

                await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                return new OkResult();
            }
            //неправильный логин или действующий пароjль
            return null;
        }
    }
}