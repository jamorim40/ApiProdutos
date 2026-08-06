using System.ComponentModel.DataAnnotations;

namespace CadastroProddutos
{
  
    public class Produto
    {
        public int Id{get;set;}
        [Required(ErrorMessage ="O nome do produto é obrigatório!")]
        [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        public string Nome{get;set;} = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage ="O preço deve ser maior que zero.")]
        public decimal Preco{get;set;} = 0;
        public int Estoque{get;set;} = 0;
        public bool Condicao{get;set;} = true;
    }

}
