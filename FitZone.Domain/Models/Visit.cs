
using FitZone.Domain.Models;


using System;
using System.Collections.Generic;
using System.Text;
 
namespace FitZone.Domain.Models
{
    public class Visit
    {
        public string VisitId { get; set; }

        public int MemberId { get; set; }

        public DateTime VisitDate { get; set; }

        public string Status { get; set; }

        public Visit()
        {
        }

        public Visit(
            string visitId,
            int memberId)
        {
            VisitId = visitId;
            MemberId = memberId;
            VisitDate = DateTime.Now;
            Status = "Completed";
        }

        // Used for visits created from finished booked sessions:
        // the visit date is the date of the session.
        public Visit(
            string visitId,
            int memberId,
            DateTime visitDate)
            : this(visitId, memberId)
        {
            VisitDate = visitDate;
        }
    }
}

