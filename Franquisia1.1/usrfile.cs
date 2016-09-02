//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Franquisia1._1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class usrfile
    {
        [Required(ErrorMessage = "Ingrese Usuario", AllowEmptyStrings = false)]
        public string idusr { get; set; }
        public string desusr { get; set; }
        [Required(ErrorMessage = "Ingrese Contraseña", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string passusr { get; set; }
        public string typeusr { get; set; }
        public string stateusr { get; set; }
        public string mail { get; set; }
        public byte[] firma_digital { get; set; }
        public string nombre_firma { get; set; }
        public string cargo_firma { get; set; }
        public string dni_firma { get; set; }
        public string msn { get; set; }
        string _PASS_PHRASE = "MySqlCSharpSystem";

        string _SALT_VALUE = "AccountingSoftware";

        string _HASH_ALGORITHM = "SHA1";

        string _PASSWORD_ITERATIONS = "2";

        string _INIT_VECTOR = "@1B2c3D4e5F6g7H8";

        string _KEY_SIZE = "256";


        public string Encripta(string cadena)
        {
            string passPhrase = this._PASS_PHRASE;

            string saltValue = this._SALT_VALUE;

            string hashAlgorithm = this._HASH_ALGORITHM;

            int passwordIterations = Convert.ToInt32(this._PASSWORD_ITERATIONS);

            string initVector = this._INIT_VECTOR;

            int keySize = Convert.ToInt32(this._KEY_SIZE);

            RijndaelSimple RijndaelSimple = new RijndaelSimple();

            return RijndaelSimple.Encrypt(cadena, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
        }

    }
}