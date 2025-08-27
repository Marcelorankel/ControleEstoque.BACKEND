namespace ControleEstoque.Core.Entities
{
    public class ProdutoPedido : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid IdPedido { get; set; }
        public Pedido Pedido { get; set; }

        public Guid IdProduto { get; set; }
        public Produto Produto { get; set; }
    }
}