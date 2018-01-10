using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnrollmentCampaign.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData.Model = new EnrollmentCampaignEntities().enrollees;
            return View();
        }

        public ActionResult Personal(int ID)
        {
            var ent = new EnrollmentCampaignEntities();
            
                var e = ent.enrollees.Find(ID);
                if (ID != 0 && e == null) return RedirectToAction("Index");            
            return View(e);
            
        }

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                var e = ent.enrollees.Find(ID);
                if (ID != 0 && e == null) return RedirectToAction("Index");

                if (ID == 0)
                {
                    e = enrollee.EmptyEnrollee();
                    e.writing_locker = new writing_locker() { enrollee_ID = 0 };
                    return View(e);
                }
                var loc = ent.writing_locker.Find(ID);
                if (loc == null)
                {
                    var l = new writing_locker() { begin_time = DateTime.Now, enrollee_ID = ID };
                    ent.writing_locker.Add(l);
                    ent.SaveChanges();
                    e.writing_locker = l;
                    return View(e);
                }
                if(loc.begin_time.Since() > writing_locker.time_limit)
                {
                    loc.begin_time = DateTime.Now;
                    ent.SaveChanges();
                    e.writing_locker = loc;
                    return View(e);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetTowns(int country_id)
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                ViewBag.list = ent.countries_enum.Find(country_id).towns_list;
                return PartialView();
            }
        }

        public ActionResult GetSpecialities(int faculty_id)
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                ViewBag.list = ent.faculty_enum.Find(faculty_id).speciality_list;
                return PartialView();
            }
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(enrollee e)
        {
            try
            {
                e.FromForm(Request.Form);
                if (e.Validate())
                {
                    if (!TryExitWritingLock(
                        new writing_locker() { enrollee_ID = e.ID, begin_time = DateTime.Parse(Request.Form.GetValues("begin_time").First()) }
                        )) { return RedirectToAction("Index"); }
                    using (var ent = new EnrollmentCampaignEntities())
                    {
                        int spec_id = int.Parse(Request.Form.GetValues("speciality").First());
                        if (ent.specialty_enum.Find(spec_id) == null) { return RedirectToAction("Index"); }
                        specialty_priorities pr = new specialty_priorities() { specialty_ID = spec_id, enrollee_ID = e.ID, priority = 1 };
                        ent.specialty_priorities.RemoveRange(ent.specialty_priorities.Where(p => (p.enrollee_ID == pr.enrollee_ID && p.priority == 1)));
                        ent.specialty_priorities.Add(pr);
                        ent.CT_results.RemoveRange(ent.CT_results.Where(r => r.enrollee_ID == e.ID));
                        ent.CT_results.AddRange(e.results.AllExistingCT);
                        if (e.ID != 0)
                        {
                            ent.enrollees.Find(e.ID).SetProperties(e);
                        }
                        else
                        {
                            ent.enrollees.Add(e);
                        }
                        ent.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                ViewBag.validationfail = true;
                e.writing_locker = new writing_locker() { enrollee_ID = e.ID, begin_time = DateTime.Parse(Request.Form.GetValues("begin_time").First()) };
                return View(e);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(writing_locker locker)
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                if (!TryExitWritingLock(locker)) return RedirectToAction("Index");
                var en = ent.enrollees.Find(locker.enrollee_ID);
                if (en != null)
                {
                    ent.enrollees.Remove(en);
                    ent.CT_results.RemoveRange(ent.CT_results.Where(r => r.enrollee_ID == en.ID));
                    ent.specialty_priorities.RemoveRange(ent.specialty_priorities.Where(p => p.enrollee_ID == en.ID));
                    ent.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Tree()
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return View(ent.Trees.Find(1));
            }
        }

        public ActionResult GetDescendants(int id)
        {
            var ent = new EnrollmentCampaignEntities();
            var test = (IEnumerable<Tree>)(ent.Trees.Where(t => t.parent_ID == id).Where(t => t.ID != id));
            var tuple = (test, id);
            return PartialView(tuple);
        }

        private bool TryExitWritingLock(writing_locker l)//true=success(deleted)/enrollee_id=0, false=out_of_time(deleted)/wrong locker
        {
            if (l.enrollee_ID == 0) return true;
            using(var ent = new EnrollmentCampaignEntities())
            {
                var w = ent.writing_locker.Find(l.enrollee_ID);
                if (w == null) return false;
                DateTime beg = new DateTime(w.begin_time.Year, w.begin_time.Month, w.begin_time.Day, w.begin_time.Hour, w.begin_time.Minute, w.begin_time.Second);
                if (beg != l.begin_time) return false;
                ent.writing_locker.Remove(w);
                ent.SaveChanges();
                if (w.begin_time.Since() > writing_locker.time_limit)
                {
                    return false;
                }
                return true;
            }
        }

        public ActionResult IndexFromLock(writing_locker locker)
        {
            TryExitWritingLock(locker);
            return RedirectToAction("Index");
        }
    }
}