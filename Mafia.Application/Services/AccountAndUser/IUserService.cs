using Mafia.Application.Paggination;
using Mafia.Domain.Dto;
using Mafia.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Mafia.Application.Services.AccountAndUser
{
    public interface IUserService
    {
        /// <summary>
        /// Получение списка всех пользователей с ролями и пагинацией
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<Pagination<UserCreate>> GetAsync(int page, int size);
        /// <summary>
        /// Создание пользователя админом
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<UserCreate> PostAsync(UserCreatePost user);
        /// <summary>
        /// Изменение пользователя админом
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ApplicationUser> PutAsync(String id, UserCreate user);
        /// <summary>
        /// Получение определенного пользователя админом
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserCreate> GetAsync(string id);
        /// <summary>
        /// Блокировка пользователя админом
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IActionResult> Block(string id);
        /// <summary>
        /// Разблокировка пользователя админом
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IActionResult> UnBlock(string id);
        /// <summary>
        /// Сброс пароля до пароля по умолчанию у пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResult> Reset(string id);
        /// <summary>
        /// Изменение пароля у пользователя 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<ActionResult> ChangePassword(string id, string oldPassword, string newPassword);
    }
}