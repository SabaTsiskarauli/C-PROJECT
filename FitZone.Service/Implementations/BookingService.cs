using System;
using System.Collections.Generic;
using System.Linq;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;

namespace FitZone.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IWorkoutSessionRepository _sessionRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IMemberRepository memberRepository,
            IMembershipRepository membershipRepository,
            IWorkoutSessionRepository sessionRepository)
        {
            _bookingRepository = bookingRepository;
            _memberRepository = memberRepository;
            _membershipRepository = membershipRepository;
            _sessionRepository = sessionRepository;
        }

        public List<Booking> GetAllBookings()
        {
            return _bookingRepository.GetBookings();
        }

        public Booking GetBookingById(string id)
        {
            return _bookingRepository.GetBookingById(id);
        }

        public List<Booking> GetMemberBookings(int memberId)
        {
            return _bookingRepository
                .GetBookings()
                .Where(x => x.MemberId == memberId)
                .ToList();
        }

        public bool CanBook(int memberId, string sessionId)
        {
            var member =
                _memberRepository.GetMemberById(memberId);

            if (member == null)
                return false;

            var session =
                _sessionRepository.GetSessionById(sessionId);

            if (session == null)
                return false;

            if (session.Status != "Active")
                return false;

            if (session.StartDateTime <= DateTime.Now)
                return false;

            if (session.AvailablePlaces <= 0)
                return false;

            var memberships =
                _membershipRepository.GetMembershipsByMemberId(
                    memberId);

            var membership =
                memberships.FirstOrDefault(
                    x => x.Status == "Active" &&
                         x.EndDate >= DateTime.Now);

            if (membership == null)
                return false;

            if (!membership.CanBook(session))
                return false;

            var duplicate =
                _bookingRepository
                    .GetBookings()
                    .Any(x =>
                        x.MemberId == memberId &&
                        x.SessionId == sessionId &&
                        x.Status == "Active");

            if (duplicate)
                return false;

            var activeBookings =
                _bookingRepository
                    .GetBookings()
                    .Where(x =>
                        x.MemberId == memberId &&
                        x.Status == "Active")
                    .ToList();

            foreach (var booking in activeBookings)
            {
                var existingSession =
                    _sessionRepository.GetSessionById(
                        booking.SessionId);

                if (existingSession == null)
                    continue;

                bool overlaps =
                    session.StartDateTime <
                        existingSession.EndDateTime
                    &&
                    session.EndDateTime >
                        existingSession.StartDateTime;

                if (overlaps)
                    return false;
            }

            return true;
        }

        public void CreateBooking(
            int memberId,
            string sessionId)
        {
            if (!CanBook(memberId, sessionId))
            {
                throw new InvalidOperationException(
                    "Member cannot book this session.");
            }

            var session =
                _sessionRepository.GetSessionById(sessionId);

            if (session == null)
                throw new InvalidOperationException(
                    "Session not found.");

            if (!session.AddMember())
                throw new InvalidOperationException(
                    "No free places available.");

            string bookingId = GenerateBookingId();

            var booking =
                new Booking(
                    bookingId,
                    memberId,
                    sessionId);

            _bookingRepository.AddBooking(booking);

            _sessionRepository.UpdateSession(session);
        }

        public void TrainerCreateBooking(
    int trainerId,
    int memberId,
    string sessionId)
        {
            var session =
                _sessionRepository.GetSessionById(sessionId);

            if (session == null)
            {
                throw new InvalidOperationException(
                    "Session not found."
                );
            }

            if (session.TrainerId != trainerId)
            {
                throw new UnauthorizedAccessException(
                    "Trainer can manage only their own sessions."
                );
            }

            CreateBooking(
                memberId,
                sessionId
            );
        }

        public void TrainerCancelBooking(
            int trainerId,
            string bookingId)
        {
            var booking =
                _bookingRepository.GetBookingById(
                    bookingId
                );

            if (booking == null)
            {
                throw new InvalidOperationException(
                    "Booking not found."
                );
            }

            var session =
                _sessionRepository.GetSessionById(
                    booking.SessionId
                );

            if (session == null)
            {
                throw new InvalidOperationException(
                    "Session not found."
                );
            }

            if (session.TrainerId != trainerId)
            {
                throw new UnauthorizedAccessException(
                    "Trainer can manage only their own sessions."
                );
            }

            if (booking.Status != "Active")
            {
                throw new InvalidOperationException(
                    "Booking is already cancelled."
                );
            }

            if (session.StartDateTime <= DateTime.Now)
            {
                throw new InvalidOperationException(
                    "Past booking cannot be cancelled."
                );
            }

            booking.Cancel();

            session.RemoveMember();

            _bookingRepository.UpdateBooking(
                booking
            );

            _sessionRepository.UpdateSession(
                session
            );
        }

        public void CancelBooking(
            string bookingId,
            int memberId)
        {
            var booking =
                _bookingRepository.GetBookingById(bookingId);

            if (booking == null)
                throw new InvalidOperationException(
                    "Booking not found.");

            if (booking.MemberId != memberId)
                throw new UnauthorizedAccessException(
                    "You can cancel only your own booking.");

            CancelBookingInternal(booking);
        }

        public void ManagerCancelBooking(string bookingId)
        {
            var booking =
                _bookingRepository.GetBookingById(bookingId);

            if (booking == null)
                throw new InvalidOperationException(
                    "Booking not found.");

            CancelBookingInternal(booking);
        }

        private void CancelBookingInternal(Booking booking)
        {
            if (booking.Status != "Active")
                throw new InvalidOperationException(
                    "Booking is already cancelled.");

            var session =
                _sessionRepository.GetSessionById(
                    booking.SessionId);

            if (session == null)
                throw new InvalidOperationException(
                    "Session not found.");

            if (session.StartDateTime <= DateTime.Now)
                throw new InvalidOperationException(
                    "Past booking cannot be cancelled.");

            booking.Cancel();

            session.RemoveMember();

            _bookingRepository.UpdateBooking(booking);
            _sessionRepository.UpdateSession(session);
        }

        private string GenerateBookingId()
        {
            var bookings =
                _bookingRepository.GetBookings();

            int maxId = 800;

            foreach (var booking in bookings)
            {
                if (string.IsNullOrWhiteSpace(booking.BookingId))
                    continue;

                string numberPart =
                    new string(
                        booking.BookingId
                            .Where(char.IsDigit)
                            .ToArray());

                if (int.TryParse(
                    numberPart,
                    out int number))
                {
                    if (number > maxId)
                        maxId = number;
                }
            }

            return "B" + (maxId + 1);
        }
    }
}