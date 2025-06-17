using Infrastructure.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using AgendaApi.Controllers.Main;
using Microsoft.Data.SqlClient;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : MainController
    {
        private readonly IAuthService _authService;

        public AuthController(INotificador notificador, IAuthService authService) : base(notificador)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza o login de um usuário e retorna o token JWT.
        /// </summary>
        /// <param name="login">Credenciais do usuário</param>
        /// <returns>Token de autenticação</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);

                var token = await _authService.Login(login);
                return CustomResponse(token);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx) NotificarErro(sqlEx.Message);
                else NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

        /// <summary>
        /// Realiza o logout de um usuário baseado no token informado.
        /// </summary>
        /// <param name="token">Token JWT do usuário</param>
        /// <returns>Mensagem de sucesso</returns>
        [AllowAnonymous]
        [HttpPost("logout/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout(string token)
        {
            try
            {
                await _authService.Logout(token);
                return CustomResponse("Logout realizado com sucesso.");
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx) NotificarErro(sqlEx.Message);
                else NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

        /// <summary>
        /// Realiza o cadastro de um novo usuário na aplicação.
        /// </summary>
        /// <param name="register">Dados do novo usuário</param>
        /// <returns>Resultado da operação</returns>
        [AllowAnonymous]
        [HttpPost("cadastro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Cadastro([FromBody] CreateApplicationUserDTO register)
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);

                var result = await _authService.Register(register);
                return CustomResponse(result);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx) NotificarErro(sqlEx.Message);
                else NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

        /// <summary>
        /// Retorna a lista de usuários cadastrados no sistema.
        /// </summary>
        /// <returns>Lista de usuários</returns>
        [AllowAnonymous]
        [HttpGet("usuarios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ListarUsuarios()
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);

                var result = await _authService.GetUsersAsync();
                return CustomResponse(result);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx) NotificarErro(sqlEx.Message);
                else NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

    }
}
