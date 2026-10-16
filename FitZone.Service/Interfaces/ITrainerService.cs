using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Services.Interfaces
{
    public interface ITrainerService
    {
        List<Trainer> GetAllTrainers();

        Trainer GetTrainerById(int id);

        Trainer GetTrainerByName(string name);

        void AddTrainer(Trainer trainer);

        void UpdateTrainer(Trainer trainer);

        void DeleteTrainer(int id);
    }
}