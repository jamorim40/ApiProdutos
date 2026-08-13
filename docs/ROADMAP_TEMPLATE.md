# Roadmap de implementação — API ASP.NET Core com CRUD e JWT

Este documento é um modelo reutilizável para APIs REST baseadas em ASP.NET Core, Entity Framework Core, SQLite (ou outro banco relacional), autenticação JWT e autorização por papéis.

> Adapte `Produto`, `Usuario` e `ApplicationDbContext` ao domínio do novo projeto. A sequência proposta define persistência, regras de negócio e segurança antes dos controllers.

## 1. Visão da arquitetura

```text
Cliente HTTP / Swagger
          │
          ▼
     Controllers
          │
          ▼
Services (interfaces e regras)
          │
          ▼
ApplicationDbContext / EF Core
          │
          ▼
     Banco de dados
```

O login percorre essas camadas e retorna um JWT. Em rotas protegidas, o middleware valida o token antes da execução do controller.

## 2. Estrutura de pastas sugerida

```text
NomeDaApi/
├── Controllers/             # Rotas e respostas HTTP
├── Data/                    # DbContext e configurações de persistência
├── Models/
│   ├── Entities/            # Entidades mapeadas para o banco
│   └── DTOs/                # Contratos de entrada e saída da API
├── Services/
│   ├── Interfaces/          # Contratos das regras de negócio
│   └── Implementations/     # Implementações dos serviços
├── Validations/             # Validadores reutilizáveis, quando necessários
├── Migrations/              # Histórico gerado pelo EF Core
├── Properties/              # Perfis de execução locais
├── docs/                    # Documentação técnica
├── appsettings.json         # Configurações padrão não sigilosas
├── appsettings.Development.json
├── Program.cs               # DI, autenticação e pipeline HTTP
└── NomeDaApi.csproj
```

Use `Entities` como plural de `Entity`. Mantenha o namespace, o nome do projeto e os nomes dos arquivos consistentes entre si.

## 3. Dependências e pré-requisitos

Use uma versão do SDK compatível com o `TargetFramework` escolhido. Para um projeto em .NET 9, por exemplo:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
```

| Dependência | Finalidade |
| --- | --- |
| `Microsoft.EntityFrameworkCore` | ORM para consultar e persistir entidades. |
| `Microsoft.EntityFrameworkCore.Sqlite` | Provedor do SQLite. |
| `Microsoft.EntityFrameworkCore.Tools` | Criação e aplicação de migrações com `dotnet ef`. |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | Validação de tokens Bearer/JWT. |
| `Swashbuckle.AspNetCore` | Interface Swagger para documentação e testes. |

## 4. Etapas de implementação

### Fase 1 — Criar a solução e definir convenções

```bash
dotnet new webapi -n NomeDaApi
cd NomeDaApi
dotnet new sln -n NomeDaApi
dotnet sln add NomeDaApi.csproj
```

Defina entidades no singular (`Produto`), coleções e tabelas no plural (`Produtos`) e papéis padronizados, como `Admin` e `Usuario`.

### Fase 2 — Instalar pacotes e configurar JWT

Adicione as dependências e registre uma configuração sem segredos reais:

```json
"Jwt": {
  "Key": "defina-uma-chave-secreta-segura-fora-do-repositorio",
  "Issuer": "NomeDaApi",
  "Audience": "NomeDaApi"
}
```

`Key`, `Issuer` e `Audience` são necessários para emitir e validar tokens. Em produção, a chave deve vir de variável de ambiente ou gerenciador de segredos.

### Fase 3 — Modelar entidades e DTOs

```csharp
public class Produto
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue)]
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; } = true;
}

public record CriarProdutoDto(string Nome, decimal Preco, int Estoque);
```

As anotações validam a entrada. DTOs evitam que o cliente altere campos internos, como `Id` e `Ativo`.

### Fase 4 — Criar o contexto e configurar o banco

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
}
```

