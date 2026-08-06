using CadastroProddutos;

namespace CadastroProdutos.Services
{
    public class ProdutosService : IProdutoService
    {
         private static List<Produto> produtos = new List<Produto>()
        {
            new Produto(){Id=1, Nome="Juliano", Preco=99.90M, Estoque=5},
            new Produto(){Id=2, Nome="Kelly", Preco=151.56M, Estoque=97}
        };

        public List<Produto> ObterTodos()
        {
            return produtos;
        }

        public Produto ObterPorId(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return produtos.FirstOrDefault(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public void Adicionar(Produto produtoNovo)
        {
            produtos.Add(produtoNovo);           
        }

        public Produto Atualizar(int id, Produto produtoAtualizado)
        {
            var produto = produtos.FirstOrDefault(x => x.Id == id);
            if (produto is null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            produto.Nome = produtoAtualizado.Nome;
            produto.Preco = produtoAtualizado.Preco;
            produto.Estoque = produtoAtualizado.Estoque;
            return produto;
        }

        public bool Remover(int id)
        {
            var produto = produtos.FirstOrDefault(x => x.Id == id);
            if ( produto is null)
            {
                return false;
            }
            produtos.Remove(produto);
           return true;
        }
    }
}