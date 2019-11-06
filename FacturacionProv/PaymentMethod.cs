using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FacturacionProv
{
    public class PaymentMethod
    {
        private int _id;
        private string _code;
        private string _description;

        public PaymentMethod()
        {
            _id = -1;
            _code = String.Empty;
            _description = String.Empty;
        }

        public PaymentMethod(string code) : this()
        {
            _code = code;
        }

        public PaymentMethod(string code, string description)
            : this()
        {
            _code = code;
            _description = description;
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

        public string code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = String.IsNullOrEmpty(value) ? "" : HttpUtility.HtmlDecode(value.Trim());
            }
        }

        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = String.IsNullOrEmpty(value) ? "" : HttpUtility.HtmlDecode(value.Trim());
            }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj is PaymentMethod)
                return code.Equals(((PaymentMethod)obj).code);
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return code.GetHashCode();
        }

    }
}
