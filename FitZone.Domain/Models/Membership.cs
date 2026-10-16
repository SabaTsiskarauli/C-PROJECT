using System;

namespace FitZone.Domain.Models
{
    public class Membership
    {
        public string MembershipId { get; set; }

        public int MemberId { get; set; }

        public string Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; protected set; }

        public int? RemainingVisits { get; protected set; }

        public string Status { get; protected set; }

        public int VisitLimit { get; protected set; }

        public Membership()
        {
        }

        public Membership(
            string membershipId,
            int memberId,
            string type,
            DateTime startDate,
            DateTime endDate,
            int? remainingVisits,
            int visitLimit)
        {
            MembershipId = membershipId;
            MemberId = memberId;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            RemainingVisits = remainingVisits;
            VisitLimit = visitLimit;
            Status = "Active";
        }

        public virtual int GetVisitLimit()
        {
            return VisitLimit;
        }

        public virtual bool CanBook(
            WorkoutSession session)
        {
            if (session == null)
                return false;

            if (Status != "Active")
                return false;

            if (DateTime.Now > EndDate)
                return false;

            if (RemainingVisits.HasValue &&
                RemainingVisits.Value <= 0)
            {
                return false;
            }

            if (session.StartDateTime <= DateTime.Now)
                return false;

            if (session.Status != "Active")
                return false;

            if (session.AvailablePlaces <= 0)
                return false;

            return true;
        }

        public bool CanEnter()
        {
            if (Status != "Active")
                return false;

            if (DateTime.Now > EndDate)
                return false;

            if (RemainingVisits.HasValue &&
                RemainingVisits.Value <= 0)
            {
                return false;
            }

            return true;
        }

        public void RegisterVisit()
        {
            if (!CanEnter())
            {
                throw new InvalidOperationException(
                    "Membership is expired or has no remaining visits.");
            }

            if (RemainingVisits.HasValue)
                RemainingVisits--;
        }

        public void Renew(
            DateTime newEndDate,
            int? newRemainingVisits)
        {
            if (newEndDate <= DateTime.Now)
            {
                throw new ArgumentException(
                    "New end date must be in the future.");
            }

            if (newRemainingVisits.HasValue &&
                newRemainingVisits.Value < 0)
            {
                throw new ArgumentException(
                    "Remaining visits cannot be negative.");
            }

            StartDate = DateTime.Now;
            EndDate = newEndDate;
            RemainingVisits = newRemainingVisits;
            Status = "Active";
        }

        public void Expire()
        {
            Status = "Expired";
        }

        public void RestoreState(
            DateTime startDate,
            DateTime endDate,
            int? remainingVisits,
            string status,
            int visitLimit)
        {
            if (endDate < startDate)
            {
                throw new ArgumentException(
                    "Membership end date cannot be before start date.");
            }

            if (remainingVisits.HasValue &&
                remainingVisits.Value < 0)
            {
                throw new ArgumentException(
                    "Remaining visits cannot be negative.");
            }

            StartDate = startDate;
            EndDate = endDate;
            RemainingVisits = remainingVisits;
            Status = string.IsNullOrWhiteSpace(status)
                ? "Active"
                : status;
            VisitLimit = visitLimit;
        }
    }
}