using System;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;

namespace FitZone.UI
{
    public class LoginMenu
    {
        private readonly IAuthService _authService;
        private readonly ManagerMenu _managerMenu;
        private readonly MemberMenu _memberMenu;
        private readonly TrainerMenu _trainerMenu;

        public LoginMenu(
            IAuthService authService,
            ManagerMenu managerMenu,
            MemberMenu memberMenu,
            TrainerMenu trainerMenu)
        {
            _authService = authService;
            _managerMenu = managerMenu;
            _memberMenu = memberMenu;
            _trainerMenu = trainerMenu;
        }

        public void Show()
        {
            while (true)
            {
                ConsoleHelper.Header("FITZONE • FITNESS CLUB");

                ConsoleHelper.Title("Welcome to FitZone!");
                Console.WriteLine();

                ConsoleHelper.MenuOption("1", "Member Login");
                ConsoleHelper.MenuOption("2", "Trainer Login");
                ConsoleHelper.MenuOption("3", "Manager Login");
                ConsoleHelper.MenuOption("4", "Register Member");
                ConsoleHelper.MenuOption("0", "Exit");

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("  Select option: ");
                Console.ResetColor();

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            MemberLogin();
                            break;

                        case "2":
                            TrainerLogin();
                            break;

                        case "3":
                            ManagerLogin();
                            break;

                        case "4":
                            RegisterMember();
                            break;

                        case "0":
                            ConsoleHelper.Info("Goodbye!");
                            return;

                        default:
                            ConsoleHelper.Error("Invalid option.");
                            ConsoleHelper.Pause();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Error(ex.Message);
                    ConsoleHelper.Pause();
                }
            }
        }

        private void MemberLogin()
        {
            ConsoleHelper.Header("MEMBER LOGIN");

            Console.Write("  Email: ");
            string email = Console.ReadLine();

            Console.Write("  Password: ");
            string password = ConsoleHelper.ReadPassword();

            Member member =
                _authService.LoginMember(email, password);

            if (member == null)
            {
                ConsoleHelper.Error(
                    "Invalid email or password.");

                ConsoleHelper.Pause();
                return;
            }

            ConsoleHelper.Success(
                $"Welcome, {member.FullName}!");

            ConsoleHelper.Pause();

            _memberMenu.Show(member);
        }

        private void TrainerLogin()
        {
            ConsoleHelper.Header("TRAINER LOGIN");

            Console.Write("  Email: ");
            string email = Console.ReadLine();

            Console.Write("  Password: ");
            string password = ConsoleHelper.ReadPassword();

            Trainer trainer =
                _authService.LoginTrainer(email, password);

            if (trainer == null)
            {
                ConsoleHelper.Error(
                    "Invalid email or password.");

                ConsoleHelper.Pause();
                return;
            }

            ConsoleHelper.Success(
                $"Welcome, {trainer.FullName}!");

            ConsoleHelper.Pause();

            _trainerMenu.Show(trainer);
        }

        private void ManagerLogin()
        {
            ConsoleHelper.Header("MANAGER LOGIN");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  🔐 Administrator access");
            Console.ResetColor();

            Console.WriteLine();

            Console.Write("  Email: ");
            string email = Console.ReadLine();

            Console.Write("  Password: ");
            string password = ConsoleHelper.ReadPassword();

            Manager manager =
                _authService.LoginManager(email, password);

            if (manager == null)
            {
                ConsoleHelper.Error(
                    "Invalid email or password.");

                ConsoleHelper.Pause();
                return;
            }

            ConsoleHelper.Success(
                $"Welcome, Manager {manager.FullName}!");

            ConsoleHelper.Pause();

            _managerMenu.Show(manager);
        }

        private void RegisterMember()
        {
            ConsoleHelper.Header("MEMBER REGISTRATION");

            Console.Write("  Full name: ");
            string fullName = Console.ReadLine();

            Console.Write("  Email: ");
            string email = Console.ReadLine();

            Console.Write("  Password: ");
            string password = ConsoleHelper.ReadPassword();

            Member member =
                _authService.RegisterMember(
                    fullName,
                    email,
                    password);

            Console.WriteLine();

            ConsoleHelper.Success(
                "Registration successful!");

            ConsoleHelper.Info(
                $"Your Member ID: {member.UserId}");

            ConsoleHelper.Pause();
        }
    }
}