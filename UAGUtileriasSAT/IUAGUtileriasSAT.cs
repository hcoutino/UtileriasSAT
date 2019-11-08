using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace UAGUtileriasSAT
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IUAGUtileriasSAT
    {
        /*Solucion Factible Procesa Invoice XML */
        [OperationContract]
        [WebInvoke(
               Method = "POST",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               RequestFormat = WebMessageFormat.Xml,
               ResponseFormat = WebMessageFormat.Xml)]
        string ProcesarInvoiceXML(string carpeta);

        [WebInvoke(
               Method = "GET",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json)]
        SfJsonInvoiceXML ProcesarInvoiceXMLREST(string carpeta);

        /*Solucion Factible Procesa Complementos de Pago */
        [OperationContract]
        [WebInvoke(
               Method = "POST",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               RequestFormat = WebMessageFormat.Xml,
               ResponseFormat = WebMessageFormat.Xml)]
        string ProcesarComplementos(string carpeta);

        [WebInvoke(
               Method = "GET",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json)]
        SfJsonComplementos ProcesarComplementosREST(string carpeta);

        /* Uag Portal Proveedores Get Password */
        [WebInvoke(
       Method = "GET",
       BodyStyle = WebMessageBodyStyle.Wrapped,
       RequestFormat = WebMessageFormat.Json,
       ResponseFormat = WebMessageFormat.Json)]
        JsonVndrPass GetVndrPassREST(string strRfc);
    }

}
