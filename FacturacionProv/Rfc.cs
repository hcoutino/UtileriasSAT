using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FacturacionProv
{
    public class Rfc
    {
        private string _value;
        private static readonly string validationExpression = "^([A-Z&Ññ]{3}|[A-Z][AEIOU][A-Z]{2})\\d{2}((01|03|05|07|08|10|12)(0[1-9]|[12]\\d|3[01])|02(0[1-9]|[12]\\d)|(04|06|09|11)(0[1-9]|[12]\\d|30))([A-Z0-9]{2}[0-9A])?$";

        public enum FiscalPersonality
        {
            Individual,
            Organization,
            Invalid
        }

        public Rfc()
        {
            _value = String.Empty;
        }

        public Rfc(string value, bool requiresValidation = false)
        {
            string tmpValue = String.IsNullOrEmpty(value) ? "" : HttpUtility.HtmlDecode(value.Trim());
            if (requiresValidation && !validate(tmpValue))
                throw new Exception("INVALID_RFC");
            else
                _value = tmpValue;
        }

        public string value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = String.IsNullOrEmpty(value) ? "" : value.Trim().ToUpper();
            }
        }

        public FiscalPersonality fiscalRegulation
        {
            get
            {
                if (isValid() && (_value.Length > 4) && (_value.Length < 14))
                {
                    return Char.IsNumber(_value, 3) ? FiscalPersonality.Organization : FiscalPersonality.Individual;
                }
                else
                    return FiscalPersonality.Invalid;
            }
        }

        public bool isValid()
        {
            return validate(_value);
        }

        public static bool validate(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value.Trim(), validationExpression);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
