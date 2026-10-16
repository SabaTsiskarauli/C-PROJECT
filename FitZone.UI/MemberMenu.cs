using System;
using System.Linq;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;

namespace FitZone.UI
{
    public class MemberMenu
    {
        private readonly IMemberService _memberService;
        private readonly IMembershipService _membershipService;
        private readonly IBookingService _bookingService;
        private readonly IWorkoutSessionService _sessionService;
        private readonly IVisitService _visitService;

        public MemberMenu(
            IMemberService memberService,
            IMembershipService membershipService,
            IBookingService bookingService,
            IWorkoutSessionService sessionService,
            IVisitService visitService)
        {
            _memberService = memberService;
            _membershipService = membershipService;
            _bookingService = bookingService;
            _sessionService = sessionService;
            _visitService = visitService;
        }

        public void Show(Member member)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine(
                    "╔════════════════════════════════════╗");
                Console.WriteLine(
                    "║         FITZONE • MEMBER           ║");
                Console.WriteLine(
                    "╚════════════════════════════════════╝");
                Console.WriteLine();

                Console.WriteLine(
                    $"Welcome, {member.FullName}!");
                Console.WriteLine();

                Console.WriteLine(
                    "  1. My profile");
                Console.WriteLine(
                    "  2. My membership");
                Console.WriteLine(
                    "  3. Buy membership");
                Console.WriteLine(
                    "  4. Upgrade to Premium");
                Console.WriteLine(
                    "  5. Available sessions");
                Console.WriteLine(
                    "  6. My bookings");
                Console.WriteLine(
                    "  7. Book session");
                Console.WriteLine(
                    "  8. Cancel booking");
                Console.WriteLine(
                    "  9. My visits");
                Console.WriteLine(
                    "  0. Logout");

                Console.Write(
                    "\nSelect option: ");

                string choice =
                    Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ShowProfile(member);
                            break;

                        case "2":
                            ShowMembership(member);
                            break;

                        case "3":
                            BuyMembership(member);
                            break;

                        case "4":
                            UpgradeToPremium(member);
                            break;

                        case "5":
                            ShowSessions();
                            break;

                        case "6":
                            ShowBookings(member.UserId);
                            break;

                        case "7":
                            CreateBooking(member.UserId);
                            break;

                        case "8":
                            CancelBooking(member.UserId);
                            break;

