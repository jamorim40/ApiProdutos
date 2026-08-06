using CadastroProddutos;

namespace CadastroProdutos.Services
{
    public interface IProdutoService
    {
        public List<Produto> ObterTodos();
        public Produto ObterPorId(int id);
        public void Adicionar(Produto produtoNovo);
        public Produto Atualizar(int id, Produto produtoAtualizado);
        public bool Remover(int id);
    }
}