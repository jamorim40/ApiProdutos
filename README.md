# CadastroProdutos API

API REST para cadastro e gerenciamento de produtos, com autenticação JWT e documentação interativa pelo Swagger.

## Funcionalidades

- Cadastro, consulta, atualização e remoção de produtos.
- Autenticação de usuários e emissão de tokens JWT.
- Proteção das rotas de escrita por papel de usuário.

## Tecnologias

- ASP.NET Core 9
- Entity Framework Core
- SQLite
- JWT Bearer Authentication
- Swagger / OpenAPI

## Pré-requisitos

- .NET SDK 9.0 ou superior.
- Terminal para executar os comandos.

## Como executar

1. Acesse a pasta do projeto:

   ```bash
   cd CadastroProdutos
   ```

2. Restaure os pacotes:

   ```bash
   dotnet restore
   ```

3. Execute a aplicação:

   ```bash
   dotnet run
   ```

4. Consulte as URLs exibidas pelo terminal. Em ambiente de desenvolvimento, o Swagger fica disponível em `/swagger`.

## Configuração do JWT

As configurações de autenticação ficam em `appsettings.json`, na seção `Jwt`:

```json
"Jwt": {
  "key": "coloque-aqui-uma-chave-secreta-para-jwt-bem-longa-e-segura",
  "Issuer": "enderecoAPI.com.br",
  "Audience": "enderecoAPI.com.br"
}
```

Em um ambiente real, não versione chaves secretas. Use variáveis de ambiente ou um gerenciador de segredos.

> A chave é lida atualmente com o nome `key` (em minúsculas). Mantenha essa nomenclatura enquanto a configuração do projeto permanecer assim.

## Endpoints principais

### Autenticação

- `POST /api/login`

Corpo da requisição:

```json
{
  "usuario": "admin",
  "senha": "123456"
}
```

### Produtos

- `GET /api/produtos`
- `GET /api/produtos/{id}`
- `POST /api/produtos`
- `PUT /api/produtos/{id}`
- `DELETE /api/produtos/{id}`

As rotas de criação, atualização e remoção exigem um token JWT válido e o papel `adimin`, conforme configurado atualmente nos controllers.

## Estrutura do projeto

- `Controllers`: endpoints e respostas HTTP.
- `Services`: regras de negócio e acesso aos dados.
- `Databases`: contexto do Entity Framework Core.
- `Models/Entitys`: entidades do domínio.
- `Migrations`: histórico das migrações do banco de dados.
- `docs`: documentação técnica, anotações, explicações e modelos reutilizáveis.

## Observações

- O banco de dados utilizado é o SQLite.
- As migrações existentes permitem criar e atualizar a estrutura do banco com o Entity Framework Core.
- Para testar rotas protegidas no Swagger, faça login, copie o token retornado e informe-o no campo de autorização no formato `Bearer {token}`.
