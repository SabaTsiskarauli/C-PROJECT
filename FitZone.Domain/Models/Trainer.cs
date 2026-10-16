using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Domain.Models
{
    public class Trainer : Person
    {
        public int TrainerId { get; set; }

        public string Specialization { get; set; }

        public string WorkoutType { get; set; }

        public Trainer()
        {
        }

        public Trainer(
            int trainerId,
            string fullName,
            string email,
            string password,
            string specialization,
            string workoutType)
            : base(trainerId, fullName, email, password)
        {
            TrainerId = trainerId;
            Specialization = specialization;
            WorkoutType = workoutType;
        }

        public Trainer(int personId, string fullName, string email, string password) : base(personId, fullName, email, password)
        {
        }
    }
}
