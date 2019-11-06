using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UAGUtileriasSAT.FacturacionProv
{
    public class Enterprise
    {
        private int _id;
        private string _name;
        private Rfc _rfc;
        private string _invoicesFolder;
        private string _logoUrl;
        private List<PaymentMethod> _paymentMethods;

        public Enterprise()
        {
            _id = -1;
            _name = String.Empty;
            _rfc = new Rfc();
            _invoicesFolder = String.Empty;
            _logoUrl = String.Empty;
            _paymentMethods = new List<PaymentMethod>();
        }

        public Enterprise(string name, string rfc)
        {
            _id = -1;
            this.name = name;
            this.rfc = new Rfc(rfc);
            _invoicesFolder = String.Empty;
            _logoUrl = String.Empty;
            _paymentMethods = new List<PaymentMethod>();
        }

        public Enterprise(string name, string rfc, string invoicesFolder)
        {
            _id = -1;
            this.name = name;
            this.rfc = new Rfc(rfc);
            this.invoicesFolder = invoicesFolder;
            _logoUrl = String.Empty;
            _paymentMethods = new List<PaymentMethod>();
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

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = String.IsNullOrEmpty(value) ? "" : HttpUtility.HtmlDecode(value.Trim());
            }
        }

        public string invoicesFolder
        {
            get
            {
                return _invoicesFolder;
            }
            set
            {
                _invoicesFolder = String.IsNullOrEmpty(value) ? "" : value.Trim();
            }
        }

        public string logoUrl
        {
            get
            {
                return _logoUrl;
            }
            set
            {
                _logoUrl = String.IsNullOrEmpty(value) ? "" : value.Trim();
            }
        }

        public Rfc rfc
        {
            get
            {
                return _rfc;
            }
            set
            {
                _rfc = value;
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

        public override string ToString()
        {
            return _name;
        }
    }
}
