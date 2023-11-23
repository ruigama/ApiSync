using ApiSync.Data;
using ApiSync.Models;
using ApiSync.Services;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data.Entity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiSync.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;

        public FuncionarioController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("AtualizarCargo")]
        public List<PesquisaCargo> AtualizarCargo() 
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("WebApiDatabase");
            int? matricula = null;

            List<PesquisaCargo> lista = new List<PesquisaCargo>();
            FuncionarioService funcionarioService = new FuncionarioService();

            lista = funcionarioService.BuscaAtualizacaoCargo();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var individual in lista)
                    {
                        Funcionario funcionario = new Funcionario();
                        Cargo cargo = new Cargo();

                        cargo = _context.Cargos.SingleOrDefault(c => c.id_cargo_humanus == individual.cargo);

                        string queryFuncionario = "SELECT * FROM funcionarios WHERE matricula = @matricula AND ativo = @ativo;";

                        using (MySqlCommand command = new MySqlCommand(queryFuncionario, connection))
                        {
                            command.Parameters.Add("@matricula", MySqlDbType.Int32).Value = individual.matricula;
                            command.Parameters.Add("ativo", MySqlDbType.Int32).Value = 1;

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                { 
                                    matricula = reader.GetInt32("matricula");
                                }
                            }
                        }
                        
                        if (matricula != null)
                        {
                            if (cargo == null)
                            {
                                funcionario.id_cargo = 99;
                            }
                            else
                            {
                                funcionario.id_cargo = cargo.id;
                            }
                            
                            string query = @"UPDATE funcionarios SET id_cargo = @id_cargo WHERE matricula = @matricula;";

                            using(MySqlCommand command = new MySqlCommand(query, connection)) 
                            {
                                command.Parameters.Add("@id_cargo", MySqlDbType.Int32).Value = funcionario.id_cargo;
                                command.Parameters.Add("@matricula", MySqlDbType.Int32).Value = matricula;

                                command.ExecuteNonQuery();
                            }

                            string mensagem = $"Matrícula {funcionario.matricula} atualizada!";
                            SalvarLogDeSucesso(mensagem);
                        }
                    }
                }                
            }
            catch (InvalidOperationException ex)
            {
                SalvarLogDeSucesso(ex.Message);
            }

            return lista;
        }

        [HttpGet("AtualizarCentroCusto")]
        public List<PesquisaCentroCusto> AtualizarCentroCusto()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            string connectionString = configuration.GetConnectionString("WebApiDatabase");
            int? matricula = null;

            List<PesquisaCentroCusto> lista = new List<PesquisaCentroCusto>();
            FuncionarioService funcionarioService = new FuncionarioService();

            lista = funcionarioService.BuscaAtualizacaoCentroCusto();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var individual in lista)
                    {
                        Funcionario funcionario = new Funcionario();

                        string queryFuncionario = "SELECT * FROM funcionarios WHERE matricula = @matricula AND ativo = @ativo;";

                        using (MySqlCommand command = new MySqlCommand(queryFuncionario, connection))
                        {
                            command.Parameters.Add("@matricula", MySqlDbType.Int32).Value = individual.matricula;
                            command.Parameters.Add("ativo", MySqlDbType.Int32).Value = 1;

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    matricula = reader.GetInt32("matricula");
                                }
                            }
                        }

                        if (matricula != null)
                        {
                            funcionario.id_centro_custo = individual.centro_custo;

                            string query = @"UPDATE funcionarios SET id_centro_custo = @id_centro_custo WHERE matricula = @matricula;";

                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.Add("@id_centro_custo", MySqlDbType.Int32).Value = funcionario.id_centro_custo;
                                command.Parameters.Add("@matricula", MySqlDbType.Int32).Value = matricula;

                                command.ExecuteNonQuery();
                            }

                            string mensagem = $"Matrícula {funcionario.matricula} com centro de custo {funcionario.id_centro_custo}!";
                            SalvarLogDeSucesso(mensagem);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                SalvarLogDeSucesso(ex.Message);
            }

            return lista;
        }

        private void SalvarLogDeSucesso(string mensagem)
        {
            DateTime dataAtual = DateTime.Now;

            string diretorioLogBase = "C:\\sincronizadorLogs\\AtualizaFuncionario";

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
