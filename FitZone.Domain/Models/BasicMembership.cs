using System;

namespace FitZone.Domain.Models
{
    public class BasicMembership : Membership
    {
        public BasicMembership()
        {
        }

        public BasicMembership(
            string membershipId,
            int memberId,
            DateTime startDate)
        {
            MembershipId = membershipId;
            MemberId = memberId;

            Type = "Basic";

            StartDate = startDate;
            EndDate = startDate.AddMonths(1);

            VisitLimit = 8;
            RemainingVisits = 8;

            Status = "Active";
        }

        public override int GetVisitLimit()
        {
            return 8;
        }

        public override bool CanBook(WorkoutSession session)
        {
            if (!base.CanBook(session))
                return false;

            if (session == null)
                return false;

            // Basic წევრს Premium-only სესიის დაჯავშნა არ შეუძლია.
            return !session.IsPremiumOnly();
        }
    }
}