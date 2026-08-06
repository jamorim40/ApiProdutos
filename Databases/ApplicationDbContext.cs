using CadastroProddutos;
using CadastroProdutos.Models.Entitys;
using Microsoft.EntityFrameworkCore;

namespace CadastroProdutos.Databases
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){}
        
        public DbSet<Produto> Produtos{get;set;}
        public DbSet<Login> Logins{get;set;}
    }
}