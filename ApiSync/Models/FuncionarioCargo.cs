using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSync.Models
{
    public class FuncionarioCargo
    {
        [key]
        public int id { get; set; }
        public int matricula { get; set; }
        public int? id_cargo { get; set; }
    }
}
