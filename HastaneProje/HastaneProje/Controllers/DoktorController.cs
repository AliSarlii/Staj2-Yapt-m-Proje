using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HastaneProje.Models;
using Newtonsoft.Json;
namespace HastaneProje.Controllers
{
    public class DoktorController : Controller
    {
        private Hastane_ProjeEntities db = new Hastane_ProjeEntities();

        //------------------------------------PROFİL İŞLEMLERİ------------------------------------


        //-----------Profilim-----------
        [SessionCheck]
        public ActionResult Profilim()
        {
            string eposta = Session["e-posta"].ToString();
            Kullanici kullanici = db.Kullanici.FirstOrDefault(k => k.e_posta.ToString() == eposta);
            if (kullanici == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.ActivePage = "Profilim";
            return View(kullanici);
        }

        //-----------ProfilDuzenle-----------
        public ActionResult ProfilDuzenle(int? Id) 
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kullanici kullanici = db.Kullanici.Find(Id);
            if (kullanici == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivePage = "Profilim";
            return View(kullanici);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult ProfilDuzenle([Bind(Include = "Id,ad,soyad,tel_No,e_posta,yetkinlik")] Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                var mail = db.Kullanici.FirstOrDefault(u => u.e_posta == kullanici.e_posta);
                if ((string)Session["e-posta"]!=kullanici.e_posta && mail!=null)
                {
                    ViewBag.Hata = true;
                    ViewBag.ActivePage = "Profilim";
                    return View(kullanici);
                }
                else
                {
                    db.Entry(kullanici).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Profilim");
                }
                
            }
            ViewBag.ActivePage = "Profilim";
            return View(kullanici);
        }

        //-----------ProfilSil-----------
        public ActionResult ProfilSil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kullanici kullanici = db.Kullanici.Find(id);
            if (kullanici == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivePage = "Profilim";
            return View(kullanici);
        }

        //POST
        [HttpPost, ActionName("ProfilSil")]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult ProfilOnayla(int id , bool veri)
        {
            Kullanici kullanici = db.Kullanici.Find(id);
            var degerler = db.Iletisim.Where(x => x.kullanici == kullanici.Id).ToList();
            var degerler2 = db.Tiroid_Verileri.Where(x => x.kullanici == kullanici.Id).ToList();
            if (!veri)
            {
                if (degerler != null)
                {
                    foreach (var i in degerler)
                    { i.kullanici = null; }
                }

                if (degerler2 != null)
                {
                    foreach (var i in degerler2)
                    { i.kullanici = null; }
                }
            }
            else if (veri)
            {
                if (degerler != null)
                {
                    foreach (var i in degerler)
                    { db.Iletisim.Remove(i); }
                }

                if (degerler2 != null)
                {
                    foreach (var i in degerler2)
                    { db.Tiroid_Verileri.Remove(i); }
                }
            }
            Session["e-posta"] = null;
            Session["admin"] = null;
            db.Kullanici.Remove(kullanici);
            db.SaveChanges();
            return RedirectToAction("Home","Home");
        }

        //------------------------------------VERİLERİM İŞLEMLERİ------------------------------------


        //-----------Verilerim-----------
        [SessionCheck]
        public ActionResult Verilerim()
        {
            string eposta = Session["e-posta"].ToString();
            Kullanici kullanici = db.Kullanici.FirstOrDefault(k => k.e_posta.ToString() == eposta);
            var tiroid_Verileri = db.Tiroid_Verileri.Where(t => t.kullanici==kullanici.Id).Include(t => t.Adenopaty1).Include(t => t.Focality1).Include(t => t.Kullanici1).Include(t => t.M1).Include(t => t.N1).Include(t => t.Pathology1).Include(t => t.Physical_Examination1).Include(t => t.Response1).Include(t => t.Risk1).Include(t => t.Stage1).Include(t => t.T1).Include(t => t.Thyroid_Function1);
            ViewBag.ActivePage = "Verilerim";
            return View(tiroid_Verileri.ToList());
        }

        //-----------VerimiDuzenle-----------
        public ActionResult VerimiDuzenle(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tiroid_Verileri tiroid_Verileri = db.Tiroid_Verileri.Find(Id);
            if (tiroid_Verileri == null)
            {
                return HttpNotFound();
            }
            ViewBag.adenopaty = new SelectList(db.Adenopaty, "Id", "adenopaty_Veri", tiroid_Verileri.adenopaty);
            ViewBag.focality = new SelectList(db.Focality, "Id", "focality_Veri", tiroid_Verileri.focality);
            ViewBag.kullanici = new SelectList(db.Kullanici, "Id", "ad", tiroid_Verileri.kullanici);
            ViewBag.m = new SelectList(db.M, "Id", "m_Veri", tiroid_Verileri.m);
            ViewBag.n = new SelectList(db.N, "Id", "n_Veri", tiroid_Verileri.n);
            ViewBag.pathology = new SelectList(db.Pathology, "Id", "pathology_Veri", tiroid_Verileri.pathology);
            ViewBag.physical_Examination = new SelectList(db.Physical_Examination, "Id", "physical_Examination_Veri", tiroid_Verileri.physical_Examination);
            ViewBag.response = new SelectList(db.Response, "Id", "response_Veri", tiroid_Verileri.response);
            ViewBag.risk = new SelectList(db.Risk, "Id", "risk_Veri", tiroid_Verileri.risk);
            ViewBag.stage = new SelectList(db.Stage, "Id", "stage_Veri", tiroid_Verileri.stage);
            ViewBag.t = new SelectList(db.T, "Id", "t_Veri", tiroid_Verileri.t);
            ViewBag.thyroid_Function = new SelectList(db.Thyroid_Function, "Id", "thyroid_Function_Veri", tiroid_Verileri.thyroid_Function);
            ViewBag.ActivePage = "Verilerim";
            var seçiliVeri = db.Tiroid_Verileri.FirstOrDefault(u => u.Id == Id);
            var seciliKullanici = db.Kullanici.FirstOrDefault(u => u.Id == seçiliVeri.kullanici);
            ViewBag.SelectedUserText = seciliKullanici?.ad;
            return View(tiroid_Verileri);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult VerimiDuzenle([Bind(Include = "Id,age,gender,smoking,hx_Smoking,hx_Radiothreapy,thyroid_Function,physical_Examination,adenopaty,pathology,focality,risk,t,n,m,stage,response,recurred,olcum_Tarihi,kullanici")] Tiroid_Verileri tiroid_Verileri)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tiroid_Verileri).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Verilerim");
            }
            ViewBag.adenopaty = new SelectList(db.Adenopaty, "Id", "adenopaty_Veri", tiroid_Verileri.adenopaty);
            ViewBag.focality = new SelectList(db.Focality, "Id", "focality_Veri", tiroid_Verileri.focality);
            ViewBag.kullanici = new SelectList(db.Kullanici, "Id", "ad", tiroid_Verileri.kullanici);
            ViewBag.m = new SelectList(db.M, "Id", "m_Veri", tiroid_Verileri.m);
            ViewBag.n = new SelectList(db.N, "Id", "n_Veri", tiroid_Verileri.n);
            ViewBag.pathology = new SelectList(db.Pathology, "Id", "pathology_Veri", tiroid_Verileri.pathology);
            ViewBag.physical_Examination = new SelectList(db.Physical_Examination, "Id", "physical_Examination_Veri", tiroid_Verileri.physical_Examination);
            ViewBag.response = new SelectList(db.Response, "Id", "response_Veri", tiroid_Verileri.response);
            ViewBag.risk = new SelectList(db.Risk, "Id", "risk_Veri", tiroid_Verileri.risk);
            ViewBag.stage = new SelectList(db.Stage, "Id", "stage_Veri", tiroid_Verileri.stage);
            ViewBag.t = new SelectList(db.T, "Id", "t_Veri", tiroid_Verileri.t);
            ViewBag.thyroid_Function = new SelectList(db.Thyroid_Function, "Id", "thyroid_Function_Veri", tiroid_Verileri.thyroid_Function);
            ViewBag.ActivePage = "Verilerim";
            return View(tiroid_Verileri);
        }

