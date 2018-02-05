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
                    return View(e);
                }
                if (writing_locker.TryEnterLock(e, ent)) { return View(e); }
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

        public ActionResult GetSpecialities(int university)
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                ViewBag.list = ent.university_enum.Find(university).speciality_list;
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
                    if (!writing_locker.TryExitWritingLock(
                        new writing_locker() { enrollee_ID = e.ID, begin_time = DateTime.Parse(Request.Form.GetValues("begin_time").First()) }
                        )) { return RedirectToAction("Index"); }
                    using (var ent = new EnrollmentCampaignEntities())
                    {


                        //speciality_priorities pr = new speciality_priorities() { speciality_ID = spec_id, enrollee_ID = e.ID, priority = 1 };
                        //ent.speciality_priorities.RemoveRange(ent.speciality_priorities.Where(p => (p.enrollee_ID == pr.enrollee_ID && p.priority == 1)));
                        //ent.speciality_priorities.Add(pr);


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
                if (!writing_locker.TryExitWritingLock(locker)) return RedirectToAction("Index");
                var en = ent.enrollees.Find(locker.enrollee_ID);
                if (en != null)
                {
                    ent.enrollees.Remove(en);
                    ent.CT_results.RemoveRange(ent.CT_results.Where(r => r.enrollee_ID == en.ID));



                    
                    var l = ent.pleas.Where(p => p.enrollee_ID == en.ID);
                    var li = l.Select(p => p.ID).ToList().Distinct();
                    ent.speciality_priorities.RemoveRange(ent.speciality_priorities.Where(p => li.Contains(p.plea_ID)));
                    ent.pleas.RemoveRange(l);



                    ent.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Tree()
        {
            return View(TreeControl.GetRoot());
        }

        public ActionResult GetDescendants(string id, bool chcked)
        {
            var e = TreeControl.GetDescendants(id);
            return PartialView((e, TreeControl.GetID(id), chcked));
        }

        public ActionResult IndexFromLock(writing_locker locker)
        {
            writing_locker.TryExitWritingLock(locker);
            return RedirectToAction("Index");
        }

        public int CountLeaves(string[] id)
        {
            return TreeControl.Count(id);
        }

        public ActionResult Plea(int ID)
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                var pl = ent.pleas.FirstOrDefault(p => p.enrollee_ID == ID);
                if (pl == null)
                {
                    return View(new Plea(ID,false));
                }
                return View(new Plea(ID, true));
            }
        }

        public ActionResult SelectSpec(string fin, string form, string period, string uni)
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return PartialView(ent.speciality_enum.Where(
                    s => (s.financing.ToString() == fin && s.form.ToString() == form && s.training_period.ToString() == period && s.university.ToString() == uni)
                   ).ToList());
            }
        }

        public ActionResult MainSpecSelected(string spec_id, int enrollee_id, bool already_exists)
        {
            try
            {
                if (!already_exists)
                {
                    Plea_group pl = new Plea_group(spec_id, enrollee_id);
                    return PartialView((pl, 1));
                }
                else
                {
                    using (var ent = new EnrollmentCampaignEntities())
                    {
                        var pl = ent.pleas.FirstOrDefault(p => p.enrollee_ID == enrollee_id);
                        var priorities = ent.speciality_priorities.Where(p => p.plea_ID == pl.ID).ToList();
                        if (priorities.Count == 0)
                        {
                            ent.pleas.Remove(pl);
                            ent.SaveChanges();
                            return View(new Plea(enrollee_id));
                        }
                        var gr = new Plea_group(priorities.First().speciality_ID.ToString(),enrollee_id);
                        gr.Order(priorities);
                        return PartialView((gr, priorities.Count));
                    }
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public void DeletePlea(int enrollee_id)
        {
            using(var ent = new EnrollmentCampaignEntities())
            {
                var removed = ent.pleas.Where(p => p.enrollee_ID == enrollee_id);
                var r = removed.Select(p => p.ID);
                ent.speciality_priorities.RemoveRange(ent.speciality_priorities.Where(p => r.Contains(p.plea_ID)));
                ent.pleas.RemoveRange(removed);
                ent.SaveChanges();
            }
        }

        public void SavePlea(int enrollee_id, int[] ids)
        {
            try
            {
                byte priority = 0;
                DeletePlea(enrollee_id);
                using (var ent = new EnrollmentCampaignEntities())
                {
                    var pl = ent.pleas.Add(new plea() { enrollee_ID = enrollee_id });
                    for(int i = 0; i < ids.Length; ++i)
                    {
                        if (ids[i] == 0) { continue; }
                        ent.speciality_priorities.Add(new speciality_priorities() { plea_ID = pl.ID, priority = priority, speciality_ID = ids[i] });
                        ++priority;
                    }
                    ent.SaveChanges();
                }
            }
            catch
            {
            }
            return;
        }

        public ActionResult ClearLocker()
        {
            using(var ent = new EnrollmentCampaignEntities())
            {
                ent.ClearLocker((int)writing_locker.time_limit.TotalSeconds);
            }
            return RedirectToAction("Index");
        }
    }
}