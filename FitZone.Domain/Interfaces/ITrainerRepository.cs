using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Domain.Interfaces
{
    public interface ITrainerRepository
    {
        List<Trainer> GetTrainers();

        Trainer GetTrainerById(int id);

        Trainer GetByTrainerName(string name);

        void AddTrainer(Trainer trainer);

        void DeleteTrainer(int id);

        void UpdateTrainer(Trainer trainer);
    }
}