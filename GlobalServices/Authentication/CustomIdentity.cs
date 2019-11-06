using System.Web;
using System.Web.Security;
using GlobalServices.Constants;
using GlobalServices.Helpers;

namespace GlobalServices.Authentication
{
    public class CustomIdentity : System.Security.Principal.IIdentity
    {
        /// <summary>
        /// Define el tipo de autenticación de la identidad.
        /// </summary>
        private const string AUTHENTICATION_TYPE = "Custom";

        /// <summary>
        /// Constantes que definen el indice dentro del arreglo obtenido
        /// a partir de _ticket.UserData.
        /// </summary>
        private const int INDICE_NOMBRE = 0;
        private const int INDICE_ID_USUARIO = 1;
        private const int INDICE_RFC = 2;
        private const int INDICE_ADMIN = 3;
        private const int INDICE_VIEW = 4;

        #region Variables
        /// <summary>
        /// Ticket del tipo Forms Authentication
        /// </summary>
        private FormsAuthenticationTicket _ticket;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene el Ticket de tipo FormsAuthenticationTicket
        /// </summary>
        public FormsAuthenticationTicket Ticket
        {
            get { return _ticket; }
        }

        /// <summary>
        /// Obtiene el tipo de autenticación Custom tipo AuthenticationType
        /// </summary>
        public string AuthenticationType
        {
            get { return AUTHENTICATION_TYPE; }
        }

        /// <summary>
        /// Obtiene si el usuario esta autenticado.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return true; }
        }

        /// <summary>
        /// Obtiene el nombre completo del usuario.
        /// </summary>
        public string Name
        {
            get
            {
                string[] userData = _ticket.UserData.Split('|');
                return userData[INDICE_NOMBRE];
            }
        }

        /// <summary>
        /// Obtiene el IdUsuario a partir del userData del Ticket.
        /// </summary>
        public int IdProveedor
        {
            get
            {
                string[] userData = _ticket.UserData.Split('|');
                int idUsuario = 0;
                int.TryParse(userData[INDICE_ID_USUARIO], out idUsuario);
                return idUsuario;
            }
        }

        public string RFC
        {
            get
            {
                string[] userData = _ticket.UserData.Split('|');
                return userData[INDICE_RFC];
            }
        }

        public bool IsAdministrator
        {
            get
            {
                string[] userData = _ticket.UserData.Split('|');
                return bool.Parse(userData[INDICE_ADMIN]);
            }
            set
            {

            }
        }
        public bool IsView
        {
            get
            {
                string[] userData = _ticket.UserData.Split('|');
                return bool.Parse(userData[INDICE_VIEW]);
            }
            set
            {

            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor que crea la identidad personalizada a partir del FormsAuthenticationTicket.
        /// </summary>
        /// <param name="ticket">Objeto de tipo FormsAuthenticationTicket</param>
        /// <remarks>
        /// 11/11/2011, MArteche: Creación.
        /// </remarks>
        public CustomIdentity(FormsAuthenticationTicket ticket)
        {
            _ticket = ticket;
        }
        #endregion

        /// <summary>
        /// Obtiene las páginas a las que el usuario tiene acceso.
        /// </summary>
        /// <returns>Arreglo de páginas</returns>
        public string[] ObtenerPaginasAcceso()
        {
            string[] pages = null;
            string cookieName = FormsAuthentication.FormsCookieName + Configuration.COOKIE_PAGE_LEVEL;
            HttpCookie pgCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (pgCookie != null)
            {
                string[] paginas = Cryptography.Desencriptar(pgCookie.Value).Split('|');
                pages = paginas[0].Split(',');
            }

            //Agrega la extension para ahorrar espacio en la cookie
            //if (pages!= null)
            //    for (int i = 0; i < pages.Length; i++)
            //        pages[i] += Configuration.EXTENSION_ASPX;

            return pages;
        }

        /// <summary>
        /// Obtiene las páginas a las que el usuario tiene acceso.
        /// </summary>
        /// <returns>Arreglo de páginas</returns>
        public string[] ObtenerIdPaginasAcceso()
        {
            string[] pages = null;
            string cookieName = FormsAuthentication.FormsCookieName + Configuration.COOKIE_PAGE_LEVEL;
            HttpCookie pgCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (pgCookie != null)
            {
                string[] paginas = Cryptography.Desencriptar(pgCookie.Value).Split('|');
                pages = paginas[1].Split(',');
            }

            return pages;
        }
    }
}