        //-----------VerimiSil-----------
        public ActionResult VerimiSil(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tiroid_Verileri tiroid_Verileri = db.Tiroid_Verileri.Find(Id);
            if (tiroid_Verileri == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivePage = "Verilerim";
            return View(tiroid_Verileri);
        }

        //POST
        [HttpPost, ActionName("VerimiSil")]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult SilmeOnayla(int Id)
        {
            Tiroid_Verileri tiroid_Verileri = db.Tiroid_Verileri.Find(Id);
            db.Tiroid_Verileri.Remove(tiroid_Verileri);
            db.SaveChanges();
            return RedirectToAction("Verilerim");
        }

        //------------------------------------VERİLER İŞLEMLERİ------------------------------------

        //-----------Veriler-----------
        public ActionResult Veriler(string query)
        {
            var result = db.Tiroid_Verileri
            .Where(c => c.kullanici != null && (string.IsNullOrEmpty(query) || c.Kullanici1.ad.Contains(query)))
            .ToList();
            ViewBag.ActivePage = "Veriler";
            return View(result);
        }

        //-----------VeriDuzenle-----------
        public ActionResult VeriDuzenle(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tiroid_Verileri tiroid_Verileri = db.Tiroid_Verileri.Find(Id);
            if (tiroid_Verileri == null)
            {
                return HttpNotFound();
            }
            ViewBag.adenopaty = new SelectList(db.Adenopaty, "Id", "adenopaty_Veri", tiroid_Verileri.adenopaty);
            ViewBag.focality = new SelectList(db.Focality, "Id", "focality_Veri", tiroid_Verileri.focality);
            ViewBag.kullanici = new SelectList(db.Kullanici, "Id", "ad", tiroid_Verileri.kullanici);
            ViewBag.m = new SelectList(db.M, "Id", "m_Veri", tiroid_Verileri.m);
            ViewBag.n = new SelectList(db.N, "Id", "n_Veri", tiroid_Verileri.n);
            ViewBag.pathology = new SelectList(db.Pathology, "Id", "pathology_Veri", tiroid_Verileri.pathology);
            ViewBag.physical_Examination = new SelectList(db.Physical_Examination, "Id", "physical_Examination_Veri", tiroid_Verileri.physical_Examination);
            ViewBag.response = new SelectList(db.Response, "Id", "response_Veri", tiroid_Verileri.response);
            ViewBag.risk = new SelectList(db.Risk, "Id", "risk_Veri", tiroid_Verileri.risk);
            ViewBag.stage = new SelectList(db.Stage, "Id", "stage_Veri", tiroid_Verileri.stage);
            ViewBag.t = new SelectList(db.T, "Id", "t_Veri", tiroid_Verileri.t);
            ViewBag.thyroid_Function = new SelectList(db.Thyroid_Function, "Id", "thyroid_Function_Veri", tiroid_Verileri.thyroid_Function);
            ViewBag.ActivePage = "Veriler";
            var seçiliVeri = db.Tiroid_Verileri.FirstOrDefault(u => u.Id == Id);
            var seciliKullanici = db.Kullanici.FirstOrDefault(u => u.Id == seçiliVeri.kullanici);
            ViewBag.SelectedUserText = seciliKullanici?.ad; 
            return View(tiroid_Verileri);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult VeriDuzenle([Bind(Include = "Id,age,gender,smoking,hx_Smoking,hx_Radiothreapy,thyroid_Function,physical_Examination,adenopaty,pathology,focality,risk,t,n,m,stage,response,recurred,olcum_Tarihi,kullanici")] Tiroid_Verileri tiroid_Verileri)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tiroid_Verileri).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Veriler");
            }
            ViewBag.adenopaty = new SelectList(db.Adenopaty, "Id", "adenopaty_Veri", tiroid_Verileri.adenopaty);
            ViewBag.focality = new SelectList(db.Focality, "Id", "focality_Veri", tiroid_Verileri.focality);
            ViewBag.kullanici = new SelectList(db.Kullanici, "Id", "ad", tiroid_Verileri.kullanici);
            ViewBag.m = new SelectList(db.M, "Id", "m_Veri", tiroid_Verileri.m);
            ViewBag.n = new SelectList(db.N, "Id", "n_Veri", tiroid_Verileri.n);
            ViewBag.pathology = new SelectList(db.Pathology, "Id", "pathology_Veri", tiroid_Verileri.pathology);
            ViewBag.physical_Examination = new SelectList(db.Physical_Examination, "Id", "physical_Examination_Veri", tiroid_Verileri.physical_Examination);
            ViewBag.response = new SelectList(db.Response, "Id", "response_Veri", tiroid_Verileri.response);
            ViewBag.risk = new SelectList(db.Risk, "Id", "risk_Veri", tiroid_Verileri.risk);
            ViewBag.stage = new SelectList(db.Stage, "Id", "stage_Veri", tiroid_Verileri.stage);
            ViewBag.t = new SelectList(db.T, "Id", "t_Veri", tiroid_Verileri.t);
            ViewBag.thyroid_Function = new SelectList(db.Thyroid_Function, "Id", "thyroid_Function_Veri", tiroid_Verileri.thyroid_Function);
            ViewBag.ActivePage = "Veriler";
            return View(tiroid_Verileri);
        }

        //-----------VeriSil-----------
        public ActionResult VeriSil(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tiroid_Verileri tiroid_Verileri = db.Tiroid_Verileri.Find(Id);
            if (tiroid_Verileri == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivePage = "Veriler";
            return View(tiroid_Verileri);
        }

        //POST
        [HttpPost, ActionName("VeriSil")]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult SilmeeOnayla(int Id)
        {
            Tiroid_Verileri tiroid_Verileri = db.Tiroid_Verileri.Find(Id);
            db.Tiroid_Verileri.Remove(tiroid_Verileri);
            db.SaveChanges();
            return RedirectToAction("Veriler");
        }

        //-----------VeriEkle-----------
        public ActionResult VeriEkle()
        {
            ViewBag.adenopaty = new SelectList(db.Adenopaty, "Id", "adenopaty_Veri");
            ViewBag.focality = new SelectList(db.Focality, "Id", "focality_Veri");
            ViewBag.kullanici = new SelectList(db.Kullanici, "Id", "ad");
            ViewBag.m = new SelectList(db.M, "Id", "m_Veri");
            ViewBag.n = new SelectList(db.N, "Id", "n_Veri");
            ViewBag.pathology = new SelectList(db.Pathology, "Id", "pathology_Veri");
            ViewBag.physical_Examination = new SelectList(db.Physical_Examination, "Id", "physical_Examination_Veri");
            ViewBag.response = new SelectList(db.Response, "Id", "response_Veri");
            ViewBag.risk = new SelectList(db.Risk, "Id", "risk_Veri");
            ViewBag.stage = new SelectList(db.Stage, "Id", "stage_Veri");
            ViewBag.t = new SelectList(db.T, "Id", "t_Veri");
            ViewBag.thyroid_Function = new SelectList(db.Thyroid_Function, "Id", "thyroid_Function_Veri");
            ViewBag.ActivePage = "VeriEkle";
            return View();
        }

        //-----------KullaniciArama-----------
        [HttpPost]
        [SessionCheck]
        public JsonResult Search(string query)
        {
            var users = db.Kullanici
            .Where(u => u.ad.Contains(query))
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.ad
            }).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult VeriEkle([Bind(Include = "Id,age,gender,smoking,hx_Smoking,hx_Radiothreapy,thyroid_Function,physical_Examination,adenopaty,pathology,focality,risk,t,n,m,stage,response,recurred,olcum_Tarihi,kullanici")] Tiroid_Verileri tiroid_Verileri)
        {
            if (ModelState.IsValid)
            {
                db.Tiroid_Verileri.Add(tiroid_Verileri);
                db.SaveChanges();
                string eposta = Session["e-posta"].ToString();
                Kullanici kullanici = db.Kullanici.FirstOrDefault(k => k.e_posta.ToString() == eposta);
                if(kullanici.Id == tiroid_Verileri.kullanici)
                {
                    return RedirectToAction("Verilerim");
                }
                else 
                {
                    return RedirectToAction("Veriler");
                } 
            }
            ViewBag.adenopaty = new SelectList(db.Adenopaty, "Id", "adenopaty_Veri", tiroid_Verileri.adenopaty);
            ViewBag.focality = new SelectList(db.Focality, "Id", "focality_Veri", tiroid_Verileri.focality);
            ViewBag.kullanici = new SelectList(db.Kullanici, "Id", "ad", tiroid_Verileri.kullanici);
            ViewBag.m = new SelectList(db.M, "Id", "m_Veri", tiroid_Verileri.m);
            ViewBag.n = new SelectList(db.N, "Id", "n_Veri", tiroid_Verileri.n);
            ViewBag.pathology = new SelectList(db.Pathology, "Id", "pathology_Veri", tiroid_Verileri.pathology);
            ViewBag.physical_Examination = new SelectList(db.Physical_Examination, "Id", "physical_Examination_Veri", tiroid_Verileri.physical_Examination);
            ViewBag.response = new SelectList(db.Response, "Id", "response_Veri", tiroid_Verileri.response);
            ViewBag.risk = new SelectList(db.Risk, "Id", "risk_Veri", tiroid_Verileri.risk);
            ViewBag.stage = new SelectList(db.Stage, "Id", "stage_Veri", tiroid_Verileri.stage);
            ViewBag.t = new SelectList(db.T, "Id", "t_Veri", tiroid_Verileri.t);
            ViewBag.thyroid_Function = new SelectList(db.Thyroid_Function, "Id", "thyroid_Function_Veri", tiroid_Verileri.thyroid_Function);
            ViewBag.ActivePage = "VeriEkle";
            return View(tiroid_Verileri);
        }

        //------------------------------------KULLANICI İŞLEMLERİ------------------------------------

        //-----------KullaniciEkle-----------
        public ActionResult KullaniciEkle()
        {
            Kullanici kullanici = new Kullanici();
            kullanici.yetkinlik = false;
            ViewBag.ActivePage = "KullaniciEkle";
            return View(kullanici);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult KullaniciEkle([Bind(Include = "Id,ad,soyad,tel_No,e_posta,yetkinlik")] Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                var mail = db.Kullanici.FirstOrDefault(u => u.e_posta == kullanici.e_posta);
                if (mail != null)
                {
                    ViewBag.Hata = true;
                    ViewBag.ActivePage = "KullaniciEkle";
                    return View(kullanici);
                }
                else
                {
                    db.Kullanici.Add(kullanici);
                    db.SaveChanges();
                    if (kullanici.yetkinlik == false)
                    {
                        return RedirectToAction("Kullanicilar");
                    }
                    else
                    {
                        return RedirectToAction("Doktorlar");
                    }
                }
                
            }
            return View(kullanici);
        }

        //-----------Kullanicilar-----------
        public ActionResult Kullanicilar(string query)
        {
            var result = string.IsNullOrEmpty(query)
                ? db.Kullanici.ToList()
                : db.Kullanici
                    .Where(c => c.ad.Contains(query))
                    
                    .ToList();
            var sonuc = result.Where(c => c.yetkinlik == false);
            ViewBag.ActivePage = "Kullanicilar";
            return View(sonuc);
        }

        //-----------KullaniciDuzenle-----------
        public ActionResult KullaniciDuzenle(int? id)
        {
            if (id == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kullanici kullanici = db.Kullanici.Find(id);
            if (kullanici == null || kullanici.yetkinlik == true)
            {
                return HttpNotFound();
            }
            ViewBag.ActivePage = "Kullanicilar";
            return View(kullanici);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult KullaniciDuzenle([Bind(Include = "Id,ad,soyad,tel_No,e_posta,yetkinlik")] Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                var mail = db.Kullanici.FirstOrDefault(u => u.e_posta == kullanici.e_posta);
                if ((string)Session["e-posta"] != kullanici.e_posta && mail != null)
                {
                    ViewBag.Hata = true;
                    ViewBag.ActivePage = "KullaniciDuzenle";
                    return View(kullanici);
                }
                else
                {
                    db.Entry(kullanici).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Kullanicilar");
                }
            }
            return View(kullanici);
        }

        //-----------KullaniciSil-----------
        public ActionResult KullaniciSil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kullanici kullanici = db.Kullanici.Find(id);
            if (kullanici == null || kullanici.yetkinlik == true)
            {
                return HttpNotFound();
            }
            ViewBag.ActivePage = "Kullanicilar";
            return View(kullanici);
        }

        //POST
        [HttpPost, ActionName("KullaniciSil")]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult DeleteConfirmed(int id, bool veri)
        {
            Kullanici kullanici = db.Kullanici.Find(id);
            var degerler = db.Iletisim.Where(x => x.kullanici == kullanici.Id).ToList();
            var degerler2 = db.Tiroid_Verileri.Where(x => x.kullanici == kullanici.Id).ToList();
            if (!veri)
            {
                if (degerler != null)
                {
                    foreach (var i in degerler)
                    { i.kullanici = null; }
                }

                if (degerler2 != null)
                {
                    foreach (var i in degerler2)
                    { i.kullanici = null; }
                }
            }
            else if (veri)
            {
                if (degerler != null)
                {
                    foreach (var i in degerler)
                    { db.Iletisim.Remove(i); }
                }

                if (degerler2 != null)
                {
                    foreach (var i in degerler2)
                    { db.Tiroid_Verileri.Remove(i); }
                }
            }
            db.Kullanici.Remove(kullanici);
            db.SaveChanges();
            return RedirectToAction("Kullanicilar");
        }

        //------------------------DOKTOR İŞLEMLERİ------------------------

        //-----------Doktorlar-----------
        public ActionResult Doktorlar(string query)
        {
            var result = string.IsNullOrEmpty(query)
                ? db.Kullanici.ToList()
                : db.Kullanici
                    .Where(c => c.ad.Contains(query))

                    .ToList();
            var sonuc = result.Where(c => c.yetkinlik == true);
            ViewBag.ActivePage = "Doktorlar";
            return View(sonuc);
        }


        //-----------------------------------MESAJ İŞLEMLERİ------------------------------------

        //-----------KullaniciMesajlar-----------
        public ActionResult KullaniciMesajlar()
        {
            var iletisims = db.Iletisim.Where(i => i.Kullanici1!=null && i.e_posta==null).ToList();
            ViewBag.ActivePage = "KullaniciMesajlar";
            return View(iletisims.ToList());
        }
        //-----------KullaniciMesajlar-----------
        public ActionResult ZiyaretciMesajlar()
        {
            var iletisims = db.Iletisim.Where(i => i.Kullanici1 == null && i.e_posta != null).ToList();
            ViewBag.ActivePage = "ZiyaretciMesajlar";
            return View(iletisims.ToList());
        }

        //-----------AJAX Okundu İşlemi-----------
        [HttpPost]
        public JsonResult Okundu(int id)
        {
            var record = db.Iletisim.Find(id);
            if (record != null)
            {
                if(record.okundu_Bilgi == false || record.okundu_Bilgi == null) 
                {
                    record.okundu_Bilgi = true;
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Kayıt başarıyla güncellendi." });
            }
            return Json(new { success = false, message = "Kayıt bulunamadı." });
        }

        //-----------MesajSil-----------
        public ActionResult MesajSil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Iletisim iletisim = db.Iletisim.Find(id);
            if (iletisim == null)
            {
                return HttpNotFound();
            }
            if (iletisim.kullanici == null)
            {
                db.Iletisim.Remove(iletisim);
                db.SaveChanges();
                return RedirectToAction("ZiyaretciMesajlar");
            }
            else
            {
                db.Iletisim.Remove(iletisim);
                db.SaveChanges();
                return RedirectToAction("KullaniciMesajlar");
            }
        }

        //-----------------------------------Makine Öğrenmesi Tahmin İşlemi------------------------------------
       
        private static readonly HttpClient client = new HttpClient();
        [SessionCheck]
        public async Task<ActionResult> Tahmin(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tiroid_Verileri veriler = db.Tiroid_Verileri.Find(Id);
            if (veriler == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dizii = new object[] 
                    {
                        veriler.age,
                        veriler.gender,
                        veriler.smoking,
                        veriler.hx_Smoking,
                        veriler.hx_Radiothreapy,
                        veriler.thyroid_Function,
                        veriler.physical_Examination,
                        veriler.adenopaty,
                        veriler.pathology,
                        veriler.focality,
                        veriler.risk,
                        veriler.t,
                        veriler.n,
                        veriler.m,
                        veriler.stage,
                        veriler.response
                    };

                    var json = JsonConvert.SerializeObject(dizii);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // POST isteğini gönderin
                    var response = await client.PostAsync("http://127.0.0.1:5000/data", content);

                    // Yanıtı kontrol edin
                    if (response.IsSuccessStatusCode)
                    {

                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        dynamic data = JsonConvert.DeserializeObject(jsonResponse);

                        // Accuracy ve testVeriPred_list değerlerini alıyoruz
                        double accuracy = data.accuracy;
                        List<int> testVeriPredList = data.testVeriPred_list.ToObject<List<int>>();

                        // Dizi veya liste olarak ViewBag'e aktarın
                        ViewBag.Accuracy = accuracy;
                        ViewBag.TestVeriPredList = testVeriPredList;

                        return View(veriler); // Başarı sayfasına yönlendir
                    }
                    else
                    {
                        ViewBag.Error = "API isteği başarısız oldu. Hata kodu: " + response.StatusCode;
                        return View("Error"); // Hata sayfasına yönlendir
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Bir hata oluştu: " + ex.Message;
                    return View("Error"); // Hata sayfasına yönlendir
                }
            }

            // Model geçersizse formu tekrar göster
            return View();
        }

        //-----------Home-----------
        public ActionResult Home()
        {
            return View();
        }

        //-----------Cikis-----------
        [SessionCheck]
        public ActionResult Cikis()
        {
            Session["e-posta"] = null;
            Session["admin"] = null;
            return RedirectToAction("Home", "Home");
        }

        //-----------DB Kapatma-----------
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}