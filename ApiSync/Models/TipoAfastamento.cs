using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSync.Models
{
    [Table("tipos_afastamentos")]
    public class TipoAfastamento
    {
        [key]
        public int id { get; set; }
        public string? descricao { get; set; }
    }
}
