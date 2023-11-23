using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSync.Models
{
    [Table("centro_custo")]
    public class CentroCusto
    {
        [Key]
        public int? id { get; set; }
        public int? nome { get; set; }
    }
}
