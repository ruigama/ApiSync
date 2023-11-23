using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSync.Models
{
    [Table("afastamentos")]
    public class Afastamento
    {
        [key]
        public int id { get; set; }
        public int id_funcionario { get; set; }
        public string? descricao { get; set; }
        public DateTime dt_inicial { get; set; }
        public DateTime dt_final { get; set; }
        public int tipo_afastamento { get; set; }
    }
}
