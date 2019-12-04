using System;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Xml;
using UAGUtileriasSAT.DataBase;

namespace UAGUtileriasSAT.Metodos
{
    public class SaveFromXML
    {
        private XmlDocument cfdi;
        private MemoryStream memoryStream;
        private string encoding;

        private PRNFACTEntities contexto;

        private string uuid;
        private decimal total;

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

        public SaveFromXML(byte[] fileBytes, string source)
        {
            contexto = new PRNFACTEntities();


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
                            throw new ArgumentException("Error Inesperado, Disculpe las molestias que esto le ocasione - ", exc.Message);
                        }
                    }

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(cfdi.NameTable);
                    nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                    nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                    XmlNode xmlVoucher = cfdi.SelectSingleNode("cfdi:Comprobante", nsmgr);
                    XmlNode xmlClient = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Receptor", nsmgr);
                    XmlNode xmlProvider = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Emisor", nsmgr);
                    XmlNode xmlTax = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Impuestos", nsmgr);
                    XmlNode xmlTimbreFiscal = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Complemento/tfd:TimbreFiscalDigital", nsmgr);
                    XmlNode xmlComplemento = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Complemento", nsmgr);
                    XmlNode xmlConceptos = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Conceptos", nsmgr);
                    //folioProv = contexto.UAG_CFDI_HDR.OrderByDescending(l => l.UAG_CFDI_FOLIOPROV).Select(l => l.UAG_CFDI_FOLIOPROV).FirstOrDefault() + 1;

                    //Cabecera XML
                    UAG_CFDI_HDR cabecera = new UAG_CFDI_HDR();
                    cabecera.SOURCE = source;
                    cabecera.UAG_CFDI_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper();
                    cabecera.UAG_CFDI_FOLIO = (xmlVoucher.Attributes["Folio"] == null) ? " " : xmlVoucher.Attributes["Folio"].Value;
                    cabecera.UAG_CFDI_FECHA = (xmlVoucher.Attributes["Fecha"] == null) ? DateTime.Now : DateTime.Parse(xmlVoucher.Attributes["Fecha"].Value);
                    cabecera.UAG_CFDI_FORMAPAGO = (xmlVoucher.Attributes["FormaPago"] == null) ? " " : xmlVoucher.Attributes["FormaPago"].Value.Trim();
                    cabecera.UAG_CFDI_METODOPAG = (xmlVoucher.Attributes["MetodoPago"] == null) ? " " : xmlVoucher.Attributes["MetodoPago"].Value;
                    cabecera.UAG_CFDI_MONEDA = (xmlVoucher.Attributes["Moneda"] == null) ? " " : xmlVoucher.Attributes["Moneda"].Value;
                    cabecera.UAG_CFDI_SERIE = xmlVoucher.Attributes["Serie"] == null ? " " : xmlVoucher.Attributes["Serie"].Value;
                    cabecera.UAG_CFDI_SUBTOTAL = (xmlVoucher.Attributes["SubTotal"] == null) ? 0 : decimal.Parse(xmlVoucher.Attributes["SubTotal"].Value, new CultureInfo("en-US"));
                    cabecera.UAG_CFDI_TIPOCOMPR = (xmlVoucher.Attributes["TipoDeComprobante"] == null) ? " " : xmlVoucher.Attributes["TipoDeComprobante"].Value;
                    cabecera.UAG_CFDI_TOTAL = (xmlVoucher.Attributes["Total"] == null) ? 0 : decimal.Parse(xmlVoucher.Attributes["Total"].Value, new CultureInfo("en-US"));
                    cabecera.UAG_CFDI_VERSION = (xmlVoucher.Attributes["Version"] == null) ? " " : xmlVoucher.Attributes["Version"].Value;
                    cabecera.UAG_CFDI_EMISOR = (xmlProvider.Attributes["Nombre"] == null) ? " " : xmlProvider.Attributes["Nombre"].Value;
                    cabecera.UAG_CFDI_EMISORRFC = (xmlProvider.Attributes["Rfc"] == null) ? " " : xmlProvider.Attributes["Rfc"].Value;
                    cabecera.UAG_CFDI_EMISORREG = (xmlProvider.Attributes["RegimenFiscal"] == null) ? " " : xmlProvider.Attributes["RegimenFiscal"].Value;
                    cabecera.UAG_CFDI_RECEPTOR = (xmlClient.Attributes["Nombre"] == null) ? " " : xmlClient.Attributes["Nombre"].Value;
                    cabecera.UAG_CFDI_RECEPTRFC = (xmlClient.Attributes["Rfc"] == null) ? " " : xmlClient.Attributes["Rfc"].Value;
                    cabecera.UAG_CFDI_USO = (xmlClient.Attributes["UsoCFDI"] == null) ? " " : xmlClient.Attributes["UsoCFDI"].Value;
                    cabecera.UAG_CFDI_TIPOCAMBIO = (xmlVoucher.Attributes["TipoCambio"] != null) ?
                        decimal.Parse(xmlVoucher.Attributes["TipoCambio"].Value.Trim() == "" ? "1.0" : xmlVoucher.Attributes["TipoCambio"].Value, new CultureInfo("en-US"))
                        : decimal.Parse("1.0", new CultureInfo("en-US"));

