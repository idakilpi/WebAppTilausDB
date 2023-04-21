using Newtonsoft.Json.Linq;
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
    public class TilauksetController : Controller
    {
        // GET: Tilaukset
        TilausDBEntities db = new TilausDBEntities();
        public ActionResult Index()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                List<Tilaukset> model = db.Tilaukset.ToList();
                ViewBag.LoggedStatus = "In";
                //db.Dispose();
                return View(model);
            }
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
            //ViewBag.Nimi = new SelectList(db.Asiakkaat, "AsiakasID", "Nimi");
            var asiakas = db.Asiakkaat;
            IEnumerable<SelectListItem> selectAsiakasLista = from a in asiakas
                                                          select new SelectListItem
                                                          {
                                                              Value = a.AsiakasID.ToString(),
                                                              Text = a.AsiakasID + " " + a.Nimi.ToString()
                                                          };
            ViewBag.AsiakasID = new SelectList(selectAsiakasLista, "Value", "Text");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TilausID,AsiakasID,Toimitusosoite, Postinumero, Tilauspvm, Toimituspvm")] Tilaukset tilaukset)
        {
            if (ModelState.IsValid)
            {
                db.Tilaukset.Add(tilaukset);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Nimi", tilaukset.AsiakasID);
            ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", tilaukset.Postinumero);
            return View(tilaukset);
        }
        public ActionResult Delete(int? id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            if (tilaukset == null) return HttpNotFound();
            return View(tilaukset);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            db.Tilaukset.Remove(tilaukset);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            if (tilaukset == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka");
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.Postinumero.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };
            ViewBag.Postinumero = new SelectList(selectPostiList, "Value", "Text",tilaukset.Postinumero);

            var asiakas = db.Asiakkaat;
            IEnumerable<SelectListItem> selectAsiakasLista = from a in asiakas
                                                             select new SelectListItem
                                                             {
                                                                 Value = a.AsiakasID.ToString(),
                                                                 Text = a.AsiakasID + " " + a.Nimi.ToString()
                                                             };
            ViewBag.AsiakasID = new SelectList(selectAsiakasLista, "Value", "Text",tilaukset.AsiakasID);
            return View(tilaukset);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TilausID,AsiakasID,Toimitusosoite, Postinumero, Tilauspvm, Toimituspvm")] Tilaukset tilaukset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tilaukset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var asiakas = db.Asiakkaat;
            IEnumerable<SelectListItem> selectAsiakasLista = from a in asiakas
                                                             select new SelectListItem
                                                             {
                                                                 Value = a.AsiakasID.ToString(),
                                                                 Text = a.AsiakasID + " " + a.Nimi.ToString()
                                                             };
            ViewBag.AsiakasID = new SelectList(selectAsiakasLista, "Value", "Text",tilaukset.AsiakasID);

            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.Postinumero.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };
            ViewBag.Postinumero = new SelectList(selectPostiList, "Value", "Text", tilaukset.Postinumero);
            return View(tilaukset);
        }

        public ActionResult OrderSummary()
        {
            var orderSummary = from o in db.Tilaukset
                               join c in db.Asiakkaat on o.AsiakasID equals c.AsiakasID
                               select new OrderSummaryData
                               {
                                   TilausID = o.TilausID,
                                   AsiakasID = c.AsiakasID,
                                   Tilauspvm = (DateTime)o.Tilauspvm,
                                   ToimitusPvm = (DateTime)o.Toimituspvm,
                                   Nimi = c.Nimi
                               };

            return View(orderSummary);
        }
    }
}