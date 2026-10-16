using System;
using System.Collections.Generic;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;

namespace FitZone.UI
{
    public class ManagerMenu
    {
        private readonly IMemberService _memberService;
        private readonly ITrainerService _trainerService;
        private readonly IManagerService _managerService;
        private readonly IMembershipService _membershipService;
        private readonly IBookingService _bookingService;
        private readonly IWorkoutSessionService _sessionService;
        private readonly IVisitService _visitService;

        public ManagerMenu(
            IMemberService memberService,
            ITrainerService trainerService,
            IManagerService managerService,
            IMembershipService membershipService,
            IBookingService bookingService,
            IWorkoutSessionService sessionService,
            IVisitService visitService)
        {
            _memberService = memberService;
            _trainerService = trainerService;
            _managerService = managerService;
            _membershipService = membershipService;
            _bookingService = bookingService;
            _sessionService = sessionService;
            _visitService = visitService;
        }

        public void Show(Manager manager)
        {
            while (true)
            {
                Console.Clear();

                PrintHeader("FITZONE • MANAGER PANEL");

                Console.WriteLine(
                    $"  Welcome, {manager.FullName}!");
                Console.WriteLine();

                Console.WriteLine(
                    "  ┌──────────────────────────────────────────┐");
                Console.WriteLine(
                    "  │              MEMBERS                     │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │  1. View members                         │");
                Console.WriteLine(
                    "  │  2. Add member                           │");
                Console.WriteLine(
                    "  │  3. Update member                        │");
                Console.WriteLine(
                    "  │  4. Delete member                        │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │              TRAINERS                    │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │  5. View trainers                        │");
                Console.WriteLine(
                    "  │  6. Add trainer                          │");
                Console.WriteLine(
                    "  │  7. Update trainer                       │");
                Console.WriteLine(
                    "  │  8. Delete trainer                       │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │              SESSIONS                    │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │  9. View sessions                        │");
                Console.WriteLine(
                    "  │ 10. Create session                       │");
                Console.WriteLine(
                    "  │ 11. Update session                       │");
                Console.WriteLine(
                    "  │ 12. Delete session                       │");
                Console.WriteLine(
                    "  │ 13. Cancel session                       │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │             MEMBERSHIPS                  │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │ 14. Purchase membership                  │");
                Console.WriteLine(
                    "  │ 15. Renew membership                     │");
                Console.WriteLine(
                    "  │ 16. Expire membership                    │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │              BOOKINGS                    │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │ 17. Create booking for member            │");
                Console.WriteLine(
                    "  │ 18. Cancel member booking                │");
                Console.WriteLine(
                    "  │ 19. View bookings                        │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │               VISITS                     │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │ 20. Register member visit                │");
                Console.WriteLine(
                    "  │ 21. View visits                          │");
                Console.WriteLine(
                    "  ├──────────────────────────────────────────┤");
                Console.WriteLine(
                    "  │ 22. View statistics                      │");
                Console.WriteLine(
                    "  │  0. Logout                               │");
                Console.WriteLine(
                    "  └──────────────────────────────────────────┘");

                Console.Write(
                    "\n  Select option: ");

                string choice =
                    Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewMembers();
                            break;

                        case "2":
                            AddMember();
                            break;

                        case "3":
                            UpdateMember();
                            break;

                        case "4":
                            DeleteMember();
                            break;

                        case "5":
                            ViewTrainers();
                            break;

                        case "6":
                            AddTrainer();
                            break;

                        case "7":
                            UpdateTrainer();
                            break;

                        case "8":
                            DeleteTrainer();
                            break;

                        case "9":
                            ViewSessions();
                            break;

                        case "10":
                            CreateSession();
                            break;

                        case "11":
                            UpdateSession();
                            break;

                        case "12":
                            DeleteSession();
                            break;

                        case "13":
                            CancelSession();
                            break;

                        case "14":
                            PurchaseMembership();
                            break;

                        case "15":
                            RenewMembership();
                            break;

                        case "16":
                            ExpireMembership();
                            break;

                        case "17":
                            CreateBooking();
                            break;

                        case "18":
                            CancelBooking();
                            break;

                        case "19":
                            ViewBookings();
                            break;

                        case "20":
                            RegisterVisit();
                            break;

                        case "21":
                            ViewVisits();
                            break;

                        case "22":
                            ViewStatistics();
                            break;

                        case "0":
                            return;

                        default:
                            Error("Invalid option.");
                            Pause();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                    Pause();
                }
            }
        }

