using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FitZone.Infrastructure
{
    public class TrainerRepository : ITrainerRepository
    {
        private readonly string path = Path.GetFullPath(
Path.Combine(
AppContext.BaseDirectory,
"..",
"..",
"..",
"..",
"FitZone.Infrastructure",
"Data",
"Trainer.json"
));

        public List<Trainer> GetTrainers()
        {
            var json = File.ReadAllText(path);

            var trainers =
                JsonSerializer.Deserialize<List<Trainer>>(json);

            return trainers ?? new List<Trainer>();
        }

        public Trainer GetTrainerById(int id)
        {
            var trainers = GetTrainers();

            return trainers.Find(x => x.TrainerId == id);
        }

        public Trainer GetByTrainerName(string name)
        {
            var trainers = GetTrainers();

            return trainers.Find(x => x.FullName == name);
        }

        public void AddTrainer(Trainer trainer)
        {
            var trainers = GetTrainers();

            trainers.Add(trainer);

            Save(trainers);
        }

        public void DeleteTrainer(int id)
        {
            var trainers = GetTrainers();

            var trainer = trainers.Find(x => x.TrainerId == id);

            if (trainer != null)
            {
                trainers.Remove(trainer);
                Save(trainers);
            }
        }

        public void UpdateTrainer(Trainer trainer)
        {
            var trainers = GetTrainers();

            var existingTrainer =
                trainers.Find(x => x.TrainerId == trainer.TrainerId);

            if (existingTrainer != null)
            {
                var index = trainers.IndexOf(existingTrainer);

                trainers[index] = trainer;

                Save(trainers);
            }
        }

        private void Save(List<Trainer> trainers)
        {
            var json = JsonSerializer.Serialize(
                trainers,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(path, json);
        }
    }
}