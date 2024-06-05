using Mafia.Application.Services.AccountAndUser;
using Mafia.Application.Utils;
using Mafia.Domain.Data.Adapters;
using Mafia.Domain.Dto;
using Mafia.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mafia.WebApi.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserControllers : ControllerApiUtil
    {
        private readonly IUserService _userService;
        private readonly MafiaDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserControllers(IUserService userService, MafiaDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/<UserControllers>
        /// <summary>
        /// Получение списка ролей
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUserRoles>>> GetAsync([FromQuery] int page, [FromQuery] int size)
        {
            var records = await _userService.GetAsync(page, size);
            if (records != null)
            {
                return Ok(records);
            }
            else
            {
                return BadRequest();
            }
        }

        // POST api/<UserControllers>
        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> PostAsync([FromBody] UserCreatePost user)
        {
            var records = await _userService.PostAsync(user);
            if (records != null)
            {
                if (records.Password != null)
                {
                    return Ok(records);
                }
                else
                {
                    return JsonData("С таким ПИН уже существует пользователь", 450);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Изменение пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationUser>> PutAsync(String id, [FromBody] UserCreate user)
        {
            var records = await _userService.PutAsync(id, user);
            if (records != null)
            {
                return Ok(records);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Получение пользователя по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetAsync(string id)
        {
            var records = await _userService.GetAsync(id);
            if (records != null)
            {
                return Ok(records);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Block")]
        public async Task<IActionResult> Block(string id)
        {
            var records = await _userService.Block(id);

            if (records != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UnBlock")]
        public async Task<IActionResult> UnBlock(string id)
        {
            var records = await _userService.UnBlock(id);

            if (records != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Сброс пароля до Test123!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Reset")]
        public async Task<ActionResult> Reset(string id)
        {
            var records = await _userService.Reset(id);

            if (records != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<ActionResult> ChangePassword(string id, string oldPassword, string newPassword)
        {
            var records = await _userService.ChangePassword(id, oldPassword, newPassword);

            if (records != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}