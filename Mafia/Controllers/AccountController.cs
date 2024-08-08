using Mafia.Application.Services.AccountAndUser;
using Mafia.Domain.Data.Adapters;
using Mafia.Domain.Dto;
using Mafia.Domain.Dto.Account;
using Mafia.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mafia.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private MafiaDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService, MafiaDbContext context, RoleManager<IdentityRole> roleManager, IHttpClientFactory clientFactory, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            _accountService = accountService;
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
            _clientFactory = clientFactory;
            _configuration = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<AuthorizationResponse>> Token([FromBody] Authorize authorize)
        {
            _logger.LogInformation($"login: {authorize.Login}");
            var records = await _accountService.Token(authorize);
            _logger.LogInformation($"login: {authorize.Login} successfull");
            return Ok(records);
        }
    }
}