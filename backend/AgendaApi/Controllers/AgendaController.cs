using Application.DTOs;
using Application.Interfaces;
using Domain.Validators;
using Infrastructure.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AgendaApi.Controllers.Main;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/agendas")]
    public class AgendaController : MainController
    {
        private readonly IAgendaService _service;
        public AgendaController(INotificador notificador, IAgendaService service) : base(notificador)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todas as agendas cadastradas.
        /// </summary>
        /// <returns>Lista de agendas</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Listar()
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);
                return CustomResponse(await _service.GetAsync<AgendaDTO>());
            }
            catch (Exception ex)
            {
                NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

        /// <summary>
        /// Retorna uma agenda específica pelo ID.
        /// </summary>
        /// <param name="id">ID da agenda</param>
        /// <returns>Agenda encontrada</returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Obter(Guid id)
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);
                return CustomResponse(await _service.GetAsync<AgendaDTO>(id));
            }
            catch (Exception ex)
            {
                NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

        /// <summary>
        /// Exclui uma agenda pelo ID.
        /// </summary>
        /// <param name="id">ID da agenda</param>
        [AllowAnonymous]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Excluir(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return CustomResponse();
            }
            catch (Exception ex)
            {
                if (ex is SqlException) NotificarErro(ex.InnerException?.Message ?? string.Empty);
                NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

        /// <summary>
        /// Insere uma nova agenda.
        /// </summary>
        /// <param name="DTO">Objeto com os dados da nova agenda</param>
        /// <returns>Agenda criada</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Inserir(CreateAgendaDTO DTO)
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);
                var result = await _service.InsertAsync<CreateAgendaDTO, CreateAgendaDTO, AgendaValidator>(DTO);
                return CustomResponse(result);
            }
            catch (Exception ex)
            {
                if (ex is SqlException) NotificarErro(ex.InnerException?.Message ?? string.Empty);
                NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

        /// <summary>
        /// Atualiza uma agenda existente.
        /// </summary>
        /// <param name="DTO">Objeto com os dados atualizados da agenda</param>
        /// <returns>Agenda atualizada</returns>
        [AllowAnonymous]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Atualizar(AgendaDTO DTO)
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);
                var result = await _service.UpdateAsync<AgendaDTO, AgendaDTO, AgendaValidator>(DTO);
                return CustomResponse(result);
            }
            catch (Exception ex)
            {
                if (ex is SqlException) NotificarErro(ex.InnerException?.Message ?? string.Empty);
                NotificarErro(ex.Message);
                return CustomResponse();
            }
        }

    }
}
