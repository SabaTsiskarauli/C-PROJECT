using System;

namespace FitZone.Domain.Models
{
    public class PremiumMembership : Membership
    {
        public PremiumMembership()
        {
        }

        public PremiumMembership(
            string membershipId,
            int memberId,
            DateTime startDate)
        {
            MembershipId = membershipId;
            MemberId = memberId;

            Type = "Premium";

            StartDate = startDate;
            EndDate = startDate.AddMonths(1);

            // Premium-ს ვიზიტების ლიმიტი არ აქვს.
            RemainingVisits = null;

            // -1 ნიშნავს Unlimited-ს.
            VisitLimit = -1;

            Status = "Active";
        }

        public override int GetVisitLimit()
        {
            return -1;
        }

        public override bool CanBook(WorkoutSession session)
        {
            return base.CanBook(session);
        }
    }
}