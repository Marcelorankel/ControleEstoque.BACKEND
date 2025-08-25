using ControleEstoque.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleEstoque.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("tbUsuario");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .HasColumnName("id")
                   .HasColumnType("char(36)");

            builder.Property(u => u.Nome)
                   .HasColumnName("nome")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.Email)
                   .HasColumnName("email")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(u => u.Senha)
                   .HasColumnName("senha")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(u => u.TipoUsuario)
                   .HasColumnName("tipoUsuario")
                   .HasColumnType("tinyint")
                   .HasDefaultValue(0);

            builder.Property(u => u.CreatedAt)
                   .HasColumnName("createdAt");

            builder.Property(u => u.UpdatedAt)
                   .HasColumnName("updatedAt");

            builder.Property(u => u.DeletedAt)
                   .HasColumnName("deletedAt");
        }
    }
}