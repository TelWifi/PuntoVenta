using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franquisia1._1.Models
{
    public static class GenCod
    {
        public static string CodVenc(appbosaEntities db, string codcia, string punemi)
        {
            try
            {
                int? codigo = 1;
                numpos np = db.numpos.Where(a => a.CODCIA.Equals(codcia) && a.PUNEMI.Equals(punemi)).FirstOrDefault();
                if (np != null) {
                    np.NUMERO++;
                    codigo = np.NUMERO;
                }
                else
                {
                    np = new numpos();
                    np.CODCIA = codcia;
                    np.PUNEMI = punemi;
                    np.NUMERO = codigo;
                    db.numpos.Add(np); 
                }
                db.SaveChanges();
                return punemi + codigo.ToString().PadLeft(8, '0');
            }
            catch (System.Data.EntityException ex) { return null; }
            catch (Exception ex) { return null; }
        }
        public static string NumDoc(appbosaEntities db, string codcia, string punemi, string codtipdoc)
        {
            try
            {
                numpemi np = db.numpemi.Where(a => a.CODCIA.Equals(codcia) && a.PUNEMI.Equals(punemi) && a.CODIGO.Equals(codtipdoc)).FirstOrDefault();
                if (np == null) { return null; }
                np.NUMERO++;
                db.SaveChanges();
                fordoc fd = db.fordoc.Where(a => a.CODIGO.Equals("01")).FirstOrDefault();
                if (fd != null)
                {
                    if (fd.COMPLETAR.Equals("S"))
                    {
                        return np.SERIE + "-" + np.NUMERO.ToString().PadLeft((int)fd.LONGNUMERO, '0');
                    }
                    return np.SERIE + "-" + np.NUMERO;
                }
                return null;
            }catch (Exception ex) { 
                return null; 
            }
        }
        public static string CodItem(appbosaEntities db, string codcia, string codigo)
        {
            try
            {
                string item;
                item = db.cond.Where(a => a.CODCIA.Equals(codcia) && a.CODIGO.Equals(codigo)).Max(a => a.ITEM);
                if (item == null) { return "0001"; }
                item = (Convert.ToInt16(item) + 1).ToString().PadLeft(4, '0');
                return item;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }
}