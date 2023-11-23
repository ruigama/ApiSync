using ApiSync.Data;
using ApiSync.Models;
using ApiSync.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace ApiSync.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class AdmitidosController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;

        public AdmitidosController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("buscaAdmitidos")]
        public async Task<IActionResult> BuscaAdmitidos()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            string connectionString = configuration.GetConnectionString("WebApiDatabase");

            try
            {
                List<Funcionario> funcionarios = new List<Funcionario>();
                AdmitidosService admitidos = new AdmitidosService();

                funcionarios = admitidos.Admitidos();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var funcionario in funcionarios)
                    {
                        string consulta = @"SELECT 1 FROM funcionarios WHERE matricula = @matricula LIMIT 1;";

                        using (MySqlCommand command = new MySqlCommand(consulta, connection))
                        {
                            command.Parameters.AddWithValue("@matricula", funcionario.matricula);
                            var result = command.ExecuteScalar();

                            if (result != null)
                            {
                                continue;
                            }
                            else
                            {
                                string cargo = @"SELECT id FROM cargos WHERE id_cargo_humanus = @id_cargo_humanus LIMIT 1;";

                                using(MySqlCommand sqlCommand = new MySqlCommand(cargo, connection))
                                {
                                    sqlCommand.Parameters.AddWithValue("@id_cargo_Humanus", funcionario.id_cargo);
                                    var resultado_cargo = sqlCommand.ExecuteScalar();

                                    if(resultado_cargo != null)
                                    {
                                        funcionario.id_cargo = Convert.ToInt32(resultado_cargo);
                                    }
                                    else
                                    {
                                        funcionario.id_cargo = 99;
                                    }
                                }

                                if(funcionario.id_cargo == 2)
                                {
                                    string campanha = @"SELECT id FROM campanhas WHERE id_centro_custo = @id_centro_custo LIMIT 1;";
                                    using(MySqlCommand sqlCampanha = new MySqlCommand(campanha, connection))
                                    {
                                        sqlCampanha.Parameters.AddWithValue("@id_centro_custo", funcionario.id_centro_custo);
                                        var resultado_campanha = sqlCampanha.ExecuteScalar();

                                        funcionario.id_campanha = Convert.ToInt32(resultado_campanha);
                                    }
                                }

                                string query = @"
                                INSERT INTO funcionarios (
                                                            matricula, 
                                                            nome,
                                                            agentid,
                                                            data_admissao,
                                                            pis,
                                                            login,
                                                            hash,
                                                            id_cargo, 
                                                            matricula_supervisor,
                                                            id_campanha,
                                                            primeiro_acesso,
                                                            ativo, 
                                                            data_nascimento,
                                                            ativa_desktop,
                                                            tipo_intervalo,
                                                            jornada_semanal,
                                                            tipo_escala,
                                                            aprovador_HE,
                                                            apovador_escalas_excepcionais,
                                                            id_centro_custo,
                                                            telefone,
                                                            data_atualiz_tel,
                                                            data_atualiz_senha,
                                                            ult_atualizacao)
                                                    VALUES (
                                                            @matricula, 
                                                            @nome, 
                                                            @agentid, 
                                                            @data_admissao, 
                                                            @pis, 
                                                            @login, 
                                                            @hash,
                                                            @id_cargo,
                                                            @matricula_supervisor,
                                                            @id_campanha,
                                                            @primeiro_acesso,
                                                            @ativo,
                                                            @data_nascimento,
                                                            @ativa_desktop,
                                                            @tipo_intervalo,
                                                            @jornada_semanal,
                                                            @tipo_escala,
                                                            @aprovador_HE,
                                                            @apovador_escalas_excepcionais,
                                                            @id_centro_custo,
                                                            @telefone,
                                                            @data_atualiz_tel,
                                                            @data_atualiz_senha,
                                                            @ult_atualizacao
                                                            )
                                                    ON DUPLICATE KEY UPDATE
                                                            matricula = @matricula,
                                                            id_cargo = @id_cargo, 
                                                            id_centro_custo = @id_centro_custo;";

                                using (MySqlCommand command1 = new MySqlCommand(query, connection))
                                {
                                    command1.Parameters.AddWithValue("@matricula", funcionario.matricula);
                                    command1.Parameters.AddWithValue("@nome", funcionario.nome);
                                    command1.Parameters.AddWithValue("@agentid", funcionario.agentid);
                                    command1.Parameters.AddWithValue("@data_admissao", funcionario.data_admissao);
                                    command1.Parameters.AddWithValue("@pis", funcionario.pis);
                                    command1.Parameters.AddWithValue("@login", funcionario.login);
                                    command1.Parameters.AddWithValue("@hash", funcionario.hash);
                                    command1.Parameters.AddWithValue("@id_cargo", funcionario.id_cargo);
                                    command1.Parameters.AddWithValue("@matricula_supervisor", funcionario.matricula_supervisor);
                                    command1.Parameters.AddWithValue("@id_campanha", funcionario.id_campanha);
                                    command1.Parameters.AddWithValue("@primeiro_acesso", funcionario.primeiro_acesso);
                                    command1.Parameters.AddWithValue("@ativo", funcionario.ativo);
                                    command1.Parameters.AddWithValue("@data_nascimento", funcionario.data_nascimento);
                                    command1.Parameters.AddWithValue("@ativa_desktop", funcionario.ativa_desktop);
                                    command1.Parameters.AddWithValue("@tipo_intervalo", funcionario.tipo_intervalo);
                                    command1.Parameters.AddWithValue("@jornada_semanal", funcionario.jornada_semanal);
                                    command1.Parameters.AddWithValue("@tipo_escala", funcionario.tipo_escala);
                                    command1.Parameters.AddWithValue("@aprovador_HE", funcionario.aprovador_HE);
                                    command1.Parameters.AddWithValue("@apovador_escalas_excepcionais", funcionario.apovador_escalas_excepcionais);
                                    command1.Parameters.AddWithValue("@id_centro_custo", funcionario.id_centro_custo);
                                    command1.Parameters.AddWithValue("@telefone", funcionario.telefone);
                                    command1.Parameters.AddWithValue("@data_atualiz_tel", funcionario.data_atualiz_tel);
                                    command1.Parameters.AddWithValue("@data_atualiz_senha", funcionario.data_atualiz_senha);
                                    command1.Parameters.AddWithValue("@ult_atualizacao", funcionario.ult_atualizacao);

                                    command1.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                return Ok(funcionarios);
            }
            catch (Exception ex)
            {
                SalvarLogDeSucesso(ex.Message);
                return StatusCode(500, "Ocorreu um erro durante a operação.");
            }
        }

        private void SalvarLogDeSucesso(string mensagem)
        {
            DateTime dataAtual = DateTime.Now;

            string diretorioLogBase = "C:\\sincronizadorLogs\\BuscaAdmitidos";

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
