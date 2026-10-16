using System;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;

namespace FitZone.UI
{
    public class TrainerMenu
    {
        private readonly ITrainerService _trainerService;
        private readonly IWorkoutSessionService _sessionService;

        public TrainerMenu(
            ITrainerService trainerService,
            IWorkoutSessionService sessionService)
        {
            _trainerService = trainerService;
            _sessionService = sessionService;
        }

        public void Show(Trainer trainer)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine(
                    "╔════════════════════════════════════╗");
                Console.WriteLine(
                    "║         FITZONE • TRAINER          ║");
                Console.WriteLine(
                    "╚════════════════════════════════════╝");

                Console.WriteLine();
                Console.WriteLine(
                    $"Welcome, {trainer.FullName}!");
                Console.WriteLine();

                Console.WriteLine(
                    "  1. My profile");
                Console.WriteLine(
                    "  2. My sessions");
                Console.WriteLine(
                    "  3. Available sessions");
                Console.WriteLine(
                    "  4. Create session");
                Console.WriteLine(
                    "  5. Update my session");
                Console.WriteLine(
                    "  6. Cancel my session");
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
                            ShowProfile(trainer);
                            break;

                        case "2":
                            ShowMySessions(
                                trainer.TrainerId);
                            break;

                        case "3":
                            ShowAvailableSessions();
                            break;

                        case "4":
                            CreateSession(
                                trainer.TrainerId);
                            break;

                        case "5":
                            UpdateSession(
                                trainer.TrainerId);
                            break;

                        case "6":
                            CancelSession(
                                trainer.TrainerId);
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
            Trainer trainer)
        {
            Console.Clear();

            Console.WriteLine(
                "========== MY PROFILE ==========");

            Console.WriteLine(
                $"ID: {trainer.TrainerId}");

            Console.WriteLine(
                $"Name: {trainer.FullName}");

            Console.WriteLine(
                $"Email: {trainer.Email}");

            Console.WriteLine(
                $"Specialization: " +
                $"{trainer.Specialization}");

            Console.WriteLine(
                $"Workout type: " +
                $"{trainer.WorkoutType}");

            Pause();
        }

        private void ShowMySessions(
            int trainerId)
        {
            Console.Clear();

            var sessions =
                _sessionService
                    .GetSessionsByTrainer(
                        trainerId);

            Console.WriteLine(
                "========== MY SESSIONS ==========");

            if (sessions.Count == 0)
            {
                Console.WriteLine(
                    "No sessions found.");
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
                        $"{session.RequiredMembershipType} | " +
                        $"{session.Status}");
                }
            }

            Console.WriteLine();
            Console.WriteLine(
                "[0] Exit");

            Pause();
        }

        private void ShowAvailableSessions()
        {
            Console.Clear();

            var sessions =
                _sessionService
                    .GetAvailableSessions();

            Console.WriteLine(
                "========== AVAILABLE SESSIONS ==========");

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

        private void CreateSession(
            int trainerId)
        {
            Console.Clear();

            Console.WriteLine(
                "========== CREATE SESSION ==========");

            Console.WriteLine();
            Console.WriteLine(
                "Enter 0 at any step to exit.");
            Console.WriteLine();

            string sessionId =
                ReadRequired(
                    "Session ID: ");

            if (sessionId == null)
                return;

            string type =
                ReadRequired(
                    "Workout type: ");

            if (type == null)
                return;

            DateTime start;

            if (!TryReadDateTime(
                    "Start date/time " +
                    "(dd.MM.yyyy HH:mm): ",
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
                "Membership access:");
            Console.WriteLine(
                "[1] Basic + Premium");
            Console.WriteLine(
                "[2] Premium only");
            Console.WriteLine(
                "[0] Exit");

            Console.Write(
                "\nChoose: ");

            string accessChoice =
                Console.ReadLine();

            if (accessChoice == "0")
                return;

            string membershipType;

            if (accessChoice == "1")
            {
                membershipType = "Basic";
            }
            else if (accessChoice == "2")
            {
                membershipType = "Premium";
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
                    sessionId,
                    trainerId,
                    type,
                    start,
                    duration,
                    capacity,
                    membershipType);

            _sessionService.CreateSession(
                session);

            Success(
                $"Session created successfully. " +
                $"Access: {membershipType}");

            Pause();
        }

        private void UpdateSession(
            int trainerId)
        {
            Console.Clear();

            Console.WriteLine(
                "========== UPDATE MY SESSION ==========");

            string id =
                ReadRequired(
                    "Session ID: ");

            if (id == null)
                return;

            var session =
                _sessionService
                    .GetSessionById(id);

            if (session == null)
            {
                Error(
                    "Session not found.");
                Pause();
                return;
            }

            if (session.TrainerId != trainerId)
            {
                Error(
                    "You can update only your own sessions.");
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
                $"Current access: " +
                $"{session.RequiredMembershipType}");

            Console.WriteLine(
                "[1] Basic + Premium");
            Console.WriteLine(
                "[2] Premium only");
            Console.WriteLine(
                "[0] Keep current / Exit");

            Console.Write(
                "\nChoose: ");

            string access =
                Console.ReadLine();

            if (access == "0")
            {
                _sessionService.UpdateSession(
                    session);

                Success(
                    "Session updated.");

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
                Error(
                    "Invalid option.");
                Pause();
                return;
            }

            _sessionService.UpdateSession(
                session);

            Success(
                "Session updated.");

            Pause();
        }

        private void CancelSession(
            int trainerId)
        {
            Console.Clear();

            Console.WriteLine(
                "========== CANCEL MY SESSION ==========");

            string id =
                ReadRequired(
                    "Session ID: ");

            if (id == null)
                return;

            var session =
                _sessionService
                    .GetSessionById(id);

            if (session == null)
            {
                Error(
                    "Session not found.");
                Pause();
                return;
            }

            if (session.TrainerId != trainerId)
            {
                Error(
                    "You can cancel only your own sessions.");
                Pause();
                return;
            }

            _sessionService.CancelSession(id);

            Success(
                "Session cancelled.");

            Pause();
        }

        private static string ReadRequired(
            string text)
        {
            while (true)
            {
                Console.Write(
                    $"  {text}");

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
            Console.Write(
                $"  {text}");

            return Console.ReadLine()?.Trim();
        }

        private static bool TryReadInt(
            string text,
            out int value)
        {
            while (true)
            {
                Console.Write(
                    $"  {text}");

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
                Console.Write(
                    $"  {text}");

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
                "Press ENTER to continue...");
            Console.ReadLine();
        }

        private static void Success(
            string message)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"[SUCCESS] {message}");
        }

        private static void Error(
            string message)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"[ERROR] {message}");
        }
    }
}