                        case "9":
                            ShowVisits(member.UserId);
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine(
                                "Invalid option.");
                            Pause();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Error: {ex.Message}");
                    Pause();
                }
            }
        }

        private void ShowProfile(
            Member member)
        {
            Console.Clear();

            Console.WriteLine(
                "========== MY PROFILE ==========");

            // Use the latest saved data, not only the object created at login
            var savedMember =
                _memberService.GetMemberById(
                    member.UserId);

            if (savedMember != null)
            {
                member.MembershipId =
                    savedMember.MembershipId;
            }

            var memberships =
                _membershipService
                    .GetMemberMemberships(
                        member.UserId);

            Membership membership = null;

            if (!string.IsNullOrWhiteSpace(
                    member.MembershipId))
            {
                membership =
                    memberships.Find(
                        x => x.MembershipId ==
                             member.MembershipId);
            }

            // Fallback: membership exists, but MembershipId
            // was not saved in Member.json
            if (membership == null)
            {
                membership =
                    memberships
                        .Where(x =>
                            x.Status == "Active" &&
                            x.EndDate >= DateTime.Now)
                        .OrderByDescending(x => x.EndDate)
                        .FirstOrDefault();

                if (membership != null)
                {
                    member.MembershipId =
                        membership.MembershipId;

                    // repair the saved member record
                    _memberService.UpdateMember(
                        member);
                }
            }

            Console.WriteLine(
                $"ID: {member.UserId}");

            Console.WriteLine(
                $"Name: {member.FullName}");

            Console.WriteLine(
                $"Email: {member.Email}");

            Console.WriteLine(
                $"Membership ID: " +
                $"{membership?.MembershipId ?? "None"}");

            if (membership != null)
            {
                Console.WriteLine(
                    $"Membership: " +
                    $"{membership.Type}");

                Console.WriteLine(
                    $"Valid until: " +
                    $"{membership.EndDate:dd.MM.yyyy}");

                Console.WriteLine(
                    $"Remaining visits: " +
                    $"{membership.RemainingVisits?.ToString() ?? "Unlimited"}");

                Console.WriteLine(
                    $"Status: " +
                    $"{membership.Status}");
            }

            Pause();
        }

        private void ShowMembership(
            Member member)
        {
            Console.Clear();

            var memberships =
                _membershipService
                    .GetMemberMemberships(
                        member.UserId);

            Console.WriteLine(
                "========== MY MEMBERSHIP ==========");
            Console.WriteLine();

            if (memberships.Count == 0)
            {
                Console.WriteLine(
                    "No membership found.");
            }
            else
            {
                foreach (var membership in memberships)
                {
                    Console.WriteLine(
                        $"ID: {membership.MembershipId}");

                    Console.WriteLine(
                        $"Type: {membership.Type}");

                    Console.WriteLine(
                        $"Start: " +
                        $"{membership.StartDate:dd.MM.yyyy}");

                    Console.WriteLine(
                        $"End: " +
                        $"{membership.EndDate:dd.MM.yyyy}");

                    Console.WriteLine(
                        $"Visits: " +
                        $"{membership.RemainingVisits?.ToString() ?? "Unlimited"}");

                    Console.WriteLine(
                        $"Status: {membership.Status}");

                    Console.WriteLine();
                }
            }

            Console.WriteLine(
                "[0] Exit");

            Pause();
        }

        private void BuyMembership(
            Member member)
        {
            Console.Clear();

            Console.WriteLine(
                "========== BUY MEMBERSHIP ==========");
            Console.WriteLine();

            Console.WriteLine(
                "[1] Basic");
            Console.WriteLine(
                "    1 month");
            Console.WriteLine(
                "    8 visits");
            Console.WriteLine();

            Console.WriteLine(
                "[2] Premium");
            Console.WriteLine(
                "    1 month");
            Console.WriteLine(
                "    Unlimited visits");
            Console.WriteLine();

            Console.WriteLine(
                "[0] Exit");

            Console.WriteLine();

            Console.Write(
                "Choose membership: ");

            string choice =
                Console.ReadLine();

            if (choice == "0")
                return;

            string membershipType;

            if (choice == "1")
            {
                membershipType = "Basic";
            }
            else if (choice == "2")
            {
                membershipType = "Premium";
            }
            else
            {
                Console.WriteLine(
                    "Invalid membership type.");
                Pause();
                return;
            }

            var membership =
                _membershipService
                    .BuyMembership(
                        member.UserId,
                        membershipType);

            member.MembershipId =
                membership.MembershipId;

            Console.WriteLine();
            Console.WriteLine(
                "Membership selected successfully.");

            Console.WriteLine(
                $"Type: {membership.Type}");

            Console.WriteLine(
                $"Membership ID: " +
                $"{membership.MembershipId}");

            Console.WriteLine(
                $"Valid until: " +
                $"{membership.EndDate:dd.MM.yyyy}");

            Console.WriteLine(
                $"Visits: " +
                $"{membership.RemainingVisits?.ToString() ?? "Unlimited"}");

            Pause();
        }

        private void UpgradeToPremium(
            Member member)
        {
            Console.Clear();

            Console.WriteLine(
                "========== UPGRADE TO PREMIUM ==========");
            Console.WriteLine();

            var memberships =
                _membershipService
                    .GetMemberMemberships(
                        member.UserId);

            var activeMembership =
                memberships.Find(
                    x =>
                        x.Status == "Active" &&
                        x.EndDate >= DateTime.Now);

            if (activeMembership == null)
            {
                Console.WriteLine(
                    "You do not have an active membership.");

                Console.WriteLine();
                Console.WriteLine(
                    "Please buy Basic or Premium first.");

                Pause();
                return;
            }

            if (activeMembership.Type.Equals(
                    "Premium",
                    StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "You already have Premium membership.");

                Pause();
                return;
            }

            if (!activeMembership.Type.Equals(
                    "Basic",
                    StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "Only Basic membership can be upgraded.");

                Pause();
                return;
            }

            Console.WriteLine(
                "Current membership: Basic");

            Console.WriteLine(
                $"Valid until: " +
                $"{activeMembership.EndDate:dd.MM.yyyy}");

            Console.WriteLine(
                $"Remaining visits: " +
                $"{activeMembership.RemainingVisits}");

            Console.WriteLine();

            Console.WriteLine(
                "[1] Upgrade to Premium");

            Console.WriteLine(
                "[0] Exit");

            Console.Write(
                "\nChoose: ");

            string choice =
                Console.ReadLine();

            if (choice == "0")
                return;

            if (choice != "1")
            {
                Console.WriteLine(
                    "Invalid option.");
                Pause();
                return;
            }

            _membershipService
                .UpgradeToPremium(
                    member.UserId);

            Console.WriteLine();
            Console.WriteLine(
                "✓ Membership upgraded to Premium.");

            Console.WriteLine(
                "✓ Premium-only sessions are now available.");

            Console.WriteLine(
                "✓ Visits are now unlimited.");

            Pause();
        }

        private void ShowSessions()
        {
            Console.Clear();

            var sessions =
                _sessionService
                    .GetAvailableSessions();

            Console.WriteLine(
                "========== AVAILABLE SESSIONS ==========");
            Console.WriteLine();

            if (sessions.Count == 0)
            {
                Console.WriteLine(
                    "No available sessions.");
            }
            else
            {
                foreach (var session in sessions)
                {
                    Console.WriteLine(
                        $"{session.SessionId} | " +
                        $"{session.Type} | " +
                        $"{session.StartDateTime:dd.MM.yyyy HH:mm} | " +
                        $"Places: " +
                        $"{session.AvailablePlaces}/" +
                        $"{session.Capacity} | " +
                        $"Access: " +
                        $"{session.RequiredMembershipType}");
                }
            }

            Console.WriteLine();
            Console.WriteLine(
                "[0] Exit");

            Pause();
        }

        private void ShowBookings(
            int memberId)
        {
            Console.Clear();

            var bookings =
                _bookingService
                    .GetMemberBookings(memberId);

            Console.WriteLine(
                "========== MY BOOKINGS ==========");
            Console.WriteLine();

            if (bookings.Count == 0)
            {
                Console.WriteLine(
                    "No bookings found.");
            }
            else
            {
                foreach (var booking in bookings)
                {
                    Console.WriteLine(
                        $"{booking.BookingId} | " +
                        $"Session: {booking.SessionId} | " +
                        $"{booking.Status}");
                }
            }

            Console.WriteLine();
            Console.WriteLine(
                "[0] Exit");

            Pause();
        }

        private void CreateBooking(
            int memberId)
        {
            Console.Clear();

            Console.WriteLine(
                "========== BOOK SESSION ==========");
            Console.WriteLine();

            Console.WriteLine(
                "[0] Exit");
            Console.WriteLine();

            Console.Write(
                "Session ID: ");

            string sessionId =
                Console.ReadLine();

            if (sessionId == "0")
                return;

            _bookingService.CreateBooking(
                memberId,
                sessionId);

            Console.WriteLine();
            Console.WriteLine(
                "Booking created successfully.");

            Pause();
        }

        private void CancelBooking(
            int memberId)
        {
            Console.Clear();

            Console.WriteLine(
                "========== CANCEL BOOKING ==========");
            Console.WriteLine();

            // Only the member's own active bookings of future sessions
            var cancellable =
                _bookingService
                    .GetMemberBookings(memberId)
                    .Where(b => b.Status == "Active")
                    .Select(b => new
                    {
                        Booking = b,
                        Session = _sessionService
                            .GetSessionById(b.SessionId)
                    })
                    .Where(x =>
                        x.Session != null &&
                        x.Session.StartDateTime > DateTime.Now)
                    .OrderBy(x => x.Session.StartDateTime)
                    .ToList();

            if (cancellable.Count == 0)
            {
                Console.WriteLine(
                    "You have no active bookings " +
                    "that can be cancelled.");

                Pause();
                return;
            }

            for (int i = 0; i < cancellable.Count; i++)
            {
                var item = cancellable[i];

                Console.WriteLine(
                    $"[{i + 1}] {item.Booking.BookingId} | " +
                    $"{item.Session.SessionId} | " +
                    $"{item.Session.Type} | " +
                    $"{item.Session.StartDateTime:dd.MM.yyyy HH:mm} | " +
                    $"Access: " +
                    $"{item.Session.RequiredMembershipType}");
            }

            Console.WriteLine();
            Console.WriteLine(
                "[0] Exit");
            Console.WriteLine();

            int number;

            while (true)
            {
                Console.Write(
                    "Choose booking to cancel: ");

                string input =
                    Console.ReadLine();

                if (input == "0")
                    return;

                if (int.TryParse(input, out number) &&
                    number >= 1 &&
                    number <= cancellable.Count)
                {
                    break;
                }

                Console.WriteLine(
                    "Invalid choice. Enter a number " +
                    "from the list or 0 to exit.");
            }

            var selected =
                cancellable[number - 1];

            Console.Write(
                $"\nCancel booking {selected.Booking.BookingId} " +
                $"({selected.Session.Type}, " +
                $"{selected.Session.StartDateTime:dd.MM.yyyy HH:mm})? " +
                "(y/n, 0 = Exit): ");

            string confirmation =
                Console.ReadLine();

            if (confirmation == "0")
                return;

            if (confirmation?.Trim().ToLower() != "y")
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Operation cancelled.");

                Pause();
                return;
            }

            _bookingService.CancelBooking(
                selected.Booking.BookingId,
                memberId);

            Console.WriteLine();
            Console.WriteLine(
                "Booking cancelled successfully.");

            Pause();
        }

        private void ShowVisits(
            int memberId)
        {
            Console.Clear();

            var visits =
                _visitService
                    .GetMemberVisits(memberId);

            Console.WriteLine(
                "========== MY VISITS ==========");
            Console.WriteLine();

            if (visits.Count == 0)
            {
                Console.WriteLine(
                    "No visits found.");
            }
            else
            {
                foreach (var visit in visits)
                {
                    Console.WriteLine(
                        $"{visit.VisitId} | " +
                        $"{visit.VisitDate:dd.MM.yyyy HH:mm} | " +
                        $"{visit.Status}");
                }
            }

            Console.WriteLine();
            Console.WriteLine(
                "[0] Exit");

            Pause();
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine(
                "Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}