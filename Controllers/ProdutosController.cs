using CadastroProddutos;
using CadastroProdutos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroProdutos.Controllers
{   [Authorize]
    [ApiController]
    [Route("api/[controller]")]
  
    public class ProdutosController : ControllerBase
    {      
       private IProdutoService iprodutosService;

    //Injeção de dependência "DI"
       public ProdutosController (IProdutoService iprodutoService)
        {
            this.iprodutosService = iprodutoService;
        }

    //Rotas e Endpoints
    [HttpGet]
    public ActionResult<List<Produto>> Get()
        {
            return Ok(iprodutosService.ObterTodos());
        }
        
    [HttpGet("{id}")]
    public ActionResult<Produto> GetById(int id)
        {
            var produto = iprodutosService.ObterPorId(id);

            if(produto is null)
            {
                return NotFound($"Produto de id {id}, não encontrado");
            }

            return Ok(produto);
        }
    [Authorize(Roles = "adimin")]
    [HttpPost]
    public ActionResult Post(Produto novoProduto)
        {
            try
            {
                iprodutosService.Adicionar(novoProduto);
                return Created();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    [Authorize(Roles = "adimin")]
    [HttpPut("{id}")]
    public ActionResult<Produto> Put(int id, Produto produtoAtualizaddo)
        {
            try
            {
            var produto = iprodutosService.Atualizar(id,produtoAtualizaddo);
            if( produto is null)
            {
                return NotFound($"Produto de id {id}, não encontrado.");
            }

            return Ok(produto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    [Authorize(Roles = "adimin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
        {
            var deletou = iprodutosService.Remover(id);
            if(deletou == false)
            {
                return NotFound($"Produto não encotrado pelo id {id}");
            }

            return NoContent();
        }
    }
}