        private void ViewMembers()
        {
            Console.Clear();
            PrintHeader("MEMBERS");

            var members =
                _memberService.GetAllMembers();

            if (members.Count == 0)
            {
                Info("No members found.");
                Pause();
                return;
            }

            foreach (var member in members)
            {
                Console.WriteLine(
                    $"  ID: {member.UserId} | " +
                    $"Name: {member.FullName} | " +
                    $"Email: {member.Email} | " +
                    $"Membership: " +
                    $"{member.MembershipId ?? "None"}");
            }

            Pause();
        }

        private void AddMember()
        {
            Console.Clear();
            PrintHeader("ADD MEMBER");

            int id;

            if (!TryReadInt(
                    "Member ID: ",
                    out id))
                return;

            string name =
                ReadRequired(
                    "Full name: ");

            if (name == null)
                return;

            string email =
                ReadRequired(
                    "Email: ");

            if (email == null)
                return;

            string password =
                ReadRequired(
                    "Password: ");

            if (password == null)
                return;

            string membershipId =
                ReadOptional(
                    "Membership ID (optional): ");

            if (membershipId == "0")
                return;

            var member =
                new Member(
                    id,
                    name,
                    email,
                    password,
                    membershipId);

            _memberService.RegisterMember(
                member);

            Success(
                "Member added successfully.");

            Pause();
        }

        private void UpdateMember()
        {
            Console.Clear();
            PrintHeader("UPDATE MEMBER");

            int id;

            if (!TryReadInt(
                    "Member ID: ",
                    out id))
                return;

            var member =
                _memberService.GetMemberById(id);

            if (member == null)
            {
                Error("Member not found.");
                Pause();
                return;
            }

            string name =
                ReadOptional(
                    $"Full name [{member.FullName}]: ");

            if (name == "0")
                return;

            string email =
                ReadOptional(
                    $"Email [{member.Email}]: ");

            if (email == "0")
                return;

            string password =
                ReadOptional(
                    "Password [unchanged]: ");

            if (password == "0")
                return;

            string membershipId =
                ReadOptional(
                    $"Membership ID [{member.MembershipId}]: ");

            if (membershipId == "0")
                return;

            if (!string.IsNullOrWhiteSpace(name))
                member.FullName = name;

            if (!string.IsNullOrWhiteSpace(email))
                member.Email = email;

            if (!string.IsNullOrWhiteSpace(password))
                member.Password = password;

            if (!string.IsNullOrWhiteSpace(membershipId))
                member.MembershipId = membershipId;

            _memberService.UpdateMember(
                member);

            Success("Member updated successfully.");

            Pause();
        }

        private void DeleteMember()
        {
            Console.Clear();
            PrintHeader("DELETE MEMBER");

            int id;

            if (!TryReadInt(
                    "Member ID: ",
                    out id))
                return;

            var member =
                _memberService.GetMemberById(id);

            if (member == null)
            {
                Error("Member not found.");
                Pause();
                return;
            }

            Console.Write(
                $"Delete {member.FullName}? (y/n, 0 = Exit): ");

            string confirmation =
                Console.ReadLine();

            if (confirmation == "0")
                return;

            if (confirmation?.ToLower() == "y")
            {
                _memberService.DeleteMember(id);
                Success("Member deleted.");
            }
            else
            {
                Info("Operation cancelled.");
            }

            Pause();
        }

        private void ViewTrainers()
        {
            Console.Clear();
            PrintHeader("TRAINERS");

            var trainers =
                _trainerService.GetAllTrainers();

            if (trainers.Count == 0)
            {
                Info("No trainers found.");
                Pause();
                return;
            }

            foreach (var trainer in trainers)
            {
                Console.WriteLine(
                    $"  ID: {trainer.TrainerId} | " +
                    $"Name: {trainer.FullName} | " +
                    $"Specialization: " +
                    $"{trainer.Specialization} | " +
                    $"Workout: {trainer.WorkoutType}");
            }

            Pause();
        }

