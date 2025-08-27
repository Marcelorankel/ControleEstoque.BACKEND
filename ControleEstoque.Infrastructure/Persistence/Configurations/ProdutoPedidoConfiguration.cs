using ControleEstoque.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleEstoque.Infrastructure.Persistence.Configurations
{
    public class ProdutoPedidoConfiguration : IEntityTypeConfiguration<ProdutoPedido>
    {
        public void Configure(EntityTypeBuilder<ProdutoPedido> builder)
        {
            builder.ToTable("tbProdutoPedido");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnType("char(36)")
                .IsRequired();

            builder.Property(e => e.IdPedido)
                .HasColumnType("char(36)")
                .IsRequired();

            builder.Property(e => e.IdProduto)
                .HasColumnType("char(36)")
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnType("datetime");

            builder.Property(e => e.UpdatedAt)
                .HasColumnType("datetime");

            builder.Property(e => e.DeletedAt)
                .HasColumnType("datetime");

            // Relacionamento com Pedido
            builder.HasOne(pp => pp.Pedido)
                .WithMany(p => p.ProdutosPedidos)  // Coleção na entidade Pedido
                .HasForeignKey(pp => pp.IdPedido)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tbProdutosPedido_Pedido");

            // Relacionamento com Produto
            builder.HasOne(pp => pp.Produto)
                .WithMany(p => p.ProdutosPedidos)  // Coleção na entidade Produto
                .HasForeignKey(pp => pp.IdProduto)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tbProdutosPedido_Produto");
        }
    }
}