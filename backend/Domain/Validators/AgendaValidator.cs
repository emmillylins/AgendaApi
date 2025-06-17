using Domain.Entities;
using FluentValidation;

namespace Domain.Validators
{
    public class AgendaValidator : AbstractValidator<Agenda>
    {
        public AgendaValidator()
        {
            RuleFor(x => x.Email)
            .NotNull().NotEmpty().WithMessage("O campo '{PropertyName}' é obrigatório.")
            .MaximumLength(30).WithMessage("O campo '{PropertyName}' deve ter até {MaxLength} caracteres.")
            .EmailAddress().WithMessage("O campo '{PropertyName}' deve conter um e-mail válido.");

            RuleFor(x => x.Nome)
            .NotNull().NotEmpty().WithMessage("O campo '{PropertyName}' é obrigatório.")
            .MaximumLength(30).WithMessage("O campo '{PropertyName}' deve ter até {MaxLength} caracteres.");

            RuleFor(x => x.Telefone)
             .NotNull()
             .NotEmpty().WithMessage("O campo '{PropertyName}' é obrigatório.")
             .MaximumLength(11).WithMessage("O campo '{PropertyName}' deve ter até {MaxLength} caracteres.")
             .Matches(@"^\d+$").WithMessage("O campo '{PropertyName}' deve conter apenas números.");
        }
    }
}
