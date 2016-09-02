using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franquisia1._1.Models
{
    public class AxuliarHash
    {
        public string clave { get; set; }
        public string descripcion { get; set; }

        public AxuliarHash(string p1, string p2)
        {
            this.clave = p1;
            this.descripcion = p2;
        }

    }
}