
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;


using System;
using System.Collections.Generic;
using System.Linq;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;
 
namespace FitZone.Services.Implementations
{
    public class VisitService : IVisitService
    {
        private readonly IVisitRepository _visitRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IWorkoutSessionRepository _sessionRepository;

        public VisitService(
            IVisitRepository visitRepository,
            IMemberRepository memberRepository,
            IMembershipRepository membershipRepository,
            IBookingRepository bookingRepository,
            IWorkoutSessionRepository sessionRepository)
        {
            _visitRepository = visitRepository;
            _memberRepository = memberRepository;
            _membershipRepository = membershipRepository;
            _bookingRepository = bookingRepository;
            _sessionRepository = sessionRepository;
        }

        public List<Visit> GetAllVisits()
        {
            ProcessFinishedSessions();

            return _visitRepository.GetVisits();
        }

        public Visit GetVisitById(string id)
        {
            return _visitRepository.GetVisitById(id);
        }

        public List<Visit> GetMemberVisits(int memberId)
        {
            ProcessFinishedSessions();

            return _visitRepository
                .GetVisitsByMemberId(memberId);
        }

        // Manual visit registration (manager menu).
        public void RegisterVisit(int memberId)
        {
            CreateVisit(memberId, null);
        }

        // Every active booking whose session has already ended is counted
        // as an attended visit: the visit is saved to Visit.json, one
        // remaining visit is used and the booking becomes "Completed".
        // If the member could not attend (no active membership or no
        // visits left) the booking becomes "Missed" and no visit is saved.
        // Returns the number of visits that were created.
        public int ProcessFinishedSessions()
        {
            var activeBookings =
                _bookingRepository
                    .GetBookings()
                    .Where(b => b.Status == "Active")
                    .ToList();

            int created = 0;

            foreach (var booking in activeBookings)
            {
                var session =
                    _sessionRepository.GetSessionById(
                        booking.SessionId);

                if (session == null ||
                    session.Status != "Active" ||
                    session.EndDateTime > DateTime.Now)
                {
                    continue;
                }

                var member =
                    _memberRepository.GetMemberById(
                        booking.MemberId);

                var membership =
                    member == null
                        ? null
                        : FindActiveMembership(booking.MemberId);

                if (membership == null || !membership.CanEnter())
                {
                    booking.MarkMissed();
                }
                else
                {
                    CreateVisit(
                        booking.MemberId,
                        session.StartDateTime);

                    booking.Complete();

                    created++;
                }

                _bookingRepository.UpdateBooking(booking);
            }

            return created;
        }

        public void DeleteVisit(string id)
        {
            var visit =
                _visitRepository.GetVisitById(id);

            if (visit == null)
                throw new InvalidOperationException(
                    "Visit not found.");

            _visitRepository.DeleteVisit(id);
        }

        private void CreateVisit(
            int memberId,
            DateTime? visitDate)
        {
            var member =
                _memberRepository.GetMemberById(memberId);

            if (member == null)
                throw new InvalidOperationException(
                    "Member not found.");

            var membership =
                FindActiveMembership(memberId);

            if (membership == null)
                throw new InvalidOperationException(
                    "Member does not have an active membership.");

            membership.RegisterVisit();

            string visitId = GenerateVisitId();

            var visit =
                visitDate.HasValue
                    ? new Visit(
                        visitId,
                        memberId,
                        visitDate.Value)
                    : new Visit(
                        visitId,
                        memberId);

            _membershipRepository.UpdateMembership(
                membership);

            _visitRepository.AddVisit(visit);
        }

        private Membership FindActiveMembership(int memberId)
        {
            return _membershipRepository
                .GetMembershipsByMemberId(memberId)
                .FirstOrDefault(
                    x =>
                        x.Status == "Active" &&
                        x.EndDate >= DateTime.Now);
        }

        private string GenerateVisitId()
        {
            var visits =
                _visitRepository.GetVisits();

            int maxId = 0;

            foreach (var visit in visits)
            {
                if (string.IsNullOrWhiteSpace(visit.VisitId))
                    continue;

                string numberPart =
                    new string(
                        visit.VisitId
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

            return "V" + (maxId + 1);
        }
    }
}

