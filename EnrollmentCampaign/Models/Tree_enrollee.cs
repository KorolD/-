using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnrollmentCampaign
{
    public enum TreeLevel { enrollee, speciality, faculty, BSUIR}

    public class TreeElement
    {
        private int _ID;
        private TreeLevel level;
        public string ID { get { return LevelToString(level) + _ID.ToString(); } }
        public int descendant_amount;
        public string value;

        public TreeElement(int id, TreeLevel lvl, int desc_amount, string val)
        {
            _ID = id;
            level = lvl;
            descendant_amount = desc_amount;
            value = val;
        }

        private static string LevelToString(TreeLevel level)
        {
            switch (level)
            {
                case TreeLevel.enrollee: return "e";
                case TreeLevel.faculty: return "f";
                case TreeLevel.speciality: return "s";
                case TreeLevel.BSUIR: return "B";
                default: return "def";
            }
        }
    }

    public  class TreeControl
    {
        public static IEnumerable<TreeElement> GetDescendants(int id, TreeLevel level)
        {
            var ent = new EnrollmentCampaignEntities();
            switch (level)
            {
                case TreeLevel.BSUIR: return ent.university_enum.ToList().Select((f)=>new TreeElement(f.ID,TreeLevel.faculty,ent.speciality_enum.Count((s)=>s.university==f.ID),f.name));
                case TreeLevel.faculty: return ent.speciality_enum.Where((s)=>s.university==id).ToList().Select((s)=>new TreeElement(s.ID, TreeLevel.speciality,ent.speciality_priorities.Count((p)=>p.priority==1&&p.speciality_ID==s.ID),
                    s.name
                    ));
                case TreeLevel.speciality: return ent.speciality_priorities.Where(p => p.priority == 1 && p.speciality_ID == id).ToList().Select((Func<speciality_priorities,TreeElement>)((p) => {
                    plea pl = ent.pleas.Find(p.plea_ID);
                    enrollee e = ent.enrollees.Find(pl.enrollee_ID);
                    return new TreeElement(pl.enrollee_ID, TreeLevel.enrollee, 0, e.surname+" "+e.name); }));
                case TreeLevel.enrollee: 
                default: return null;
            }
        }

        public static IEnumerable<TreeElement> GetDescendants(string id)
        {
            try
            {
                if (id.StartsWith("e"))
                {
                    return GetDescendants(int.Parse(id.Substring(1)), TreeLevel.enrollee);
                }
                if (id.StartsWith("f"))
                {
                    return GetDescendants(int.Parse(id.Substring(1)), TreeLevel.faculty);
                }
                if (id.StartsWith("s"))
                {
                    return GetDescendants(int.Parse(id.Substring(1)), TreeLevel.speciality);
                }
                if (id.StartsWith("B"))
                {
                    return GetDescendants(int.Parse(id.Substring(1)), TreeLevel.BSUIR);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static int GetID(string id)
        {
            try
            {
                if(id.StartsWith("e")|| id.StartsWith("f") || id.StartsWith("s") || id.StartsWith("B"))
                {
                    return int.Parse(id.Substring(1));
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static TreeElement GetRoot()
        {
            using(var ent = new EnrollmentCampaignEntities())
            {
                return new TreeElement(0, TreeLevel.BSUIR, ent.university_enum.Count(), "BSUIR");
            }
        }

        public static int Count(string[] id)
        {
            if (id == null) return 0;
            using (var ent = new EnrollmentCampaignEntities())
            {
                if (id.Length == 1 && id[0].StartsWith("B"))
                {
                    return ent.enrollees.Count();
                }
                int c = 0;
                List<int> s_id = new List<int>(), f_id = new List<int>();
                foreach (string s in id)
                {
                    if (s.StartsWith("e"))
                    {
                        ++c;
                        continue;
                    }
                    if (s.StartsWith("f"))
                    {
                        f_id.Add(int.Parse(s.Substring(1)));
                    }
                    if (s.StartsWith("s"))
                    {
                        s_id.Add(int.Parse(s.Substring(1)));
                    }
                }
                if (f_id.Count != 0)
                {
                    var add = ent.university_enum.Where(f => f_id.Contains(f.ID)).Select(f => f.ID);

                    s_id.AddRange(ent.speciality_enum.Where(s=>add.Contains(s.university)).Select(s=>s.ID));
                }
                c+=ent.speciality_priorities.Count(p => (p.priority == 1 && s_id.Contains(p.speciality_ID)));
                return c;
            }
        }
    }
}