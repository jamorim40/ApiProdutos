using System.ComponentModel.DataAnnotations;

namespace CadastroProdutos.Models.Entitys
{
    public class Login
    {
        [Required]
        public int Id {get;set;}
        [Required]
        public string Usuario{get;set;} = string.Empty;
        [Required]
        public string Senha{get;set;} = string.Empty;
        [Required]
        public string Papel{get;set;} = string.Empty;
    }
}