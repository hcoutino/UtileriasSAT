namespace GlobalServices.Constants
{
    public class Configuration
    {
        /// <summary>
        /// Alias de la llave generada por Enterprise Library en el bloque de criptography.
        /// </summary>
        public const string CRYPTOGRAPHY_KEY = "TripleDESCryptoServiceProvider";

        /// <summary>
        /// Nombre del Exception Handling Policy 
        /// </summary>
        public const string EXCEPTION_POLICY = "LogOnlyPolicy";

        /// <summary>
        /// Nombre de la sesión de configuración. 
        /// </summary>
        public const string SESION_CONFIGURACION = "Configuracion";

        /// <summary>
        /// Ruta de la página para mensajes de error genericos.
        /// </summary>
        public const string MENSAJE_SISTEMA_URL = "~/mensajes/error.aspx";

        /// <summary>
        /// Directorio Raíz.
        /// </summary>
        public const string RUTA_RAIZ = "~/";

        /// <summary>
        /// Directorio Raíz de recursos compartidos.
        /// </summary>
        public const string RUTA_RAIZ_SHARED = "~/shared";

        /// <summary>
        /// Directorio Raíz de recursos compartidos.
        /// </summary>
        public const string RUTA_SHARED = "shared";

        /// <summary>
        /// Ruta Relativa de las images de Temas
        /// </summary>
        public const string RUTA_RAIZ_TEMAS = "~/App_Themes";

        /// <summary>
        /// Directorio Images de un tema.
        /// </summary>
        public const string RUTA_IMAGES = "/images";

        /// <summary>
        /// Constante para el nombre del CultureInfo en-US
        /// </summary>
        public const string CULTURE_NAME_US = "en-US";

        /// <summary>
        /// Nombre de Imagen Default
        /// </summary>
        public const string IMAGEN_DEFAULT = "default.jpg";

        /// <summary>
        /// Nombre de Imagen del Top de email
        /// </summary>
        public const string IMAGEN_EMAIL_TOP = "top_mail.jpg";

        /// <summary>
        /// Nombre de Imagen del Bottom de email
        /// </summary>
        public const string IMAGEN_EMAIL_BOTTOM = "bottom_mail.jpg";

        /// <summary>
        /// Nombre de Imagen preview de Video
        /// </summary>
        public const string IMAGEN_PREVIEW_VIDEO = "preview_video.gif";

        /// <summary>
        /// Nombre de Imagen preview de Video
        /// </summary>
        public const string IMAGEN_PREVIEW_THUMB_VIDEO = "thumb_video.jpg";

        /// <summary>
        /// ruta de la imagen generada dinamicamente
        /// </summary>
        public const string RUTA_IMAGEN_DINAMICA = "~/ImageGenerator.axd?image=";

        /// <summary>
        /// Latitud del centro de mapa de Google Maps
        /// </summary>
        public const double LATITUD_CENTRO_MEXICO = 20.1165501224845;

        /// <summary>
        /// Longitud del centro de mapa 
        /// </summary>
        public const double LONGITUD_CENTRO_MEXICO = -98.7506103515625;

        /// <summary>
        /// Latitud del centro de mapa de Google Maps
        /// </summary>
        public const double LATITUD_CENTRO_GUADALAJARA = 20.668123294358143;

        /// <summary>
        /// Longitud del centro de mapa 
        /// </summary>
        public const double LONGITUD_CENTRO_GUADALAJARA = -103.35868835449219;

        /// <summary>
        /// Define el nombre de la cookie que contiene el acceso a páginas.
        /// </summary>
        public const string COOKIE_PAGE_LEVEL = "_PAGE_LEVEL";



        /// <summary>
        /// Constante que el título de la página en el administrador
        /// </summary>
        public const string TITULO_ADMINISTRADOR = "Administrador :: Sitio Web";

        /// <summary>
        /// Constante que el saludo inicial en el sitio administrador
        /// </summary>
        public const string SALUDO_INICIAL = "Bienvenido Administrador";

        /// <summary>
        /// Constante que define el número de registros por páginas en los gridviews.
        /// </summary>
        public const int PAGE_SIZE = 25;

        /// <summary>
        /// Constante para especificar el orden de los listados de datos. ASCENDENTE
        /// </summary>
        public const string ORDEN_ASC = "asc";

        /// <summary>
        /// Constante para especificar el orden de los listados de datos. DESCENDENTE
        /// </summary>
        public const string ORDEN_DESC = "desc";

        /// <summary>
        /// Constante utilizada para definir la palabra reservada de LINQ para hacer ordenamientos
        /// </summary>
        public const string ORDEN_ORDERBY = "OrderBy";

        /// <summary>
        /// Constante para especificar el orden de los listados de datos. DESCENDENTE
        /// </summary>
        public const string ORDEN_ORDERBY_DESCENDING = "OrderByDescending";


        /// <summary>
        /// Identificador de la constante de sesion que almacena los mensajes que se presentan al usuario
        /// </summary>
        public const string SESION_MENSAJE = "mensajeEditor";

        /// <summary>
        /// Image Draw 5.0
        /// Constantes para especificar el aspecto de la imagen.
        /// </summary>
        public const string RATIO_FIT = "Fit";

        public const string RATIO_FIT_IF_MAX = "FitIfMax";

        public const string RATIO_HEIGHT_BASED = "HeightBased";

        public const string RATIO_WIDTH_BASED = "WidthBased";

        public const string RATIO_NONE = "None";

        public const int JPEG_COMPRESSION_LEVEL = 90;


        /// <summary>
        /// Constante para especificar la extension de archivo tipo PDF
        /// </summary>
        public const string EXTENSION_PDF = ".pdf";


        // <summary>
        /// Constante para especificar la extension de archivo tipo swf
        /// </summary>
        public const string EXTENSION_SWF = ".swf";


        /// <summary>
        /// Constante para especificar la extension de archivo tipo jpg
        /// </summary>
        public const string EXTENSION_JPG = ".jpg";


        /// <summary>
        /// Constante para especificar la extension de archivo tipo gif
        /// </summary>
        public const string EXTENSION_GIF = ".gif";

        /// <summary>
        /// Constante para especificar la extension de archivo tipo png
        /// </summary>
        public const string EXTENSION_PNG = ".png";

        /// <summary>
        /// Constante para especificar la extension de archivo tipo bmp
        /// </summary>
        public const string EXTENSION_BMP = ".bmp";

        /// <summary>
        /// Constante para especificar la extension de archivo tipo aspx
        /// </summary>
        public const string EXTENSION_ASPX = ".aspx";

        /// <summary>
        /// Constante para especificar la extension de archivo tipo txt
        /// </summary>
        public const string EXTENSION_TXT = ".txt";

        /// <summary>
        /// Constante para especificar la extension de archivo tipo htm
        /// </summary>
        public const string EXTENSION_HTM = ".htm";

        /// <summary>
        /// Numero de decimales al que se limita un numero flotanto/doble
        /// </summary>
        public const int NUMERO_DECIMALES = 2;

        /// <summary>
        /// Constante abreviación de Mega Bytes
        /// </summary>
        public const string MEGAS = "MB";


        /// <summary>
        /// Numero de bytes equivalentes a un mega
        /// </summary>
        public const int MEGA_BYTES = 1048576;

        /// <summary>
        /// Tamaño maximo de un archivo para su carga en el sistema
        /// </summary>
        public const int FILE_SIZE_MAX = 4;

        /// <summary>
        /// Constante para especificar la extension de archivo tipo flv
        /// </summary>
        public const string EXTENSION_FLV = ".flv";
    }
}
