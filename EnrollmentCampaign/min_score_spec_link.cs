//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnrollmentCampaign
{
    using System;
    using System.Collections.Generic;
    
    public partial class min_score_spec_link
    {
        public int speciality_ID { get; set; }
        public short min_score_ID { get; set; }
        public byte priority { get; set; }
    
        public virtual CT_min_scores CT_min_scores { get; set; }
        public virtual CT_priorities_enum CT_priorities_enum { get; set; }
        public virtual speciality_enum speciality_enum { get; set; }
    }
}