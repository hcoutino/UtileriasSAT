using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using UAGUtileriasSAT.DataBase;

namespace UAGUtileriasSAT.Metodos
{
    public class Generales
    {
        //Public Variable
        public decimal total;
        public decimal TOTAL
        {
            get
            {
                return total;
            }
            set
            {
                total = value;
            }
        }
        public string uuid;
        public string UUID
        {
            get
            {
                return uuid;
            }
            set
            {
                uuid = value;
            }
        }

        //Functions 
        public static bool ValidateExistenceXML(byte[] fileBytes)
        {
            bool flag = false;
            PRNFACTEntities ContextXML = new PRNFACTEntities();

            MemoryStream memoryStream = new MemoryStream(fileBytes);
            XmlDocument cfdi;
            string encoding;
            string UUID;
            UAG_CFDI_HDR cabeceraCFDI;

            if (memoryStream.CanSeek)
            {
                try
                {
                    cfdi = new XmlDocument();

                    try
                    {
                        XmlTextReader reader = new XmlTextReader(memoryStream);
                        cfdi.Load(reader);
                    }
                    catch (Exception)
                    {
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        encoding = "ISO-8859-1";
                        StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                        try
                        {
                            cfdi.Load(streamReader);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
                    nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                    nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                    XmlNode xmlTimbreFiscal = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Complemento/tfd:TimbreFiscalDigital", nsmgr);
                    XmlNode xmlVoucher = cfdi.SelectSingleNode("cfdi:Comprobante", nsmgr);

                    UUID = xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper().Trim();
                    cabeceraCFDI = ContextXML.UAG_CFDI_HDR.Where(l => l.UAG_CFDI_UUID == UUID).FirstOrDefault();

                    if (cabeceraCFDI != null)
                    {
                        flag = true;
                    }

                }
                catch (Exception)
                {

                }
            }
            return flag;

        }
        public static bool ValidateExistenceXMLCompag(byte[] fileBytes)
        {
            bool flag = false;
            PRNFACTEntities ContextCompag = new PRNFACTEntities();

            MemoryStream memoryStream = new MemoryStream(fileBytes);
            XmlDocument cfdi;
            string encoding;
            string UUID;
            UAG_COMPAG_HDR cabeceraCompag;

            if (memoryStream.CanSeek)
            {
                try
                {
                    cfdi = new XmlDocument();

                    try
                    {
                        XmlTextReader reader = new XmlTextReader(memoryStream);
                        cfdi.Load(reader);
                    }
                    catch (Exception)
                    {
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        encoding = "ISO-8859-1";
                        StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                        try
                        {
                            cfdi.Load(streamReader);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
                    nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                    nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                    XmlNode xmlTimbreFiscal = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Complemento/tfd:TimbreFiscalDigital", nsmgr);

                    UUID = xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper().Trim();
                    cabeceraCompag = ContextCompag.UAG_COMPAG_HDR.Where(l => l.UAG_COMPAG_UUID == UUID).FirstOrDefault();

                    if (cabeceraCompag != null)
                    {
                        flag = true;
                    }

                }
                catch (Exception)
                {

                }
            }
            return flag;
        }
        public string ValidateRFC(byte[] fileBytes, string rfc)
        {
            string flag = "";

            MemoryStream memoryStream = new MemoryStream(fileBytes);
            XmlDocument cfdi;
            string encoding;

            if (memoryStream.CanSeek)
            {
                try
                {
                    cfdi = new XmlDocument();

                    try
                    {
                        XmlTextReader reader = new XmlTextReader(memoryStream);
                        cfdi.Load(reader);
                    }
                    catch (Exception)
                    {
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        encoding = "ISO-8859-1";
                        StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                        try
                        {
                            cfdi.Load(streamReader);
                        }
                        catch (Exception)
                        {
                            flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                        }
                    }

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
                    nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                    nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                    XmlNode xmlProvider = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Emisor", nsmgr);

                    string RFCEmisor = "";

                    if (xmlProvider.Attributes["Rfc"] != null)
                    {
                        RFCEmisor = xmlProvider.Attributes["Rfc"].Value;
                    }
                    else if (xmlProvider.Attributes["rfc"] != null)
                    {
                        RFCEmisor = xmlProvider.Attributes["rfc"].Value;
                    }
                    else if (xmlProvider.Attributes["RFC"] != null)
                    {
                        RFCEmisor = xmlProvider.Attributes["RFC"].Value;
                    }

                    if (RFCEmisor.Equals(rfc.Trim()))
                    {
                        flag = "true";
                    }

                }
                catch (Exception)
                {
                    flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                }
            }
            return flag;
        }
        public static List<string> ValidateForm(byte[] fileBytes)
        {
            List<string> mensajes = new List<string>();
            List<string> uuids = new List<string>();

            XmlDocument cfdi;
            MemoryStream memoryStream = new MemoryStream(fileBytes);
            string encoding;
            string CompUUID;

            cfdi = new XmlDocument();

            try
            {
                XmlTextReader reader = new XmlTextReader(memoryStream);
                cfdi.Load(reader);
            }
            catch (Exception)
            {
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                encoding = "ISO-8859-1";
                StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                try
                {
                    cfdi.Load(streamReader);
                }
                catch (Exception exc)
                {
                    throw new ArgumentException("Error Inesperado, Disculpe las molestias que esto le ocasione - ", exc.Message);
                }
            }

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
            nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
            nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

            XmlNode xmlVoucher = cfdi.SelectSingleNode("cfdi:Comprobante", nsmgr);
            XmlNode xmlClient = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Receptor", nsmgr);
            XmlNode xmlProvider = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Emisor", nsmgr);
            XmlNode xmlTimbreFiscal = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Complemento/tfd:TimbreFiscalDigital", nsmgr);
            XmlNode xmlComplemento = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Complemento", nsmgr);
            XmlNode xmlConceptos = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Conceptos", nsmgr);
            XmlNode xmlCfdiRelacionados = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:CfdiRelacionados", nsmgr);

            CompUUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper().Trim();


            bool flag = false;
            bool flag2 = false;
            for (int x = 0; x < xmlComplemento.ChildNodes.Count; x++)
            {
                if (xmlComplemento.ChildNodes[x].LocalName == "Pagos")
                {
                    flag = true;
                    XmlNode pago = xmlComplemento.ChildNodes.Item(x);
                    XmlNode infoPago = pago.ChildNodes.Item(0);
                    for (int j = 0; j < infoPago.ChildNodes.Count; j++)
                    {
                        if (infoPago.ChildNodes[j].LocalName == "DoctoRelacionado")
                        {
                            flag2 = true;
                            break;
                        }
                    }
                }
            }
            /*CFDI Relacionados Nodo Externo a nivel de Comprobante*/
            if (xmlCfdiRelacionados != null)
            {
                for (int x2 = 0; x2 < xmlCfdiRelacionados.ChildNodes.Count; x2++)
                {
                    if (xmlCfdiRelacionados.ChildNodes[x2].Name == "cfdi:CfdiRelacionado" || xmlCfdiRelacionados.ChildNodes[x2].Name == "CfdiRelacionado")
                    {
                        flag2 = true;
                        break;
                    }
                }
            }
            /*Valida las Banderas*/
            if (!flag)
                mensajes.Add("El UUID (" + CompUUID + ") cargado no tiene el complemento de pago establecido por el SAT.");
            if (!flag2)
                mensajes.Add("El UUID (" + CompUUID + ") cargado no cumple con el estandar de un Documento Relacionado de pago establecido por el SAT.");

            return mensajes;
        }
        public static bool ValidateMessage(string message)
        {
            List<UAG_MESSAGES_SOLUCIONFACTIBLE> messages = new List<UAG_MESSAGES_SOLUCIONFACTIBLE>();
            PRNFACTEntities contextoMsg = new PRNFACTEntities();
            messages = contextoMsg.UAG_MESSAGES_SOLUCIONFACTIBLE
            .Where(i => i.STATUS.Equals("A")).ToList();

            foreach (var messageTemp in messages)
            {
                if (message.Contains(messageTemp.MESSAGE))
                {
                    return true;
                }
            }
            return false;
        }
        public static string GetVndrPass(string strRfc)
        {
            PRNFACTEntities contextoVndr = new PRNFACTEntities();
            string strPassEncrypt = string.Empty;
            UAG_VENDOR UAGVNDR;
            UAGVNDR = contextoVndr.UAG_VENDOR.Where(i => i.VNDR_RFC == strRfc).FirstOrDefault();
            strPassEncrypt = UAGVNDR.VNDR_PASSWORD;
            return strPassEncrypt;
        }

        //validar impuestos de traslado y retencion
        public static string getImpuestos(byte[] fileBytes, string UUID)
        {
            string flag = "";
            //produccion
            PRNFACTEntities contextoCfdi = new PRNFACTEntities();
            MemoryStream memoryStream = new MemoryStream(fileBytes);
            XmlDocument cfdi;
            string encoding;
            decimal totalXMLTraslados = 0M;
            decimal totalXMLRetenidos = 0M;
            decimal total = 0M;
            decimal subtotal = 0M;
            decimal totalDBTraslado = 0M;
            decimal totalDBRetenido = 0M;

            if (memoryStream.CanSeek)
            {
                try
                {
                    cfdi = new XmlDocument();

                    try
                    {
                        XmlTextReader reader = new XmlTextReader(memoryStream);
                        cfdi.Load(reader);
                    }
                    catch (Exception)
                    {
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        encoding = "ISO-8859-1";
                        StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                        try
                        {
                            cfdi.Load(streamReader);
                        }
                        catch (Exception)
                        {
                            flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                        }
                    }
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
                    nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                    nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                    XmlNode xmlImpuestos = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Impuestos", nsmgr);
                    if (xmlImpuestos != null)
                    {
                        totalXMLTraslados = (xmlImpuestos.Attributes["TotalImpuestosTrasladados"] == null) ? 0M
                                        : Decimal.Parse(xmlImpuestos.Attributes["TotalImpuestosTrasladados"].Value, CultureInfo.InvariantCulture);
                        totalXMLRetenidos = (xmlImpuestos.Attributes["TotalImpuestosRetenidos"] == null) ? 0M
                                        : Decimal.Parse(xmlImpuestos.Attributes["TotalImpuestosRetenidos"].Value, CultureInfo.InvariantCulture);
                    }

                    XmlNode xmlVoucher = cfdi.SelectSingleNode("cfdi:Comprobante", nsmgr);
                    subtotal = (xmlVoucher.Attributes["SubTotal"] == null) ? 0M
                                    : Decimal.Parse(xmlVoucher.Attributes["SubTotal"].Value, CultureInfo.InvariantCulture);
                    total = (xmlVoucher.Attributes["Total"] == null) ? 0M
                                    : Decimal.Parse(xmlVoucher.Attributes["Total"].Value, CultureInfo.InvariantCulture);

                    List<UAG_CFDI_LN_IMP> impuestosRetenido = new List<UAG_CFDI_LN_IMP>();
                    List<UAG_CFDI_LN_IMP> impuestosTraslado = new List<UAG_CFDI_LN_IMP>();

                    impuestosRetenido = contextoCfdi.UAG_CFDI_LN_IMP
                                        .Where(i => i.UAG_CFDI_UUID == UUID
                                                && i.UAG_CFDI_TIPOIMP == "R")
                                        .ToList();
                    if (impuestosRetenido != null && impuestosRetenido.Count > 0)
                    {
                        foreach (UAG_CFDI_LN_IMP impuestoR in impuestosRetenido)
                        {
                            totalDBRetenido = totalDBRetenido + impuestoR.UAG_CFDI_IMPORTE;
                        }
                    }
                    else
                    {
                        totalDBRetenido = 0M;
                    }

                    impuestosTraslado = contextoCfdi.UAG_CFDI_LN_IMP
                                        .Where(i => i.UAG_CFDI_UUID == UUID
                                                && i.UAG_CFDI_TIPOIMP == "T")
                                        .ToList();
                    if (impuestosTraslado != null && impuestosTraslado.Count > 0)
                    {
                        foreach (UAG_CFDI_LN_IMP impuestoR in impuestosTraslado)
                        {
                            totalDBTraslado = totalDBTraslado + impuestoR.UAG_CFDI_IMPORTE;
                        }
                    }
                    else
                    {
                        totalDBTraslado = 0M;
                    }

                    if (totalDBTraslado <= totalXMLTraslados + .99M && totalDBTraslado >= totalXMLTraslados - .99M)
                    {
                        flag = "";
                    }
                    else
                    {
                        totalDBTraslado = total - subtotal;
                        if (totalDBTraslado <= totalXMLTraslados + .99M && totalDBTraslado >= totalXMLTraslados - .99M)
                        {
                            flag = "";
                        }
                        else
                        {
                            if (flag.Count() > 0)
                            {
                                flag = flag + ";";
                            }
                            flag = "La suma de los impuestos de traslado no coicide con el TotalImpuestosTrasladados.";
                        }
                    }

                    if (totalDBRetenido <= totalXMLRetenidos + .99M && totalDBRetenido >= totalXMLRetenidos - .99M)
                    {
                        if (flag.Equals(""))
                            flag = "";
                    }
                    else
                    {
                        if (flag.Count() > 0)
                        {
                            flag = flag + ";";
                        }
                        flag = "La suma de los impuestos de Retencion no coicide con el TotalImpuestosRetenidos.";
                    }

                }
                catch (Exception)
                {
                    flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                }
            }
            return flag;
        }
        public static string getDescuentos(byte[] fileBytes, string UUID)
        {
            string flag = "";
            //produccion
            PRNFACTEntities contextoCfdi = new PRNFACTEntities();
            MemoryStream memoryStream = new MemoryStream(fileBytes);
            XmlDocument cfdi;
            string encoding;
            decimal totalXMLDescuento;
            decimal totalDBDescuento = 0M;


            if (memoryStream.CanSeek)
            {
                try
                {
                    cfdi = new XmlDocument();

                    try
                    {
                        XmlTextReader reader = new XmlTextReader(memoryStream);
                        cfdi.Load(reader);
                    }
                    catch (Exception)
                    {
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        encoding = "ISO-8859-1";
                        StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                        try
                        {
                            cfdi.Load(streamReader);
                        }
                        catch (Exception)
                        {
                            flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                        }
                    }
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
                    nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                    nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                    XmlNode xmlVoucher = cfdi.SelectSingleNode("cfdi:Comprobante", nsmgr);
                    totalXMLDescuento = (xmlVoucher.Attributes["Descuento"] == null) ? 0M
                                    : Decimal.Parse(xmlVoucher.Attributes["Descuento"].Value, CultureInfo.InvariantCulture);

                    List<UAG_CFDI_LN> descuentos = new List<UAG_CFDI_LN>();



                    descuentos = contextoCfdi.UAG_CFDI_LN
                                        .Where(i => i.UAG_CFDI_UUID == UUID)
                                        .ToList();
                    if (descuentos != null && descuentos.Count > 0)
                    {
                        foreach (UAG_CFDI_LN descuento in descuentos)
                        {
                            totalDBDescuento = totalDBDescuento + descuento.UAG_CFDI_DESCUENTO;
                        }
                    }
                    else
                    {
                        totalDBDescuento = 0M;
                    }

                    if (totalDBDescuento <= totalXMLDescuento + .99M && totalDBDescuento >= totalXMLDescuento - .99M)
                    {
                        flag = "";
                    }
                    else
                    {
                        if (flag.Count() > 0)
                        {
                            flag = flag + ";";
                        }
                        flag = "La suma de los descuentos en las lineas no es igual al total de los descuentos.";
                    }

                }
                catch (Exception)
                {
                    flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                }
            }
            return flag;
        }
        public static string getSubtotal(byte[] fileBytes, string UUID)
        {
            string flag = "";
            //produccion
            PRNFACTEntities contextoCfdi = new PRNFACTEntities();
            MemoryStream memoryStream = new MemoryStream(fileBytes);
            XmlDocument cfdi;
            string encoding;
            decimal totalXMLSubTotal;
            decimal totalDBSubtotal = 0M;


            if (memoryStream.CanSeek)
            {
                try
                {
                    cfdi = new XmlDocument();

                    try
                    {
                        XmlTextReader reader = new XmlTextReader(memoryStream);
                        cfdi.Load(reader);
                    }
                    catch (Exception)
                    {
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        encoding = "ISO-8859-1";
                        StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                        try
                        {
                            cfdi.Load(streamReader);
                        }
                        catch (Exception)
                        {
                            flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                        }
                    }
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
                    nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                    nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                    XmlNode xmlVoucher = cfdi.SelectSingleNode("cfdi:Comprobante", nsmgr);
                    totalXMLSubTotal = (xmlVoucher.Attributes["SubTotal"] == null) ? 0M
                                    : Decimal.Parse(xmlVoucher.Attributes["SubTotal"].Value, CultureInfo.InvariantCulture);

                    List<UAG_CFDI_LN> importes = new List<UAG_CFDI_LN>();



                    importes = contextoCfdi.UAG_CFDI_LN
                                        .Where(i => i.UAG_CFDI_UUID == UUID)
                                        .ToList();
                    if (importes != null && importes.Count > 0)
                    {
                        foreach (UAG_CFDI_LN importe in importes)
                        {
                            totalDBSubtotal = totalDBSubtotal + importe.UAG_CFDI_IMPORTE;
                        }
                    }
                    else
                    {
                        totalDBSubtotal = 0M;
                    }

                    if (totalDBSubtotal <= totalXMLSubTotal + .99M && totalDBSubtotal >= totalXMLSubTotal - .99M)
                    {
                        flag = "";
                    }
                    else
                    {
                        if (flag.Count() > 0)
                        {
                            flag = flag + ";";
                        }
                        flag = "La suma de los importes en las lineas no es igual al SubTotal.";
                    }

                }
                catch (Exception)
                {
                    flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                }
            }
            return flag;
        }
        public static string getTipoCambio(byte[] fileBytes, string UUID)
        {
            string flag = "";
            //produccion
            PRNFACTEntities contextoCfdi = new PRNFACTEntities();
            MemoryStream memoryStream = new MemoryStream(fileBytes);
            XmlDocument cfdi;
            string encoding;
            decimal tipoCambio;
            string Moneda = "";


            if (memoryStream.CanSeek)
            {
                try
                {
                    cfdi = new XmlDocument();

                    try
                    {
                        XmlTextReader reader = new XmlTextReader(memoryStream);
                        cfdi.Load(reader);
                    }
                    catch (Exception)
                    {
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        encoding = "ISO-8859-1";
                        StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                        try
                        {
                            cfdi.Load(streamReader);
                        }
                        catch (Exception)
                        {
                            flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                        }
                    }
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
                    nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                    nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                    XmlNode xmlVoucher = cfdi.SelectSingleNode("cfdi:Comprobante", nsmgr);
                    tipoCambio = (xmlVoucher.Attributes["TipoCambio"] == null) ? 0M
                                    : Decimal.Parse(xmlVoucher.Attributes["TipoCambio"].Value.Trim() == "" ?
                                    "1.0" : xmlVoucher.Attributes["TipoCambio"].Value, CultureInfo.InvariantCulture);
                    Moneda = (xmlVoucher.Attributes["Moneda"] == null) ? "MXN"
                                    : xmlVoucher.Attributes["Moneda"].Value;


                    if ((Moneda.Equals("MXN") && (tipoCambio == 1 || tipoCambio == 0)) || (!Moneda.Equals("MXN") && tipoCambio > 0))
                    {
                        flag = "";
                    }
                    else
                    {
                        flag = "El campo TipoCambio no tiene el valor.";
                    }

                }
                catch (Exception)
                {
                    flag = "Error Inesperado, Disculpe las molestias que esto le ocasione";
                }
            }
            return flag;
        }
        public static bool DeleteUUID(string uuid)
        {
            PRNFACTEntities contextoCfdi = new PRNFACTEntities();
            UAG_CFDI_HDR hdr = new UAG_CFDI_HDR();
            List<UAG_CFDI_LN> ln = new List<UAG_CFDI_LN>();
            List<UAG_CFDI_LN_IMP> ln_imp = new List<UAG_CFDI_LN_IMP>();
            List<UAG_CFDI_IM_LOC> im_loc = new List<UAG_CFDI_IM_LOC>();
            //UAG_PO_UUID ppo = new UAG_PO_UUID();

            hdr = contextoCfdi.UAG_CFDI_HDR
                    .Where(i => i.UAG_CFDI_UUID == uuid)
                    .FirstOrDefault();
            ln = contextoCfdi.UAG_CFDI_LN
                        .Where(i => i.UAG_CFDI_UUID == uuid)
                        .ToList();
            ln_imp = contextoCfdi.UAG_CFDI_LN_IMP
                        .Where(i => i.UAG_CFDI_UUID == uuid)
                        .ToList();
            im_loc = contextoCfdi.UAG_CFDI_IM_LOC
                        .Where(i => i.UAG_CFDI_UUID == uuid)
                        .ToList();
            /*ppo = contextoCfdi.UAG_PO_UUID
                        .Where(i => i.UAG_CFDI_UUID == uuid
                                && i.STATUS_CONCIL == "0")
                        .FirstOrDefault();*/

            if (hdr != null)
            {
                contextoCfdi.UAG_CFDI_HDR.Remove(hdr);
                contextoCfdi.SaveChanges();
            }

            if (ln != null && ln.Count > 0)
            {
                foreach (UAG_CFDI_LN lnT in ln)
                {
                    contextoCfdi.UAG_CFDI_LN.Remove(lnT);
                    contextoCfdi.SaveChanges();
                }
            }

            if (ln_imp != null && ln_imp.Count > 0)
            {
                foreach (UAG_CFDI_LN_IMP ln_impT in ln_imp)
                {
                    contextoCfdi.UAG_CFDI_LN_IMP.Remove(ln_impT);
                    contextoCfdi.SaveChanges();
                }
            }

            if (im_loc != null && im_loc.Count > 0)
            {
                foreach (UAG_CFDI_IM_LOC im_locT in im_loc)
                {
                    contextoCfdi.UAG_CFDI_IM_LOC.Remove(im_locT);
                    contextoCfdi.SaveChanges();
                }
            }
            /*
            if (ppo != null)
            {
                contextoCfdi.UAG_PO_UUID.Remove(ppo);
                contextoCfdi.SaveChanges();
            }*/

            return true;
        }
    }
}