using System;
using System.Collections.Generic;
using System.Linq;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;

namespace FitZone.Services.Implementations
{
    public class WorkoutSessionService : IWorkoutSessionService
    {
        private readonly IWorkoutSessionRepository _sessionRepository;
        private readonly ITrainerRepository _trainerRepository;
        private readonly IBookingRepository _bookingRepository;

        public WorkoutSessionService(
            IWorkoutSessionRepository sessionRepository,
            ITrainerRepository trainerRepository,
            IBookingRepository bookingRepository)
        {
            _sessionRepository = sessionRepository;
            _trainerRepository = trainerRepository;
            _bookingRepository = bookingRepository;
        }

        public List<WorkoutSession> GetAllSessions()
        {
            return _sessionRepository.GetSessions();
        }

        public WorkoutSession GetSessionById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return _sessionRepository.GetSessionById(id);
        }

        public List<WorkoutSession> GetSessionsByTrainer(int trainerId)
        {
            return _sessionRepository
                .GetSessionsByTrainerId(trainerId);
        }

        public List<WorkoutSession> GetAvailableSessions()
        {
            return _sessionRepository
                .GetSessions()
                .Where(x =>
                    x.Status == "Active" &&
                    x.StartDateTime > DateTime.Now &&
                    x.AvailablePlaces > 0)
                .OrderBy(x => x.StartDateTime)
                .ToList();
        }

        public List<WorkoutSession> SearchSessions(
            DateTime date,
            string workoutType,
            int? trainerId)
        {
            var query = _sessionRepository
                .GetSessions()
                .AsEnumerable();

            query = query.Where(
                x => x.StartDateTime.Date == date.Date);

            if (!string.IsNullOrWhiteSpace(workoutType))
            {
                query = query.Where(
                    x => x.Type != null &&
                         x.Type.Equals(
                             workoutType,
                             StringComparison.OrdinalIgnoreCase));
            }

            if (trainerId.HasValue)
            {
                query = query.Where(
                    x => x.TrainerId == trainerId.Value);
            }

            return query
                .OrderBy(x => x.StartDateTime)
                .ToList();
        }

        public void CreateSession(WorkoutSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            if (string.IsNullOrWhiteSpace(session.SessionId))
                throw new ArgumentException(
                    "Session ID is required.");

            if (session.StartDateTime <= DateTime.Now)
                throw new ArgumentException(
                    "Session must be in the future.");

            if (session.DurationMinutes <= 0)
                throw new ArgumentException(
                    "Duration must be greater than zero.");

            if (session.Capacity <= 0)
                throw new ArgumentException(
                    "Capacity must be greater than zero.");

            if (session.OccupiedPlaces < 0)
                throw new ArgumentException(
                    "Occupied places cannot be negative.");

            if (session.OccupiedPlaces > session.Capacity)
                throw new ArgumentException(
                    "Occupied places cannot be greater than capacity.");

            if (!IsValidMembershipType(
                    session.RequiredMembershipType))
            {
                throw new ArgumentException(
                    "Membership type must be Basic or Premium.");
            }

            if (_sessionRepository.GetSessionById(
                    session.SessionId) != null)
            {
                throw new InvalidOperationException(
                    "Session with this ID already exists.");
            }

            var trainer =
                _trainerRepository.GetTrainerById(
                    session.TrainerId);

            if (trainer == null)
                throw new InvalidOperationException(
                    "Trainer not found.");

            CheckTrainerOverlap(session);

            _sessionRepository.AddSession(session);
        }

        public void UpdateSession(WorkoutSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            if (string.IsNullOrWhiteSpace(session.SessionId))
                throw new ArgumentException(
                    "Session ID is required.");

            var existing =
                _sessionRepository.GetSessionById(
                    session.SessionId);

            if (existing == null)
                throw new InvalidOperationException(
                    "Session not found.");

            if (session.StartDateTime <= DateTime.Now)
                throw new ArgumentException(
                    "Session must be in the future.");

            if (session.DurationMinutes <= 0)
                throw new ArgumentException(
                    "Duration must be greater than zero.");

            if (session.Capacity <= 0)
                throw new ArgumentException(
                    "Capacity must be greater than zero.");

            if (session.OccupiedPlaces < 0)
                throw new ArgumentException(
                    "Occupied places cannot be negative.");

            if (session.Capacity < session.OccupiedPlaces)
                throw new ArgumentException(
                    "Capacity cannot be lower than occupied places.");

            if (!IsValidMembershipType(
                    session.RequiredMembershipType))
            {
                throw new ArgumentException(
                    "Membership type must be Basic or Premium.");
            }

            var trainer =
                _trainerRepository.GetTrainerById(
                    session.TrainerId);

            if (trainer == null)
                throw new InvalidOperationException(
                    "Trainer not found.");

            CheckTrainerOverlap(
                session,
                session.SessionId);

            _sessionRepository.UpdateSession(session);
        }

        public void DeleteSession(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(
                    "Session ID is required.");

            var session =
                _sessionRepository.GetSessionById(id);

            if (session == null)
                throw new InvalidOperationException(
                    "Session not found.");

            if (session.StartDateTime <= DateTime.Now)
                throw new InvalidOperationException(
                    "Past session cannot be deleted.");

            var bookings =
                _bookingRepository
                    .GetBookings()
                    .Where(x =>
                        x.SessionId == id &&
                        x.Status == "Active")
                    .ToList();

            foreach (var booking in bookings)
            {
                booking.Cancel();

                _bookingRepository.UpdateBooking(
                    booking);
            }

            _sessionRepository.DeleteSession(id);
        }

        public void CancelSession(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(
                    "Session ID is required.");

            var session =
                _sessionRepository.GetSessionById(id);

            if (session == null)
                throw new InvalidOperationException(
                    "Session not found.");

            if (session.Status == "Cancelled")
                throw new InvalidOperationException(
                    "Session is already cancelled.");

            if (session.StartDateTime <= DateTime.Now)
                throw new InvalidOperationException(
                    "Past session cannot be cancelled.");

            var bookings =
                _bookingRepository
                    .GetBookings()
                    .Where(x =>
                        x.SessionId == id &&
                        x.Status == "Active")
                    .ToList();

            session.Cancel();

            foreach (var booking in bookings)
            {
                booking.Cancel();

                if (session.OccupiedPlaces > 0)
                    session.RemoveMember();

                _bookingRepository.UpdateBooking(
                    booking);
            }

            _sessionRepository.UpdateSession(session);
        }

        private void CheckTrainerOverlap(
            WorkoutSession session,
            string ignoredSessionId = null)
        {
            var trainerSessions =
                _sessionRepository
                    .GetSessionsByTrainerId(
                        session.TrainerId);

            foreach (var existing in trainerSessions)
            {
                if (existing.Status == "Cancelled")
                    continue;

                if (existing.SessionId == ignoredSessionId)
                    continue;

                bool overlaps =
                    session.StartDateTime <
                        existing.EndDateTime
                    &&
                    session.EndDateTime >
                        existing.StartDateTime;

                if (overlaps)
                {
                    throw new InvalidOperationException(
                        "Trainer already has another session at this time.");
                }
            }
        }

        private bool IsValidMembershipType(
            string membershipType)
        {
            if (string.IsNullOrWhiteSpace(
                    membershipType))
            {
                return false;
            }

            return membershipType.Equals(
                       "Basic",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   membershipType.Equals(
                       "Premium",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}