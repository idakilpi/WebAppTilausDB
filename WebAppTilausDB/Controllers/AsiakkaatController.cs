using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppTilausDB.Models;
using WebAppTilausDB.ViewModels;

namespace WebAppTilausDB.Controllers
{
    public class AsiakkaatController : Controller
    {
        // GET: Asiakkaat
        TilausDBEntities db = new TilausDBEntities();
        public ActionResult Index()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login","Home");
            }
            else
            {
                ViewBag.LoggedStatus = "In";
                //var asiakkaat = db.Asiakkaat;
                //return View(asiakkaat.ToList());
                List<Asiakkaat> model = db.Asiakkaat.ToList();
                return View(model);
            }
        }
        public ActionResult Edit(int? id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            if (asiakkaat == null) 
            { 
                return HttpNotFound(); 
            }
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.Postinumero.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };
            ViewBag.Postinumero = new SelectList(selectPostiList, "Value","Text", asiakkaat.Postinumero);
            return View(asiakkaat);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult Edit([Bind(Include = "AsiakasID,Nimi,Osoite,Postinumero")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asiakkaat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.Postinumero.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };
            ViewBag.Postinumero = new SelectList(selectPostiList, "Value", "Text", asiakkaat.Postinumero);
            return View(asiakkaat);
        }

        public ActionResult Create()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.Postinumero.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };
            ViewBag.Postinumero = new SelectList(selectPostiList, "Value", "Text");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AsiakasID, Nimi, Osoite, Postinumero")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Asiakkaat.Add(asiakkaat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", asiakkaat.Postinumero);
            return View(asiakkaat);
        }

        public ActionResult Delete(int? id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            if (asiakkaat == null) 
            { 
                return HttpNotFound(); 
            }
            return View(asiakkaat);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            db.Asiakkaat.Remove(asiakkaat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult AsiakkaatSummary()
        {
            var asiakkaatSummary = from a in db.Asiakkaat
                               select new AsiakkaatSummary
                               {
                                AsiakasID = a.AsiakasID,
                                Nimi = a.Nimi,     
                                Osoite = a.Osoite,
                                Postinumero = a.Postinumero
                                };
            return View(asiakkaatSummary);
        }
    }
}