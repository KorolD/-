using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnrollmentCampaign
{
    public partial class Tree
    {
        private int _descendant_amount = -1;
        public int descendant_amount
        {
            get
            {
                if (_descendant_amount > -1) return _descendant_amount;
                using (var ent = new EnrollmentCampaignEntities())
                {
                    _descendant_amount = ent.Trees.Count(t => (t.parent_ID == ID&&t.ID!=ID));
                    return _descendant_amount;
                }
            }
        }
    }
}