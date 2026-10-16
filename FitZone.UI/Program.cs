using System;
using System.Linq;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Infrastructure;
using FitZone.Infrastructure.Repositories;
using FitZone.Services.Implementations;
using FitZone.Services.Interfaces;
using FitZone.UI;
using FitZone.Services.Security;


class Program
{
    static void Main()
    {
        // Repositories

        IMemberRepository memberRepository =
            new MemberRepository();

        ITrainerRepository trainerRepository =
            new TrainerRepository();

        IManagerRepository managerRepository =
            new ManagerRepository();

        IMembershipRepository membershipRepository =
            new MembershipRepository();

        IBookingRepository bookingRepository =
            new BookingRepository();

        IWorkoutSessionRepository sessionRepository =
            new WorkoutSessionRepository();

        IVisitRepository visitRepository =
            new VisitRepository();


        // Services

        IMemberService memberService =
            new MemberService(memberRepository);

        ITrainerService trainerService =
            new TrainerService(trainerRepository);

        IManagerService managerService =
            new ManagerService(managerRepository);

        IMembershipService membershipService =
            new MembershipService(
                membershipRepository,
                memberRepository);

        IBookingService bookingService =
            new BookingService(
                bookingRepository,
                memberRepository,
                membershipRepository,
                sessionRepository);

        IWorkoutSessionService sessionService =
            new WorkoutSessionService(
                sessionRepository,
                trainerRepository,
                bookingRepository);

        IVisitService visitService =
            new VisitService(
                visitRepository,
                memberRepository,
                membershipRepository,
                bookingRepository,
                sessionRepository);

        IAuthService authService =
            new AuthService(
                memberRepository,
                trainerRepository,
                managerRepository);


        // One-time fix for records saved before password hashing
        // was added to MemberService / TrainerService.

        MigratePlainPasswords(
            memberService,
            trainerService);


        // Bookings of sessions that have already ended are counted
        // as visits and saved to Visit.json.

        try
        {
            visitService.ProcessFinishedSessions();
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Finished sessions could not be processed: " +
                $"{ex.Message}");
        }


        // UI

        var managerMenu = new ManagerMenu(
            memberService,
            trainerService,
            managerService,
            membershipService,
            bookingService,
            sessionService,
            visitService);

        var managers = managerService.GetAllManagers();

        if (managers.Count == 0)
        {
            var admin = new Manager(
                1,
                "FitZone Manager",
                "manager@gmail.com",
                "Manager123"
            );

            managerService.AddManager(admin);
        }

        var memberMenu = new MemberMenu(
            memberService,
            membershipService,
            bookingService,
            sessionService,
            visitService);

        var trainerMenu = new TrainerMenu(
            trainerService,
            sessionService);

        var trainers = trainerService.GetAllTrainers();

        if (trainers.Count == 0)
        {
            var trainer = new Trainer(
                1,
                "FitZone Trainer",
                "trainer@gmail.com",
                "Trainer123"
            );

            trainerService.AddTrainer(trainer);
        }

        var loginMenu = new LoginMenu(
            authService,
            managerMenu,
            memberMenu,
            trainerMenu);


        // Start application

        loginMenu.Show();
    }

    // Finds members/trainers whose saved password is not a BCrypt hash
    // and saves them again. MemberService.UpdateMember and
    // TrainerService.UpdateTrainer hash plain passwords automatically,
    // so the original passwords keep working after login.
    private static void MigratePlainPasswords(
        IMemberService memberService,
        ITrainerService trainerService)
    {
        var plainMembers =
            memberService
                .GetAllMembers()
                .Where(x => IsPlainPassword(x.Password))
                .ToList();

        foreach (var member in plainMembers)
        {
            try
            {
                memberService.UpdateMember(member);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Password migration skipped for member " +
                    $"{member.UserId}: {ex.Message}");
            }
        }

        var plainTrainers =
            trainerService
                .GetAllTrainers()
                .Where(x => IsPlainPassword(x.Password))
                .ToList();

        foreach (var trainer in plainTrainers)
        {
            try
            {
                trainerService.UpdateTrainer(trainer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Password migration skipped for trainer " +
                    $"{trainer.TrainerId}: {ex.Message}");
            }
        }
    }

    private static bool IsPlainPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        bool looksLikeBCrypt =
            password.Length == 60 &&
            password.StartsWith("$2");

        return !looksLikeBCrypt;
    }
}