using ApiSync.Models;
using System.Net;
using System.Xml;

namespace ApiSync.Services
{
    public class DemitidosService
    {
        public List<Funcionario> BuscaDemitidos()
        {
            List<Funcionario> lista = new List<Funcionario>();
            try
            {
                string _webServiceUrl = "https://web38.seniorcloud.com.br/g5-senior-services/rubi_Synccom_senior_g5_rh_fp_colaboradoresDemitidos";
                //string consultaPis = "https://web38.seniorcloud.com.br/g5-senior-services/rubi_Synccom_senior_g5_rh_fp_integracoes";

                var soap = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">" + "\n" +
                    @"   <soapenv:Header/>" + "\n" +
                    @"   <soapenv:Body>" + "\n" +
                    @"      <ser:ColaboradoresDemitidos>" + "\n" +
                    @"         <password></password>" + "\n" +
                    @"         <user></user>" + "\n" +
                    @"         <encryption>0</encryption>" + "\n" +
                    @"         <parameters>" + "\n" +
                    @"            <!--Optional:-->" + "\n" +
                    @"            <abrTipCol>1</abrTipCol>" + "\n" +
                    @"            <!--Optional:-->" + "\n" +
                    @"            <fimPer>" + DateTime.Now.AddDays(+0).ToString("dd/MM/yyyy") + "</fimPer>" + "\n" +
                    @"            <!--Optional:-->" + "\n" +
                    @"            <flowInstanceID>?</flowInstanceID>" + "\n" +
                    @"            <!--Optional:-->" + "\n" +
                    @"            <flowName>?</flowName>" + "\n" +
                    @"            <!--Optional:-->" + "\n" +
                    @"            <iniPer>" + DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy") + "</iniPer>" + "\n" +
                    @"            <!--Optional:-->" + "\n" +
                    @"            <numEmp>1</numEmp>" + "\n" +
                    @"         </parameters>" + "\n" +
                    @"      </ser:ColaboradoresDemitidos>" + "\n" +
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

                WebResponse response = req.GetResponse();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());

                XmlNodeList result = xmlDoc.DocumentElement.FirstChild.FirstChild.FirstChild.SelectNodes("TMCSColaboradores");

                foreach (XmlElement item in result)
                {
                    Funcionario func = new Funcionario();

                    func.matricula = Convert.ToInt32(item.SelectSingleNode("numCad").InnerText);
                    var dataOriginal = item.SelectSingleNode("datAdm").InnerText;
                    DateTime dataConvertida = DateTime.ParseExact(dataOriginal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    func.data_admissao = dataConvertida.ToString("yyyy-MM-dd");
                    //func.data_admissao = Convert.ToDateTime(item.SelectSingleNode("datAdm").InnerText);
                    func.nome = item.SelectSingleNode("nomFun").InnerText;
                    var data_atual = DateTime.Now;
                    var data_demissao = Convert.ToDateTime(item.SelectSingleNode("datAfa").InnerText);
                    func.ativo = 0;

                    if (data_atual != data_demissao)
                    {
                        lista.Add(func);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return lista;
        }
    }
}
