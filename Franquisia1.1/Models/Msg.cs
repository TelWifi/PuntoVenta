using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franquisia1._1.Models
{
    public static class Msg
    {
        public static string CODIGO = "c\u00F3digo";
        public static string PARAMETRO = "par\u00E1metro";
        public static string CODCONSUMO = "El " + CODIGO + " del consumo";
        public static string CODDIV = "El " + CODIGO + " de la divisi\u00F3n de atenci\u00F3n";
        public static string CODUND = "El " + CODIGO + " de la unidad de atenci\u00F3n";
        public static string NUMERO = "n\u00FAmero";
        public static string NUMDOC = NUMERO+" de documento";
        public static string PermisoDenegado = "ERROR: Ud. no tiene los permisos para realizar la operaci\u00F3n";
        public static string OpExitosa = "EXITO: Operaci\u00F3n completada";
        public static string ErrParam = "ERROR: Error al obtener "+PARAMETRO+"s intenernos";
        public static string ADCPF = "ERROR: Acceso denegado, el consumo est\u00E1 en el proceso de facturaci\u00F3n";
        public static string PwdIncorrecta = "ERROR: Contrase\u00F1a incorrecta";
        public static string AttrNoNuloVacio(string attr)
        {
            return "ERROR: " + attr + " no puede ser nulo o vac\u00EDo";
        }
        public static string AttrNoExiste(string attr)
        {
            return "ERROR: " + attr + " no existe";
        }
        public static string ErrGenerar(string attr)
        {
            return "ERROR: Error al generar "+attr;
        }
    }
}