using System;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using ApiSync.Models;

namespace ApiSync.Services
{
    public class EscalaService
    {
        public List<EscalaFuncionario> PesquisaEscala(List<FuncionarioPesquisa> funcionarios)
        {
            List<EscalaFuncionario> listagem = new List<EscalaFuncionario>();

            
            foreach(FuncionarioPesquisa func in funcionarios)
            {
                try
                {
                    int diaSemana = 7;
                    int diaHoje = Convert.ToInt32(DateTime.Now.DayOfWeek);
                    string dia_escala = null;
                    int numCad = (int)func.matricula;

                    if (diaHoje != 0)
                    {

                        var escalaUrl = "https://web38.seniorcloud.com.br/g5-senior-services/rubi_Synccom_senior_g5_rh_fp_integracoes";

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
                                            @"          <campos>CODESC</campos>" + "\n" +
                                            @"          <ordenacao></ordenacao>" + "\n" +
                                            @"          <filtro>" + "\n" +
                                            @"            <campo>NUMCAD</campo>" + "\n" +
                                            @"            <condicao>=</condicao>" + "\n" +
                                            @"            <valor>" + numCad + "</valor>" + "\n" +
                                            @"          </filtro>" + "\n" +
                                            @"        </consulta>" + "\n" +
                                            @"      </parameters>" + "\n" +
                                            @"    </ser:ConsultarTabelas>" + "\n" +
                                            @"  </soapenv:Body>" + "\n" +
                                            @"</soapenv:Envelope>";

                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(escalaUrl);
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
                            throw new Exception($"Erro na busca da matrícula {func.matricula} na tabela R034FUN");
                        }

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(response.GetResponseStream());

                        XmlNodeList result = xmlDoc.SelectNodes("//resultado//campo");

                        var codEsc = 0;

                        for (int i = 0; i < result.Count; i++)
                        {
                            codEsc = Convert.ToInt32(result[i].SelectSingleNode("valor").InnerText);
                            break;
                        }

                        var bodyPD = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                                    @"  <soapenv:Body>" + "\n" +
                                                    @"    <ser:ConsultarTabelas>" + "\n" +
                                                    @"      <user></user>" + "\n" +
                                                    @"      <password></password>" + "\n" +
                                                    @"      <encryption>0</encryption>" + "\n" +
                                                    @"      <parameters>" + "\n" +
                                                    @"        <consulta>" + "\n" +
                                                    @"          <id></id>" + "\n" +
                                                    @"          <tabela>R006ESC</tabela>" + "\n" +
                                                    @"          <campos>TIPESC</campos>" + "\n" +
                                                    @"          <ordenacao></ordenacao>" + "\n" +
                                                    @"          <filtro>" + "\n" +
                                                    @"            <campo>CODESC</campo>" + "\n" +
                                                    @"            <condicao>=</condicao>" + "\n" +
                                                    @"            <valor>" + codEsc + "</valor>" + "\n" +
                                                    @"          </filtro>" + "\n" +
                                                    @"        </consulta>" + "\n" +
                                                    @"      </parameters>" + "\n" +
                                                    @"    </ser:ConsultarTabelas>" + "\n" +
                                                    @"  </soapenv:Body>" + "\n" +
                                                    @"</soapenv:Envelope>";

                        HttpWebRequest reqPD = (HttpWebRequest)WebRequest.Create(escalaUrl);
                        reqPD.ContentType = "text/xml";
                        reqPD.Method = "POST";

                        using (Stream stm = reqPD.GetRequestStream())
                        {
                            using (StreamWriter stmw = new StreamWriter(stm))
                            {
                                stmw.Write(bodyPD);
                            }
                        }

                        HttpWebResponse responsePD = (HttpWebResponse)reqPD.GetResponse();

                        if (responsePD.StatusCode != HttpStatusCode.OK)
                        {
                            throw new Exception($"Erro na busca do tipoEsc na tabela R006ESC da matrícula {func.matricula}");
                        }

                        XmlDocument xmlDocPD = new XmlDocument();
                        xmlDocPD.Load(responsePD.GetResponseStream());

                        XmlNodeList resultPD = xmlDocPD.SelectNodes("//resultado//campo");

                        var tipEsc = "";
                        var tabela = "";
                        var campo = "";

                        for (int i = 0; i < resultPD.Count; i++)
                        {
                            tipEsc = resultPD[i].SelectSingleNode("valor").InnerText;
                            break;
                        }

                        if (tipEsc == "P")
                        {
                            tabela = "R006HOR";
                            campo = "CODHOR";

                            var corpo = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                                    @"  <soapenv:Body>" + "\n" +
                                                    @"    <ser:ConsultarTabelas>" + "\n" +
                                                    @"      <user></user>" + "\n" +
                                                    @"      <password></password>" + "\n" +
                                                    @"      <encryption>0</encryption>" + "\n" +
                                                    @"      <parameters>" + "\n" +
                                                    @"        <consulta>" + "\n" +
                                                    @"          <id></id>" + "\n" +
                                                    @"          <tabela>" + tabela + "</tabela>" + "\n" +
                                                    @"          <campos>" + campo + "</campos>" + "\n" +
                                                    @"          <ordenacao></ordenacao>" + "\n" +
                                                    @"          <filtro>" + "\n" +
                                                    @"            <campo>CODESC</campo>" + "\n" +
                                                    @"            <condicao>=</condicao>" + "\n" +
                                                    @"            <valor>" + codEsc + "</valor>" + "\n" +
                                                    @"          </filtro>" + "\n" +
                                                    @"        </consulta>" + "\n" +
                                                    @"      </parameters>" + "\n" +
                                                    @"    </ser:ConsultarTabelas>" + "\n" +
                                                    @"  </soapenv:Body>" + "\n" +
                                                    @"</soapenv:Envelope>";

                            HttpWebRequest requisicao = (HttpWebRequest)WebRequest.Create(escalaUrl);
                            requisicao.ContentType = "text/xml";
                            requisicao.Method = "POST";

                            using (Stream stm2 = requisicao.GetRequestStream())
                            {
                                using (StreamWriter stmw2 = new StreamWriter(stm2))
                                {
                                    stmw2.Write(corpo);
                                }
                            }

                            HttpWebResponse resposta = (HttpWebResponse)requisicao.GetResponse();

                            if (resposta.StatusCode != HttpStatusCode.OK)
                            {
                                throw new Exception($"Erro na busca do CODHOR na tabela R006HOR da matrícula {func.matricula}");
                            }

                            XmlDocument r006ces = new XmlDocument();
                            r006ces.Load(resposta.GetResponseStream());
                            XmlNodeList resultado = r006ces.SelectNodes("//resultado//campo");

                            var codHor = 0;
                            var codigos = new List<int>();
                            for (int i = 0; i < resultado.Count; i++)
                            {
                                codHor = Convert.ToInt32(resultado[i].SelectSingleNode("valor").InnerText);
                                codigos.Add(codHor);
                            }
                            int pnt = diaHoje - 1;

                            for (int escP = pnt; escP < codigos.Count; escP++)
                            {
                                if (codigos[escP] != 0 || codigos[escP] != 9998 || codigos[escP] != 9999)
                                {
                                    var corpo2 = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                                        @"  <soapenv:Body>" + "\n" +
                                                        @"    <ser:ConsultarTabelas>" + "\n" +
                                                        @"      <user></user>" + "\n" +
                                                        @"      <password></password>" + "\n" +
                                                        @"      <encryption>0</encryption>" + "\n" +
                                                        @"      <parameters>" + "\n" +
                                                        @"        <consulta>" + "\n" +
                                                        @"          <id></id>" + "\n" +
                                                        @"          <tabela>R004MHR</tabela>" + "\n" +
                                                        @"          <campos></campos>" + "\n" +
                                                        @"          <ordenacao></ordenacao>" + "\n" +
                                                        @"          <filtro>" + "\n" +
                                                        @"            <campo>CODHOR</campo>" + "\n" +
                                                        @"            <condicao>=</condicao>" + "\n" +
                                                        @"            <valor>" + codigos[escP] + "</valor>" + "\n" +
                                                        @"          </filtro>" + "\n" +
                                                        @"        </consulta>" + "\n" +
                                                        @"      </parameters>" + "\n" +
                                                        @"    </ser:ConsultarTabelas>" + "\n" +
                                                        @"  </soapenv:Body>" + "\n" +
                                                        @"</soapenv:Envelope>";

                                    HttpWebRequest requisicao2 = (HttpWebRequest)WebRequest.Create(escalaUrl);
                                    requisicao2.ContentType = "text/xml";
                                    requisicao2.Method = "POST";

                                    using (Stream stm = requisicao2.GetRequestStream())
                                    {
                                        using (StreamWriter stmw = new StreamWriter(stm))
                                        {
                                            stmw.Write(corpo2);
                                        }
                                    }

                                    HttpWebResponse resposta2 = (HttpWebResponse)requisicao2.GetResponse();

                                    if (resposta2.StatusCode != HttpStatusCode.OK)
                                    {
                                        throw new Exception($"Erro na busca do CODHOR na tabela R004MHR da matrícula {func.matricula}");
                                    }

                                    XmlDocument r004mhr = new XmlDocument();
                                    r004mhr.Load(resposta2.GetResponseStream());
                                    //XmlNodeList resultado2 = r004mhr.SelectNodes("//ocorrencia");
                                    XmlNodeList resultado2 = r004mhr.SelectNodes("//resultado//campo");

                                    var seqMar = 0;
                                    var horBat1 = 0;
                                    var horBat2 = 0;
                                    var horBat3 = 0;
                                    var horBat4 = 0;

                                    for (int node = 0; node < resultado2.Count; node++)
                                    {
                                        if (resultado2[node]["nome"].InnerXml == "SEQMAR")
                                        {

                                            seqMar = Convert.ToInt32(resultado2[node].SelectSingleNode("valor").InnerText);

                                            if (seqMar == 1)
                                            {
                                                node += +2;
                                                horBat1 = Convert.ToInt32(resultado2[node]["valor"].InnerText);
                                            }
                                            if (seqMar == 2)
                                            {
                                                node += +2;
                                                horBat2 = Convert.ToInt32(resultado2[node]["valor"].InnerText);
                                            }
                                            if (seqMar == 3)
                                            {
                                                node += +2;
                                                horBat3 = Convert.ToInt32(resultado2[node]["valor"].InnerText);
                                            }
                                            if (seqMar == 4)
                                            {
                                                node += +2;
                                                horBat4 = Convert.ToInt32(resultado2[node]["valor"].InnerText);
                                            }
                                        }
                                    }

                                    var ini_jornada_segundos = horBat1 * 60;
                                    var horasBat1 = ini_jornada_segundos / 3600;
                                    var minutosBat1 = ini_jornada_segundos % 3600 / 60;
                                    var segundosBat1 = ini_jornada_segundos % 60;

                                    var ini_intervalo_segundos = horBat2 * 60;
                                    var horasBat2 = ini_intervalo_segundos / 3600;
                                    var minutosBat2 = ini_intervalo_segundos % 3600 / 60;
                                    var segundosBat2 = ini_intervalo_segundos % 60;

                                    var fim_intervalo_segundos = horBat3 * 60;
                                    var horasBat3 = fim_intervalo_segundos / 3600;
                                    var minutosBat3 = fim_intervalo_segundos % 3600 / 60;
                                    var segundosBat3 = fim_intervalo_segundos % 60;

                                    var fim_expediente_segundos = horBat4 * 60;
                                    var horasBat4 = fim_expediente_segundos / 3600;
                                    var minutosBat4 = fim_expediente_segundos % 3600 / 60;
                                    var segundosBat4 = fim_expediente_segundos % 60;

                                    //while (diaHoje <= diaSemana)
                                    //{
                                    EscalaFuncionario funcionarioEscala = new EscalaFuncionario();
                                    funcionarioEscala.id_funcionario = func.id;
                                    //funcionarioEscala.matricula = func.matricula;
                                    if (diaHoje == 7)
                                    {
                                        funcionarioEscala.dia_semana = 0;
                                    }
                                    else
                                    {
                                        funcionarioEscala.dia_semana = diaHoje;
                                    }
                                    //funcionarioEscala.DiaSemana = diaHoje;
                                    funcionarioEscala.tipo_escala = 1;

                                    if (funcionarioEscala.dia_semana != 0 || codHor != 9998 || codHor != 9999)
                                    {
                                        funcionarioEscala.ini_expediente = new TimeSpan(horasBat1, minutosBat1, segundosBat1);
                                        funcionarioEscala.ini_intervalo = new TimeSpan(horasBat2, minutosBat2, segundosBat2);
                                        funcionarioEscala.fim_intervalo = new TimeSpan(horasBat3, minutosBat3, segundosBat3);

                                        if (horBat4 > 0)
                                        {
                                            funcionarioEscala.fim_expediente = new TimeSpan(horasBat4, minutosBat4, segundosBat4);
                                        }
                                        else
                                        {
                                            funcionarioEscala.fim_expediente = new TimeSpan(horasBat2, minutosBat2, segundosBat2);
                                        }
                                    }
                                    listagem.Add(funcionarioEscala);
                                    diaHoje++;
                                    //}
                                }
                            }
                        }
                        else
                        {
                            int dia = 0;
                            while (diaHoje <= diaSemana)
                            {
                                tabela = "R006CES";
                                campo = "DATESC,CODHOR";

                                var corpo = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                                        @"  <soapenv:Body>" + "\n" +
                                                        @"    <ser:ConsultarTabelas>" + "\n" +
                                                        @"      <user></user>" + "\n" +
                                                        @"      <password></password>" + "\n" +
                                                        @"      <encryption>0</encryption>" + "\n" +
                                                        @"      <parameters>" + "\n" +
                                                        @"        <consulta>" + "\n" +
                                                        @"          <id></id>" + "\n" +
                                                        @"          <tabela>" + tabela + "</tabela>" + "\n" +
                                                        @"          <campos>" + campo + "</campos>" + "\n" +
                                                        @"          <ordenacao></ordenacao>" + "\n" +
                                                        @"          <filtro>" + "\n" +
                                                        @"            <campo>CODESC</campo>" + "\n" +
                                                        @"            <condicao>=</condicao>" + "\n" +
                                                        @"            <valor>" + codEsc + "</valor>" + "\n" +
                                                        @"          </filtro>" + "\n" +
                                                        @"        </consulta>" + "\n" +
                                                        @"      </parameters>" + "\n" +
                                                        @"    </ser:ConsultarTabelas>" + "\n" +
                                                        @"  </soapenv:Body>" + "\n" +
                                                        @"</soapenv:Envelope>";

                                HttpWebRequest requisicao = (HttpWebRequest)WebRequest.Create(escalaUrl);
                                requisicao.ContentType = "text/xml";
                                requisicao.Method = "POST";

                                using (Stream stm2 = requisicao.GetRequestStream())
                                {
                                    using (StreamWriter stmw2 = new StreamWriter(stm2))
                                    {
                                        stmw2.Write(corpo);
                                    }
                                }

                                HttpWebResponse resposta = (HttpWebResponse)requisicao.GetResponse();

                                if (resposta.StatusCode != HttpStatusCode.OK)
                                {
                                    throw new Exception($"Erro na busca do CODHOR na tabela R006CES da matrícula {func.matricula}");
                                }

                                XmlDocument r006ces = new XmlDocument();
                                r006ces.Load(resposta.GetResponseStream());
                                XmlNodeList resultado = r006ces.SelectNodes("//resultado//campo");

                                var codHor = 0;

                                for (int i = 0; i < resultado.Count; i++)
                                {

                                    DateTime dateTime = DateTime.Now;

                                    var dataCodHor = resultado[i].SelectSingleNode("valor").InnerText;
                                    if (dataCodHor == DateTime.Now.AddDays(dia).ToString("dd/MM/yyyy"))
                                    {
                                        i++;
                                        codHor = Convert.ToInt32(resultado[i].SelectSingleNode("valor").InnerText);
                                        break;
                                    }

                                }

                                if (codHor != 0)
                                {
                                    var corpo2 = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                                                        @"  <soapenv:Body>" + "\n" +
                                                        @"    <ser:ConsultarTabelas>" + "\n" +
                                                        @"      <user></user>" + "\n" +
                                                        @"      <password></password>" + "\n" +
                                                        @"      <encryption>0</encryption>" + "\n" +
                                                        @"      <parameters>" + "\n" +
                                                        @"        <consulta>" + "\n" +
                                                        @"          <id></id>" + "\n" +
                                                        @"          <tabela>R004MHR</tabela>" + "\n" +
                                                        @"          <campos></campos>" + "\n" +
                                                        @"          <ordenacao></ordenacao>" + "\n" +
                                                        @"          <filtro>" + "\n" +
                                                        @"            <campo>CODHOR</campo>" + "\n" +
                                                        @"            <condicao>=</condicao>" + "\n" +
                                                        @"            <valor>" + codHor + "</valor>" + "\n" +
                                                        @"          </filtro>" + "\n" +
                                                        @"        </consulta>" + "\n" +
                                                        @"      </parameters>" + "\n" +
                                                        @"    </ser:ConsultarTabelas>" + "\n" +
                                                        @"  </soapenv:Body>" + "\n" +
                                                        @"</soapenv:Envelope>";

                                    HttpWebRequest requisicao2 = (HttpWebRequest)WebRequest.Create(escalaUrl);
                                    requisicao2.ContentType = "text/xml";
                                    requisicao2.Method = "POST";

                                    using (Stream stm = requisicao2.GetRequestStream())
                                    {
                                        using (StreamWriter stmw = new StreamWriter(stm))
                                        {
                                            stmw.Write(corpo2);
                                        }
                                    }

                                    HttpWebResponse resposta2 = (HttpWebResponse)requisicao2.GetResponse();

                                    if (resposta2.StatusCode != HttpStatusCode.OK)
                                    {
                                        throw new Exception($"Erro na busca do CODHOR na tabela R004MHR da matrícula {func.matricula}");
                                    }

                                    XmlDocument r004mhr = new XmlDocument();
                                    r004mhr.Load(resposta2.GetResponseStream());
                                    //XmlNodeList resultado2 = r004mhr.SelectNodes("//ocorrencia");
                                    XmlNodeList resultado2 = r004mhr.SelectNodes("//resultado//campo");

                                    var seqMar = 0;
                                    var horBat1 = 0;
                                    var horBat2 = 0;
                                    var horBat3 = 0;
                                    var horBat4 = 0;

                                    for (int node = 0; node < resultado2.Count; node++)
                                    {
                                        if (resultado2[node]["nome"].InnerXml == "SEQMAR")
                                        {

                                            seqMar = Convert.ToInt32(resultado2[node].SelectSingleNode("valor").InnerText);

                                            if (seqMar == 1)
                                            {
                                                node += +2;
                                                horBat1 = Convert.ToInt32(resultado2[node]["valor"].InnerText);
                                            }
                                            if (seqMar == 2)
                                            {
                                                node += +2;
                                                horBat2 = Convert.ToInt32(resultado2[node]["valor"].InnerText);
                                            }
                                            if (seqMar == 3)
                                            {
                                                node += +2;
                                                horBat3 = Convert.ToInt32(resultado2[node]["valor"].InnerText);
                                            }
                                            if (seqMar == 4)
                                            {
                                                node += +2;
                                                horBat4 = Convert.ToInt32(resultado2[node]["valor"].InnerText);
                                            }
                                        }
                                    }

                                    var ini_jornada_segundos = horBat1 * 60;
                                    var horasBat1 = ini_jornada_segundos / 3600;
                                    var minutosBat1 = ini_jornada_segundos % 3600 / 60;
                                    var segundosBat1 = ini_jornada_segundos % 60;

                                    var ini_intervalo_segundos = horBat2 * 60;
                                    var horasBat2 = ini_intervalo_segundos / 3600;
                                    var minutosBat2 = ini_intervalo_segundos % 3600 / 60;
                                    var segundosBat2 = ini_intervalo_segundos % 60;

                                    var fim_intervalo_segundos = horBat3 * 60;
                                    var horasBat3 = fim_intervalo_segundos / 3600;
                                    var minutosBat3 = fim_intervalo_segundos % 3600 / 60;
                                    var segundosBat3 = fim_intervalo_segundos % 60;

                                    var fim_expediente_segundos = horBat4 * 60;
                                    var horasBat4 = fim_expediente_segundos / 3600;
                                    var minutosBat4 = fim_expediente_segundos % 3600 / 60;
                                    var segundosBat4 = fim_expediente_segundos % 60;

                                    EscalaFuncionario funcionarioEscala = new EscalaFuncionario();
                                    funcionarioEscala.id_funcionario = func.id;
                                    //funcionarioEscala.matricula = func.matricula;
                                    funcionarioEscala.dia_semana = diaHoje;
                                    funcionarioEscala.tipo_escala = 1;

                                    if (funcionarioEscala.dia_semana != 0)
                                    {
                                        funcionarioEscala.ini_expediente = new TimeSpan(horasBat1, minutosBat1, segundosBat1);
                                        funcionarioEscala.ini_intervalo = new TimeSpan(horasBat2, minutosBat2, segundosBat2);
                                        funcionarioEscala.fim_intervalo = new TimeSpan(horasBat3, minutosBat3, segundosBat3);

                                        if (horBat4 > 0)
                                        {
                                            funcionarioEscala.fim_expediente = new TimeSpan(horasBat4, minutosBat4, segundosBat4);
                                        }
                                        else
                                        {
                                            funcionarioEscala.fim_expediente = new TimeSpan(horasBat2, minutosBat2, segundosBat2);
                                        }
                                    }
                                    listagem.Add(funcionarioEscala);
                                }
                                dia++;
                                diaHoje++;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                    string mensagemDeErro = $"Erro ao processar o item : {ex.Message}";
                    EscreverErroEmArquivo(mensagemDeErro);
                    continue;

                }
            }

            return listagem;
        }

        private static void EscreverErroEmArquivo(string mensagem)
        {
            DateTime dataAtual = DateTime.Now;

            string diretorioLogBase = "C:\\sincronizadorLogs\\BuscaEscala";

            
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