        private void AddTrainer()
        {
            Console.Clear();
            PrintHeader("ADD TRAINER");

            int id;

            if (!TryReadInt(
                    "Trainer ID: ",
                    out id))
                return;

            string name =
                ReadRequired("Full name: ");

            if (name == null)
                return;

            string email =
                ReadRequired("Email: ");

            if (email == null)
                return;

            string password =
                ReadRequired("Password: ");

            if (password == null)
                return;

            string specialization =
                ReadRequired("Specialization: ");

            if (specialization == null)
                return;

            string workoutType =
                ReadRequired("Workout type: ");

            if (workoutType == null)
                return;

            var trainer =
                new Trainer(
                    id,
                    name,
                    email,
                    password,
                    specialization,
                    workoutType);

            _trainerService.AddTrainer(
                trainer);

            Success("Trainer added successfully.");

            Pause();
        }

        private void UpdateTrainer()
        {
            Console.Clear();
            PrintHeader("UPDATE TRAINER");

            int id;

            if (!TryReadInt(
                    "Trainer ID: ",
                    out id))
                return;

            var trainer =
                _trainerService.GetTrainerById(id);

            if (trainer == null)
            {
                Error("Trainer not found.");
                Pause();
                return;
            }

            string name =
                ReadOptional(
                    $"Name [{trainer.FullName}]: ");

            if (name == "0")
                return;

            string email =
                ReadOptional(
                    $"Email [{trainer.Email}]: ");

            if (email == "0")
                return;

            string password =
                ReadOptional(
                    "Password [unchanged]: ");

            if (password == "0")
                return;

            string specialization =
                ReadOptional(
                    $"Specialization [{trainer.Specialization}]: ");

            if (specialization == "0")
                return;

            string workoutType =
                ReadOptional(
                    $"Workout type [{trainer.WorkoutType}]: ");

            if (workoutType == "0")
                return;

            if (!string.IsNullOrWhiteSpace(name))
                trainer.FullName = name;

            if (!string.IsNullOrWhiteSpace(email))
                trainer.Email = email;

            if (!string.IsNullOrWhiteSpace(password))
                trainer.Password = password;

            if (!string.IsNullOrWhiteSpace(specialization))
                trainer.Specialization =
                    specialization;

            if (!string.IsNullOrWhiteSpace(workoutType))
                trainer.WorkoutType =
                    workoutType;

            _trainerService.UpdateTrainer(
                trainer);

            Success("Trainer updated.");

            Pause();
        }

        private void DeleteTrainer()
        {
            Console.Clear();
            PrintHeader("DELETE TRAINER");

            int id;

            if (!TryReadInt(
                    "Trainer ID: ",
                    out id))
                return;

            var trainer =
                _trainerService.GetTrainerById(id);

            if (trainer == null)
            {
                Error("Trainer not found.");
                Pause();
                return;
            }

            Console.Write(
                $"Delete trainer {trainer.FullName}? " +
                "(y/n, 0 = Exit): ");

            string confirmation =
                Console.ReadLine();

            if (confirmation == "0")
                return;

            if (confirmation?.ToLower() == "y")
            {
                _trainerService.DeleteTrainer(id);
                Success("Trainer deleted.");
            }
            else
            {
                Info("Operation cancelled.");
            }

            Pause();
        }

        private void ViewSessions()
        {
            Console.Clear();
            PrintHeader("WORKOUT SESSIONS");

            var sessions =
                _sessionService.GetAllSessions();

            if (sessions.Count == 0)
            {
                Info("No sessions found.");
                Pause();
                return;
            }

            foreach (var session in sessions)
            {
                Console.WriteLine(
                    $"  ID: {session.SessionId} | " +
                    $"Trainer: {session.TrainerId} | " +
                    $"Type: {session.Type} | " +
                    $"Start: " +
                    $"{session.StartDateTime:dd.MM.yyyy HH:mm} | " +
                    $"Duration: " +
                    $"{session.DurationMinutes} min | " +
                    $"Places: " +
                    $"{session.AvailablePlaces}/" +
                    $"{session.Capacity} | " +
                    $"Access: " +
                    $"{session.RequiredMembershipType} | " +
                    $"Status: {session.Status}");
            }

            Pause();
        }

