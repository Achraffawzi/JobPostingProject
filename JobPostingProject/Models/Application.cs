//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JobPostingProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Application
    {
        public int CandidateID { get; set; }
        public int AnnouncementID { get; set; }
        public System.DateTime ApplicationDate { get; set; }
        public string CoverLetter { get; set; }
    
        public virtual Announcement Announcement { get; set; }
        public virtual Candidate Candidate { get; set; }
    }
}
