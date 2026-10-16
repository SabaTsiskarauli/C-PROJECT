using System;
using System.Linq;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;
using FitZone.Services.Security;

namespace FitZone.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ITrainerRepository _trainerRepository;
        private readonly IManagerRepository _managerRepository;

        public AuthService(
            IMemberRepository memberRepository,
            ITrainerRepository trainerRepository,
            IManagerRepository managerRepository)
        {
            _memberRepository = memberRepository;
            _trainerRepository = trainerRepository;
            _managerRepository = managerRepository;
        }

        public Member LoginMember(
            string email,
            string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var member = _memberRepository
                .GetMembers()
                .FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Email) &&
                    x.Email.Equals(
                        email,
                        StringComparison.OrdinalIgnoreCase));

            if (member == null)
                return null;

            return PasswordHasher.Verify(
                password,
                member.Password)
                ? member
                : null;
        }

        public Trainer LoginTrainer(
            string email,
            string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var trainer = _trainerRepository
                .GetTrainers()
                .FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Email) &&
                    x.Email.Equals(
                        email,
                        StringComparison.OrdinalIgnoreCase));

            if (trainer == null)
                return null;

            return PasswordHasher.Verify(
                password,
                trainer.Password)
                ? trainer
                : null;
        }

        public Manager LoginManager(
            string email,
            string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var manager = _managerRepository
                .GetManagers()
                .FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Email) &&
                    x.Email.Equals(
                        email,
                        StringComparison.OrdinalIgnoreCase));

            if (manager == null)
                return null;

            return PasswordHasher.Verify(
                password,
                manager.Password)
                ? manager
                : null;
        }

        public Member RegisterMember(
            string fullName,
            string email,
            string password)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException(
                    "Full name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email is required.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(
                    "Password is required.");

            bool emailExists =
                _memberRepository.GetMembers().Any(x =>
                    !string.IsNullOrWhiteSpace(x.Email) &&
                    x.Email.Equals(
                        email,
                        StringComparison.OrdinalIgnoreCase))
                ||
                _trainerRepository.GetTrainers().Any(x =>
                    !string.IsNullOrWhiteSpace(x.Email) &&
                    x.Email.Equals(
                        email,
                        StringComparison.OrdinalIgnoreCase))
                ||
                _managerRepository.GetManagers().Any(x =>
                    !string.IsNullOrWhiteSpace(x.Email) &&
                    x.Email.Equals(
                        email,
                        StringComparison.OrdinalIgnoreCase));

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "This email is already registered.");
            }

            int newId = GenerateMemberId();

            string passwordHash =
                PasswordHasher.Hash(password);

            var member = new Member(
                newId,
                fullName,
                email,
                passwordHash,
                null);

            _memberRepository.AddMember(member);

            return member;
        }

        private int GenerateMemberId()
        {
            var members =
                _memberRepository.GetMembers();

            if (members.Count == 0)
                return 1001;

            return members.Max(x => x.UserId) + 1;
        }
    }
}