using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franquisia1._1.Controllers
{
    public class ClaservController : Controller
    {
        private appbosaEntities db = new appbosaEntities();

        //
        // GET: /Claserv/
            
        public ActionResult Index()
        {
            return Json(new { foo = "bar", baz = "Blech" });
        }
        //
        // GET: /Claserv/

        public ActionResult Prueba(string categoria)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string listajson = serializer.Serialize(db.claserv.ToList());
            return Json(new { categorias=listajson }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Claserv/Details/5

        public ActionResult Details(string id = null)
        {
            claserv claserv = db.claserv.Find(id);
            if (claserv == null)
            {
                return HttpNotFound();
            }
            return View(claserv);
        }

        //
        // GET: /Claserv/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Claserv/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(claserv claserv)
        {
            if (ModelState.IsValid)
            {
                db.claserv.Add(claserv);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(claserv);
        }

        //
        // GET: /Claserv/Edit/5

        public ActionResult Edit(string id = null)
        {
            claserv claserv = db.claserv.Find(id);
            if (claserv == null)
            {
                return HttpNotFound();
            }
            return View(claserv);
        }

        //
        // POST: /Claserv/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(claserv claserv)
        {
            if (ModelState.IsValid)
            {
                db.Entry(claserv).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(claserv);
        }

        //
        // GET: /Claserv/Delete/5

        public ActionResult Delete(string id = null)
        {
            claserv claserv = db.claserv.Find(id);
            if (claserv == null)
            {
                return HttpNotFound();
            }
            return View(claserv);
        }

        //
        // POST: /Claserv/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            claserv claserv = db.claserv.Find(id);
            db.claserv.Remove(claserv);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}