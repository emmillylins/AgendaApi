using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser(){}
        public ApplicationUser(string cpf, string? dataNasc, string nome)
        {
            Cpf = cpf;
            DataNasc = dataNasc;
            Nome = nome;
        }

        public string Cpf { get; private set; }
        public string Nome { get; private set; }
        public string? DataNasc { get; private set; }
    }
}
