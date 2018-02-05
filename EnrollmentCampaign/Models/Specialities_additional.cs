using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnrollmentCampaign
{
    public partial class speciality_group
    {
        private List<speciality_enum> _specialities;
        public List<speciality_enum> specialities
        {
            get
            {
                if (_specialities == null)
                {
                    using(var ent = new EnrollmentCampaignEntities())
                    {
                        _specialities = ent.speciality_enum.Where(s => s.group_ID == ID).ToList();
                    }
                }
                return _specialities;
            }
        }

        public List<financing_enum> FinancingList()
        {
            using(var ent = new EnrollmentCampaignEntities())
            {
                return specialities.Select(s => s.financing).Distinct().Select(f=>ent.financing_enum.Find(f)).ToList();
            }
        }

        public List<form_enum> FormList()
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return specialities.Select(s => s.form).Distinct().Select(f => ent.form_enum.Find(f)).ToList();
            }
        }

        public List<training_period_enum> PeriodList()
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return specialities.Select(s => s.training_period).Distinct().Select(f => ent.training_period_enum.Find(f)).ToList();
            }
        }

        public List<SelectListItem> EnrollInList()
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return specialities.Select(s => s.enroll_in).Distinct().Select(f => ent.enroll_in_enum.Find(f)).Select(f => new SelectListItem() { Value = f.ID.ToString(), Text = f.name }).ToList();
            }
        }

        public List<university_enum> UniversityList()
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return specialities.Select(s => s.university).Distinct().Select(f => ent.university_enum.Find(f)).ToList();
            }
        }
    }

    public class SimpleSpeciality
    {
        public readonly int ID, university;
        public readonly string name;
        public readonly byte financing, form, period;

        public SimpleSpeciality(speciality_enum s)
        {
            ID = s.ID;
            university = s.university;
            name = s.name;
            financing = s.financing;
            form = s.form;
            period = s.training_period;
        }
    }

    public class Plea
    {
        public List<financing_enum> financing;
        public List<form_enum> forms;
        public List<training_period_enum> periods;
        public List<university_enum> universities;
        public readonly int enrollee_id;
        public readonly bool exists;

        public Plea(int enrolle_id, bool already_exists=false)
        {
            using(var ent = new EnrollmentCampaignEntities())
            {
                //string str;
                //foreach (var s in specialities) str = s.name;
                financing = ent.financing_enum.ToList();
                forms = ent.form_enum.ToList();
                periods = ent.training_period_enum.ToList();
                universities = ent.university_enum.ToList();
                enrollee_id = enrolle_id;
                exists = already_exists;
            }
        }
    }

    public class Plea_group
    {
        public List<SimpleSpeciality> specialities;
        public List<financing_enum> financing;
        public List<form_enum> forms;
        public List<training_period_enum> periods;
        public List<university_enum> universities;
        public int positions;
        public readonly int enrollee_id;

        public Plea_group(string id, int enrollee_ID)
        {
            using(var ent  = new EnrollmentCampaignEntities())
            {
                var spec = ent.speciality_enum.FirstOrDefault(s => s.ID.ToString() == id);
                if (spec == null) throw new ArgumentException();

                var gr = ent.speciality_group.Find(spec.group_ID);
                specialities = gr.specialities.Select(s => new SimpleSpeciality(s)).ToList();
                financing = gr.FinancingList();
                forms = gr.FormList();
                periods = gr.PeriodList();
                universities = gr.UniversityList();
                positions = gr.select_limit;
                SimpleSpeciality temp, selected_first = specialities.First(s => s.ID == spec.ID);
                int index = specialities.IndexOf(selected_first);
                temp = specialities[index];
                specialities[index] = specialities[0];
                specialities[0] = temp;
                enrollee_id = enrollee_ID;
            }
        }

        public void Order(List<speciality_priorities> list)
        {
            SimpleSpeciality temp;
            int index;
            foreach(var pr in list)
            {
                index = specialities.IndexOf(specialities.First(s => s.ID == pr.speciality_ID));
                temp = specialities[index];
                specialities[index] = specialities[pr.priority];
                specialities[pr.priority] = temp;
            }
        }
    }
}