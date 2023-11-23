using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSync.Models
{
    public class PesquisaAfastamento
    {
        public int? numCad { get; set; }
        public string? datAfa { get; set; }
        public string? datTer { get; set; }
        public int? sitAfa { get; set; }
        public string? desSit { get; set; }
    }
}
