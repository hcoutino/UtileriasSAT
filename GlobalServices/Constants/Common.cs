namespace GlobalServices.Constants
{
    public class Common
    {
        #region Valores

        /// <summary>
        /// Constante que define en el listado, los valores activos
        /// </summary>
        public const string VAlOR_VERDADERO = "True";

        /// <summary>
        /// Constante que define en el listado, los valores activos
        /// </summary>
        public const string VALOR_FALSO = "False";

        /// <summary>
        /// Constante que define en el listado, los valores activos
        /// </summary>
        public const string VALOR_ACTIVO = "1";

        /// <summary>
        /// Constante que define en el listado, los valores inactivos
        /// </summary>
        public const string VALOR_INACTIVO = "0";

        /// <summary>
        /// Constante que define inicializacion de variables enteras en cero(0).
        /// </summary>
        public const int VALOR_CERO = 0;


        /// <summary>
        /// Constante que define inicializacion de variables enteras en uno(1).
        /// </summary>
        public const int VALOR_UNO = 1;

        /// <summary>
        /// Constante que define inicializacion de variables enteras en mes uno(-1).
        /// </summary>
        public const int VALOR_MENOS_UNO = -1;
        #endregion

        #region Formatos
        /// <summary>
        /// Constante que define el Formato para números Decimales.
        /// </summary>
        public const string FORMATO_DECIMAL = "{0:c}";

        #endregion

        #region Textos

        public const string TEXTO_TELEFONO = "Télefono: ";
        public const string TEXTO_FAX = " Fax: ";

        /// <summary>
        /// Texto para los contextkey inical de los 
        /// cascadingdropdown list
        /// </summary>
        public const string TEXTO_ALL = "all";
        #endregion

        #region CaracteresEspeciales

        /// <summary>
        ///Constante utilizada para separar arreglos de cadenas.
        /// </summary>
        public const string SEPARADOR_ARREGLOS = "|";


        /// <summary>
        /// Constante utilizada para separar variables dentro de un QUERY STRING.
        /// </summary>
        public const string SEPARADOR_QUERYS_STRING = "&";


        /// <summary>
        ///Constante utilizada para separar variables dentro de un QUERY STRING (=).
        /// </summary>
        public const string SEPARADOR_IGUAL = "=";


        /// <summary>
        /// Caracter delimitador (+)
        /// </summary>
        public const string SEPARADOR_PALABRAS_CLAVES = "+";

        /// <summary>
        /// Caracter (?)
        /// </summary>
        public const string SEPARADOR_URL_QUERY = "?";
        #endregion

        #region Caracteres

        /// <summary>
        /// Espacio en blanco
        /// </summary>
        public const string CARACTER_ESPACIO = " ";

        /// <summary>
        /// Constante delimitador, signo coma (,)
        /// </summary>
        public const string CARACTER_COMA = ",";

        /// <summary>
        /// Constante delimitador, signo guión medio (-)
        /// </summary>
        public const string CARACTER_GUION_MEDIO = " - ";

        /// <summary>
        /// Constante delimitador, signo guión medio (-)
        /// sin espacios
        /// </summary>
        public const string CARACTER_GUION_MEDIO_SIN_ESPACIOS = "-";

        /// <summary>
        /// Constante delimitador, signo guión bajo (_)
        /// </summary>
        public const string CARACTER_GUION_BAJO = "_";

        /// <summary>
        /// Constante delimitador, signo diagonal (/)
        /// </summary>
        public const string CARACTER_DIAGONAL = "/";

        /// <summary>
        /// Constante delimitador, signo inicio parentesis (()
        /// </summary>
        public const string CARACTER_PARENTESIS_ABRIR = "(";

        /// <summary>
        /// Constante delimitador, signo inicio parentesis ())
        /// </summary>
        public const string CARACTER_PARENTESIS_CERRAR = ")";

        /// <summary>
        /// Constante delimitador, signo (...) para finalizar textos.
        /// </summary>
        public const string CARACTER_PUNTOS_SUSPENSIVOS = " ...";

        /// <summary>
        /// Caracter (.)
        /// </summary>
        public const string CARACTER_PUNTO = ".";

        /// <summary>
        /// Caracter (..)
        /// </summary>
        public const string CARACTER_PUNTOS_DOBLE = "..";

        /// <summary>
        /// Caracter porcentaje (%)
        /// </summary>
        public const string CARACTER_PORCENTAJE = "%";

        #endregion

        #region SQL

        /// <summary>
        /// Constante que define si un valor es diferente a otro en SQL
        /// </summary>
        public const string SQL_DIFERENTE = "<>";

        /// <summary>
        /// Constante que define el orden aleatorio de SQL
        /// </summary>
        public const string SQL_ALEATORIO = "NEWID()";

        #endregion
    }
}
