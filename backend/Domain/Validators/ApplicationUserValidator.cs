using Domain.Entities;
using FluentValidation;

namespace Domain.Validators
{
    public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
    {
        public ApplicationUserValidator()
        {
            RuleFor(x => x.Email)
            .NotNull().NotEmpty().WithMessage("O campo '{PropertyName}' é obrigatório.")
            .MaximumLength(30).WithMessage("O campo '{PropertyName}' deve ter até {MaxLength} caracteres.")
            .EmailAddress().WithMessage("O campo '{PropertyName}' deve conter um e-mail válido.");

            RuleFor(x => x.Nome)
            .NotNull().NotEmpty().WithMessage("O campo '{PropertyName}' é obrigatório.")
            .MaximumLength(30).WithMessage("O campo '{PropertyName}' deve ter até {MaxLength} caracteres.");

            RuleFor(x => x.DataNasc)
             .NotNull()
             .NotEmpty().WithMessage("O campo '{PropertyName}' é obrigatório.")
             .MaximumLength(8).WithMessage("O campo '{PropertyName}' deve ter até {MaxLength} caracteres.")
             .Matches(@"^\d+$").WithMessage("O campo '{PropertyName}' deve conter apenas números.");

            RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("O campo '{PropertyName}' é obrigatório.")
            .Length(11).WithMessage("O CPF deve conter exatamente 11 dígitos.")
            .Matches(@"^\d{11}$").WithMessage("O CPF deve conter apenas números.")
            .Must(CpfValido).WithMessage("O CPF informado é inválido.");
        }

        private bool CpfValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11 || cpf.Distinct().Count() == 1) return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            tempCpf += digito1;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf.EndsWith($"{digito1}{digito2}");
        }

    }
}
