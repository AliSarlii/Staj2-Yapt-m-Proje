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
    public class KullaniciController : Controller
    {
        private Hastane_ProjeEntities db = new Hastane_ProjeEntities();

        //-----------Home-----------
        public ActionResult Home()
        {
            return View();
        }

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
                db.Entry(kullanici).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profilim");
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
        public ActionResult ProfilOnayla(int id, bool veri)
        {
            Kullanici kullanici = db.Kullanici.Find(id);
            var degerler = db.Iletisim.Where(x => x.kullanici == kullanici.Id).ToList();
            var degerler2 = db.Tiroid_Verileri.Where(x => x.kullanici == kullanici.Id).ToList();
            if (!veri)
            {
                if (degerler != null)
                {
                    foreach (var i in degerler)
                    {
                        i.kullanici = null;
                    }
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
                    {
                        db.Iletisim.Remove(i);
                    }
                }

                if (degerler2 != null)
                {
                    foreach (var i in degerler2)
                    {
                        db.Tiroid_Verileri.Remove(i);
                    }
                }
            }
            Session["e-posta"] = null;
            Session["kullanici"] = null;
            db.Kullanici.Remove(kullanici);
            db.SaveChanges();
            return RedirectToAction("Home", "Home");
        }

        //-----------Verilerim-----------
        [SessionCheck]
        public ActionResult Verilerim()
        {
            string eposta = Session["e-posta"].ToString();
            Kullanici kullanici = db.Kullanici.FirstOrDefault(k => k.e_posta.ToString() == eposta);
            if (kullanici == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var tiroidVerileri = db.Tiroid_Verileri
                        .Where(k => k.kullanici == kullanici.Id)
                        .ToList();
                ViewBag.ActivePage = "Verilerim";
                return View(tiroidVerileri);
            }
        }

        //-----------Iletisim-----------
        [SessionCheck]
        public ActionResult Iletisim()
        {
            string eposta = Session["e-posta"].ToString();
            Iletisim iletisim = new Iletisim();
            iletisim.kullanici = db.Kullanici.FirstOrDefault(k => k.e_posta.ToString() == eposta).Id;
            ViewBag.ActivePage = "Iletisim";
            return View(iletisim);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult Iletisim([Bind(Include = "Id,kullanici,e_posta,mesaj,okundu_Bilgi")] Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                db.Iletisim.Add(iletisim);
                db.SaveChanges();
                return RedirectToAction("Iletisim");
            }

            ViewBag.kullanici = new SelectList(db.Kullanici, "Id", "ad", iletisim.kullanici);
            ViewBag.ActivePage = "Iletisim";
            return View(iletisim);
        }

        //-----------Tahmin-----------

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