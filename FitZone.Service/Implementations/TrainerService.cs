using System;
using System.Collections.Generic;
using System.Linq;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;
using FitZone.Services.Security;

namespace FitZone.Services.Implementations
{
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerRepository _trainerRepository;

        public TrainerService(ITrainerRepository trainerRepository)
        {
            _trainerRepository = trainerRepository;
        }

        public List<Trainer> GetAllTrainers()
        {
            return _trainerRepository.GetTrainers();
        }

        public Trainer GetTrainerById(int id)
        {
            return _trainerRepository.GetTrainerById(id);
        }

        public Trainer GetTrainerByName(string name)
        {
            return _trainerRepository.GetByTrainerName(name);
        }

        public void AddTrainer(Trainer trainer)
        {
            if (trainer == null)
                throw new ArgumentNullException(nameof(trainer));

            if (string.IsNullOrWhiteSpace(trainer.FullName))
                throw new ArgumentException("Trainer name is required.");

            if (string.IsNullOrWhiteSpace(trainer.Email))
                throw new ArgumentException("Trainer email is required.");

            if (string.IsNullOrWhiteSpace(trainer.Password))
                throw new ArgumentException("Password is required.");

            if (string.IsNullOrWhiteSpace(trainer.Specialization))
                throw new ArgumentException("Specialization is required.");

            if (_trainerRepository.GetTrainerById(trainer.TrainerId) != null)
                throw new InvalidOperationException("Trainer with this ID already exists.");

            if (IsEmailTaken(trainer))
                throw new InvalidOperationException("This email is already used by another trainer.");

            trainer.Password = HashIfNeeded(trainer.Password);

            _trainerRepository.AddTrainer(trainer);
        }

        public void UpdateTrainer(Trainer trainer)
        {
            if (trainer == null)
                throw new ArgumentNullException(nameof(trainer));

            if (_trainerRepository.GetTrainerById(trainer.TrainerId) == null)
                throw new InvalidOperationException("Trainer not found.");

            if (IsEmailTaken(trainer))
                throw new InvalidOperationException("This email is already used by another trainer.");

            // Unchanged password is already a hash and stays as it is;
            // a new plain password is hashed here.
            if (!string.IsNullOrWhiteSpace(trainer.Password))
                trainer.Password = HashIfNeeded(trainer.Password);

            _trainerRepository.UpdateTrainer(trainer);
        }

        public void DeleteTrainer(int id)
        {
            if (_trainerRepository.GetTrainerById(id) == null)
                throw new InvalidOperationException("Trainer not found.");

            _trainerRepository.DeleteTrainer(id);
        }

        private bool IsEmailTaken(Trainer trainer)
        {
            return _trainerRepository
                .GetTrainers()
                .Any(x =>
                    x.TrainerId != trainer.TrainerId &&
                    !string.IsNullOrWhiteSpace(x.Email) &&
                    x.Email.Equals(
                        trainer.Email,
                        StringComparison.OrdinalIgnoreCase));
        }

        private static string HashIfNeeded(string password)
        {
            return IsBCryptHash(password)
                ? password
                : PasswordHasher.Hash(password);
        }

        private static bool IsBCryptHash(string value)
        {
            return value != null &&
                   value.Length == 60 &&
                   (value.StartsWith("$2a$") ||
                    value.StartsWith("$2b$") ||
                    value.StartsWith("$2x$") ||
                    value.StartsWith("$2y$"));
        }
    }
}