using System.Net;
using System.Xml;
using System;
using ApiSync.Models;

namespace ApiSync.Services
{
    public class FuncionarioService
    {
        public List<PesquisaCargo> BuscaAtualizacaoCargo()
        {
            List<PesquisaCargo> lista = new List<PesquisaCargo>();

            try
            {
                string dataPesquisa = DateTime.Now.AddDays(-50).ToString("dd/MM/yyyy");

                string pesquisa = "https://web38.seniorcloud.com.br/g5-senior-services/rubi_Synccom_senior_g5_rh_fp_integracoes";
                var body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                    @"  <soapenv:Body>" + "\n" +
                                    @"    <ser:ConsultarTabelas>" + "\n" +
                                    @"      <user></user>" + "\n" +
                                    @"      <password></password>" + "\n" +
                                    @"      <encryption>0</encryption>" + "\n" +
                                    @"      <parameters>" + "\n" +
                                    @"        <consulta>" + "\n" +
                                    @"          <id></id>" + "\n" +
                                    @"          <tabela>R034FUN</tabela>" + "\n" +
                                    @"          <campos>NUMCAD,CODCAR</campos>" + "\n" +
                                    @"          <ordenacao></ordenacao>" + "\n" +
                                    @"          <filtro>" + "\n" +
                                    @"            <campo>DATCAR</campo>" + "\n" +
                                    @"            <condicao>></condicao>" + "\n" +
                                    @"            <valor>" + dataPesquisa + "</valor>" + "\n" +
                                    @"          </filtro>" + "\n" +
                                    @"        </consulta>" + "\n" +
                                    @"      </parameters>" + "\n" +
                                    @"    </ser:ConsultarTabelas>" + "\n" +
                                    @"  </soapenv:Body>" + "\n" +
                                    @"</soapenv:Envelope>";

                HttpWebRequest requisiacao = (HttpWebRequest)WebRequest.Create(pesquisa);
                requisiacao.ContentType = "text/xml";
                requisiacao.Method = "POST";

                using (Stream pis = requisiacao.GetRequestStream())
                {
                    using (StreamWriter pisResult = new StreamWriter(pis))
                    {
                        pisResult.Write(body);
                    }
                }

                HttpWebResponse resposta = (HttpWebResponse)requisiacao.GetResponse();

                if (resposta.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Erro ao realizar pesquisa na tabela R034FUN Atualizações de Cargo!");
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(resposta.GetResponseStream());

                XmlNodeList result = xmlDoc.SelectNodes("//resultado//campo");

                for (int i = 0; i < result.Count; i++)
                {
                    PesquisaCargo cargo = new PesquisaCargo();
                    cargo.matricula = Convert.ToInt32(result[i].SelectSingleNode("valor").InnerText);
                    i++;
                    cargo.cargo = Convert.ToInt32(result[i].SelectSingleNode("valor").InnerText);
                    lista.Add(cargo);
                }
            }
            catch (Exception ex)
            {
                string mensagemDeErro = $"Erro ao processar o item : {ex.Message}";
                EscreverErroEmArquivo(mensagemDeErro);
            }

            return lista;
        }

        public List<PesquisaCentroCusto> BuscaAtualizacaoCentroCusto()
        {
            List<PesquisaCentroCusto> lista = new List<PesquisaCentroCusto>();

            try
            {
                string dataPesquisa = DateTime.Now.AddDays(-450).ToString("dd/MM/yyyy");

                string pesquisa = "https://web38.seniorcloud.com.br:30401/g5-senior-services/rubi_Synccom_senior_g5_rh_fp_integracoes";
                var body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                    @"  <soapenv:Body>" + "\n" +
                                    @"    <ser:ConsultarTabelas>" + "\n" +
                                    @"      <user></user>" + "\n" +
                                    @"      <password></password>" + "\n" +
                                    @"      <encryption>0</encryption>" + "\n" +
                                    @"      <parameters>" + "\n" +
                                    @"        <consulta>" + "\n" +
                                    @"          <id></id>" + "\n" +
                                    @"          <tabela>R034FUN</tabela>" + "\n" +
                                    @"          <campos>NUMCAD,CODCCU</campos>" + "\n" +
                                    @"          <ordenacao></ordenacao>" + "\n" +
                                    @"          <filtro>" + "\n" +
                                    @"            <campo>DATCCU</campo>" + "\n" +
                                    @"            <condicao>></condicao>" + "\n" +
                                    @"            <valor>" + dataPesquisa + "</valor>" + "\n" +
                                    @"          </filtro>" + "\n" +
                                    @"        </consulta>" + "\n" +
                                    @"      </parameters>" + "\n" +
                                    @"    </ser:ConsultarTabelas>" + "\n" +
                                    @"  </soapenv:Body>" + "\n" +
                                    @"</soapenv:Envelope>";

                HttpWebRequest requisiacao = (HttpWebRequest)WebRequest.Create(pesquisa);
                requisiacao.ContentType = "text/xml";
                requisiacao.Method = "POST";

                using (Stream pis = requisiacao.GetRequestStream())
                {
                    using (StreamWriter pisResult = new StreamWriter(pis))
                    {
                        pisResult.Write(body);
                    }
                }

                HttpWebResponse resposta = (HttpWebResponse)requisiacao.GetResponse();

                if (resposta.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Erro ao realizar pesquisa na tabela R034FUN Atualizações do Centro de Custo!");
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(resposta.GetResponseStream());

                XmlNodeList result = xmlDoc.SelectNodes("//resultado//campo");

                for (int i = 0; i < result.Count; i++)
                {
                    PesquisaCentroCusto centro_custo = new PesquisaCentroCusto();
                    centro_custo.matricula = Convert.ToInt32(result[i].SelectSingleNode("valor").InnerText);
                    i++;
                    centro_custo.centro_custo = Convert.ToInt32(result[i].SelectSingleNode("valor").InnerText);
                    lista.Add(centro_custo);
                }
            }
            catch (Exception ex)
            {
                string mensagemDeErro = $"Erro ao processar o item : {ex.Message}";
                EscreverErroEmArquivo(mensagemDeErro);
            }

            return lista;
        }

        private static void EscreverErroEmArquivo(string mensagem)
        {
            DateTime dataAtual = DateTime.Now;

            string diretorioLogBase = "C:\\sincronizadorLogs\\BuscaCargoCentroCusto";


            string diretorioAnoMes = Path.Combine(diretorioLogBase, dataAtual.ToString("yyyy"), dataAtual.ToString("MM"));

            if (!Directory.Exists(diretorioAnoMes))
            {
                Directory.CreateDirectory(diretorioAnoMes);
            }

            string nomeArquivo = $"{dataAtual.ToString("dd")}_logDeErros.txt";

            string caminhoArquivoLog = Path.Combine(diretorioAnoMes, nomeArquivo);

            try
            {
                using (StreamWriter writer = new StreamWriter(caminhoArquivoLog, true))
                {
                    writer.WriteLine($"{dataAtual}: {mensagem}");
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine("Erro de E/S ao escrever no arquivo de log: " + ioEx.Message);
            }
        }
    }
}
