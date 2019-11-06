using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace UAGUtileriasSAT.FacturacionProv
{
    public class Invoice
    {
        private int _id;
        private Enterprise _provider;
        private Enterprise _client;
        private bool _providerNameSpecified;
        private string _providerInvoiceId;
        private double _paymentAmount;
        private Currency _currency;
        private string _currencySymbol;
        private double _exchangeRate;
        private double _originalPaymentAmount;
        private DateTime _invoiceDate;
        private DateTime _receptionDate;
        private DateTime _openFromDate;
        private DateTime _openToDate;
        private string _annotations;
        private byte[] _binaryFile;
        private string _encoding;
        private int _fileSize;
        private System.Xml.XmlDocument _cfdi;
        private InvoiceFileExtension _fileExtension;
        private Guid? _uid;
        private List<PaymentMethod> _paymentMethods;
        private int OpenYearTo;
        private int OpenYearFrom;
        public string UAGRFC;

        public enum InvoiceFileExtension
        {
            XML,
            PDF,
            INVALID
        }

        public enum Currency
        {
            MXN,
            USD,
            OTHER,
            INVALID
        }

        public Invoice()
        {
            _id = -1;
            _provider = new Enterprise();
            _client = new Enterprise();
            _providerInvoiceId = String.Empty;
            _paymentAmount = 0;
            _invoiceDate = DateTime.MinValue;
            _receptionDate = DateTime.Now;
            _annotations = String.Empty;
            _binaryFile = null;
            _fileExtension = InvoiceFileExtension.INVALID;
            _currency = Currency.MXN;
            _currencySymbol = "MXN";
            _exchangeRate = 1;
            _originalPaymentAmount = 0;
            _uid = null;
            _paymentMethods = new List<PaymentMethod>();
        }

        public Invoice(byte[] fileBytes, DateTime Open_From_Date, DateTime Open_To_Date, int _openYearFrom, int _openYearTo, string _UAGRFC)
        {
            string emisorNombre = "";
            string emisorRFC = "";
            string receptorNombre = "";
            string receptorRFC = "";
            string comprobanteTotal = "";
            string comprobanteFolio = "";
            string comprobantemetodoPago = "";
            string comprobanteFecha = "";
            _openFromDate = Open_From_Date;
            _openToDate = Open_To_Date;
            _id = -1;
            _receptionDate = DateTime.Now;
            _annotations = String.Empty;
            _binaryFile = fileBytes;
            _fileSize = _binaryFile.Length;
            _fileExtension = InvoiceFileExtension.INVALID;
            _paymentMethods = new List<PaymentMethod>();
            UAGRFC = _UAGRFC;
            OpenYearFrom = _openYearFrom;
            OpenYearTo = _openYearTo;
            
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(_binaryFile);
            if (memoryStream.CanSeek)
            {
                byte[] bof = new byte[4];
                memoryStream.Read(bof, 0, 4);

                /*
                    EF BB BF       = utf-8
                    FF FE          = ucs-2le, ucs-4le, and ucs-16le
                    FE FF          = utf-16 and ucs-2
                    00 00 FE FF    = ucs-4                 
                 */
                if (((bof[0] == 0xEF) && (bof[1] == 0xBB) && (bof[2] == 0xBF)) ||
                    ((bof[0] == 0xFF) && (bof[1] == 0xFE)) ||
                    ((bof[0] == 0xFE) && (bof[1] == 0xFF)) ||
                    ((bof[0] == 0x0) && (bof[1] == 0x0) && (bof[2] == 0xFE) && (bof[3] == 0xFF)))
                {
                    if ((bof[0] == 0xEF) && (bof[1] == 0xBB) && (bof[2] == 0xBF))
                        _encoding = "UTF-8";
                    else
                        _encoding = "Unicode";
                }
                else
                {
                    if ((bof[0] == 0x3C) && (bof[1] == 0x3F) && (bof[2] == 0x78) && (bof[3] == 0x6D))
                    {
                        _encoding = "UTF-8";
                        _fileExtension = InvoiceFileExtension.XML;
                    }
                    else
                    {
                        _encoding = "ASCII";

                        if ((bof[0] == 0x25) && (bof[1] == 0x50) && (bof[2] == 0x44) && (bof[3] == 0x46))
                        {
                            _fileExtension = InvoiceFileExtension.PDF;
                            _currency = Currency.MXN;
                            _exchangeRate = 1;
                        }
                    }
                }

                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                if (!String.IsNullOrEmpty(_encoding) && (_fileExtension != InvoiceFileExtension.PDF))
                {
                    _cfdi = new System.Xml.XmlDocument();
                    try
                    {
                        System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(memoryStream);
                        _cfdi.Load(reader);
                    }
                    catch (Exception)
                    {
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        _encoding = "ISO-8859-1";
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(memoryStream, System.Text.Encoding.GetEncoding(_encoding));

                        try
                        {
                            _cfdi.Load(streamReader);
                        }
                        catch (Exception)
                        {
                            _fileExtension = InvoiceFileExtension.INVALID;
                            return;
                        }
                    }

                    _fileExtension = InvoiceFileExtension.XML;
                    try
                    {
                        System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(cfdi.NameTable);
                        nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                        nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                        System.Xml.XmlNode xmlVoucher = cfdi.SelectSingleNode("cfdi:Comprobante", nsmgr);
                        System.Xml.XmlNode xmlClient = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Receptor", nsmgr);
                        System.Xml.XmlNode xmlProvider = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Emisor", nsmgr);
                        System.Xml.XmlNode xmlTimbreFiscal = cfdi.SelectSingleNode("cfdi:Comprobante/cfdi:Complemento/tfd:TimbreFiscalDigital", nsmgr);

                        if (xmlVoucher.Attributes["version"] == null)
                        {
                            //version 3.3
                            if (xmlVoucher.Attributes["Version"] != null)
                            {
                                emisorNombre = "Nombre";
                                emisorRFC = "Rfc";
                                receptorNombre = "Nombre";
                                receptorRFC = "Rfc";
                                comprobanteTotal = "Total";
                                comprobanteFolio = "Folio";
                                comprobantemetodoPago = "MetodoPago";
                                comprobanteFecha = "Fecha";
                            }
                        }
                        else
                        {
                            //version 3.2 or last
                            emisorNombre = "nombre";
                            emisorRFC = "rfc";
                            receptorNombre = "nombre";
                            receptorRFC = "rfc";
                            comprobanteTotal = "total";
                            comprobanteFolio = "folio";
                            comprobantemetodoPago = "metodoDePago";
                            comprobanteFecha = "fecha";
                        }


                        provider = new Enterprise(xmlProvider.Attributes[emisorNombre] == null ?
                                    xmlProvider.Attributes[emisorRFC].Value.Trim() : xmlProvider.Attributes[emisorNombre].Value,
                                    xmlProvider.Attributes[emisorRFC].Value.Trim());
                        _providerNameSpecified = xmlProvider.Attributes[emisorNombre] != null;
                        client = new Enterprise(xmlClient.Attributes[receptorNombre] == null ?
                                    xmlClient.Attributes[receptorRFC].Value.Trim() : xmlClient.Attributes[receptorNombre].Value,
                                    xmlClient.Attributes[receptorRFC].Value.Trim());
                        _originalPaymentAmount = xmlVoucher.Attributes[comprobanteTotal] == null ?
                                    0 : Double.Parse(xmlVoucher.Attributes[comprobanteTotal].Value);
                        providerInvoiceId = xmlVoucher.Attributes[comprobanteFolio] == null ?
                                    "" : xmlVoucher.Attributes[comprobanteFolio].Value;

                        if (xmlClient.Attributes[receptorRFC].Value == UAGRFC)
                            client.id = 1;

                        _currency = xmlVoucher.Attributes["Moneda"] == null ? Currency.MXN :
                            ((xmlVoucher.Attributes["Moneda"].Value.ToUpper().Replace(".", "").Trim().Contains("MX") ||
                             (xmlVoucher.Attributes["Moneda"].Value.ToUpper().Replace(".", "").Trim().Equals("MN") ||
                             xmlVoucher.Attributes["Moneda"].Value.ToUpper().Replace("É", "E").Trim().Contains("MEX")) ? Currency.MXN : Currency.OTHER));
                        if (_currency == Currency.MXN)
                            _exchangeRate = 1;
                        else
                        {
                            if (xmlVoucher.Attributes["TipoCambio"] == null)
                                _exchangeRate = 0;
                            else
                            {
                                Double.TryParse(xmlVoucher.Attributes["TipoCambio"].Value, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("es-MX"), out _exchangeRate);
                                if ((_exchangeRate == 0) && (xmlVoucher.Attributes["TipoCambio"].Value.ToUpper().Contains("PESOS")))
                                    _exchangeRate = 1;
                            }
                        }
                        _currencySymbol = xmlVoucher.Attributes["Moneda"] == null ? "MXN" : xmlVoucher.Attributes["Moneda"].Value;
                        paymentAmount = _originalPaymentAmount * _exchangeRate;
                        invoiceDate = System.Xml.XmlConvert.ToDateTime(xmlVoucher.Attributes[comprobanteFecha].Value,
                                        System.Xml.XmlDateTimeSerializationMode.Utc);
                        uid = Guid.Parse(xmlTimbreFiscal.Attributes["UUID"].Value.ToUpper());

                        //version 3.2 or last
                        string[] tmpPaymentMethods = xmlVoucher.Attributes[comprobantemetodoPago] == null ?
                                        new string[0] : xmlVoucher.Attributes[comprobantemetodoPago].Value.Split(',');

                        foreach (string tmpPaymentMethod in tmpPaymentMethods)
                        {
                            paymentMethods.Add(new PaymentMethod(tmpPaymentMethod));
                        }
                    }
                    catch (Exception)
                    {
                        _fileExtension = InvoiceFileExtension.INVALID;
                    }
                }
            }
        }

        public int openYearFrom
        {
            get
            {
                return OpenYearFrom;
            }
            set
            {
                OpenYearFrom = value;
            }
        }
        public int openYearTo
        {
            get
            {
                return OpenYearTo;
            }
            set
            {
                OpenYearTo = value;
            }
        }

        public DateTime openFromDate
        {
            get
            {
                return _openFromDate;
            }
            set
            {
                _openFromDate = value;
            }
        }

        public DateTime openToDate
        {
            get
            {
                return _openToDate;
            }
            set
            {
                _openToDate = value;
            }
        }
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public Enterprise provider
        {
            get
            {
                return _provider;
            }
            set
            {
                _provider = value;
            }
        }

        public Enterprise client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
            }
        }

        public string providerInvoiceId
        {
            get
            {
                return _providerInvoiceId;
            }
            set
            {
                _providerInvoiceId = String.IsNullOrEmpty(value) ? "" : value.Trim().Replace(" ", "");
            }
        }

        public double originalPaymentAmount
        {
            get
            {
                return _originalPaymentAmount;
            }
            set
            {
                _originalPaymentAmount = value;
            }
        }

        public double paymentAmount
        {
            get
            {
                return _paymentAmount;
            }
            set
            {
                _paymentAmount = value;
            }
        }

        public DateTime invoiceDate
        {
            get
            {
                return _invoiceDate;
            }
            set
            {
                _invoiceDate = value;
            }
        }

        public DateTime receptionDate
        {
            get
            {
                return _receptionDate;
            }
            set
            {
                _receptionDate = value;
            }
        }

        public string annotations
        {
            get
            {
                return _annotations;
            }
            set
            {
                _annotations = String.IsNullOrEmpty(value) ? "" : value.Trim();
            }
        }

        public byte[] binaryFile
        {
            get
            {
                return _binaryFile;
            }
            set
            {
                _binaryFile = value;
            }
        }

        public string encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                _encoding = String.IsNullOrEmpty(value) ? "" : value.Trim();
            }
        }

        public int fileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
            }
        }

        public int kbFileSize
        {
            get
            {
                return Convert.ToInt32(Math.Round(Convert.ToDecimal(fileSize / 1024)));
            }
        }

        public InvoiceFileExtension fileExtension
        {
            get
            {
                return _fileExtension;
            }
            set
            {
                _fileExtension = value;
            }
        }

        public System.Xml.XmlDocument cfdi
        {
            get
            {
                return _cfdi;
            }
        }

        public Currency currency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value;
            }
        }

        public string currencySymbol
        {
            get
            {
                return _currencySymbol;
            }
            set
            {
                _currencySymbol = String.IsNullOrEmpty(value) ? "" : value.Trim();
            }
        }

        public double exchangeRate
        {
            get
            {
                return _exchangeRate;
            }
            set
            {
                _exchangeRate = value;
            }
        }

        public string repositoryFileName
        {
            get
            {
                return String.Format("{0:D8}_{1:yyyyMMdd}_{2}_{3}.{4}", id, invoiceDate, provider.rfc, formatProviderInvoiceId(providerInvoiceId), fileExtension == InvoiceFileExtension.XML ? "xml" : fileExtension == InvoiceFileExtension.PDF ? "pdf" : "dat");
            }
        }

        public bool providerNameSpecified
        {
            get
            {
                return _providerNameSpecified;
            }
        }

        public Guid? uid
        {
            get
            {
                return _uid;
            }
            set
            {
                _uid = value;
            }
        }

        public List<PaymentMethod> paymentMethods
        {
            get
            {
                if (_paymentMethods == null)
                    _paymentMethods = new List<PaymentMethod>();
                return _paymentMethods;
            }
            set
            {
                _paymentMethods = value;
            }
        }

        private string formatProviderInvoiceId(string code)
        {
            string result = String.Empty;
            if (code.Length > 4)
                result = code.Substring(0, 4);
            else
            {
                result = code;
                while (result.Length < 4)
                    result = "0" + result;
            }
            return result;
        }


        public List<string> getValidationMessages()
        {
            List<string> result = new List<string>();
            

            if (binaryFile.Length < 1)
                result.Add("No se logró cargar el archivo de la factura electrónica.");

            if (fileExtension == InvoiceFileExtension.INVALID)
                result.Add("El archivo .xml o .pdf parece estar corrupto.");

            if ((client == null) || (client.id < 1))
            {
                result.Add("No se especificaron datos del receptor de la factura electrónica.");
            }
            else
            {
                if (!client.rfc.isValid())
                {
                    result.Add("El RFC del receptor de la factura electrónica parece ser inválido.");
                }
            }

            if (provider == null)
                result.Add("No se especificaron datos del emisor de la factura electrónica.");
            else
            {
                if (!provider.rfc.isValid())
                {
                    result.Add("El RFC del emisor de la factura electrónica parece ser inválido.");
                }
                else
                {
                    if ((provider.rfc.fiscalRegulation == Rfc.FiscalPersonality.Organization) && (fileExtension != InvoiceFileExtension.XML))
                        result.Add("El formato PDF de factura electrónica ya no es válido para personas morales.");
                }
            }

            if (invoiceDate.Date < new DateTime(2014, 1, 1))
                result.Add("No se permiten facturas anteriores al 1ro de Enero del 2014.");

            /*if(invoiceDate.Date.Year != DateTime.Now.Year)
                result.Add("No se permiten facturas fuera del año en curso.");*/

           /* if (!(invoiceDate.Date.Year >= OpenYearFrom && invoiceDate.Date.Year <= OpenYearTo))
                result.Add("No se permiten facturas fuera del año en curso.");*/

            /*if (invoiceDate.Month != DateTime.Now.Month)
            {
                DateTime past = DateTime.Today.AddMonths(-1);
                var now = DateTime.Now;
                var first = new DateTime(now.Year, now.Month, 1);
                //var finalVal = first.AddDays(3);
                var finalVal = DateTime.Today.AddDays(-30);

                if (invoiceDate.Month != past.Month || invoiceDate >= finalVal)
                {
                    result.Add("La fecha limite (" + string.Format(" { 0:dd:/MM/yyyy}" , finalVal) + "para la recepcion ha sido exedida");
                }
                
            }*/

            /*var finalVal = DateTime.Today.AddDays(-30);
            if (invoiceDate.Month != DateTime.Now.Month && invoiceDate < finalVal)
            {
                result.Add("La fecha limite (" + string.Format("{0:dd/MM/yyyy}", finalVal) + ") para la recepcion de la factura ha sido excedida.");
            }*/

            if (!(invoiceDate >= openFromDate && invoiceDate <= openToDate))
            {
                result.Add("La fecha limite (" + string.Format("{0:dd/MM/yyyy}", openFromDate) + "-" + string.Format("{0:dd/MM/yyyy}", openToDate) + ") para la recepcion de la factura ha sido excedida.");
            }

            if (_exchangeRate <= 0)
                result.Add("La factura electrónica se encuentra especificada en una moneda extranjera, pero no se especificó el tipo de cambio de la misma.");

            if (paymentAmount <= 0)
                result.Add("El monto acreditable de la factura debe ser superior a $0.00");


            return result;
        }
    }
}