                    if (xmlTax == null)
                        cabecera.UAG_CFDI_IVA = 0;
                    else
                    {
                        if (xmlTax.Attributes["TotalImpuestosTrasladados"] == null)
                            cabecera.UAG_CFDI_IVA = 0;
                        else
                            cabecera.UAG_CFDI_IVA = decimal.Parse(xmlTax.Attributes["TotalImpuestosTrasladados"].Value, new CultureInfo("en-US"));
                    }

                    if (xmlVoucher.Attributes["Folio"] == null && xmlVoucher.Attributes["Serie"] == null)
                    {
                        cabecera.UAG_CFDI_FOLIOPROV = cabecera.UAG_CFDI_UUID.Substring(6, 30);
                    }
                    else if (xmlVoucher.Attributes["Serie"] == null && xmlVoucher.Attributes["Folio"] != null)
                    {
                        cabecera.UAG_CFDI_FOLIOPROV = xmlVoucher.Attributes["Folio"].Value;
                    }
                    else if (xmlVoucher.Attributes["Serie"] != null && xmlVoucher.Attributes["Folio"] == null)
                    {
                        cabecera.UAG_CFDI_FOLIOPROV = cabecera.UAG_CFDI_UUID.Substring(6, 30);
                    }
                    else if (xmlVoucher.Attributes["Serie"] != null && xmlVoucher.Attributes["Folio"] != null)
                    {
                        cabecera.UAG_CFDI_FOLIOPROV = xmlVoucher.Attributes["Serie"].Value + xmlVoucher.Attributes["Folio"].Value;
                    }


                    cabecera.UAG_CFDI_DOC = fileBytes;
                    cabecera.UAG_CFDI_TOTAL_LINEAS = (xmlConceptos.ChildNodes.Count > 0) ? (short)xmlConceptos.ChildNodes.Count : (short)0;
                    contexto.UAG_CFDI_HDR.Add(cabecera);
                    contexto.SaveChanges();

                    uuid = xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper();
                    total = (xmlVoucher.Attributes["Total"] == null) ? 0 : decimal.Parse(xmlVoucher.Attributes["Total"].Value, new CultureInfo("en-US"));

