using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSync.Models
{
    [Table("funcionarios")]
    public class FuncionarioPesquisa
    {
        [key]
        public int id { get; set; }
        public string? nome { get; set; }
        public int matricula { get; set; }
        public int? ativo { get; set; }
    }
}
