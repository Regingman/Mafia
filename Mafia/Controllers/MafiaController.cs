﻿using Mafia.Application.Services;
using Mafia.Application.Services.Mafia;
using Mafia.Domain.Entities.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mafia.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MafiaController : ControllerBase
    {
        private readonly IMafiaService _mafiaService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MafiaController(IMafiaService mafiaService, IHubContext<ChatHub> hubContext)
        {
            _mafiaService = mafiaService;
            _hubContext = hubContext;
        }
        /// <summary>
        /// Список комнат для админа
        /// </summary>
        /// <returns></returns>
        // GET: api/Mafia/ListRoom
        [HttpGet("ListRoom")]
        public async Task<ActionResult<List<Room>>> ListRoom([FromQuery] int page, [FromQuery] int size)
        {
            var rooms = await _mafiaService.ListRoomAsync(page, size);
            return Ok(rooms);
        }

        /// <summary>
        /// Создание комнаты для админа
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Mafia/CreateRoom
        [HttpPost("CreateRoom")]
        public ActionResult<Room> CreateRoom([FromBody] CreateRoomRequest request)
        {
            var room = _mafiaService.CreateRoom(request.AdminId, request.RoomMafia, request.PlayerCount);
            return Ok(room);
        }

        // PUT: api/Mafia/DisablePlayer
        /// <summary>
        /// Заблокировать пользователя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("DisablePlayer")]
        public IActionResult DisablePlayer([FromBody] DisablePlayerRequest request)
        {
            _mafiaService.DisablePlayer(request.PlayerId);
            return Ok();
        }

        // POST: api/Mafia/UserCreate
        /// <summary>
        /// Подключение нового игрока к комнате
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UserCreate")]
        public async Task<ActionResult<int>> UserCreate([FromBody] UserCreateRequest request)
        {
            var userId = await _mafiaService.UserCreate(request.UserId, request.RoomNumber, request.RoomPassword, request.Name, request.Age, request.Gender, request.Photo);

            return Ok(userId);
        }

        // POST: api/Mafia/UserCreate
        /// <summary>
        /// Переподключение в игру
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("RefreshConnection")]
        public async Task<ActionResult<int>> RefreshConnection([FromBody] UserRefreshRequest request)
        {
            var userId = await _mafiaService.UserRefresh(request.UserId, request.RoomNumber, request.RoomPassword);

            return Ok(userId);
        }


        /// <summary>
        /// Начало игры
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        // POST: api/Mafia/StartGame
        [HttpPost("StartGame")]
        public async Task<IActionResult> StartGameAsync([FromQuery] int roomId)
        {
            _mafiaService.StartGame(roomId);
            // Получаем всех игроков и их роли в комнате
            var playerStatuses = _mafiaService.GetAllPlayerStatusLive(roomId);

            foreach (var playerStatus in playerStatuses)
            {

                await _hubContext.Clients.User(playerStatus.PlayerUserName).SendAsync("GameStarted", playerStatus.Role);
            }

            return Ok();
        }

        /// <summary>
        /// Админ получает список всех живых игроков
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        // GET: api/Mafia/GetAllPlayerStatusLive/{roomId}
        [HttpGet("GetAllPlayerStatusLive/{roomId}")]
        public ActionResult<List<PlayerStatus>> GetAllPlayerStatusLive(int roomId)
        {
            var statuses = _mafiaService.GetAllPlayerStatusLive(roomId);
            return Ok(statuses);
        }

        /// <summary>
        /// Пользователь получает список живых игроков
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        // GET: api/Mafia/GetAllPlayerStatusLiveUser/{roomId}
        [HttpGet("GetAllPlayerStatusLiveUser/{roomId}")]
        public ActionResult<List<PlayerStatus>> GetAllPlayerStatusLiveUser(int roomId)
        {
            var statuses = _mafiaService.GetAllPlayerStatusLiveUser(roomId);
            return Ok(statuses);
        }

        /// <summary>
        /// Изменение этапа игры, день, ночь, мафия ходит и т д
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/Mafia/RoomStageUpdate
        [HttpPut("RoomStageUpdate")]
        public IActionResult RoomStageUpdate([FromBody] RoomStageUpdateRequest request)
        {
            _mafiaService.RoomStageUpdate(request.RoomId, request.StageUpdateType);
            return Ok();
        }

        /// <summary>
        /// Выбор мафии
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Mafia/PlayerVote
        [HttpPost("MafiaVote")]
        public IActionResult PlayerVote([FromQuery] int roomId, [FromQuery] string playerId)
        {
            _mafiaService.MafiaVote(roomId, playerId);
            return Ok();
        }

        /// <summary>
        /// Выбор коммисара
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Mafia/PlayerVote
        [HttpPost("CommisarVote")]
        public async Task<IActionResult> CommisarVoteAsync([FromQuery] int roomId, [FromQuery] string playerId)
        {
            await _mafiaService.CommisarVote(roomId, playerId);
            return Ok();
        }

        /// <summary>
        /// Выбор путаны
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Mafia/PlayerVote
        [HttpPost("PutanaVote")]
        public async Task<IActionResult> PutanaVoteAsync([FromQuery] int roomId, [FromQuery] string playerId)
        {
            await _mafiaService.PutanaVote(roomId, playerId);
            return Ok();
        }

        /// <summary>
        /// Выбор доктора
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Mafia/PlayerVote
        [HttpPost("DoctorVote")]
        public async Task<IActionResult> DoctorVoteAsync([FromQuery] int roomId, [FromQuery] string playerId)
        {
            await _mafiaService.DoctorVote(roomId, playerId);
            return Ok();
        }

        /// <summary>
        /// Голосование игроков днем за выбор убить его
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Mafia/PlayerVote
        [HttpPost("PlayerVote")]
        public async Task<IActionResult> PlayerVoteAsync([FromQuery] int roomId, [FromQuery] string playerId)
        {
            _mafiaService.PlayerVote(roomId, playerId);
            return Ok();
        }

        /// <summary>
        /// Изменение статуса игры, выиграли проиграл
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        // GET: api/Mafia/UpdateGameStatus/{roomId}
        [HttpGet("UpdateGameStatus/{roomId}")]
        public ActionResult<GameStatus> UpdateGameStatus(int roomId)
        {
            var status = _mafiaService.UpdateGameStatus(roomId);
            return Ok(status);
        }
    }

    // Пример моделей запросов
    public class UserCreateRequest
    {
        public string UserId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomPassword { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string Photo { get; set; }
    }

    // Пример моделей запросов
    public class UserRefreshRequest
    {
        public string UserId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomPassword { get; set; }
    }

    public class CreateRoomRequest
    {
        public string AdminId { get; set; }
        public int RoomMafia { get; set; }
        public int PlayerCount { get; set; }
    }

    public class DisablePlayerRequest
    {
        public string PlayerId { get; set; }
    }

    public class RoomStageUpdateRequest
    {
        public int RoomId { get; set; }
        public RoomStageUpdateType StageUpdateType { get; set; }
    }

    public class PlayerVoteRequest
    {
        public int RoomId { get; set; }
        public string VotingPlayerId { get; set; }
        public string TargetPlayerId { get; set; }
    }
}
