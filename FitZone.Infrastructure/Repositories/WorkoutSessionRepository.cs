using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FitZone.Infrastructure
{
    public class WorkoutSessionRepository :
        IWorkoutSessionRepository
    {
        private readonly string path =
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "FitZone.Infrastructure",
                    "Data",
                    "WorkoutSession.json"
                )
            );

        private readonly JsonSerializerOptions options =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        public WorkoutSessionRepository()
        {
            EnsureFileExists();
            SeedSessionsIfEmpty();
        }

        private void EnsureFileExists()
        {
            string directory =
                Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(directory) &&
                !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[]");
                return;
            }

            string json =
                File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                File.WriteAllText(path, "[]");
            }
        }

        public List<WorkoutSession> GetSessions()
        {
            EnsureFileExists();

            string json =
                File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
                return new List<WorkoutSession>();

            try
            {
                return JsonSerializer.Deserialize<
                    List<WorkoutSession>>(
                        json,
                        options
                    ) ?? new List<WorkoutSession>();
            }
            catch (JsonException)
            {
                throw new InvalidOperationException(
                    "WorkoutSession.json contains invalid JSON."
                );
            }
        }

        public WorkoutSession GetSessionById(
            string id)
        {
            return GetSessions()
                .FirstOrDefault(
                    x => x.SessionId == id);
        }

        public List<WorkoutSession>
            GetSessionsByTrainerId(
                int trainerId)
        {
            return GetSessions()
                .Where(x =>
                    x.TrainerId == trainerId)
                .ToList();
        }

        public void AddSession(
            WorkoutSession session)
        {
            if (session == null)
                throw new ArgumentNullException(
                    nameof(session));

            var sessions =
                GetSessions();

            if (sessions.Any(x =>
                x.SessionId == session.SessionId))
            {
                throw new InvalidOperationException(
                    "Session with this ID already exists."
                );
            }

            sessions.Add(session);

            Save(sessions);
        }

        public void DeleteSession(
            string id)
        {
            var sessions =
                GetSessions();

            var session =
                sessions.FirstOrDefault(
                    x => x.SessionId == id);

            if (session == null)
                throw new InvalidOperationException(
                    "Session not found."
                );

            sessions.Remove(session);

            Save(sessions);
        }

        public void UpdateSession(
            WorkoutSession session)
        {
            if (session == null)
                throw new ArgumentNullException(
                    nameof(session));

            var sessions =
                GetSessions();

            var existing =
                sessions.FirstOrDefault(
                    x =>
                        x.SessionId ==
                        session.SessionId);

            if (existing == null)
                throw new InvalidOperationException(
                    "Session not found."
                );

            int index =
                sessions.IndexOf(existing);

            sessions[index] = session;

            Save(sessions);
        }

        private void SeedSessionsIfEmpty()
        {
            var sessions = GetSessions();

            if (sessions.Count > 0)
                return;

            DateTime tomorrow =
                DateTime.Today.AddDays(1);

            sessions.Add(
                new WorkoutSession(
                    "WS501",
                    1,
                    "Yoga",
                    tomorrow.AddHours(10),
                    60,
                    15,
                    "Basic"
                )
            );

            sessions.Add(
                new WorkoutSession(
                    "WS502",
                    1,
                    "Functional Training",
                    tomorrow.AddHours(12),
                    60,
                    12,
                    "Basic"
                )
            );

            sessions.Add(
                new WorkoutSession(
                    "WS503",
                    2,
                    "Pilates",
                    tomorrow.AddHours(15),
                    60,
                    10,
                    "Basic"
                )
            );

            sessions.Add(
                new WorkoutSession(
                    "WS504",
                    2,
                    "Personal Group Training",
                    tomorrow.AddHours(17),
                    75,
                    8,
                    "Premium"
                )
            );

            sessions.Add(
                new WorkoutSession(
                    "WS505",
                    1,
                    "Advanced HIIT",
                    tomorrow.AddDays(1).AddHours(18),
                    60,
                    10,
                    "Premium"
                )
            );

            sessions.Add(
                new WorkoutSession(
                    "WS506",
                    2,
                    "Premium Pilates",
                    tomorrow.AddDays(1).AddHours(20),
                    60,
                    8,
                    "Premium"
                )
            );

            Save(sessions);
        }

        private void Save(
            List<WorkoutSession> sessions)
        {
            string json =
                JsonSerializer.Serialize(
                    sessions,
                    options
                );

            File.WriteAllText(
                path,
                json
            );
        }
    }
}