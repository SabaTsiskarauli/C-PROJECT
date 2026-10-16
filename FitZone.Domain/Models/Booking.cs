using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Domain.Models
{
    public class Booking
    {
        public string BookingId { get; set; }

        public int MemberId { get; set; }

        public string SessionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }

        public Booking()
        {
        }

        public Booking(
            string bookingId,
            int memberId,
            string sessionId)
        {
            BookingId = bookingId;
            MemberId = memberId;
            SessionId = sessionId;
            CreatedAt = DateTime.Now;
            Status = "Active";
        }

        public void Cancel()
        {
            Status = "Cancelled";
        }

        // The booked session has ended and the member's visit was counted.
        public void Complete()
        {
            Status = "Completed";
        }

        // The booked session has ended, but the member could not attend
        // (membership expired or no visits left).
        public void MarkMissed()
        {
            Status = "Missed";
        }
    }
}