using Mafia.Application.Utils;
using Mafia.Domain.Data;
using Mafia.Domain.Data.Adapters;
using Mafia.Domain.Dto;
using Mafia.Domain.Dto.Account;
using Mafia.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Application.Services.AccountAndUser
{
    public class AccountService : IAccountService
    {
        private readonly MafiaDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(ILogger<AccountService> logger, MafiaDbContext context, RoleManager<IdentityRole> roleManager, IHttpClientFactory clientFactory, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
            _clientFactory = clientFactory;
            _configuration = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<AuthorizationResponse> Token(Authorize authorize)
        {

            if (authorize.Login == null || authorize.Password == null)
            {
                _logger.LogError("Invalid username or Password.");
                //return BadRequest(new { errorText = "Invalid username or password." });
                return null;
            }

            _logger.LogInformation("Login attempt. Login:{0}", authorize.Login);
            SecureString secure_password = new SecureString();
            foreach (char p in authorize.Password)
            {
                secure_password.AppendChar(p);
            }
            var identity = await GetIdentityAsync(authorize.Login, authorize.Password);
            if (identity == null)
            {
                _logger.LogError("Invalid attempt");
                return null;
                //return BadRequest(new { errorText = "Invalid username or password." });
            }

            var lt = _configuration["AuthOptions:lifetime"];
            if (!int.TryParse(lt, out int LT))
            {
                LT = 30;
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DotNetEnv.Env.GetString("JWT_KEY", "Variable not found")));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: signIn);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            var roles = identity.Claims
                 .Where(c => c.Type == ClaimTypes.Role)
                 .Select(c => c.Value).ToList();

            var u = await _userManager.FindByNameAsync(authorize.Login);
            List<string> privileges = new List<string>();
            if (roles.Count > 0)
            {
                var actions = _context.ActionForUsers.ToList();
                var actionsAndRoles = _context.ControllerAndRoles
                    .Include(e => e.ActionForUser)
                    .Include(e => e.IdentityRole)
                    .ToList();

                foreach (var temp in roles)
                {
                    var role = await _roleManager.FindByNameAsync(temp);
                    var rolesAndActions = actionsAndRoles
                        .Where(e => e.IdentityRoleId == role.Id)
                        .Select(e => e.ActionForUserId)
                        .ToList();

                    privileges.AddRange(actions
                        .Where(e => rolesAndActions.Contains(e.Id))
                        .Select(e => e.Name)
                        .ToList());
                }
            }

            // создаем один claim
            var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, u.UserName),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(", ", roles))
                    };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            var keyy = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DotNetEnv.Env.GetString("JWT_KEY", "Variable not found")));
            var signIny = new SigningCredentials(keyy, SecurityAlgorithms.HmacSha256);
            var tokeny = new JwtSecurityToken(
                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                claimsIdentity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: signIny);
            var encodedJwty = new JwtSecurityTokenHandler().WriteToken(tokeny);



            var response = new AuthorizationResponse(
            name: identity.Name,
            login: identity.Name,
            doljnost: roles.First(),
            fio: u.FIO,
            roles: string.Join(",", roles),
            //get privilege
            privileges: privileges,
            currentUser: u,
            jwtToken: encodedJwty,
            admin: roles.Contains("ADMIN")
        );
            Result.Success();
            _logger.LogInformation("Returning token");
            return response;

        }

        public async Task<AuthorizationResponse> GetPrivilegios(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var roles = await _userManager.GetRolesAsync(user);

            List<string> privileges = new List<string>();
            if (roles.Count > 0)
            {
                var actions = _context.ActionForUsers.ToList();
                var actionsAndRoles = _context.ControllerAndRoles
                    .Include(e => e.ActionForUser)
                    .Include(e => e.IdentityRole)
                    .ToList();

                foreach (var temp in roles)
                {
                    var role = await _roleManager.FindByNameAsync(temp);
                    var rolesAndActions = actionsAndRoles
                        .Where(e => e.IdentityRoleId == role.Id)
                        .Select(e => e.ActionForUserId)
                        .ToList();

                    privileges.AddRange(actions
                        .Where(e => rolesAndActions.Contains(e.Id))
                        .Select(e => e.Name)
                        .ToList());
                }
            }

            var response = new AuthorizationResponse();
            response.Roles = string.Join(",", roles);
            //get privilege
            _logger.LogInformation("Returning token");
            return response;
        }

        public async Task<bool> CheckUserAsync(UserInfoForAuth user)
        {
            try
            {
                var appUser = await _userManager.FindByNameAsync(user.pin);
                if (appUser == null)
                {
                    if (await _roleManager.FindByNameAsync(user.role) == null)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(user.role));
                    }
                    var org = _context.Organisations.FirstOrDefault(e => e.NameRu == user.full_name);
                    if (org == null)
                    {
                        return false;
                    }
                    ApplicationUser tempUserAp = new ApplicationUser { Email = "Test@mail.ru", UserName = user.pin, OrganisationId = org.Id };
                    var result = await _userManager.CreateAsync(tempUserAp, user.pass);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(tempUserAp, user.role);
                    }
                    return true;
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"CheckUserAsync with error: {e.Message}");
                return false;
            }
        }

        public async Task<ClaimsIdentity> GetIdentityAsync(string login)
        {
            try
            {
                var u = await _userManager.FindByNameAsync(login);
                if (u != null)
                {
                    await _userManager.UpdateSecurityStampAsync(u);
                    await _signInManager.SignInAsync(u, isPersistent: false);
                    //если пользователь найден
                    if (u != null)
                    {
                        var roles = await _userManager.GetRolesAsync(u);
                        // создаем один claim
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(", ", roles))
                    };
                        ClaimsIdentity claimsIdentity =
                            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                                ClaimsIdentity.DefaultRoleClaimType);
                        return claimsIdentity;
                    }

                }
                else
                {
                    // если пользователя не найдено
                    _logger.LogError("User not found");
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetIdentityAsync with error: {e.Message}");
                return null;
            }
        }
        public async Task<ClaimsIdentity> GetIdentityAsync(string login, string password)
        {
            try
            {
                var u = await _userManager.FindByNameAsync(login);
                await _userManager.UpdateSecurityStampAsync(u);
                var result = await _signInManager.PasswordSignInAsync(login, password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //если пользователь найден
                    if (u != null)
                    {
                        var roles = await _userManager.GetRolesAsync(u);
                        // создаем один claim
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(", ", roles))
                    };
                        ClaimsIdentity claimsIdentity =
                            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                                ClaimsIdentity.DefaultRoleClaimType);
                        return claimsIdentity;
                    }
                }
                else
                {
                    // если пользователя не найдено
                    _logger.LogError("User not found");
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetIdentityAsync with error: {e.Message}");
                return null;
            }
        }
    }
}