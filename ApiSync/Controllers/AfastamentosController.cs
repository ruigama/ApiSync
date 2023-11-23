using ApiSync.Data;
using ApiSync.Models;
using ApiSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiSync.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AfastamentosController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;

        public AfastamentosController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("buscaAfastamentos")]
        public List<PesquisaAfastamento> BuscaAfastamentos()
        {
            AfastamentoService afastamentoService = new AfastamentoService();
            List<PesquisaAfastamento> afastamentos = afastamentoService.BuscaAusencias();

            foreach(PesquisaAfastamento afastamento in afastamentos) 
            {
                try
                {
                    Afastamento afastado = new Afastamento();
                    afastado.dt_inicial = Convert.ToDateTime(afastamento.datAfa);
                    afastado.dt_final = Convert.ToDateTime(afastamento.datTer);

                    FuncionarioPesquisa funcionario = _context.FuncionariosPesquisa.Where(f => f.matricula == afastamento.numCad).FirstOrDefault();
                    List<Afastamento> id_afastamento = (List<Afastamento>)_context.Afastamentos.Where
                        (
                            af => af.id_funcionario == funcionario.id
                            && af.dt_inicial == afastado.dt_inicial
                            && af.dt_final == afastado.dt_final
                        ).ToList();

                    if (id_afastamento.Count == 0)
                    {
                        afastado.id_funcionario = funcionario.id;
                        afastado.tipo_afastamento = (int)afastamento.sitAfa;
                        TipoAfastamento tipos_afastamento = _context.TipoAfastamentos.Where(taf => taf.id == afastado.tipo_afastamento).FirstOrDefault();
                        afastado.descricao = tipos_afastamento.descricao;

                        string mensagem = $"Inserido afastamento para a matrícula {funcionario.matricula}!";
                        SalvarLogDeSucesso(mensagem);

                        _context.Add(afastado);
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    SalvarLogDeSucesso(ex.Message);
                }
            }
            _context.SaveChanges();

            return afastamentos;
        }

        private void SalvarLogDeSucesso(string mensagem)
        {
            DateTime dataAtual = DateTime.Now;

            string diretorioLogBase = "C:\\sincronizadorLogs\\InsereAfastamento";

            string diretorioAnoMes = Path.Combine(diretorioLogBase, dataAtual.ToString("yyyy"), dataAtual.ToString("MM"));

            if (!Directory.Exists(diretorioAnoMes))
            {
                Directory.CreateDirectory(diretorioAnoMes);
            }

            string nomeArquivo = $"{dataAtual.ToString("dd")}_baseDeDados.txt";

            string caminhoArquivoLog = Path.Combine(diretorioAnoMes, nomeArquivo);

            try
            {
                using (StreamWriter writer = new StreamWriter(caminhoArquivoLog))
                {
                    writer.WriteLine($"{dataAtual}: {mensagem}");
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine("Erro de E/S ao escrever no arquivo de log de sucesso: " + ioEx.Message);
            }
        }
    }
}
