using System.Data.Entity.Core.Metadata.Edm;

namespace Domain.Entities
{
    public class Agenda
    {
        public Agenda() { }

        public Agenda(string nome, string email, string telefone)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
    }
}
