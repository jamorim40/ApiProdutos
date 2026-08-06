
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CadastroProdutos.Models.Entitys;
using CadastroProdutos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CadastroProdutos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration configuracaoUsuario;
        private readonly ILoginService iloginservice;

        public LoginController(IConfiguration configuracaoUsuario, ILoginService iloginservice)
        {
            this.configuracaoUsuario = configuracaoUsuario;
            this.iloginservice = iloginservice;
        }

       
        [HttpPost]
        public ActionResult Login(Login login)
        {
            if (string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Senha))
            {
                return BadRequest("Usuário e senha são obrigatórios.");
            }

            var usuarioAutenticado = iloginservice.Autenticar(login);
            if (usuarioAutenticado is null)
            {
                return Unauthorized("Credenciais inválidas.");
            }

            var jwtConfig = configuracaoUsuario.GetSection("Jwt");
            var chave = jwtConfig["Key"] ?? throw new InvalidOperationException("A chave JWT não foi configurada.");
            var key = Encoding.ASCII.GetBytes(chave);

            var tokenHandler = new JwtSecurityTokenHandler();
            var descricaoToken = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, usuarioAutenticado.Usuario),
                    new Claim(ClaimTypes.Role, usuarioAutenticado.Papel)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtConfig["Issuer"],
                Audience = jwtConfig["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(descricaoToken);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString });
        }
    }
}