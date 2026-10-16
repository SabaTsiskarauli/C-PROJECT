using System;

namespace FitZone.Domain.Models
{
    public class WorkoutSession
    {
        public string SessionId { get; set; }

        public int TrainerId { get; set; }

        public string Type { get; set; }

        public DateTime StartDateTime { get; set; }

        public int DurationMinutes { get; set; }

        public int Capacity { get; set; }

        public string Status { get; set; }

        public int OccupiedPlaces { get; set; }

        public string RequiredMembershipType { get; set; }

        public int AvailablePlaces
        {
            get
            {
                return Capacity - OccupiedPlaces;
            }
        }

        public DateTime EndDateTime
        {
            get
            {
                return StartDateTime.AddMinutes(
                    DurationMinutes);
            }
        }

        public WorkoutSession()
        {
        }

        public WorkoutSession(
            string sessionId,
            int trainerId,
            string type,
            DateTime startDateTime,
            int durationMinutes,
            int capacity,
            string requiredMembershipType = "Basic")
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException(
                    "Session ID is required.");

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException(
                    "Workout type is required.");

            if (durationMinutes <= 0)
                throw new ArgumentException(
                    "Duration must be greater than 0.");

            if (capacity <= 0)
                throw new ArgumentException(
                    "Capacity must be greater than 0.");

            SessionId = sessionId;
            TrainerId = trainerId;
            Type = type;
            StartDateTime = startDateTime;
            DurationMinutes = durationMinutes;
            Capacity = capacity;

            Status = "Active";

            OccupiedPlaces = 0;

            RequiredMembershipType =
                NormalizeMembershipType(
                    requiredMembershipType);
        }

        public bool IsPremiumOnly()
        {
            return string.Equals(
                RequiredMembershipType,
                "Premium",
                StringComparison.OrdinalIgnoreCase);
        }

        public bool AddMember()
        {
            if (Status != "Active")
                return false;

            if (OccupiedPlaces >= Capacity)
                return false;

            OccupiedPlaces++;

            return true;
        }

        public bool RemoveMember()
        {
            if (OccupiedPlaces <= 0)
                return false;

            OccupiedPlaces--;

            return true;
        }

        public void Cancel()
        {
            Status = "Cancelled";
        }

        private static string NormalizeMembershipType(
            string membershipType)
        {
            if (string.Equals(
                    membershipType,
                    "Premium",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Premium";
            }

            return "Basic";
        }
    }
}