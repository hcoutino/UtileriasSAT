using System.ServiceModel;
using System.ServiceModel.Web;


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
        string ProcesarInvoiceXML(string carpeta, string source);

        [WebInvoke(
               Method = "GET",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json)]
        SfJsonInvoiceXML ProcesarInvoiceXMLREST(string carpeta, string source);

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

        /*Genera Referncia*/
        [OperationContract]
        [WebInvoke(
               Method = "GET",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json)]
        RestResponse GenerarReferenciaREST(string SOURCE_REQ, int ID_SEQ_NUM, string DOCUMENT_ID, int DAYS_DURATION, decimal AMOUNT);


        [OperationContract]
        [WebInvoke(
               Method = "POST",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               RequestFormat = WebMessageFormat.Xml,
               ResponseFormat = WebMessageFormat.Xml)]
        string GenerarReferencia(string SOURCE_REQ, int ID_SEQ_NUM, string DOCUMENT_ID, int DAYS_DURATION, decimal AMOUNT);

    }

}
