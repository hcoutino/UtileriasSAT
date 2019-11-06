using System;

namespace GlobalServices.Authentication
{
    public class CustomPrincipal : System.Security.Principal.IPrincipal
    {
        #region Variables
        /// <summary>
        /// Variable de tipo CustomIdentity 
        /// </summary>
        private CustomIdentity _identity;

        /// <summary>
        /// Arreglo de tipo String con los roles del usuario.
        /// </summary>
        private string[] _roles;

        /// <summary>
        /// Arreglo de tipo String con las paginas del usuario.
        /// </summary>
        private string[] _pages;


        #endregion

        #region Propiedades

        /// <summary>
        /// Propiedad de tipo Identidad Principal
        /// </summary>
        public System.Security.Principal.IIdentity Identity
        {
            get { return _identity; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor de la Identidad principal, recibe CustomIdentity y asigna los roles del usuario.
        /// </summary>
        /// <param name="identity">Objeto tipo CustomIdentity</param>
        public CustomPrincipal(CustomIdentity identity)
        {
            _identity = identity;
            //_pages = identity.ObtenerPaginasAcceso();
        }
        #endregion

        #region Metodos

        /// <summary>
        ///  Valida si identidad principal se encuentra en el rol especificado.
        /// </summary>
        /// <param name="role">rol de usuario</param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            return Array.IndexOf(_roles, role) >= 0 ? true : false;
        }

        /// <summary>
        /// Valida si identidad principal se encuentra en todos los roles especificados.
        /// </summary>
        /// <param name="roles">Arreglo de roles de usuario.</param>
        /// <returns></returns>
        public bool IsInAllRoles(params string[] roles)
        {
            foreach (string searchrole in roles)
            {
                if (Array.IndexOf(_roles, searchrole) < 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Valida si identidad principal se encuentra en uno de los roles especificados.
        /// </summary>
        /// <param name="roles"> de roles de usuario.</param>
        /// <returns></returns>
        public bool IsInAnyRoles(params string[] roles)
        {
            foreach (string searchrole in roles)
            {
                if (Array.IndexOf(_roles, searchrole) > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///  Valida si identidad principal tiene acceso a la página especificada.
        /// </summary>
        /// <param name="page">Página actual</param>
        /// <returns></returns>
        public bool hasPageAccess(string page)
        {
            if (_pages != null)
                return Array.IndexOf(_pages, page) >= 0 ? true : false;
            else
                return false;
        }

        #endregion
    }
}
