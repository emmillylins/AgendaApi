using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings
{
    public class AgendaMap : IEntityTypeConfiguration<Agenda>
    {
        public void Configure(EntityTypeBuilder<Agenda> builder)
        {
            builder.ToTable("Agenda").HasKey(k => k.Id);

            builder.Property(p => p.Email).HasColumnType("varchar(30)");
            builder.Property(p => p.Nome).HasColumnType("varchar(30)");
            builder.Property(p => p.Telefone).HasColumnType("varchar(11)");
        }
    }
}