Registre o contexto no `Program.cs`:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Fase 5 — Gerar e aplicar migrações

```bash
dotnet ef migrations add CriarEstruturaInicial
dotnet ef database update
```

Não altere uma migração que já tenha sido aplicada em outro ambiente. Para cada mudança estrutural, crie uma nova migração com nome descritivo em PascalCase, como `AdicionarPapelAoUsuario`.

### Fase 6 — Definir serviços

```csharp
public interface IProdutoService
{
    Task<IReadOnlyList<Produto>> ObterTodosAsync();
    Task<Produto?> ObterPorIdAsync(int id);
    Task<Produto> AdicionarAsync(Produto produto);
}
```

As regras de negócio devem permanecer no serviço; os controllers devem apenas traduzir requisições em chamadas ao serviço e devolver a resposta HTTP adequada.

```csharp
builder.Services.AddScoped<IProdutoService, ProdutoService>();
```

### Fase 7 — Criar controllers e contratos HTTP

Implemente, para cada recurso:

1. `GET /api/recursos` — listagem.
2. `GET /api/recursos/{id}` — consulta individual.
3. `POST /api/recursos` — criação (`201 Created`).
4. `PUT /api/recursos/{id}` — atualização.
5. `DELETE /api/recursos/{id}` — exclusão física ou lógica (`204 No Content`).

Use restrições de rota, como `{id:int}`, para impedir que valores incompatíveis alcancem a ação.

### Fase 8 — Implementar login e autorização JWT

Armazene hashes de senha, nunca senhas em texto puro. Inclua o papel do usuário no token:

```csharp
new Claim(ClaimTypes.Role, usuario.Papel)
```

Configure a autenticação e a autorização:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* TokenValidationParameters */ });
builder.Services.AddAuthorization();

app.UseAuthentication();
app.UseAuthorization();
```

Proteja endpoints administrativos com um nome de papel consistente:

```csharp
[Authorize(Roles = "Admin")]
[HttpPost]
public Task<IActionResult> Criar(CriarProdutoDto dto) => ...;
```

### Fase 9 — Configurar Swagger e testar

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

Teste o fluxo completo:

1. Crie ou disponibilize um usuário de teste.
2. Faça `POST /api/login` e copie o token.
3. Autorize no Swagger com `Bearer {token}`.
4. Teste listagem, criação, atualização e remoção.
5. Confirme os retornos `401` para ausência de token e `403` para papel insuficiente.

### Fase 10 — Verificar qualidade e entrega

```bash
dotnet restore
dotnet build
dotnet test
dotnet run
```

Checklist:

- [ ] Não há segredos, tokens ou senhas reais no repositório.
- [ ] As senhas são armazenadas com hash forte.
- [ ] As validações retornam erros HTTP consistentes.
- [ ] Os controllers usam DTOs quando necessário.
- [ ] Todas as alterações de schema têm uma migração.
- [ ] Papéis, namespaces, arquivos e pastas têm nomes consistentes.
- [ ] O Swagger está configurado para JWT no ambiente de desenvolvimento.
- [ ] O README informa como executar a aplicação e usar os endpoints principais.

## 5. Dependências entre etapas

```text
Projeto e pacotes
       ↓
Entidades e DTOs
       ↓
DbContext e connection string
       ↓
Migrações e banco
       ↓
Interfaces e serviços
       ↓
Controllers
       ↓
JWT e autorização
       ↓
Swagger, testes e documentação
```

## 6. Ordem de commits sugerida

1. `chore: criar solução e configurar dependências`
2. `feat: criar entidades e contexto do banco`
3. `feat: adicionar migração inicial`
4. `feat: implementar serviços de produtos`
5. `feat: criar endpoints de produtos`
6. `feat: adicionar autenticação JWT e autorização por papel`
7. `docs: configurar Swagger e documentar execução`
8. `test: adicionar testes de serviços e endpoints`
