using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ProductSeed
    {
        public ModelBuilder Seed(ModelBuilder mdBuilder)
        {
            mdBuilder = SeedAgenda(mdBuilder);

            return mdBuilder;
        }

        public ModelBuilder SeedAgenda(ModelBuilder mdBuilder)
        {
            mdBuilder.Entity<Agenda>().HasData(
                new Agenda("Júlia Maria", "julia@maria.com", "81988998899"),
                new Agenda("Jõao Pedro", "joão@pedro.com", "81933663366"),
                new Agenda("Pedro Farias", "pedro@farias.com", "11952635263")
            );
            return mdBuilder;
        }
    }
}
