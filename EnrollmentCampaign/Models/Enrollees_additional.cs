using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnrollmentCampaign
{
    public partial class enrollee
    {
        private CT_results_list _results;
        public CT_results_list results
        {
            get
            {
                if (_results != null) return _results;
                _results = new CT_results_list(ID);
                return _results;
            }
            set
            {
                _results = value;
            }
        }

        public string GetTownName()
        {
            EnrollmentCampaignEntities ent = new EnrollmentCampaignEntities();
            var town = ent.towns_enum.Find(town_ID);
            return town.name;
        }

        public countries_enum GetCountry()
        {
            EnrollmentCampaignEntities ent = new EnrollmentCampaignEntities();
            var town = ent.towns_enum.Find(town_ID);
            return ent.countries_enum.Find(town.country_ID);
        }

        public string GetBirthdate()
        {
            return birthdate.GetDateTimeFormats()[0];
        }

        public string GetBirthdateEdit()
        {
            return birthdate.GetDateTimeFormats()[42].Substring(0, 10);
        }

        public specialty_enum GetSpeciality(int priority)
        {
            using(var ent = new EnrollmentCampaignEntities())
            {
                var s=ent.specialty_priorities.FirstOrDefault(p => (p.priority == priority && p.enrollee_ID == ID));
                if (s == null) return ent.specialty_enum.First();
                return ent.specialty_enum.Find(s.specialty_ID);
            }
        }

        public specialty_priorities GetPriority(int priority)
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return ent.specialty_priorities.First(p => (p.priority == priority && p.enrollee_ID == ID));
            }
        }

        public void FromForm(System.Collections.Specialized.NameValueCollection Form)
        {
            _results = new CT_results_list();
            for (int i = 0; i < CT_results_list.MAX_CT; i++)
            {
                int id = int.Parse(Form.GetValues("CT_ID" + i.ToString()).First());
                if (id != 0)
                {
                    _results.AllExistingCT.Add(new CT_results()
                    {
                        enrollee_ID = ID,
                        CT_ID = id,
                        result = byte.Parse(Form.GetValues("CT_res" + i.ToString()).First())
                    });
                }
            }
            _results.AllExistingCT = _results.AllExistingCT.GroupBy(r => r.CT_ID).Select(gr => gr.First()).ToList();
            _results.EditableCT = null;
        }

        public void SetProperties(enrollee e)
        {
            name = e.name;
            surname = e.surname;
            patronymic = e.patronymic;
            town_ID = e.town_ID;
            street = e.street;
            house = e.house;
            block = e.block;
            apartment = e.apartment;
            birthdate = e.birthdate;
            passport = e.passport;
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (string.IsNullOrWhiteSpace(surname)) return false;
            if (string.IsNullOrWhiteSpace(patronymic)) return false;
            if (string.IsNullOrWhiteSpace(passport)) return false;
            if (string.IsNullOrWhiteSpace(street)) return false;
            if (birthdate > DateTime.Today || birthdate < DateTime.Today.AddYears(-100)) return false;
            return true;
        }

        public static enrollee EmptyEnrollee()
        {
            using (var ent = new EnrollmentCampaignEntities())
                return new enrollee()
                {
                    town_ID = ent.towns_enum.First().ID,
                    house = 1,
                    birthdate = new DateTime(2000, 1, 1)
                };
        }
    }

    public partial class towns_enum
    {
        //public static List<SelectListItem> GetTownsInCountry(int country_id)
        //{
        //    var ent = new EnrollmentCampaignEntities();

        //        var towns = ent.towns_enum.Where(t => t.country_ID == country_id);
        //        var list = new List<SelectListItem>(towns.Select(t => new SelectListItem() { Value = t.ID.ToString(), Text = t.name }));
        //        return list;

        //}
    }

    public partial class countries_enum
    {
        private List<SelectListItem> _towns_list;
        public List<SelectListItem> towns_list
        {
            get
            {
                if (_towns_list != null) return _towns_list;
                using (var ent = new EnrollmentCampaignEntities())
                {
                    var towns = ent.towns_enum.Where(t => t.country_ID == ID);
                    _towns_list = new List<SelectListItem>(towns.Select(t => new SelectListItem() { Value = t.ID.ToString(), Text = t.name }));
                    return _towns_list;
                }
            }
        }

        private static List<SelectListItem> _countries_list;
        public static List<SelectListItem> countries_list
        {
            get
            {
                if (_countries_list != null) return _countries_list;
                using (var ent = new EnrollmentCampaignEntities())
                {
                    var countries = ent.countries_enum;
                    _countries_list = new List<SelectListItem>(countries.Select(t => new SelectListItem() { Value = t.ID.ToString(), Text = t.name}));
                    return _countries_list;
                }
            }
        }
    }

    public partial class CT_results
    {
        public CT_enum GetCT()
        {
            using(var ent = new EnrollmentCampaignEntities())
            {
                return ent.CT_enum.Find(CT_ID);
            }
        }
    }

    public partial class specialty_enum
    {
        public string GetFullName()
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return name + "  -  " + ent.faculty_enum.Find(faculty_ID).name;
            }
        }

        public faculty_enum GetFaculty()
        {
            using (var ent = new EnrollmentCampaignEntities())
            {
                return ent.faculty_enum.Find(faculty_ID);
            }
        }
    }

    public partial class faculty_enum
    {
        private static List<SelectListItem> _faculty_list;
        public static List<SelectListItem> faculty_list
        {
            get
            {
                if (_faculty_list != null) return _faculty_list;
                using(var ent = new EnrollmentCampaignEntities())
                {
                    var faculties = ent.faculty_enum;
                    _faculty_list = new List<SelectListItem>(faculties.Select(f => new SelectListItem() { Value = f.ID.ToString(), Text = f.name }));
                    return _faculty_list;
                }
            }
        }

        private List<SelectListItem> _speciality_list;
        public List<SelectListItem> speciality_list
        {
            get
            {
                if (_speciality_list != null) return _speciality_list;
                using(var ent=new EnrollmentCampaignEntities())
                {
                    _speciality_list = new List<SelectListItem>(ent.specialty_enum.Where(s => s.faculty_ID == ID).Select(s => new SelectListItem() { Value = s.ID.ToString(), Text = s.name }));
                    return _speciality_list;
                }
            }
        }
    }

    public class CT_results_list
    {
        public static readonly int MAX_CT = 3;
        public readonly int enrollee_ID;
        public List<CT_results> AllExistingCT { get; set; }

        public CT_results_list(int enrollee_ID)
        {
            this.enrollee_ID = enrollee_ID;
            using (var ent = new EnrollmentCampaignEntities())
            {
                AllExistingCT = ent.CT_results.Where(r => r.enrollee_ID == enrollee_ID).ToList();
                if (AllExistingCT.Count > MAX_CT) throw new ArgumentException("Количество цт не должно превышать" + MAX_CT.ToString());
            }
        }

        private List<CT_results> _editable;
        public List<CT_results> EditableCT
        {
            get
            {
                if (_editable != null) return _editable;
                _editable = new List<CT_results>(AllExistingCT);
                while (_editable.Count < MAX_CT)
                {
                    _editable.Add(new CT_results() { CT_ID = 0, result = 15 });
                }
                return _editable;
            }
            set { _editable = value; }
        }

        public CT_results_list() { AllExistingCT = new List<CT_results>(); }

        private static List<SelectListItem> _CT_list;
        public static List<SelectListItem> CT_list
        {
            get
            {
                if (_CT_list != null) return _CT_list;
                using (var ent = new EnrollmentCampaignEntities())
                {
                    _CT_list = new List<SelectListItem>(ent.CT_enum.Select(c => new SelectListItem() { Value = c.ID.ToString(), Text = c.name }));
                    _CT_list.Add(new SelectListItem() { Value = "0", Text = "Null" });
                    return _CT_list;
                }
            }
        }
    }

    public partial class writing_locker
    {
        public static TimeSpan time_limit = new TimeSpan(0, 0, 20, 0, 0);
    }

    public static class for_writing_locker
    {
        public static TimeSpan Since(this DateTime t)
        {
            return DateTime.Now.Subtract(t);
        }
    }
}