using System.ComponentModel.DataAnnotations;

namespace ApiSync.Models
{
    public class Cargo
    {
        [Key]
        public int id { get; set; }
        public string? cargo { get; set; }
        public string? descricao { get; set; }
        public int? tipo_acesso { get; set; }
        public int? supervisiona { get; set; }
        public int? id_cargo_humanus { get; set; }
    }
}
