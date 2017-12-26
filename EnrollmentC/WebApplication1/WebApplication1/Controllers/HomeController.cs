using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Enrollment_campaign_ns.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            enrollees en = new enrollees() { country="Belarus"};
            TempData["en"] = en;
            return RedirectToAction("Enrollee_form");
        }

        [HttpPost]
        public ActionResult Login(enrollees e)
        {
            using (Enrollment_campaign_entities sf = new Enrollment_campaign_entities())
            {
                var en = sf.enrollees.FirstOrDefault(new Func<enrollees, bool>(x =>
                {
                    if (x.email.TrimEnd(' ') == e.email && x.password.TrimEnd(' ') == e.password) return true; return false;
                }));
                if (en == null)
                {

                    //не нашлось сочетание логин-пароль
                    return View();
                    return RedirectToAction("Index");
                }
                if (en.country == null) en.country = "Belarus";
                TempData["en"] = en;
                return RedirectToAction("Enrollee_form");
            }
        }

        [HttpGet]
        public ActionResult Enrollee_form()
        {
            var wer = TempData["en"];
            return View(wer);
        }

        [HttpPost, ActionName("Enrollee_form")]
        public ActionResult Enrollee_formPost(enrollees e)
        {
            using (Enrollment_campaign_entities db = new Enrollment_campaign_entities())
            {
                if (ModelState.IsValid)
                {
                    if (e.country == "Belarus") e.country = null;
                    var en = db.enrollees.FirstOrDefault(new Func<enrollees, bool>(x => { return (x.ID == e.ID) ? true : false; }));
                    if (en != null)
                    {
                        en.Update(e);
                    }
                    else
                    {
                        db.enrollees.Add(e);
                    }
                    //try
                    //{
                        db.SaveChanges();
                    //}
                    //catch
                    //{
                    //    ошибка валидации
                    //    return View(e);
                    //}
                    return RedirectToAction("Index");
                }
                //ошибка валидации
                return View(e);
            }
        }

        [ActionName("Delete")]
        public ActionResult Delete(enrollees e)
        {
            using (var db = new Enrollment_campaign_entities())
            {
                var deleting = db.enrollees.FirstOrDefault(new Func<enrollees, bool>(x => { return x.ID == e.ID; }));
                if ( deleting!= null)
                {
                    db.enrollees.Remove(deleting);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}