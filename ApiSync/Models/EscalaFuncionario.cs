using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSync.Models
{
    [Table("funcionario_escala")]
    public class EscalaFuncionario
    {
        [Key]
        public int id_funcionario { get; set; }
        [Key]
        public int dia_semana { get; set; }
        public int tipo_escala { get; set; }
        public TimeSpan? ini_expediente { get; set; }
        public TimeSpan? ini_intervalo { get; set; }
        public TimeSpan? fim_intervalo { get; set; }
        public TimeSpan? fim_expediente { get; set; }
    }
}
