using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSync.Models
{
    public class Funcionario
    {
        [Key]
        public int id { get; set; }
        public string? nome { get; set; }
        public int? matricula { get; set; }
        public int? id_tipo_discador { get; set; }
        public int? id_campanha { get; set; }
        public int? agentid { get; set; }
        public int? agentid_aspect { get; set; }
        public int? ramal { get; set; }
        public string? data_admissao { get; set; }
        public string? pis { get; set; }
        public string? login { get; set; }
        public int? id_cargo { get; set; }
        public int? matricula_supervisor { get; set; }
        public string? foto { get; set; }
        public int? primeiro_acesso { get; set; }
        public int? ativo { get; set; }
        public int? ativa_desktop { get; set; }
        public string? data_nascimento { get; set; }
        public int? tipo_intervalo { get; set; }
        public int? jornada_semanal { get; set; }
        public int? tipo_escala { get; set; }
        public string? aprovador_HE { get; set; }
        public string? apovador_escalas_excepcionais { get; set; }
        public int? id_centro_custo { get; set; }
        public string? telefone { get; set; }
        public int? quartil { get; set; }
        public DateTime? data_atualiz_tel { get; set; }
        public DateTime? data_atualiz_senha { get; set; }
        public int? empresa { get; set; }
        public DateTime? ult_atualizacao { get; set; }
        public string? login_santander { get; set; }
        public string? hash { get; set; }
        public string? desc_horario { get; set; }
    }
}
