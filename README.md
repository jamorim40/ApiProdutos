# ApiProdutos
# CadastroProdutos API

API REST para cadastro e gerenciamento de produtos, com autenticação via JWT e documentação automática com Swagger.

## Objetivo

Esta aplicação permite:
- cadastrar, listar, atualizar e remover produtos;
- autenticar usuários e gerar tokens JWT;
- proteger rotas de escrita com autorização por papel.

## Tecnologias

- ASP.NET Core 9
- Entity Framework Core
- SQLite
- JWT Bearer Authentication
- Swagger / OpenAPI

## Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:
- .NET SDK 9.0 ou superior
- um terminal/console para executar os comandos

## Como executar

1. Entre na pasta do projeto:
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

4. A API ficará disponível em:
   - http://localhost:5000 (ou a porta configurada pelo ambiente)
   - https://localhost:5001 (quando habilitado)
   - Swagger em http://localhost:5000/swagger

## Configuração do JWT

O projeto lê as configurações de autenticação no arquivo appsettings.json. Atualize os valores de `Jwt` antes de usar a API em ambiente real:

```json
"Jwt": {
  "key": "sua-chave-secreta-muito-longa",
  "Issuer": "enderecoAPI.com.br",
  "Audience": "enderecoAPI.com.br"
}
```

## Endpoints principais

### Login
- POST /api/login
- Corpo esperado:

```json
{
  "usuario": "admin",
  "senha": "123456"
}
```

### Produtos
- GET /api/produtos
- GET /api/produtos/{id}
- POST /api/produtos
- PUT /api/produtos/{id}
- DELETE /api/produtos/{id}

As rotas de criação, atualização e remoção exigem um token JWT válido e permissão de papel `adimin`.

## Estrutura do projeto

- Controllers: controladores da API
- Services: regras de negócio e acesso aos dados
- Databases: contexto do Entity Framework e configuração do banco
- Models: entidades do domínio
- Migrations: histórico de migrações do banco

## Observações

- O banco de dados utilizado é SQLite.
- O projeto já conta com migrações, então a estrutura do banco pode ser criada automaticamente com o Entity Framework.
- Para testes via Swagger, faça login primeiro e copie o token gerado para o campo de autorização.
