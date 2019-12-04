using System.ServiceModel;
using System.ServiceModel.Web;


namespace UAGUtileriasSAT
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISolucionFactibleWsdl" in both code and config file together.
    [ServiceContract]
    public interface ISolucionFactibleWsdl
    {
        /*Solucion Factible Servicios SAT */
        [OperationContract]
        [WebInvoke(
             Method = "GET",
             BodyStyle = WebMessageBodyStyle.Wrapped,
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json)]
        SfJsonEFOEDO SolFactServiciosSAT_REST(string RFC, string CHECKTYPE);


        [OperationContract]
        [WebInvoke(
               Method = "POST",
               BodyStyle = WebMessageBodyStyle.Wrapped,
               RequestFormat = WebMessageFormat.Xml,
               ResponseFormat = WebMessageFormat.Xml)]
        string SolFactServiciosSAT(string RFC, string CHECKTYPE);
    }
}