        private void CreateSession()
        {
            Console.Clear();
            PrintHeader("CREATE SESSION");

            Console.WriteLine(
                "  Enter 0 at any step to exit.");
            Console.WriteLine();

            string id =
                ReadRequired("Session ID: ");

            if (id == null)
                return;

            int trainerId;

            if (!TryReadInt(
                    "Trainer ID: ",
                    out trainerId))
                return;

            var trainer =
                _trainerService.GetTrainerById(
                    trainerId);

            if (trainer == null)
            {
                Error("Trainer not found.");
                Pause();
                return;
            }

            string type =
                ReadRequired("Workout type: ");

            if (type == null)
                return;

            DateTime start;

            if (!TryReadDateTime(
                    "Start date/time (dd.MM.yyyy HH:mm): ",
                    out start))
                return;

            if (start <= DateTime.Now)
            {
                Error(
                    "Session must be in the future.");
                Pause();
                return;
            }

            int duration;

            if (!TryReadInt(
                    "Duration (minutes): ",
                    out duration))
                return;

            if (duration <= 0)
            {
                Error(
                    "Duration must be greater than 0.");
                Pause();
                return;
            }

            int capacity;

            if (!TryReadInt(
                    "Capacity: ",
                    out capacity))
                return;

            if (capacity <= 0)
            {
                Error(
                    "Capacity must be greater than 0.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine(
                "  Membership access:");
            Console.WriteLine(
                "  [1] Basic + Premium");
            Console.WriteLine(
                "  [2] Premium only");
            Console.WriteLine(
                "  [0] Exit");

            Console.Write(
                "\n  Choose: ");

            string accessChoice =
                Console.ReadLine();

            if (accessChoice == "0")
                return;

            string requiredMembershipType;

            if (accessChoice == "1")
            {
                requiredMembershipType = "Basic";
            }
            else if (accessChoice == "2")
            {
                requiredMembershipType = "Premium";
            }
            else
            {
                Error(
                    "Invalid membership access.");
                Pause();
                return;
            }

            var session =
                new WorkoutSession(
                    id,
                    trainerId,
                    type,
                    start,
                    duration,
                    capacity,
                    requiredMembershipType);

            _sessionService.CreateSession(
                session);

            Success(
                $"Session created. Access: " +
                $"{requiredMembershipType}");

            Pause();
        }

        private void UpdateSession()
        {
            Console.Clear();
            PrintHeader("UPDATE SESSION");

            string id =
                ReadRequired("Session ID: ");

            if (id == null)
                return;

            var session =
                _sessionService.GetSessionById(id);

            if (session == null)
            {
                Error("Session not found.");
                Pause();
                return;
            }

            string type =
                ReadOptional(
                    $"Workout type [{session.Type}]: ");

            if (type == "0")
                return;

            if (!string.IsNullOrWhiteSpace(type))
                session.Type = type;

            Console.WriteLine();
            Console.WriteLine(
                $"  Current access: " +
                $"{session.RequiredMembershipType}");

            Console.WriteLine(
                "  [1] Basic + Premium");
            Console.WriteLine(
                "  [2] Premium only");
            Console.WriteLine(
                "  [0] Keep current / Exit");

            Console.Write(
                "  Access: ");

            string access =
                Console.ReadLine();

            if (access == "0")
            {
                _sessionService.UpdateSession(
                    session);

                Success("Session updated.");

                Pause();
                return;
            }

            if (access == "1")
            {
                session.RequiredMembershipType =
                    "Basic";
            }
            else if (access == "2")
            {
                session.RequiredMembershipType =
                    "Premium";
            }
            else
            {
                Error("Invalid access option.");
                Pause();
                return;
            }

            _sessionService.UpdateSession(
                session);

            Success("Session updated.");

            Pause();
        }

        private void DeleteSession()
        {
            Console.Clear();
            PrintHeader("DELETE SESSION");

            string id =
                ReadRequired("Session ID: ");

            if (id == null)
                return;

            _sessionService.DeleteSession(id);

            Success("Session deleted.");

            Pause();
        }

        private void CancelSession()
        {
            Console.Clear();
            PrintHeader("CANCEL SESSION");

            string id =
                ReadRequired("Session ID: ");

            if (id == null)
                return;

            _sessionService.CancelSession(id);

            Success(
                "Session cancelled. " +
                "Active bookings were cancelled.");

            Pause();
        }

        private void PurchaseMembership()
        {
            Console.Clear();
            PrintHeader("PURCHASE MEMBERSHIP");

            int memberId;

            if (!TryReadInt(
                    "Member ID: ",
                    out memberId))
                return;

            var member =
                _memberService.GetMemberById(
                    memberId);

            if (member == null)
            {
                Error("Member not found.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  [1] Basic");
            Console.WriteLine("  [2] Premium");
            Console.WriteLine("  [0] Exit");

            Console.Write(
                "\n  Membership type: ");

            string type =
                Console.ReadLine();

            if (type == "0")
                return;

            string membershipType;

            if (type == "1")
                membershipType = "Basic";
            else if (type == "2")
                membershipType = "Premium";
            else
            {
                Error(
                    "Invalid membership type.");
                Pause();
                return;
            }

            var membership =
                _membershipService.BuyMembership(
                    memberId,
                    membershipType);

            Success(
                $"Membership purchased: " +
                $"{membership.Type}");

            Console.WriteLine(
                $"  ID: {membership.MembershipId}");

            Console.WriteLine(
                $"  Valid until: " +
                $"{membership.EndDate:dd.MM.yyyy}");

            Pause();
        }

        private void RenewMembership()
        {
            Console.Clear();
            PrintHeader("RENEW MEMBERSHIP");

            string membershipId =
                ReadRequired("Membership ID: ");

            if (membershipId == null)
                return;

            DateTime newEndDate;

            if (!TryReadDateTime(
                    "New end date (dd.MM.yyyy HH:mm): ",
                    out newEndDate))
                return;

            Console.Write(
                "  Remaining visits " +
                "(-1 = unlimited, 0 = Exit): ");

            string visitsInput =
                Console.ReadLine();

            if (visitsInput == "0")
                return;

            int? visits = null;

            if (visitsInput != "-1")
            {
                if (int.TryParse(
                        visitsInput,
                        out int parsed))
                {
                    visits = parsed;
                }
                else
                {
                    Error(
                        "Invalid visit count.");
                    Pause();
                    return;
                }
            }

            _membershipService.RenewMembership(
                membershipId,
                newEndDate,
                visits);

            Success("Membership renewed.");

            Pause();
        }

        private void ExpireMembership()
        {
            Console.Clear();
            PrintHeader("EXPIRE MEMBERSHIP");

            string id =
                ReadRequired("Membership ID: ");

            if (id == null)
                return;

            _membershipService.ExpireMembership(id);

            Success("Membership expired.");

            Pause();
        }

        private void CreateBooking()
        {
            Console.Clear();
            PrintHeader(
                "CREATE BOOKING FOR MEMBER");

            int memberId;

            if (!TryReadInt(
                    "Member ID: ",
                    out memberId))
                return;

            string sessionId =
                ReadRequired("Session ID: ");

            if (sessionId == null)
                return;

            if (!_bookingService.CanBook(
                    memberId,
                    sessionId))
            {
                Error(
                    "Member cannot book this session.");
                Pause();
                return;
            }

            _bookingService.CreateBooking(
                memberId,
                sessionId);

            Success(
                "Booking created successfully.");

            Pause();
        }

        private void CancelBooking()
        {
            Console.Clear();
            PrintHeader(
                "CANCEL MEMBER BOOKING");

            string bookingId =
                ReadRequired("Booking ID: ");

            if (bookingId == null)
                return;

            _bookingService.ManagerCancelBooking(
                bookingId);

            Success(
                "Booking cancelled successfully.");

            Pause();
        }

        private void ViewBookings()
        {
            Console.Clear();
            PrintHeader("BOOKINGS");

            var bookings =
                _bookingService.GetAllBookings();

            if (bookings.Count == 0)
            {
                Info("No bookings found.");
                Pause();
                return;
            }

            foreach (var booking in bookings)
            {
                Console.WriteLine(
                    $"  ID: {booking.BookingId} | " +
                    $"Member: {booking.MemberId} | " +
                    $"Session: {booking.SessionId} | " +
                    $"Created: " +
                    $"{booking.CreatedAt:dd.MM.yyyy HH:mm} | " +
                    $"Status: {booking.Status}");
            }

            Pause();
        }

        private void RegisterVisit()
        {
            Console.Clear();
            PrintHeader(
                "REGISTER MEMBER VISIT");

            int memberId;

            if (!TryReadInt(
                    "Member ID: ",
                    out memberId))
                return;

            _visitService.RegisterVisit(
                memberId);

            Success(
                "Visit registered successfully.");

            Pause();
        }

        private void ViewVisits()
        {
            Console.Clear();
            PrintHeader("VISITS");

            var visits =
                _visitService.GetAllVisits();

            if (visits.Count == 0)
            {
                Info("No visits found.");
                Pause();
                return;
            }

            foreach (var visit in visits)
            {
                Console.WriteLine(
                    $"  ID: {visit.VisitId} | " +
                    $"Member: {visit.MemberId} | " +
                    $"Date: " +
                    $"{visit.VisitDate:dd.MM.yyyy HH:mm} | " +
                    $"Status: {visit.Status}");
            }

            Pause();
        }

        private void ViewStatistics()
        {
            Console.Clear();
            PrintHeader("FITZONE STATISTICS");

            var members =
                _memberService.GetAllMembers();

            var trainers =
                _trainerService.GetAllTrainers();

            var sessions =
                _sessionService.GetAllSessions();

            var bookings =
                _bookingService.GetAllBookings();

            var visits =
                _visitService.GetAllVisits();

            var memberships =
                _membershipService.GetAllMemberships();

            Console.WriteLine(
                $"  Members        : {members.Count}");

            Console.WriteLine(
                $"  Trainers       : {trainers.Count}");

            Console.WriteLine(
                $"  Sessions       : {sessions.Count}");

            Console.WriteLine(
                $"  Bookings       : {bookings.Count}");

            Console.WriteLine(
                $"  Visits         : {visits.Count}");

            Console.WriteLine(
                $"  Memberships    : {memberships.Count}");

            int activeBookings = 0;

            foreach (var booking in bookings)
            {
                if (booking.Status == "Active")
                    activeBookings++;
            }

            Console.WriteLine();

            Console.WriteLine(
                $"  Active bookings: " +
                $"{activeBookings}");

            Pause();
        }

        private static void PrintHeader(
            string title)
        {
            Console.WriteLine();

            Console.WriteLine(
                "  ╔══════════════════════════════════════════════╗");

            Console.WriteLine(
                $"  ║  {title.PadRight(42)}║");

            Console.WriteLine(
                "  ╚══════════════════════════════════════════════╝");

            Console.WriteLine();
        }

        private static string ReadRequired(
            string text)
        {
            while (true)
            {
                Console.Write($"  {text}");

                string value =
                    Console.ReadLine();

                if (value == "0")
                    return null;

                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();

                Console.WriteLine(
                    "  Value cannot be empty.");
            }
        }

        private static string ReadOptional(
            string text)
        {
            Console.Write($"  {text}");

            return Console.ReadLine()?.Trim();
        }

        private static bool TryReadInt(
            string text,
            out int value)
        {
            while (true)
            {
                Console.Write($"  {text}");

                string input =
                    Console.ReadLine();

                if (input == "0")
                {
                    value = 0;
                    return false;
                }

                if (int.TryParse(
                        input,
                        out value))
                {
                    return true;
                }

                Console.WriteLine(
                    "  Please enter a valid number.");
            }
        }

        private static bool TryReadDateTime(
            string text,
            out DateTime value)
        {
            while (true)
            {
                Console.Write($"  {text}");

                string input =
                    Console.ReadLine();

                if (input == "0")
                {
                    value = default;
                    return false;
                }

                if (DateTime.TryParse(
                        input,
                        out value))
                {
                    return true;
                }

                Console.WriteLine(
                    "  Invalid date/time.");
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine(
                "  Press ENTER to continue...");
            Console.ReadLine();
        }

        private static void Success(
            string message)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"  [SUCCESS] {message}");
        }

        private static void Error(
            string message)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"  [ERROR] {message}");
        }

        private static void Info(
            string message)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"  [INFO] {message}");
        }
    }
}