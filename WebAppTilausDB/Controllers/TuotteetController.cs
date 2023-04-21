using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppTilausDB.Models;

namespace WebAppTilausDB.Controllers
{
    public class TuotteetController : Controller
    {
        // GET: Tuotteet
        TilausDBEntities db = new TilausDBEntities();
        public ActionResult Index()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.LoggedStatus = "In";
                List<Tuotteet> model = db.Tuotteet.ToList();
                return View(model);
            }
        }

        public ActionResult Delete(int? id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            if (tuotteet == null) return HttpNotFound();
            return View(tuotteet);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            db.Tuotteet.Remove(tuotteet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            if (tuotteet == null) return HttpNotFound();
            return View(tuotteet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Katso https://go.microsoft.com/fwlink/?LinkId=317598
        public ActionResult Edit([Bind(Include = "TuoteID,Nimi,Ahinta,Image")] Tuotteet tuote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tuote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tuote);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(Tuotteet tuote)
        {
            if (ModelState.IsValid)
            {
                TilausDBEntities db = new TilausDBEntities();
                db.Tuotteet.Add(tuote);
                db.SaveChanges();
            }
            return View(tuote);
        }
        public ActionResult ProductDetails(int id)
        {
            var product = db.Tuotteet.FirstOrDefault(p => p.TuoteID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var model = new Tuotteet()
            {
                TuoteID = product.TuoteID,
                Nimi = product.Nimi,
                Ahinta = product.Ahinta,
                Image = product.Image
            };

            return PartialView("_ProductDetails", model);
        }
        public JsonResult TopSellingProducts()
        {
            var top10Products = (from tilausrivi in db.Tilausrivit
                                 group tilausrivi by tilausrivi.TuoteID into tuotteet
                                 join tuote in db.Tuotteet on tuotteet.FirstOrDefault().TuoteID equals tuote.TuoteID
                                 orderby tuotteet.Sum(x => x.Ahinta * x.Maara) descending
                                 select new
                                 {
                                     ProductName = tuote.Nimi,
                                     TotalPrice = tuotteet.Sum(x => x.Ahinta * x.Maara)
                                 }).Take(10);

            return Json(top10Products, JsonRequestBehavior.AllowGet);
        }
    }
}