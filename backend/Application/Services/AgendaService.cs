using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Exceptions;
using System.Data;

namespace Application.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IBaseRepository<Agenda> _repository;
        private readonly DataDbContext _context;
        private readonly IMapper _mapper;

        public AgendaService(IBaseRepository<Agenda> repository, IMapper mapper, DataDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<TOutputModel>> GetAsync<TOutputModel>() where TOutputModel : class
        {
            try
            {
                var entities = await _repository.SelectAsync();

                var outputModels = entities.Select(_mapper.Map<TOutputModel>);
                return outputModels;
            }
            catch (Exception) { throw; }
        }

        public async Task<TOutputModel> GetAsync<TOutputModel>(Guid id) where TOutputModel : class
        {
            try
            {
                var entity = await _repository.SelectAsync(id) ?? throw new NotFoundException("Agenda não encontrada.");

                var outputModel = _mapper.Map<TOutputModel>(entity);
                return outputModel;
            }
            catch (Exception) { throw; }
        }

        public async Task<TOutputModel> InsertAsync<TInputModel, TOutputModel, TValidator>(TInputModel inputModel)
            where TInputModel : class
            where TOutputModel : class
            where TValidator : AbstractValidator<Agenda>
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var entity = _mapper.Map<Agenda>(inputModel);
                    Validation<TValidator>(entity);

                    var Agendas = await _repository.SelectAsync();

                    // Inserir nova Agenda
                    await _repository.InsertAsync(entity);
                    await _context.SaveChangesAsync();

                    // Commit da transação
                    await transaction.CommitAsync();

                    var outputModel = _mapper.Map<TOutputModel>(entity);
                    return outputModel;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<TOutputModel> UpdateAsync<TInputModel, TOutputModel, TValidator>(TInputModel inputModel)
            where TInputModel : class
            where TOutputModel : class
            where TValidator : AbstractValidator<Agenda>
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Mapeamento da entidade
                    var entity = _mapper.Map<Agenda>(inputModel) ?? throw new Exception("Erro de mapeamento.");

                    Validation<TValidator>(entity);

                    // Atualizar a entidade
                    _repository.Update(entity);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // Mapear o resultado para o modelo de saída
                    var outputModel = _mapper.Map<TOutputModel>(entity);
                    return outputModel;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                // Abordagem direta sem carregar a entidade primeiro
                var entity = new Agenda { Id = id };
                _context.Agenda.Attach(entity);
                _context.Agenda.Remove(entity);

                var affectedRows = await _context.SaveChangesAsync();

                if (affectedRows == 0)
                    throw new NotFoundException("Registro não encontrado ou já excluído");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("O registro foi modificado por outro usuário", ex);
            }
        }

        #region métodos auxiliares
        private static void Validation<TValidator>(Agenda entity) where TValidator : AbstractValidator<Agenda>
        {
            try
            {
                var validator = Activator.CreateInstance<TValidator>();
                var result = validator.Validate(entity);

                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(error => new string(error.ErrorMessage));
                    var errorString = string.Join(Environment.NewLine, errors);

                    throw new Exception(errorString);
                }
            }
            catch (Exception) { throw; }
        }
        #endregion
    }
}
