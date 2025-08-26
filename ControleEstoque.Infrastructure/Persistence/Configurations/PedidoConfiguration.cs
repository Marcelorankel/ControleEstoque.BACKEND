using ControleEstoque.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleEstoque.Infrastructure.Persistence.Configurations
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("tbPedido");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("char(36)")
                .IsRequired();

            builder.Property(p => p.IdUsuario)
                .HasColumnName("idUsuario")
                .HasColumnType("char(36)")
                .IsRequired();

            builder.Property(p => p.DocumentoCliente)
                .HasColumnName("documentoCliente")
                .HasMaxLength(15);

            builder.Property(p => p.DataPedido)
                .HasColumnName("dataPedido")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(p => p.ValorTotal)
                .HasColumnName("valorTotal")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnName("createdAt")
                .HasColumnType("datetime");

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("updatedAt")
                .HasColumnType("datetime");

            builder.Property(p => p.DeletedAt)
                .HasColumnName("deletedAt")
                .HasColumnType("datetime");

            // Relacionamento com Usuario
            builder.HasOne(p => p.Usuario)
                .WithMany(u => u.Pedidos) // coleção em Usuario
                .HasForeignKey(p => p.IdUsuario)
                .HasConstraintName("FK_tbPedido_Usuario");
        }
    }
}