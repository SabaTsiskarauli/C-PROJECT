using FitZone.Domain.Models;
using System;
using System.Collections.Generic;

namespace FitZone.Services.Interfaces
{
    public interface IWorkoutSessionService
    {
        List<WorkoutSession> GetAllSessions();

        WorkoutSession GetSessionById(
            string id);

        List<WorkoutSession> GetSessionsByTrainer(
            int trainerId);

        List<WorkoutSession> GetAvailableSessions();

        List<WorkoutSession> SearchSessions(
            DateTime date,
            string workoutType,
            int? trainerId);

        void CreateSession(
            WorkoutSession session);

        void UpdateSession(
            WorkoutSession session);

        void DeleteSession(
            string id);

        void CancelSession(
            string id);
    }
}