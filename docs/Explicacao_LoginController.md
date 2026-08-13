Explicação do LoginController
=============================

Este documento descreve o funcionamento de `LoginController`, com foco nas validações e na geração do token JWT.

1. Namespace e imports
----------------------

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CadastroProdutos.Models.Entitys;
using CadastroProdutos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
```

- `System.IdentityModel.Tokens.Jwt`: criação e serialização de tokens JWT.
- `System.Security.Claims`: criação das claims incluídas no token.
- `System.Text`: conversão da chave JWT em bytes.
- `CadastroProdutos.Models.Entitys`: acesso ao modelo `Login`.
- `CadastroProdutos.Services`: acesso à interface `ILoginService`.
- `Microsoft.AspNetCore.Mvc`: uso de `ControllerBase` e respostas HTTP.
- `Microsoft.IdentityModel.Tokens`: assinatura e validação do token.

> O projeto usa atualmente o diretório e namespace `Entitys`. Em novos projetos, prefira `Entities`, que é a grafia correta em inglês.

2. Classe `LoginController`
---------------------------

```csharp
[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
```

- `[ApiController]`: identifica a classe como um controller de API.
- `[Route("api/[controller]")]`: para `LoginController`, define a rota `/api/login`.
- `ControllerBase`: disponibiliza respostas como `Ok`, `BadRequest` e `Unauthorized`.

3. Injeção de dependência
-------------------------

```csharp
private readonly IConfiguration configuracaoUsuario;
private readonly ILoginService iloginservice;

public LoginController(IConfiguration configuracaoUsuario, ILoginService iloginservice)
{
    this.configuracaoUsuario = configuracaoUsuario;
    this.iloginservice = iloginservice;
}
```

- `IConfiguration` lê valores de `appsettings.json`, como a chave JWT.
- `ILoginService` verifica a existência do usuário e a validade da senha.
- A injeção de dependência mantém o controller desacoplado da implementação do serviço.

4. Método de login e validação inicial
--------------------------------------

```csharp
[HttpPost]
public ActionResult Login(Login login)
{
    if (string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Senha))
    {
        return BadRequest("Usuário e senha são obrigatórios.");
    }
}
```

`[HttpPost]` indica que o método responde a requisições POST. O parâmetro `login` recebe os dados enviados, normalmente em JSON. Caso usuário ou senha estejam ausentes, nulos ou contenham apenas espaços, a API retorna `400 Bad Request`.

5. Validação das credenciais
----------------------------

```csharp
var usuarioAutenticado = iloginservice.Autenticar(login);
if (usuarioAutenticado is null)
{
    return Unauthorized("Credenciais inválidas.");
}
```

O controller delega a autenticação ao `ILoginService`. Se o serviço não encontrar um usuário válido, a API retorna `401 Unauthorized`.

6. Leitura da configuração JWT
------------------------------

```csharp
var jwtConfig = configuracaoUsuario.GetSection("Jwt");
var chave = jwtConfig["Key"] ?? throw new InvalidOperationException("A chave JWT não foi configurada.");
var key = Encoding.ASCII.GetBytes(chave);
```

`GetSection("Jwt")` obtém a seção JWT do `appsettings.json`. A chave secreta é obrigatória; se ela não estiver configurada, uma exceção é lançada. Em seguida, `Encoding.ASCII.GetBytes` a converte em bytes para a assinatura do token.

> A chave no `LoginController` é lida como `Key`, enquanto o `Program.cs` e o `appsettings.json` atuais usam `key`. A configuração do .NET trata essas chaves sem diferenciar maiúsculas de minúsculas, mas padronize a grafia para melhorar a legibilidade.

7. Criação do token JWT
-----------------------

```csharp
var descricaoToken = new SecurityTokenDescriptor
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
```

- `Subject`: reúne as claims do usuário.
- `ClaimTypes.Name`: registra o nome do usuário.
- `ClaimTypes.Role`: registra o papel do usuário, usado pela autorização.
- `Expires`: define a expiração do token; neste caso, uma hora.
- `Issuer` e `Audience`: identificam o emissor e o público do token.
- `SigningCredentials`: define a chave e o algoritmo de assinatura HMAC SHA-256.

8. Geração e retorno do token
------------------------------

```csharp
var token = tokenHandler.CreateToken(descricaoToken);
var tokenString = tokenHandler.WriteToken(token);

return Ok(new { token = tokenString });
```

`CreateToken` cria o token com as propriedades configuradas e `WriteToken` o converte em texto. A API retorna `200 OK` com o token que o cliente deve enviar nas próximas requisições protegidas.

Resumo
------

O `LoginController` valida as credenciais recebidas, delega a autenticação ao serviço de login e devolve um JWT contendo o nome e o papel do usuário autenticado.
