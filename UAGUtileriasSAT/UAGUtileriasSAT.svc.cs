using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using UAGUtileriasSAT.DataBase;
using UAGUtileriasSAT.Metodos;
using UAGUtileriasSAT.FacturacionProv;
using UAGUtileriasSAT.solucionfactible;
using UtileriasGlobales.Helpers;

namespace UAGUtileriasSAT
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class UAGUtileriasSAT_cs : IUAGUtileriasSAT
    {
        //Public Variable
        public List<string> mensajes;

        // Procesar Invoice XML
        public SfJsonInvoiceXML ProcesarInvoiceXMLREST(string strcarpeta, string strsource)
        {
            //produccion
            PRNFACTEntities contextoCFDI = new PRNFACTEntities();
            string source = strsource;

            SfJsonInvoiceXML retorno = new SfJsonInvoiceXML();
            retorno.PreExistentes = 0;
            retorno.Procesados = 0;
            retorno.CfdiError = new List<CfdiError>();
            CfdiError operacionError = new CfdiError();
            bool ExisteError = true;
            string uuid;
            decimal total;

            string strerror = string.Empty;
            string strLocalUrl = ConfigurationManager.AppSettings["LocalUrl"];
            string strSolucionFactibleUsuario = ConfigurationManager.AppSettings["SolucionFactibleUsuario"];
            string strSolucionFactiblePassword = ConfigurationManager.AppSettings["SolucionFactiblePassword"];

            PRNFACTEntities contextUAG = new PRNFACTEntities();
            PRDUAGPKEntities contextFSCM = new PRDUAGPKEntities();
            string ip = Dns.GetHostName();


            string[] filenames = new string[] { };

            string host = HttpContext.Current.Request.Url.Host.ToLower();

            try //Path
            {
                if (host == "localhost")
                {
                    /*Local Host*/
                    string LocalPath = System.Environment.CurrentDirectory;
                    filenames = Directory.GetFiles(strLocalUrl + strcarpeta + "/");
                }
                else
                {
                    filenames = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/CFDI/" + strcarpeta + "/"));

                }
            }
            catch (IOException ex)
            {
                strerror = ex.GetType().Name + " Message " + ex.Message;
            }
            if (filenames.Any())
            {
                foreach (string filename in filenames)
                {
                    List<string> mensajesRel = new List<string>();
                    List<string> mensajesForm = new List<string>();
                    //HCR por cada uno de los XML que se lean de la Carpeta sumo un Complemento 
                    retorno.Invoice++;

                    //Flag si existe un Error por cada uno de los archivos
                    ExisteError = false;

                    try
                    {
                        byte[] archivoXML = File.ReadAllBytes(filename);
                        //mensajesForm = Generales.ValidateForm(archivoXML);
                        DateTime fechatemp = DateTime.Today;
                        //DateTime fecha1 = new DateTime(fechatemp.Year, fechatemp.Month, 1);
                        DateTime fecha1 = new DateTime(fechatemp.Year - 1 , 1, 1);
                        DateTime fecha2;

                        if (fechatemp.Month + 1 < 13)
                        { fecha2 = new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1); }
                        else
                        { fecha2 = new DateTime(fechatemp.Year + 1, 1, 1).AddDays(-1); }



                       /* DateTime openFromDate = fecha1;
                        DateTime openToDate = fecha2;
                        int OYF = 2019;
                        int OYT = 2020;
                        string UagfRFC = "UAG7806127I8";*/


                        /*Get Config from PS*/
                        string BUSSINES_UNIT = "UAG01";
                        DateTime openFromDate = (DateTime)contextFSCM.PS_UAG_CFDI_CNFG_V.Where(l => l.BUSINESS_UNIT == BUSSINES_UNIT).Select(l => l.OPEN_FROM_DATE).FirstOrDefault().Value.AbsoluteStart();
                        DateTime openToDate = (DateTime)contextFSCM.PS_UAG_CFDI_CNFG_V.Where(l => l.BUSINESS_UNIT == BUSSINES_UNIT).Select(l => l.OPEN_TO_DATE).FirstOrDefault().Value.AbsoluteEnd();
                        int OYF = (int)contextFSCM.PS_UAG_CFDI_CNFG_V.Where(l => l.BUSINESS_UNIT == BUSSINES_UNIT).Select(l => l.OPEN_YEAR_FROM).FirstOrDefault();
                        int OYT = (int)contextFSCM.PS_UAG_CFDI_CNFG_V.Where(l => l.BUSINESS_UNIT == BUSSINES_UNIT).Select(l => l.OPEN_YEAR_TO).FirstOrDefault();
                        string UagfRFC = (string)contextFSCM.PS_UAG_CFDI_CNFG_V.Where(l => l.BUSINESS_UNIT == BUSSINES_UNIT).Select(l => l.UAG_CFDI_RECEPTRFC).FirstOrDefault();
                        string userid = (string)contextFSCM.PS_UAG_CFDI_CNFG_V.Where(l => l.BUSINESS_UNIT == BUSSINES_UNIT).Select(l => l.FO_USERID).FirstOrDefault();
                        string pw = (string)contextFSCM.PS_UAG_CFDI_CNFG_V.Where(l => l.BUSINESS_UNIT == BUSSINES_UNIT).Select(l => l.FO_PASSWORD).FirstOrDefault();
                        

                        Invoice validation = new Invoice(archivoXML, openFromDate, openToDate, OYF, OYT, UagfRFC);
                        mensajes = validation.getValidationMessages();
                        if (mensajes.Count == 0 || mensajes[0].Equals("No se permiten facturas fuera del mes en curso."))
                        {
                            if (!Generales.ValidateExistenceXML(archivoXML))
                            {
                                //Procesa CDFI
                                RecibeCFDPortType ws = new RecibeCFDPortTypeClient("RecibeCFDHttpsSoap12Endpoint");
                                recibeCFDRequest request = new recibeCFDRequest();
                                request.usuario = strSolucionFactibleUsuario;
                                request.password = strSolucionFactiblePassword;
                                request.cfd = archivoXML;
                                recibeCFDResponse response = ws.recibeCFD(request);

                                if (response.@return.status == "OK"
                                    || (response.@return.status == "ERROR"
                                        && (Generales.ValidateMessage(response.@return.mensaje)
                                          )
                                        )
                                    )
                                {
                                    SaveFromXML custom = new SaveFromXML(archivoXML, source);
                                    string impuestos = Generales.getImpuestos(archivoXML, custom.UUID);
                                    string descuentos = Generales.getDescuentos(archivoXML, custom.UUID);
                                    string tipoCambio = Generales.getTipoCambio(archivoXML, custom.UUID);
                                    string subTotal = Generales.getSubtotal(archivoXML, custom.UUID);
                                    if (!impuestos.Equals(""))
                                    {
                                        Generales.DeleteUUID(custom.UUID);
                                        mensajes.Add(impuestos);
                                    }
                                    else if (!descuentos.Equals(""))
                                    {
                                        Generales.DeleteUUID(custom.UUID);
                                        mensajes.Add(descuentos);
                                    }
                                    else if (!tipoCambio.Equals(""))
                                    {
                                        Generales.DeleteUUID(custom.UUID);
                                        mensajes.Add(tipoCambio);
                                    }
                                    else if (!subTotal.Equals(""))
                                    {
                                        Generales.DeleteUUID(custom.UUID);
                                        mensajes.Add(subTotal);
                                    }
                                    else
                                    {


                                        uuid = custom.UUID;
                                        total = custom.TOTAL;

                                        UAG_LOG log = new UAG_LOG();
                                        log.UAG_LOG_USER = UagfRFC;
                                        log.UAG_LOG_DESCR = "Se cargo una factura con UUID:" + custom.UUID;
                                        log.UAG_LOG_ACT = "Agregar";
                                        log.UAG_LOG_URL = HttpContext.Current.Request.Url.AbsolutePath.Replace(".aspx", string.Empty);
                                        log.UAG_LOG_FEC = DateTime.Now;
                                        log.UAG_LOG_IP = host;
                                        contextoCFDI.UAG_LOG.Add(log);
                                        contextoCFDI.SaveChanges();


                                        UAG_UUID_LOG log2 = new UAG_UUID_LOG();
                                        log2.SOURCE = source;
                                        log2.UAG_CFDI_UUID = custom.UUID;
                                        log2.UAG_LOG_FEC = DateTime.Now;
                                        log2.UAG_LOG_ACCCION = "A";
                                        log2.UAG_LOG_DESCR = "Save XML";
                                        log2.UAG_LOG_IP = host;
                                        contextoCFDI.UAG_UUID_LOG.Add(log2);
                                        contextoCFDI.SaveChanges();

                                        mensajes.Add("Factura cargada correctamente");
                                    }

                                }

                            }
                        }
                        else
                        {
                            string errores = "";
                            foreach (string mensaje in mensajes)
                            {
                                errores += mensaje + "<br>";
                            }

                            operacionError.FileName = System.IO.Path.GetFileName(filename);
                            operacionError.Detalle = errores;
                            retorno.CfdiError.Add(operacionError);
                            ExisteError = true;
                        }
                    }
                    catch (System.Exception)
                    {
                        //retorno.Errores++;
                        //retorno.DetallesErrores.Add(aux);
                        //response.Error = true;
                    }
                    if (ExisteError)
                    {
                        retorno.ConErrores++;
                    }
                }
            }
            else
            {
                // No se Encontraron Archivos en la Carpeta
            }
            return retorno;
        }
        public string ProcesarInvoiceXML(string strcarpeta, string strsource)
        {
            return JsonConvert.SerializeObject(ProcesarInvoiceXMLREST(strcarpeta, strsource));
        }

        // ProcesarComplementos
        public SfJsonComplementos ProcesarComplementosREST(string carpeta)
        {
            SfJsonComplementos retorno = new SfJsonComplementos();
            retorno.PreExistentes = 0;
            retorno.Procesados = 0;
            retorno.ComplementoError = new List<ComplementoError>();
            ComplementoError operacionError = new ComplementoError();
            bool ExisteError = true;

            string LocalUrl = ConfigurationManager.AppSettings["LocalUrl"];
            string strLocalUrl = ConfigurationManager.AppSettings["LocalUrl"];

            string strerror = string.Empty;
            OperacionComplemento aux = null;

            PRNFACTEntities contextUAG = new PRNFACTEntities();
            string ip = Dns.GetHostName();
            string RFC = "XAXX010101000";

            string[] filenames = new string[] { };

            string host = HttpContext.Current.Request.Url.Host.ToLower();

            try
            {
                if (host == "localhost")
                {
                    /*Local Host*/
                    string LocalPath = System.Environment.CurrentDirectory;
                    filenames = Directory.GetFiles(strLocalUrl + carpeta + "/");
                }
                else
                {
                    filenames = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Complementos/" + carpeta + "/"));

                }
            }
            catch (IOException ex)
            {
                strerror = ex.GetType().Name + " Message " + ex.Message;
            }


            if (filenames.Any())
            {
                foreach (string filename in filenames)
                {
                    List<string> mensajesRel = new List<string>();
                    List<string> mensajesForm = new List<string>();
                    //HCR por cada uno de los XML que se lean de la Carpeta sumo un Complemento 
                    retorno.Complementos++;

                    //Flag si existe un Error por cada uno de los archivos
                    ExisteError = false;

                    try
                    {
                        byte[] archivoXML = File.ReadAllBytes(filename);
                        mensajesForm = Generales.ValidateForm(archivoXML);

                        if (mensajesForm.Count == 0)
                        {

                            if (!Generales.ValidateExistenceXMLCompag(archivoXML))
                            {
                                if (mensajesRel.Count == 0)
                                {
                                    string uuid = string.Empty;
                                    SaveCompag custom = new SaveCompag(archivoXML, "COM", ref uuid, System.IO.Path.GetFileName(filename), out aux);

                                    //Solo si se procesa el CFDI sumo a procesados
                                    retorno.Procesados++;

                                    //response.Detalle += "Archivo:" + System.IO.Path.GetFileName(filename) + " UUID:" + uuid + "<br />";

                                    UAG_LOG log = new UAG_LOG();
                                    log.UAG_LOG_USER = RFC;
                                    log.UAG_LOG_DESCR = "Se cargo un complemento de pago con UUID:" + uuid;
                                    log.UAG_LOG_ACT = "Agregar";
                                    log.UAG_LOG_URL = HttpContext.Current.Request.Url.AbsolutePath.Replace(".aspx", string.Empty);
                                    log.UAG_LOG_FEC = DateTime.Now;
                                    log.UAG_LOG_IP = ip;
                                    contextUAG.UAG_LOG.Add(log);
                                    contextUAG.SaveChanges();


                                    UAG_UUID_LOG log2 = new UAG_UUID_LOG();
                                    log2.SOURCE = "COM";
                                    log2.UAG_CFDI_UUID = uuid;
                                    log2.UAG_LOG_FEC = DateTime.Now;
                                    log2.UAG_LOG_ACCCION = "a";
                                    log2.UAG_LOG_DESCR = "Save XML";
                                    log2.UAG_LOG_IP = ip;
                                    contextUAG.UAG_UUID_LOG.Add(log2);
                                    contextUAG.SaveChanges();


                                }
                                else
                                {
                                    string errores = "";

                                    foreach (string mensaje in mensajesRel)
                                    {
                                        errores += mensaje + "<br>";
                                    }
                                }
                            }
                            else
                            {
                                //Por cada complemento ya existente que se trato de cargar le sumo uno 
                                retorno.PreExistentes++;

                                UAG_LOG log = new UAG_LOG();
                                log.UAG_LOG_USER = RFC;
                                log.UAG_LOG_DESCR = "Se trato de cargar un complemento ya cargado";
                                log.UAG_LOG_ACT = "Error";
                                log.UAG_LOG_URL = HttpContext.Current.Request.Url.AbsolutePath.Replace(".aspx", string.Empty);
                                log.UAG_LOG_FEC = DateTime.Now;
                                log.UAG_LOG_IP = ip;
                                contextUAG.UAG_LOG.Add(log);
                                contextUAG.SaveChanges();
                            }
                        }
                        else
                        {
                            string errores = "";

                            foreach (string mensaje in mensajesForm)
                            {
                                errores += mensaje;
                            }

                            operacionError.FileName = System.IO.Path.GetFileName(filename);
                            operacionError.Detalle = errores;
                            retorno.ComplementoError.Add(operacionError);
                            ExisteError = true;
                        }
                    }
                    catch (System.Exception)
                    {
                        //retorno.Errores++;
                        //retorno.DetallesErrores.Add(aux);
                        //response.Error = true;
                    }
                    if (ExisteError)
                    {
                        retorno.ConErrores++;
                    }
                }
            }
            else
            {
                //retorno.Detalle += "<br />No se encontraron archivos en la carpeta indicada";
            }
            return retorno;
        }
        public string ProcesarComplementos(string carpeta)
        {
            return JsonConvert.SerializeObject(ProcesarComplementosREST(carpeta));
        }

        //Get Vendor Pass
        public JsonVndrPass GetVndrPassREST(string strRfc)
        {
            JsonVndrPass retorno = new JsonVndrPass();

            string VndrPassEn = string.Empty;
            VndrPassEn = Generales.GetVndrPass(strRfc);
            retorno.RFC = strRfc;
            retorno.VNDRPASS = biSafe.clsRijndael.Decrypt(VndrPassEn);
            /*Return*/
            return retorno;
        }
        public string GetVndrPass(string strRfc)
        {
            return JsonConvert.SerializeObject(GetVndrPassREST(strRfc));
        }

        //Genera Referncias
        public RestResponse GenerarReferenciaREST(string SOURCE_REQ, int ID_SEQ_NUM, string DOCUMENT_ID, int DAYS_DURATION, decimal AMOUNT)
        {
            RestResponse data = new RestResponse();

            if (!string.IsNullOrEmpty(DOCUMENT_ID))
            {
                string UAG_BNK_ID_REF = Generales.CalculateReference(ID_SEQ_NUM, DOCUMENT_ID.ToInteger(), DateTime.Now.AddDays(DAYS_DURATION), AMOUNT);
                DateTime END_DATE = DateTime.Now.AddDays(DAYS_DURATION);

                PRDUAGPKEntities context = new PRDUAGPKEntities();
                PS_UAG_REF_BNK_TBL log = new PS_UAG_REF_BNK_TBL
                {
                    GUID = Guid.NewGuid().ToString(),
                    AMOUNT = AMOUNT,
                    SOURCE_REQ = SOURCE_REQ,
                    ID_SEQ_NUM = ID_SEQ_NUM,
                    DOCUMENT_ID = DOCUMENT_ID.ToString(),
                    DAYS_DURATION = DAYS_DURATION,
                    UAG_BNK_ID_REF = UAG_BNK_ID_REF,
                    END_DATE = END_DATE,
                    CREATE_DATE = DateTime.Now
                };
                context.PS_UAG_REF_BNK_TBL.Add(log);
                context.SaveChanges();

                data = new RestResponse()
                {
                    response = true,
                    message = "Referencia generada correctamente",
                    ID_SEQ_NUM = ID_SEQ_NUM,
                    DOCUMENT_ID = DOCUMENT_ID,
                    DAYS_DURATION = DAYS_DURATION,
                    AMOUNT = AMOUNT,
                    END_DATE = string.Format("{0:yyyy-MM-dd}", END_DATE),
                    UAG_BNK_ID_REF = UAG_BNK_ID_REF
                };
            }
            else
            {
                data = new RestResponse()
                {
                    response = false,
                    message = "El parametro DOCUMENT_ID no tiene un formato correcto"
                };
            }
            return data;
        }
        public string GenerarReferencia(string SOURCE_REQ, int ID_SEQ_NUM, string DOCUMENT_ID, int DAYS_DURATION, decimal AMOUNT)
        {
            return JsonConvert.SerializeObject(GenerarReferenciaREST(SOURCE_REQ, ID_SEQ_NUM, DOCUMENT_ID, DAYS_DURATION, AMOUNT));
        }

    }
    /*Genera Referncia Response*/
    public class RestResponse
    {
        public bool response { get; set; }
        public string message { get; set; }
        public int ID_SEQ_NUM { get; set; }
        public string DOCUMENT_ID { get; set; }
        public int DAYS_DURATION { get; set; }
        public decimal AMOUNT { get; set; }
        public string END_DATE { get; set; }
        public string UAG_BNK_ID_REF { get; set; }

    }
    /*Uag Rest Response */
    public class JsonVndrPass
    {
        public string RFC { get; set; }
        public string VNDRPASS { get; set; }
    }
    public class SfJsonBuzonTributario
    {
        public string fechaEmitida { get; set; }
        public string fechaRecibida { get; set; }
        public string folio { get; set; }
        public double importe { get; set; }
        public string moneda { get; set; }
        public string nombreEmisor { get; set; }
        public string rfcEmisor { get; set; }
        public string rid { get; set; }
        public string serie { get; set; }
        public string uuid { get; set; }
    }
    /*Invoice XML*/
    public class SfJsonInvoiceXML
    {
        public int Invoice { get; set; }
        public int Procesados { get; set; }
        public int PreExistentes { get; set; }
        public int ConErrores { get; set; }
        public List<CfdiError> CfdiError { get; set; }
    }
    public class CfdiError
    {
        public string FileName { get; set; }
        public string Detalle { get; set; }
    }
    /*Complementos*/
    public class SfJsonComplementos
    {
        public int Complementos { get; set; }
        public int Procesados { get; set; }
        public int PreExistentes { get; set; }
        public int ConErrores { get; set; }
        public List<ComplementoError> ComplementoError { get; set; }
    }
    public class ComplementoError
    {
        public string FileName { get; set; }
        public string Detalle { get; set; }
    }
    public class OperacionComplemento
    {
        public string Archivo { get; set; }
        public string UUID { get; set; }
        public bool OperacionHDR { get; set; }
        public int OperacionesLN { get; set; }
        public int OperacionesLNPYMNT { get; set; }
        public int OperacionesPYMNT_DOCREL { get; set; }
        public string Detalle { get; set; }
        public bool Error { get; set; }
    }

}
