using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HastaneProje.Models;

namespace HastaneProje.Controllers
{
    
    public class HomeController : Controller
    {
        private Hastane_ProjeEntities db = new Hastane_ProjeEntities();

        //-----------Home-----------
        public ActionResult Home()
        {
            return View();
        }

        //-----------Hakkinda-----------
        public ActionResult Hakkinda()
        {
            return View();
        }

        //-----------Iletisim-----------
        public ActionResult Iletisim()
        {
            Iletisim iletisim = new Iletisim();
            iletisim.okundu_Bilgi = false;
            
            return View(iletisim);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Iletisim([Bind(Include = "Id,kullanici,e_posta,mesaj,okundu_Bilgi")] Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                db.Iletisim.Add(iletisim);
                db.SaveChanges();
                ViewBag.Mesaj = true;    
            }
            return View();
        }

        //-----------Makine_Ogrenmesi-----------
        public ActionResult Makine_Ogrenmesi()
        {
            return View();
        }

        //-----------Giris-----------
        public ActionResult Giris()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Giris(string email)
        {
            try
            {
                // SMTP istemcisi oluşturma
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("azraill03838@gmail.com", "kgkh conf ybod agkj"),
                    EnableSsl = true
                };

                // Yeni bir MailMessage oluşturma
                MailMessage msj = new MailMessage
                {
                    From = new MailAddress("azraill03838@gmail.com", "noreply") // Gönderen e-posta adresi ve adı
                };
                Random random = new Random();
                int rastgeleSayi = random.Next(1000, 10000);
                Session["rastgelesayi"] = rastgeleSayi;
                Session["e-posta"] = email;
                msj.To.Add(email); // Alıcı e-posta adresi
                msj.Subject = "Giriş"; // E-posta konusu
                msj.Body = $@"
    <html>
    <body>
        <h2>Giriş İçin Tek Kullanımlık Kod</h2>
        <p>Hastane sistemine girmek için aşağıda tek kullanımlık kod verilmiştir. Kodu kimseyle paylaşmayınız.</p>
        <h3><b>{rastgeleSayi}</b></h3>
        <p>İyi Günler.</p>
    </body>
    </html>
"; // E-posta içeriği
                msj.IsBodyHtml = true;
                client.Send(msj);
                TempData["dogrula"] = true;
                return RedirectToAction("Dogrula");
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya bilgi verme
                ViewBag.Error = "Mesaj gönderilirken hata oluştu: " + ex.Message;
            }

            return View();
        }

        //-----------Dogrula-----------
        public ActionResult Dogrula() 
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Dogrula(int? sayi)
        {
            if (Session["e-posta"] == null)
            {
                return RedirectToAction("Giris", "Home");
            }
            if (sayi == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                string sayii = Session["rastgelesayi"].ToString();
                if (sayi == Convert.ToInt32(sayii))
                {
                    string eposta = Session["e-posta"].ToString();
                    var kullanici = db.Kullanici.FirstOrDefault(k => k.e_posta.ToString() == eposta);
                    if (kullanici != null)
                    {
                        if (kullanici.yetkinlik == true)
                        {
                            Session["admin"] = true;
                            return RedirectToAction("Home", "Doktor");
                        }
                        else
                        {
                            Session["kullanici"] = true;
                            return RedirectToAction("Home", "Kullanici");
                        }
                    }
                    else
                    {
                        TempData["mail"] = Session["e-posta"].ToString();
                        return RedirectToAction("UyeOl");
                    }
                }
                else

                    return View();
            }

        }

        //-----------UyeOl-----------
        public ActionResult UyeOl()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UyeOl([Bind(Include = "Id,ad,soyad,tel_No,e_posta,yetkinlik")] Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                Session["e-posta"] = kullanici.e_posta;
                Session["kullanici"] = true;
                db.Kullanici.Add(kullanici);
                db.SaveChanges();
            }
            return RedirectToAction("Home", "Kullanici");
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