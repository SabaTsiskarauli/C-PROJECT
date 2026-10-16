using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Domain.Interfaces
{
    public interface IWorkoutSessionRepository
    {
        List<WorkoutSession> GetSessions();

        WorkoutSession GetSessionById(string id);

        List<WorkoutSession> GetSessionsByTrainerId(int trainerId);

        void AddSession(WorkoutSession session);

        void DeleteSession(string id);

        void UpdateSession(WorkoutSession session);
    }
}