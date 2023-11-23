using ApiSync.Models;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ApiSync.Services
{
    public class AdmitidosService
    {
        public string? cpf { get; set; }
        public List<Funcionario> Admitidos()
        {
            List<Funcionario> lista = new List<Funcionario>();

            var inicio = DateTime.UtcNow.AddDays(-20);
            var fim = DateTime.UtcNow.AddDays(+10);

            var iniPer = inicio.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var fimPer = fim.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            string _webServiceUrl = "https://web38.seniorcloud.com.br:30401/g5-senior-services/rubi_Synccom_senior_g5_rh_fp_colaboradoresAdmitidos";
            string consultaPis = "https://web38.seniorcloud.com.br:30401/g5-senior-services/rubi_Synccom_senior_g5_rh_fp_integracoes";

            var soap = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                @"   <soapenv:Header/>" + "\n" +
                @"   <soapenv:Body>" + "\n" +
                @"      <ser:ColaboradoresAdmitidos>" + "\n" +
                @"         <user>cliente_webservice</user>" + "\n" +
                @"         <password>2easy@2022</password>" + "\n" +                
                @"         <encryption>0</encryption>" + "\n" +
                @"         <parameters>" + "\n" +
                @"            <numEmp>1</numEmp>" + "\n" +
                @"            <abrTipCol>1</abrTipCol>" + "\n" +
                @"            <iniPer>" + iniPer + "</iniPer>" + "\n" +  
                @"            <fimPer>" + fimPer + "</fimPer>" + "\n" +
                @"            <abrNumCad>?</abrNumCad>" + "\n" +
                @"            <tipBus>?</tipBus>" + "\n" +
                @"            <codTap>?</codTap>" + "\n" +
                @"            <codThp>?</codThp>" + "\n" +
                @"            <datRef>?</datRef>" + "\n" +
                @"         </parameters>" + "\n" +
                @"      </ser:ColaboradoresAdmitidos>" + "\n" +
                @"   </soapenv:Body>" + "\n" +
                @"</soapenv:Envelope>" + "\n" +
                @"";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_webServiceUrl);
            req.ContentType = "text/xml";
            req.Method = "POST";

            using (Stream stm = req.GetRequestStream())
            {
                using (StreamWriter stmw = new StreamWriter(stm))
                {
                    stmw.Write(soap);
                }
            }

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                EscreverErroEmArquivo("Erro na busca de Admitidos no endpoint de Admitidos!");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(response.GetResponseStream());

            XmlNodeList result = xmlDoc.DocumentElement.FirstChild.FirstChild.FirstChild.SelectNodes("TMCSColaboradores");

            foreach (XmlElement item in result)
            {
                try
                {
                    Funcionario func = new Funcionario();
                    Funcionario funcionario = new Funcionario();
                    //func.id = Convert.ToInt32(item.SelectSingleNode("numEmp").InnerText);
                    func.matricula = Convert.ToInt32(item.SelectSingleNode("numCad").InnerText);
                    func.nome = item.SelectSingleNode("nomFun").InnerText;

                    var dataOriginal = item.SelectSingleNode("datAdm").InnerText;
                    DateTime dataConvertida = DateTime.ParseExact(dataOriginal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    func.data_admissao = dataConvertida.ToString("yyyy-MM-dd");

                    func.id_cargo = item.SelectSingleNode("codCar") == null ? 99 : Convert.ToInt32(item.SelectSingleNode("codCar").InnerText);
                    func.ativo = item.SelectSingleNode("desSit").InnerText == "Trabalhando" ? 1 : 0;

                    var dt_nasc_original = item.SelectSingleNode("datNas").InnerText;
                    DateTime dt_nasc_convertida = DateTime.ParseExact(dt_nasc_original, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    func.data_nascimento = dt_nasc_convertida.ToString("yyyy-MM-dd");                    

                    //func.data_nascimento = item.SelectSingleNode("datNas") == null ? new DateTime?() : Convert.ToDateTime(item.SelectSingleNode("datNas").InnerText);
                    func.id_centro_custo = item.SelectSingleNode("codCcu") == null ? new int?() : Convert.ToInt32(item.SelectSingleNode("codCcu").InnerText);
                    var cpf = item.SelectSingleNode("numCpf").InnerText;
                    this.cpf = cpf.Replace(".", "").Replace("-", "");
                    func.login = "W" + func.nome.Split(' ')[0].ToUpper() + "_" + func.matricula;
                    func.hash = CreateMD5(func.login);
                    var data_atual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                    func.data_atualiz_senha = Convert.ToDateTime(data_atual);
                    func.data_atualiz_tel = Convert.ToDateTime(data_atual);
                    func.ult_atualizacao = Convert.ToDateTime(data_atual);
                    var telefone = item.SelectSingleNode("dddTel").InnerText + "" + item.SelectSingleNode("numTel").InnerText;
                    func.telefone = telefone.Substring(0, 11);
                    //func.telefone = item.SelectSingleNode("dddTel").InnerText + "" + item.SelectSingleNode("numTel").InnerText;
                    func.matricula_supervisor = null;
                    func.primeiro_acesso = 1;

                    if(func.id_cargo == 1)
                    {
                        func.agentid = 0;
                        func.jornada_semanal = 36;
                        func.apovador_escalas_excepcionais = "N";
                        func.aprovador_HE = "N";
                    }
                    else if(func.id_cargo == 10035 || func.id_cargo == 10153 || func.id_cargo == 2)
                    {
                        func.agentid_aspect = null;
                        func.apovador_escalas_excepcionais = "S";
                        func.aprovador_HE = "N";
                        func.agentid = null;
                        func.jornada_semanal = 44;
                    }
                    else
                    {
                        func.agentid_aspect = null;
                        func.agentid = null;
                        func.jornada_semanal = 44;
                        func.apovador_escalas_excepcionais = "N";
                        func.aprovador_HE = "N";
                    }

                    func.tipo_intervalo = 1;
                    func.ativa_desktop = null;
                    func.tipo_escala = 1;

                    var body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                @"  <soapenv:Body>" + "\n" +
                                @"    <ser:ConsultarTabelas>" + "\n" +
                                @"      <user>cliente_webservice</user>" + "\n" +
                                @"      <password>2easy@2022</password>" + "\n" +
                                @"      <encryption>0</encryption>" + "\n" +
                                @"      <parameters>" + "\n" +
                                @"        <consulta>" + "\n" +
                                @"          <id></id>" + "\n" +
                                @"          <tabela>R034FUN</tabela>" + "\n" +
                                @"          <campos>NumPis</campos>" + "\n" +
                                @"          <ordenacao></ordenacao>" + "\n" +
                                @"          <filtro>" + "\n" +
                                @"            <campo>NUMCPF</campo>" + "\n" +
                                @"            <condicao>=</condicao>" + "\n" +
                                @"            <valor>" + this.cpf + "</valor>" + "\n" +
                                @"          </filtro>" + "\n" +
                                @"        </consulta>" + "\n" +
                                @"      </parameters>" + "\n" +
                                @"    </ser:ConsultarTabelas>" + "\n" +
                                @"  </soapenv:Body>" + "\n" +
                                @"</soapenv:Envelope>";

                    HttpWebRequest requisiacao = (HttpWebRequest)WebRequest.Create(consultaPis);
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
                        throw new Exception($"Erro na busca do PIS na tabela R034FUN da matrícula {func.matricula}");
                    }

                    XmlDocument xmlPis = new XmlDocument();
                    xmlPis.Load(resposta.GetResponseStream());

                    XmlNodeList resultado = xmlPis.SelectNodes("//valor");
                    for (int i = 0; i < resultado.Count; i++)
                    {
                        func.pis = resultado[i].InnerText;
                    }


                    lista.Add(func);
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

        public string CreateMD5(string password)
        {
            // byte array representation of that string
            byte[] encodedPassword = new UTF8Encoding().GetBytes(password);

            // need MD5 to calculate the hash
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            // string representation (similar to UNIX format)
            string encoded = BitConverter.ToString(hash)
               // without dashes
               .Replace("-", string.Empty)
               // make lowercase
               .ToLower();

            return encoded;
        }

        private static void EscreverErroEmArquivo(string mensagem)
        {
            DateTime dataAtual = DateTime.Now;

            string diretorioLogBase = "C:\\sincronizadorLogs\\BuscaAdmitidos";


            string diretorioAnoMes = Path.Combine(diretorioLogBase, dataAtual.ToString("yyyy"), dataAtual.ToString("MM"));

            if (!Directory.Exists(diretorioAnoMes))
            {
                Directory.CreateDirectory(diretorioAnoMes);
            }

            string nomeArquivo = $"{dataAtual.ToString("dd")}_logDeBuscaAdmitidos.txt";

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
