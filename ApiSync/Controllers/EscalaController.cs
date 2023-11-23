using ApiSync.Data;
using ApiSync.Models;
using Microsoft.AspNetCore.Mvc;
using ApiSync.Services;
using MySqlConnector;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiSync.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EscalaController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;

        public EscalaController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("buscaEscala")]
        public List<EscalaFuncionario> BuscaEscala()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("WebApiDatabase");

            EscalaService escalaService = new EscalaService();
            List<EscalaFuncionario> escalaFuncionarios = new List<EscalaFuncionario>();
            List<FuncionarioPesquisa> funcionarios = new List<FuncionarioPesquisa>();

            funcionarios = _context.FuncionariosPesquisa.Where(f => f.ativo == 1).ToList();

            escalaFuncionarios = escalaService.PesquisaEscala(funcionarios);

            try
            {
                using(MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var escala in escalaFuncionarios)
                    {
                        string query = @"
                        INSERT INTO funcionario_escala (
                                                    id_funcionario, 
                                                    dia_semana, 
                                                    tipo_escala, 
                                                    ini_expediente, 
                                                    ini_intervalo, 
                                                    fim_intervalo, 
                                                    fim_expediente)
                                            VALUES (
                                                    @id_funcionario, 
                                                    @dia_semana, 
                                                    @tipo_escala, 
                                                    @ini_expediente, 
                                                    @ini_intervalo, 
                                                    @fim_intervalo, 
                                                    @fim_expediente)
                                            ON DUPLICATE KEY UPDATE
                                                    id_funcionario = @id_funcionario,
                                                    tipo_escala = @tipo_escala, 
                                                    ini_expediente = @ini_expediente, 
                                                    fim_expediente = @fim_expediente;";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id_funcionario", escala.id_funcionario);
                            command.Parameters.AddWithValue("@dia_semana", escala.dia_semana);
                            command.Parameters.AddWithValue("@tipo_escala", escala.tipo_escala);
                            command.Parameters.AddWithValue("@ini_expediente", escala.ini_expediente);
                            command.Parameters.AddWithValue("@ini_intervalo", escala.ini_intervalo);
                            command.Parameters.AddWithValue("@fim_intervalo", escala.fim_intervalo);
                            command.Parameters.AddWithValue("@fim_expediente", escala.fim_expediente);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                SalvarLogDeSucesso("Sincronizador executado com sucesso!");
            }
            catch (InvalidOperationException ex)
            {
                SalvarLogDeSucesso(ex.Message);
            }

            return escalaFuncionarios;
        }

        [HttpGet("porMatricula")]
        public List<EscalaFuncionario> BuscaPorMatricula(int matricula)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("WebApiDatabase");

            List<EscalaFuncionario> retornoEscala = new List<EscalaFuncionario>();
            EscalaService escalaService = new EscalaService();
            List<FuncionarioPesquisa> funcionarios = new List<FuncionarioPesquisa>();

            FuncionarioPesquisa funcionario = _context.FuncionariosPesquisa.Where(f => f.matricula == matricula).FirstOrDefault();
            funcionarios.Add(funcionario);

            retornoEscala = escalaService.PesquisaEscala(funcionarios);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                foreach (var escala in retornoEscala)
                {
                    string query = @"
                    INSERT INTO funcionario_escala (
                                                    id_funcionario, 
                                                    dia_semana, 
                                                    tipo_escala, 
                                                    ini_expediente, 
                                                    ini_intervalo, 
                                                    fim_intervalo, 
                                                    fim_expediente)
                                            VALUES (
                                                    @id_funcionario, 
                                                    @dia_semana, 
                                                    @tipo_escala, 
                                                    @ini_expediente, 
                                                    @ini_intervalo, 
                                                    @fim_intervalo, 
                                                    @fim_expediente)
                                            ON DUPLICATE KEY UPDATE
                                                    id_funcionario = @id_funcionario,
                                                    tipo_escala = @tipo_escala, 
                                                    ini_expediente = @ini_expediente, 
                                                    fim_expediente = @fim_expediente;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_funcionario", escala.id_funcionario);
                        command.Parameters.AddWithValue("@dia_semana", escala.dia_semana);
                        command.Parameters.AddWithValue("@tipo_escala", escala.tipo_escala);
                        command.Parameters.AddWithValue("@ini_expediente", escala.ini_expediente);
                        command.Parameters.AddWithValue("@ini_intervalo", escala.ini_intervalo);
                        command.Parameters.AddWithValue("@fim_intervalo", escala.fim_intervalo);
                        command.Parameters.AddWithValue("@fim_expediente", escala.fim_expediente);

                        command.ExecuteNonQuery();
                    }
                }
            }

            return retornoEscala;
        }

        private void SalvarLogDeSucesso(string mensagem)
        {
            DateTime dataAtual = DateTime.Now;

            string diretorioLogBase = "C:\\sincronizadorLogs\\BuscaEscala";

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
