using System;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Xml;
using UAGUtileriasSAT.DataBase;

namespace UAGUtileriasSAT.Metodos
{
    public class SaveCompag
    {
        private XmlDocument cfdi;
        private MemoryStream memoryStream;
        private string encoding;

        PRNFACTEntities contexto = new PRNFACTEntities();

        public SaveCompag(byte[] fileBytes, string source, ref string uuid, string filename, out OperacionComplemento operacion)
        {
            operacion = new OperacionComplemento();
            operacion.Archivo = filename;
            operacion.Error = false;

            memoryStream = new MemoryStream(fileBytes);
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
                        catch (Exception exc)
                        {
                            operacion.Error = true;
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

                    try
                    {
                        //Cabecera XML
                        UAG_COMPAG_HDR cabecera = new UAG_COMPAG_HDR();
                        cabecera.SOURCE = source;
                        cabecera.UAG_COMPAG_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper().Trim();
                        cabecera.UAG_COMPAG_FOLIO = (xmlVoucher.Attributes["Folio"] == null) ? " " : xmlVoucher.Attributes["Folio"].Value;
                        cabecera.UAG_COMPAG_FECHA = (xmlVoucher.Attributes["Fecha"] == null) ? DateTime.Now : DateTime.Parse(xmlVoucher.Attributes["Fecha"].Value);
                        cabecera.UAG_COMPAG_MONEDA = (xmlVoucher.Attributes["Moneda"] == null) ? " " : xmlVoucher.Attributes["Moneda"].Value;
                        cabecera.UAG_COMPAG_SERIE = xmlVoucher.Attributes["Serie"] == null ? " " : xmlVoucher.Attributes["Serie"].Value;
                        cabecera.UAG_COMPAG_SUBTOTAL = (xmlVoucher.Attributes["SubTotal"] == null) ? 0 : decimal.Parse(xmlVoucher.Attributes["SubTotal"].Value, new CultureInfo("en-US"));
                        cabecera.UAG_COMPAG_TIPOCOMPR = (xmlVoucher.Attributes["TipoDeComprobante"] == null) ? " " : xmlVoucher.Attributes["TipoDeComprobante"].Value;
                        cabecera.UAG_COMPAG_TOTAL = (xmlVoucher.Attributes["Total"] == null) ? 0 : decimal.Parse(xmlVoucher.Attributes["Total"].Value, new CultureInfo("en-US"));
                        cabecera.UAG_COMPAG_VERSION = (xmlVoucher.Attributes["Version"] == null) ? " " : xmlVoucher.Attributes["Version"].Value;
                        cabecera.UAG_COMPAG_EMISOR = (xmlProvider.Attributes["Nombre"] == null) ? " " : xmlProvider.Attributes["Nombre"].Value;
                        cabecera.UAG_COMPAG_EMISORRFC = (xmlProvider.Attributes["Rfc"] == null) ? " " : xmlProvider.Attributes["Rfc"].Value;
                        cabecera.UAG_COMPAG_EMISORREG = (xmlProvider.Attributes["RegimenFiscal"] == null) ? " " : xmlProvider.Attributes["RegimenFiscal"].Value;
                        cabecera.UAG_COMPAG_RECEPTOR = (xmlClient.Attributes["Nombre"] == null) ? " " : xmlClient.Attributes["Nombre"].Value;
                        cabecera.UAG_COMPAG_RECEPTORRFC = (xmlClient.Attributes["Rfc"] == null) ? " " : xmlClient.Attributes["Rfc"].Value;
                        cabecera.UAG_COMPAG_USO = (xmlClient.Attributes["UsoCFDI"] == null) ? " " : xmlClient.Attributes["UsoCFDI"].Value;
                        cabecera.UAG_COMPAG_FOLIOPROV = (xmlVoucher.Attributes["Folio"] == null) ? " " : xmlVoucher.Attributes["Folio"].Value;
                        cabecera.UAG_COMPAG_DOC = fileBytes;
                        contexto.UAG_COMPAG_HDR.Add(cabecera);
                        contexto.SaveChanges();
                        operacion.OperacionHDR = true;

                        uuid = cabecera.UAG_COMPAG_UUID;
                        operacion.UUID = uuid;
                    }
                    catch (Exception)
                    {
                        operacion.OperacionHDR = false;
                        operacion.Error = true;
                        operacion.Detalle += "<br />Error al insertar HDR";
                    }


                    int count = 1;
                    for (int x = 0; x < xmlConceptos.ChildNodes.Count; x++)
                    {
                        try
                        {
                            XmlNode concepto = xmlConceptos.ChildNodes.Item(x);
                            XmlNode impuestos = concepto.ChildNodes.Item(0);

                            UAG_COMPAG_LN linea = new UAG_COMPAG_LN();
                            linea.UAG_COMPAG_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper().Trim();
                            linea.UAG_COMPAG_NUM_LINEA = count;
                            linea.UAG_COMPAG_PRODSERV = (concepto.Attributes["ClaveProdServ"] == null) ? " " : concepto.Attributes["ClaveProdServ"].Value;
                            linea.UAG_COMPAG_CLVUNIDAD = (concepto.Attributes["ClaveUnidad"] == null) ? " " : concepto.Attributes["ClaveUnidad"].Value;
                            linea.UAG_COMPAG_QTY = (concepto.Attributes["Cantidad"] == null) ? 0 : decimal.Parse(concepto.Attributes["Cantidad"].Value, new CultureInfo("en-US"));
                            linea.UAG_COMPAG_DESCR = (concepto.Attributes["Descripcion"] == null) ? " " : concepto.Attributes["Descripcion"].Value;
                            linea.UAG_COMPAG_VALORUNIT = (concepto.Attributes["ValorUnitario"] == null) ? 0 : decimal.Parse(concepto.Attributes["ValorUnitario"].Value, new CultureInfo("en-US"));
                            linea.UAG_COMPAG_IMPORTE = (concepto.Attributes["Importe"] == null) ? 0 : decimal.Parse(concepto.Attributes["Importe"].Value, new CultureInfo("en-US"));
                            contexto.UAG_COMPAG_LN.Add(linea);
                            contexto.SaveChanges();
                            operacion.OperacionesLN++;
                        }
                        catch (Exception)
                        {
                            operacion.Error = true;
                            operacion.Detalle += "<br />Error al insertar LN";
                        }

                        count++;
                    }

                    int countPago = 1;
                    for (int x = 0; x < xmlComplemento.ChildNodes.Count; x++)
                    {
                        if (xmlComplemento.ChildNodes[x].LocalName == "Pagos")
                        {
                            XmlNode pago = xmlComplemento.ChildNodes.Item(x);

                            for (int i = 0; i < pago.ChildNodes.Count; i++)
                            {
                                try
                                {
                                    XmlNode infoPago = pago.ChildNodes.Item(i);
                                    XmlNode infoRelacionado = null;
                                    if (xmlCfdiRelacionados != null)
                                    {
                                        infoRelacionado = xmlCfdiRelacionados.ChildNodes.Item(i);
                                    }

                                    UAG_COMPAG_PYMNT pagoDB = new UAG_COMPAG_PYMNT();
                                    pagoDB.UAG_COMPAG_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper().Trim();
                                    pagoDB.UAG_COMPAG_LN = countPago;
                                    pagoDB.UAG_COMPAG_CTABEN = (infoPago.Attributes["CtaBeneficiario"] == null) ? " " : infoPago.Attributes["CtaBeneficiario"].Value;
                                    pagoDB.UAG_COMPAG_CTABENRFC = (infoPago.Attributes["RfcEmisorCtaBen"] == null) ? " " : infoPago.Attributes["RfcEmisorCtaBen"].Value;
                                    pagoDB.UAG_COMPAG_CTAORD = (infoPago.Attributes["CtaOrdenante"] == null) ? " " : infoPago.Attributes["CtaOrdenante"].Value;
                                    pagoDB.UAG_COMPAG_CTAORDRFC = (infoPago.Attributes["RfcEmisorCtaOrd"] == null) ? " " : infoPago.Attributes["RfcEmisorCtaOrd"].Value;
                                    pagoDB.UAG_COMPAG_NUMOPR = (infoPago.Attributes["NumOperacion"] == null) ? " " : infoPago.Attributes["NumOperacion"].Value;
                                    pagoDB.UAG_COMPAG_MONTO = (infoPago.Attributes["Monto"] == null) ? 0 : decimal.Parse(infoPago.Attributes["Monto"].Value, new CultureInfo("en-US"));
                                    pagoDB.UAG_COMPAG_MONEDA = (infoPago.Attributes["MonedaP"] == null) ? " " : infoPago.Attributes["MonedaP"].Value;
                                    pagoDB.UAG_COMPAG_FORMAPAG = (infoPago.Attributes["FormaDePagoP"] == null) ? " " : infoPago.Attributes["FormaDePagoP"].Value;
                                    pagoDB.UAG_COMPAG_FECHAPAG = (infoPago.Attributes["FechaPago"] == null) ? DateTime.Now : DateTime.Parse(infoPago.Attributes["FechaPago"].Value);
                                    contexto.UAG_COMPAG_PYMNT.Add(pagoDB);
                                    contexto.SaveChanges();
                                    operacion.OperacionesLNPYMNT++;

                                    int countDocRel = 1;
                                    /*Registrea Documento Relacionado del Pago siempre y cuando este en el nodo de Pago*/
                                    for (int j = 0; j < infoPago.ChildNodes.Count; j++)
                                    {
                                        if (infoPago.ChildNodes[j].LocalName == "DoctoRelacionado")
                                        {
                                            try
                                            {
                                                XmlNode docRel = infoPago.ChildNodes.Item(j);
                                                UAG_COMPAG_PYMNT_DOCREL docRelDB = new UAG_COMPAG_PYMNT_DOCREL();
                                                docRelDB.UAG_COMPAG_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper().Trim();
                                                docRelDB.UAG_COMPAG_LN = countPago;
                                                docRelDB.UAG_COMPAG_DOC_LN = countDocRel;
                                                docRelDB.UAG_COMPAG_FOLIO = (docRel.Attributes["Folio"] == null) ? " " : docRel.Attributes["Folio"].Value;
                                                docRelDB.UAG_COMPAG_SERIE = (docRel.Attributes["Serie"] == null) ? " " : docRel.Attributes["Serie"].Value;
                                                docRelDB.UAG_COMPAG_SALINSO = (docRel.Attributes["ImpSaldoInsoluto"] == null) ? 0 : decimal.Parse(docRel.Attributes["ImpSaldoInsoluto"].Value, new CultureInfo("en-US"));
                                                docRelDB.UAG_COMPAG_IMPPAG = (docRel.Attributes["ImpPagado"] == null) ? 0 : decimal.Parse(docRel.Attributes["ImpPagado"].Value, new CultureInfo("en-US"));
                                                docRelDB.UAG_COMPAG_SALANTE = (docRel.Attributes["ImpSaldoAnt"] == null) ? 0 : decimal.Parse(docRel.Attributes["ImpSaldoAnt"].Value, new CultureInfo("en-US"));
                                                docRelDB.UAG_COMPAG_NUMPAR = (docRel.Attributes["NumParcialidad"] == null) ? 0 : decimal.Parse(docRel.Attributes["NumParcialidad"].Value, new CultureInfo("en-US"));
                                                docRelDB.UAG_COMPAG_METPAG = (docRel.Attributes["MetodoDePagoDR"] == null) ? " " : docRel.Attributes["MetodoDePagoDR"].Value;
                                                docRelDB.UAG_COMPAG_MONEDA = (docRel.Attributes["MonedaDR"] == null) ? " " : docRel.Attributes["MonedaDR"].Value;
                                                docRelDB.UAG_COMPAG_DOCREL = (docRel.Attributes["IdDocumento"] == null) ? " " : docRel.Attributes["IdDocumento"].Value.ToUpper().Trim();
                                                contexto.UAG_COMPAG_PYMNT_DOCREL.Add(docRelDB);
                                                contexto.SaveChanges();
                                                operacion.OperacionesPYMNT_DOCREL++;
                                            }
                                            catch (Exception)
                                            {
                                                operacion.Error = true;
                                                operacion.Detalle += "<br />Error al insertar PYMNT_DOCREL";
                                            }

                                            countDocRel++;
                                        }
                                    }
                                    if (xmlCfdiRelacionados != null)
                                    {
                                        /*Guarda Documento Relacionado del Externo*/
                                        if (infoRelacionado.Attributes["UUID"] != null)
                                        {
                                            try
                                            {
                                                UAG_COMPAG_PYMNT_DOCREL docRelDB = new UAG_COMPAG_PYMNT_DOCREL();
                                                docRelDB.UAG_COMPAG_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper().Trim();
                                                docRelDB.UAG_COMPAG_LN = countPago;
                                                docRelDB.UAG_COMPAG_DOC_LN = countDocRel;
                                                docRelDB.UAG_COMPAG_FOLIO = " ";
                                                docRelDB.UAG_COMPAG_SERIE = " ";
                                                docRelDB.UAG_COMPAG_SALINSO = 0;
                                                docRelDB.UAG_COMPAG_IMPPAG = (infoPago.Attributes["Monto"] == null) ? 0 : decimal.Parse(infoPago.Attributes["Monto"].Value, new CultureInfo("en-US"));
                                                docRelDB.UAG_COMPAG_SALANTE = (infoPago.Attributes["Monto"] == null) ? 0 : decimal.Parse(infoPago.Attributes["Monto"].Value, new CultureInfo("en-US"));
                                                docRelDB.UAG_COMPAG_NUMPAR = 1;
                                                docRelDB.UAG_COMPAG_METPAG = "PPD";
                                                docRelDB.UAG_COMPAG_MONEDA = (infoPago.Attributes["MonedaP"] == null) ? " " : infoPago.Attributes["MonedaP"].Value;
                                                docRelDB.UAG_COMPAG_DOCREL = (infoRelacionado.Attributes["UUID"] == null) ? " " : infoRelacionado.Attributes["UUID"].Value.ToUpper().Trim();
                                                contexto.UAG_COMPAG_PYMNT_DOCREL.Add(docRelDB);
                                                contexto.SaveChanges();
                                                operacion.OperacionesPYMNT_DOCREL++;
                                            }
                                            catch (Exception)
                                            {
                                                operacion.Error = true;
                                                operacion.Detalle += "<br />Error al insertar PYMNT_DOCREL";
                                            }

                                            countDocRel++;
                                        }
                                    }


                                }
                                catch (DbEntityValidationException e)
                                {
                                    operacion.Error = true;
                                    operacion.Detalle += "<br />Error al insertar PYMNT, " + e.Message;
                                    operacion.Detalle += "<br />Validation Errors: ";
                                    foreach (var eve in e.EntityValidationErrors)
                                    {
                                        operacion.Detalle += string.Format("<br />Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                    }

                                }
                                catch (Exception e)
                                {
                                    operacion.Error = true;
                                    operacion.Detalle += "<br />Error al insertar PYMNT " + e.Message;
                                }

                                countPago++;
                            }
                        }
                    }


                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw new ArgumentException("Error Inesperado, Ver consola - ", e.Message);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error Inesperado, Disculpe las molestias que esto le ocasione - ", ex.Message);
                }
            }
        }
    }
}