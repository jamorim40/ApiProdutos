using CadastroProdutos.Models.Entitys;

namespace CadastroProdutos.Services
{
    public interface ILoginService
    {
        Login? Autenticar(Login usuario);
    }
}