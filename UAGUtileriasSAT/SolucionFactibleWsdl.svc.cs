using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace UAGUtileriasSAT
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SolucionFactibleWsdl" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SolucionFactibleWsdl.svc or SolucionFactibleWsdl.svc.cs at the Solution Explorer and start debugging.
    public class SolucionFactibleWsdl : ISolucionFactibleWsdl
    {
        /*HCR 30/07/2019 Solucion Factible Servicios SAT*/
        public SfJsonEFOEDO SolFactServiciosSAT_REST(string RFC, string CHECKTYPE)
        {
            string ResultJson = string.Empty;
            SfJsonEFOEDO data = new SfJsonEFOEDO();

            string UagUsuario = ConfigurationManager.AppSettings["SolucionFactibleUsuario"];
            string UagPassword = ConfigurationManager.AppSettings["SolucionFactiblePassword"];
            string URLServiciosSAT = ConfigurationManager.AppSettings["SolucionFactibleEFOEDO"];


            if (!string.IsNullOrEmpty(RFC))
            {
                if (!string.IsNullOrEmpty(CHECKTYPE))
                {
                    //For Basic Authentication
                    string authInfo = UagUsuario + ":" + UagPassword;
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                    string strurl = string.Format(URLServiciosSAT + RFC + "/" + CHECKTYPE);
                    WebRequest RequestObjget = WebRequest.Create(strurl);
                    RequestObjget.Method = "GET";
                    RequestObjget.Headers["Authorization"] = "Basic " + authInfo;
                    HttpWebResponse ResponseObjGet = null;
                    ResponseObjGet = (HttpWebResponse)RequestObjget.GetResponse();

                    string streamresulttest = null;
                    using (Stream stream = ResponseObjGet.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                        streamresulttest = sr.ReadToEnd();
                        sr.Close();
                    }
                    dynamic JsonSf = JsonConvert.DeserializeObject<RootObject>(streamresulttest);

                    data = new SfJsonEFOEDO()
                    {
                        ResRFC = JsonSf.payload.rfc,
                        ResCheckType = JsonSf.payload.checkType,
                        ResExistente = JsonSf.payload.resultado.existente,
                        ResNombre = JsonSf.payload.resultado.nombre,
                        ResSituacion = JsonSf.payload.resultado.situacion,
                        ResNFechaPresSat = JsonSf.payload.resultado.numeroFechaPresuncionSat,
                        ResFPubPresSat = JsonSf.payload.resultado.fechaPublicacionPresuncionSat,
                        ResNFechaPresDof = JsonSf.payload.resultado.numeroFechaPresuncionDof,
                        ResFPubPresDof = JsonSf.payload.resultado.fechaPublicacionPresuncionDof,
                        ResNFechaDesvSat = JsonSf.payload.resultado.numeroFechaDesvirtuadosSat,
                        ResFDesvSat = JsonSf.payload.resultado.fechaDesvirtuadosSat,
                        ResFDesvDof = JsonSf.payload.resultado.fechaDesvirtuadosDof,
                        ResNFechaDef = JsonSf.payload.resultado.numeroFechaDefinitivo,
                        ResFPubDefSat = JsonSf.payload.resultado.fechaPublicacionDefinitivosSat,
                        ResFPubDefDof = JsonSf.payload.resultado.fechaPublicacionDefinitivosDof,
                        ResNFechaFavoSat = JsonSf.payload.resultado.numeroFechaFavorablesSat,
                        ResNFechaFavDof = JsonSf.payload.resultado.numeroFechaFavorableDof,
                        ResFFavoSat = JsonSf.payload.resultado.fechaFavorableSat,
                        ResFFavoDof = JsonSf.payload.resultado.fechaFavorableDof

                    };

                }
            }
            else
            {

            }
            return data;
        }

        public string SolFactServiciosSAT(string RFC, string CHECKTYPE)
        {
            return JsonConvert.SerializeObject(SolFactServiciosSAT_REST(RFC, CHECKTYPE));
        }
    }
    /*Solucion Factible Rest Response*/

    public class SfJsonEFOEDO
    {
        public string ResRFC { get; set; }
        public string ResCheckType { get; set; }
        public bool ResExistente { get; set; }
        public string ResNombre { get; set; }
        public string ResSituacion { get; set; }
        public string ResNFechaPresSat { get; set; }
        public string ResFPubPresSat { get; set; }
        public string ResNFechaPresDof { get; set; }
        public string ResFPubPresDof { get; set; }
        public string ResNFechaDesvSat { get; set; }
        public string ResNFechaDef { get; set; }
        public string ResFDesvSat { get; set; }
        public string ResFDesvDof { get; set; }
        public string ResFPubDefSat { get; set; }
        public string ResFPubDefDof { get; set; }
        public string ResNFechaFavoSat { get; set; }
        public string ResNFechaFavDof { get; set; }
        public string ResFFavoSat { get; set; }
        public string ResFFavoDof { get; set; }
    }
    public class RootObject
    {
        public Payload payload { get; set; }
    }
    public class Payload
    {
        public string rfc { get; set; }
        public string checkType { get; set; }
        public Resultado resultado { get; set; }
    }
    public class Resultado
    {
        public bool existente { get; set; }
        public string nombre { get; set; }
        public string situacion { get; set; }
        public string numeroFechaPresuncionSat { get; set; }
        public string fechaPublicacionPresuncionSat { get; set; }
        public string numeroFechaPresuncionDof { get; set; }
        public string fechaPublicacionPresuncionDof { get; set; }
        public string numeroFechaDesvirtuadosSat { get; set; }
        public string fechaDesvirtuadosSat { get; set; }
        public string fechaDesvirtuadosDof { get; set; }
        public string numeroFechaDefinitivo { get; set; }
        public string fechaPublicacionDefinitivosSat { get; set; }
        public string fechaPublicacionDefinitivosDof { get; set; }
        public string numeroFechaFavorablesSat { get; set; }
        public string fechaFavorableSat { get; set; }
        public string numeroFechaFavorableDof { get; set; }
        public string fechaFavorableDof { get; set; }
    }
}
