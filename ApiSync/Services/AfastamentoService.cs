using ApiSync.Models;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Xml;

namespace ApiSync.Services
{
    public class AfastamentoService
    {
        public List<PesquisaAfastamento> BuscaAusencias()
        {
            var dataHoje = DateTime.UtcNow.AddDays(+10);
            var dataPesquisa = dataHoje.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            List<PesquisaAfastamento> lista = new List<PesquisaAfastamento>();

            var afastUrl = "https://web38.seniorcloud.com.br:30401/g5-senior-services/rubi_Synccom_senior_g5_rh_fp_integracoes";

            var body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                @"  <soapenv:Body>" + "\n" +
                                @"    <ser:ConsultarTabelas>" + "\n" +
                                @"      <user>cliente_webservice</user>" + "\n" +
                                @"      <password>2easy@2022</password>" + "\n" +
                                @"      <encryption>0</encryption>" + "\n" +
                                @"      <parameters>" + "\n" +
                                @"        <consulta>" + "\n" +
                                @"          <id></id>" + "\n" +
                                @"          <tabela>R038AFA</tabela>" + "\n" +
                                @"          <campos>NumCad,DatAfa,DatTer,SitAfa</campos>" + "\n" +
                                @"          <ordenacao></ordenacao>" + "\n" +
                                @"          <filtro>" + "\n" +
                                @"            <campo>DatAfa</campo>" + "\n" +
                                @"            <condicao>></condicao>" + "\n" +
                                @"            <valor>" + dataPesquisa + "</valor>" + "\n" +
                                @"          </filtro>" + "\n" +
                                @"        </consulta>" + "\n" +
                                @"      </parameters>" + "\n" +
                                @"    </ser:ConsultarTabelas>" + "\n" +
                                @"  </soapenv:Body>" + "\n" +
                                @"</soapenv:Envelope>";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(afastUrl);
            req.ContentType = "text/xml";
            req.Method = "POST";

            using (Stream stm = req.GetRequestStream())
            {
                using (StreamWriter stmw = new StreamWriter(stm))
                {
                    stmw.Write(body);
                }
            }

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                EscreverErroEmArquivo("Erro na busca de Admitidos no endpoint de Afastamentos!");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(response.GetResponseStream());

            XmlNodeList result = xmlDoc.SelectNodes("//resultado//campo");

            List<Afastamento> lista1 = new List<Afastamento>();


            for (int node = 0; node < result.Count; node++)
            {
                try
                {
                    PesquisaAfastamento afastamentos = new PesquisaAfastamento();

                    if (result[node]["nome"].InnerXml == "NUMCAD")
                    {
                        afastamentos.numCad = Convert.ToInt32(result[node]["valor"].InnerText);
                        node++;
                        if (result[node]["nome"].InnerXml == "DATAFA")
                        {
                            var datAfaOriginal = result[node].SelectSingleNode("valor").InnerText;
                            DateTime datAfaConvertida = DateTime.ParseExact(datAfaOriginal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            afastamentos.datAfa = datAfaConvertida.ToString("yyyy-MM-dd");

                            node++;
                        }
                        if (result[node]["nome"].InnerXml == "DATTER")
                        {
                            var datFerOriginal = result[node].SelectSingleNode("valor").InnerText;
                            DateTime datFerConvertida = DateTime.ParseExact(datFerOriginal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            afastamentos.datTer = datFerConvertida.ToString("yyyy-MM-dd");
                            node++;
                        }
                        if (result[node]["nome"].InnerXml == "SITAFA")
                        {
                            afastamentos.sitAfa = Convert.ToInt32(result[node]["valor"].InnerText);
                        }
                    }

                    var afastBody = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                    @"  <soapenv:Body>" + "\n" +
                                    @"    <ser:ConsultarTabelas>" + "\n" +
                                    @"      <user>cliente_webservice</user>" + "\n" +
                                    @"      <password>2easy@2022</password>" + "\n" +
                                    @"      <encryption>0</encryption>" + "\n" +
                                    @"      <parameters>" + "\n" +
                                    @"        <consulta>" + "\n" +
                                    @"          <id></id>" + "\n" +
                                    @"          <tabela>R010SIT</tabela>" + "\n" +
                                    @"          <campos>DesSit</campos>" + "\n" +
                                    @"          <ordenacao></ordenacao>" + "\n" +
                                    @"          <filtro>" + "\n" +
                                    @"            <campo>CodSit</campo>" + "\n" +
                                    @"            <condicao>=</condicao>" + "\n" +
                                    @"            <valor>" + afastamentos.sitAfa + "</valor>" + "\n" +
                                    @"          </filtro>" + "\n" +
                                    @"        </consulta>" + "\n" +
                                    @"      </parameters>" + "\n" +
                                    @"    </ser:ConsultarTabelas>" + "\n" +
                                    @"  </soapenv:Body>" + "\n" +
                                    @"</soapenv:Envelope>";

                    HttpWebRequest requisicao = (HttpWebRequest)WebRequest.Create(afastUrl);
                    requisicao.ContentType = "text/xml";
                    requisicao.Method = "POST";

                    using (Stream afaStm = requisicao.GetRequestStream())
                    {
                        using (StreamWriter afaStmw = new StreamWriter(afaStm))
                        {
                            afaStmw.Write(afastBody);
                        }
                    }

                    HttpWebResponse resposta = (HttpWebResponse)requisicao.GetResponse();

                    XmlDocument afaXmlDoc = new XmlDocument();
                    afaXmlDoc.Load(resposta.GetResponseStream());

                    XmlNodeList resultado = afaXmlDoc.SelectNodes("//resultado//campo");

                    for (int i = 0; i < resultado.Count; i++)
                    {
                        afastamentos.desSit = resultado[i]["valor"].InnerText;
                    }

                    lista.Add(afastamentos);
                }
                catch (Exception ex)
                {
                    string mensagemDeErro = $"Erro ao processar o item : {ex.Message}";
                    EscreverErroEmArquivo(mensagemDeErro);
                    continue;
                }
            }

            return lista;
        }

        private static void EscreverErroEmArquivo(string mensagem)
        {
            DateTime dataAtual = DateTime.Now;

            string diretorioLogBase = "C:\\sincronizadorLogs\\BuscaAfastamentos";


            string diretorioAnoMes = Path.Combine(diretorioLogBase, dataAtual.ToString("yyyy"), dataAtual.ToString("MM"));

            if (!Directory.Exists(diretorioAnoMes))
            {
                Directory.CreateDirectory(diretorioAnoMes);
            }

            string nomeArquivo = $"{dataAtual.ToString("dd")}_logDeBuscaAfastamentos.txt";

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
