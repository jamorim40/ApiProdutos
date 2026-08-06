using CadastroProddutos;
using CadastroProdutos.Databases;


namespace CadastroProdutos.Services;

public class ProdutosDatabaseService : IProdutoService
{
    private ApplicationDbContext banco;

    public ProdutosDatabaseService(ApplicationDbContext banco)
    {
        this.banco = banco;
    }
    public List<Produto> ObterTodos()
    {
        
        return banco.Produtos.Where(x => x.Condicao).ToList();
    }

    public Produto ObterPorId(int id)
    {
        return banco.Produtos.FirstOrDefault(x => x.Id == id && x.Condicao == true)!;

    }

    public Produto Atualizar(int id, Produto produtoAtualizado)
    {
        ValidarProdutos(produtoAtualizado);
        var produto = banco.Produtos.FirstOrDefault(x => x.Id == id);
        if (produto is null)
        {
            return null!;
        }
        produto.Nome = produtoAtualizado.Nome;
        produto.Preco = produtoAtualizado.Preco;
        produto.Estoque = produtoAtualizado.Estoque;
        produto.Condicao = produtoAtualizado.Condicao;
        banco.SaveChanges();
        return produto;
    }
    public void Adicionar(Produto produtoNovo)
    {
        ValidarProdutos(produtoNovo);
        banco.Produtos.Add(produtoNovo);
        banco.SaveChanges();
    }

    public bool Remover(int id)
    {
        var produto = banco.Produtos.FirstOrDefault(x => x.Id == id);
        if (produto == null)
        {
            return false;
        }
        produto.Condicao = false;
        banco.SaveChanges();
        return true;
    }

      public void ValidarProdutos(Produto validacaoProduto)
    {
        if (validacaoProduto.Nome == " " || validacaoProduto.Nome is null)
        {
            throw new Exception("Não é permitido cadastrar produto padrão");
        }

        if (validacaoProduto.Estoque < 0)
        {
            throw new Exception("Não é permitido saldo menos que zero.");
        }
    }

  

}