                    for (int x = 0; x < xmlComplemento.ChildNodes.Count; x++)
                    {
                        if (xmlComplemento.ChildNodes[x].Name == "implocal:ImpuestosLocales")
                        {
                            int countImpLocal = 1;
                            XmlNode impuestosLocales = xmlComplemento.ChildNodes.Item(x);
                            XmlNode retencionesLocales = impuestosLocales.ChildNodes.Item(0);
                            XmlNode trasladosLocales = impuestosLocales.ChildNodes.Item(1);

                            if (retencionesLocales != null)
                            {
                                UAG_CFDI_IM_LOC retencion = new UAG_CFDI_IM_LOC();
                                retencion.UAG_CFDI_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper();
                                retencion.UAG_CFDI_NUM_LINEA = countImpLocal;
                                retencion.UAG_CFDI_TIPOIMP = "R";
                                retencion.UAG_CFDI_IMP_LOC = (retencionesLocales.Attributes["ImpLocRetenido"] == null) ? " " : retencionesLocales.Attributes["ImpLocRetenido"].Value;
                                retencion.UAG_CFDI_TASACUOTA = (retencionesLocales.Attributes["TasadeRetencion"] == null) ? 0 : decimal.Parse(retencionesLocales.Attributes["TasadeRetencion"].Value, new CultureInfo("en-US"));
                                retencion.UAG_CFDI_IMPORTE = (retencionesLocales.Attributes["Importe"] == null) ? 0 : decimal.Parse(retencionesLocales.Attributes["Importe"].Value, new CultureInfo("en-US"));
                                contexto.UAG_CFDI_IM_LOC.Add(retencion);
                                contexto.SaveChanges();
                                countImpLocal++;
                            }

                            if (trasladosLocales != null)
                            {
                                UAG_CFDI_IM_LOC traslado = new UAG_CFDI_IM_LOC();
                                traslado.UAG_CFDI_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper();
                                traslado.UAG_CFDI_NUM_LINEA = countImpLocal;
                                traslado.UAG_CFDI_TIPOIMP = "T";
                                traslado.UAG_CFDI_IMP_LOC = (trasladosLocales.Attributes["ImpLocTrasladado"] == null) ? " " : trasladosLocales.Attributes["ImpLocTrasladado"].Value;
                                traslado.UAG_CFDI_TASACUOTA = (trasladosLocales.Attributes["TasadeTraslado"] == null) ? 0 : decimal.Parse(trasladosLocales.Attributes["TasadeTraslado"].Value, new CultureInfo("en-US"));
                                traslado.UAG_CFDI_IMPORTE = (trasladosLocales.Attributes["Importe"] == null) ? 0 : decimal.Parse(trasladosLocales.Attributes["Importe"].Value, new CultureInfo("en-US"));
                                contexto.UAG_CFDI_IM_LOC.Add(traslado);
                                contexto.SaveChanges();
                            }

                        }
                    }
                    int count = 1;
                    for (int x = 0; x < xmlConceptos.ChildNodes.Count; x++)
                    {
                        XmlNode concepto = xmlConceptos.ChildNodes.Item(x);
                        XmlNode impuestos = concepto.ChildNodes.Item(0);

                        try
                        {
                            UAG_CFDI_LN linea = new UAG_CFDI_LN();
                            linea.UAG_CFDI_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper();
                            linea.UAG_CFDI_NUM_LINEA = count;
                            linea.UAG_CFDI_PRODSERV = (concepto.Attributes["ClaveProdServ"] == null) ? " " : concepto.Attributes["ClaveProdServ"].Value;
                            linea.UAG_CFDI_CLVUNIDAD = (concepto.Attributes["ClaveUnidad"] == null) ? " " : concepto.Attributes["ClaveUnidad"].Value.Trim();
                            linea.UAG_CFDI_CANTIDAD = (concepto.Attributes["Cantidad"] == null) ? 0 : decimal.Parse(concepto.Attributes["Cantidad"].Value, new CultureInfo("en-US"));

                            linea.UAG_CFDI_DESCR = (concepto.Attributes["Descripcion"] == null) ? " " :
                                concepto.Attributes["Descripcion"].Value.Length > 499 ?
                                    concepto.Attributes["Descripcion"].Value.Substring(1, 499) :
                                        concepto.Attributes["Descripcion"].Value;

                            linea.UAG_CFDI_VALORUNIT = (concepto.Attributes["ValorUnitario"] == null) ? 0 : decimal.Parse(concepto.Attributes["ValorUnitario"].Value, new CultureInfo("en-US"));
                            linea.UAG_CFDI_IMPORTE = (concepto.Attributes["Importe"] == null) ? 0 : decimal.Parse(concepto.Attributes["Importe"].Value, new CultureInfo("en-US"));
                            linea.UAG_CFDI_DESCUENTO = (concepto.Attributes["Descuento"] != null) ? decimal.Parse(concepto.Attributes["Descuento"].Value, new CultureInfo("en-US")) : decimal.Parse("0.00", new CultureInfo("en-US"));
                            contexto.UAG_CFDI_LN.Add(linea);
                            contexto.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            string messaex1 = string.Empty;
                            string messaex2 = string.Empty;
                            messaex1 = e.Message;
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                messaex1 = "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" +
                                    eve.Entry.Entity.GetType().Name + eve.Entry.State;
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    messaex2 = "- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + ve.ErrorMessage;
                                }


                            }
                        }
                        if (impuestos != null)
                        {
                            int countImp = 1;
                            for (int y = 0; y < impuestos.ChildNodes.Count; y++)
                            {
                                if (impuestos.ChildNodes[y].Name == "cfdi:Traslados")
                                {

                                    XmlNode trasladosConceptoT = impuestos.ChildNodes.Item(y);
                                    for (int tras = 0; tras < trasladosConceptoT.ChildNodes.Count; tras++)
                                    {
                                        XmlNode trasladosConcepto = trasladosConceptoT.ChildNodes.Item(tras);

                                        UAG_CFDI_LN_IMP lineaImp = new UAG_CFDI_LN_IMP();
                                        lineaImp.UAG_CFDI_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper();
                                        lineaImp.UAG_CFDI_NUM_LINEA = count;
                                        lineaImp.UAG_CFDI_NUMLN_IMP = countImp;
                                        lineaImp.UAG_CFDI_TIPOIMP = "T";
                                        lineaImp.UAG_CFDI_BASE = (trasladosConcepto.Attributes["Base"] == null) ? 0 : decimal.Parse(trasladosConcepto.Attributes["Base"].Value, new CultureInfo("en-US"));
                                        lineaImp.UAG_CFDI_IMPUESTO = (trasladosConcepto.Attributes["Impuesto"] == null) ? " " : trasladosConcepto.Attributes["Impuesto"].Value;
                                        lineaImp.UAG_CFDI_TIPOFACTO = (trasladosConcepto.Attributes["TipoFactor"] == null) ? " " : trasladosConcepto.Attributes["TipoFactor"].Value;
                                        lineaImp.UAG_CFDI_TASACUOTA = (trasladosConcepto.Attributes["TasaOCuota"] == null) ? 0 : decimal.Parse(trasladosConcepto.Attributes["TasaOCuota"].Value, new CultureInfo("en-US"));
                                        lineaImp.UAG_CFDI_IMPORTE = (trasladosConcepto.Attributes["Importe"] == null) ? 0 : decimal.Parse(trasladosConcepto.Attributes["Importe"].Value, new CultureInfo("en-US"));
                                        contexto.UAG_CFDI_LN_IMP.Add(lineaImp);
                                        contexto.SaveChanges();

                                        countImp++;
                                    }
                                }

                                if (impuestos.ChildNodes[y].Name == "cfdi:Retenciones")
                                {

                                    XmlNode retencionesConceptoT = impuestos.ChildNodes.Item(y);
                                    for (int ret = 0; ret < retencionesConceptoT.ChildNodes.Count; ret++)
                                    {
                                        XmlNode retencionesConcepto = retencionesConceptoT.ChildNodes.Item(ret);
                                        UAG_CFDI_LN_IMP lineaImp = new UAG_CFDI_LN_IMP();
                                        lineaImp.UAG_CFDI_UUID = (xmlTimbreFiscal.Attributes["UUID"] == null) ? " " : xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper();
                                        lineaImp.UAG_CFDI_NUM_LINEA = count;
                                        lineaImp.UAG_CFDI_NUMLN_IMP = countImp;
                                        lineaImp.UAG_CFDI_TIPOIMP = "R";
                                        lineaImp.UAG_CFDI_BASE = (retencionesConcepto.Attributes["Base"] == null) ? 0 : decimal.Parse(retencionesConcepto.Attributes["Base"].Value, new CultureInfo("en-US"));
                                        lineaImp.UAG_CFDI_IMPUESTO = (retencionesConcepto.Attributes["Impuesto"] == null) ? " " : retencionesConcepto.Attributes["Impuesto"].Value;
                                        lineaImp.UAG_CFDI_TIPOFACTO = (retencionesConcepto.Attributes["TipoFactor"] == null) ? " " : retencionesConcepto.Attributes["TipoFactor"].Value;
                                        lineaImp.UAG_CFDI_TASACUOTA = (retencionesConcepto.Attributes["TasaOCuota"] == null) ? 0 : decimal.Parse(retencionesConcepto.Attributes["TasaOCuota"].Value, new CultureInfo("en-US"));
                                        lineaImp.UAG_CFDI_IMPORTE = (retencionesConcepto.Attributes["Importe"] == null) ? 0 : decimal.Parse(retencionesConcepto.Attributes["Importe"].Value, new CultureInfo("en-US"));
                                        contexto.UAG_CFDI_LN_IMP.Add(lineaImp);
                                        contexto.SaveChanges();
                                        countImp++;
                                    }
                                }
                            }
                        }


                        count++;
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