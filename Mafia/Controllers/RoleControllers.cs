using Mafia.Application;
using Mafia.Application.Paggination;
using Mafia.Domain.Data.Adapters;
using Mafia.Domain.Dto;
using Mafia.Domain.Entities.Privilegios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mafia.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class RoleControllers : ControllerBase
    {
        private readonly MafiaDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleControllers(MafiaDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // GET: api/<RoleControllers>
        /// <summary>
        /// Получение списка ролей
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleAndPermissions>>> Get([FromQuery] int page, [FromQuery] int size)
        {
            var permissions = await _context.ControllerAndRoles
                .Include(e => e.ActionForUser)
                .Include(e => e.IdentityRole)
                .ToListAsync();
            var roles = await _roleManager.Roles
                .Where(e => e.Id != "2952269d-afda-4686-8925-b50e1b045906")
                .Select(e => new RoleAndPermissions { Id = e.Id, Name = e.Name }).ToListAsync();

            roles.ForEach(e =>
            {
                e.Permissions = permissions.Where(x => x.IdentityRoleId == e.Id)
                .Select(x => new Permissions { Name = x.ActionForUser.Name, Id = x.ActionForUser.Id })
                .ToList();
            });

            try
            {
                var list = await PaginationService.GetPagination(roles, page, size);
                return new JsonResult(list);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        // GET: api/<RoleControllers>
        /// <summary>
        /// Получение Роли по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RoleAndPermissions>>> Get(string id)
        {
            var permissions = await _context.ControllerAndRoles
                .Include(e => e.ActionForUser)
                .ToListAsync();
            var roles = await _roleManager.Roles.Select(e => new RoleAndPermissions { Id = e.Id, Name = e.Name }).ToListAsync();

            roles.ForEach(e =>
            {
                e.Permissions = permissions.Where(x => x.IdentityRoleId == e.Id)
                .Select(x => new Permissions { Name = x.ActionForUser.Name, Id = x.ActionForUser.Id })
                .ToList();
            });

            try
            {
                return new JsonResult(roles.FirstOrDefault(e => e.Id == id));
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        // GET: api/<RoleControllers>
        /// <summary>
        /// Создание роли
        /// </summary>
        /// <param name="roleT"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] IdentityRoleName roleT)
        {

            try
            {
                var role = await _roleManager.CreateAsync(new IdentityRole(roleT.name));
                return Ok(role);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        public class IdentityRoleName
        {
            public string name { get; set; }
        }
    }
}
