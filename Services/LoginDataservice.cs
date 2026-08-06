using CadastroProdutos.Databases;
using CadastroProdutos.Models.Entitys;

namespace CadastroProdutos.Services
{
    public class LoginDataservice : ILoginService
    {
        private readonly ApplicationDbContext banco;

        public LoginDataservice(ApplicationDbContext banco)
        {
            this.banco = banco;
        }

        public Login? Autenticar(Login usuario)
        {
            var usuarioDoBanco = banco.Logins.FirstOrDefault(u => u.Usuario == usuario.Usuario);

            if (usuarioDoBanco is null)
            {
                return null;
            }

            if (usuarioDoBanco.Senha != usuario.Senha)
            {
                return null;
            }
           
            return usuarioDoBanco;
        }
    }
}