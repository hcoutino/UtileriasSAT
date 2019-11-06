using System;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;

namespace GlobalServices.Helpers
{
    public static class Cryptography
    {
        #region Metodos

        /*/// <summary>
        /// Encripta una cadena de texto.
        /// Desde el EnterpriseLibrary
        /// </summary>
        /// <param name="cadena">cadena en texto plano.</param>
        /// <returns>cadena encriptada.</returns>
        public static string Encriptar(string cadena)
        {

            String cadenaEncriptada = !String.IsNullOrEmpty(cadena) ? Cryptographer.EncryptSymmetric(Constants.Configuration.CRYPTOGRAPHY_KEY, cadena) : String.Empty;
            return cadenaEncriptada;
        }

        /// <summary>
        /// DesenEncripta una cadena de texto previamente encriptada.
        /// Desde el EnterpriseLibrary
        /// </summary>
        /// <param name="cadena">cadena encriptada.</param>
        /// <returns>cadena en texto plano.</returns>
        public static string Desencriptar(string cadena)
        {
            String cadenaDesencriptada = !String.IsNullOrEmpty(cadena) ?
                 Cryptographer.DecryptSymmetric(Constants.Configuration.CRYPTOGRAPHY_KEY, cadena.Replace(" ", "+")) : String.Empty;

            return cadenaDesencriptada;
        }*/

        /// <summary>
        /// Encripta una cadena de texto.
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string Encriptar(string toEncrypt, bool useHashing = false)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file

            string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DesenEncripta una cadena de texto previamente encriptada.
        /// </summary>
        /// <param name="cipherString"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string Desencriptar(string cipherString, bool useHashing = false)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = //Convert.FromBase64String(cipherString);
            UTF8Encoding.UTF8.GetBytes(cipherString);
            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();



            //ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length );
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// Encripta la cadena enviada con el algoritmo MD5
        /// </summary>
        /// <param name="cadena">Cadena a encriptar</param>
        /// <returns>Cadena encriptado</returns>
        /// <remarks>
        /// 06/10/2012, MArteche: Creación.
        /// </remarks>
        public static string EncriptarMD5(string cadena)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();

            stream = md5.ComputeHash(encoding.GetBytes(cadena));

            for (int i = 0; i < stream.Length; i++)
                sb.AppendFormat("{0:x2}", stream[i]);

            return sb.ToString();

        }

        #endregion 
    }
}
