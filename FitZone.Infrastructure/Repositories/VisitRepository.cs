using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;

namespace FitZone.Infrastructure
{
    public class VisitRepository : IVisitRepository
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
                "Visit.json"
            ));

        public List<Visit> GetVisits()
        {
            try
            {
                EnsureFileExists();

                var json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<Visit>();

                var visits =
                    JsonSerializer.Deserialize<List<Visit>>(json);

                return visits ?? new List<Visit>();
            }
            catch (JsonException ex)
            {
                HandleCorruptedFile(ex);

                return new List<Visit>();
            }
            catch (IOException ex)
            {
                FileLogger.Error(
                    $"Visit.json could not be read: {path}", ex);

                throw new InvalidOperationException(
                    "Visit data could not be read. " +
                    "See the error log for details.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                FileLogger.Error(
                    $"No access to Visit.json: {path}", ex);

                throw new InvalidOperationException(
                    "Visit data could not be read. " +
                    "See the error log for details.", ex);
            }
        }

        public Visit GetVisitById(string id)
        {
            var visits = GetVisits();

            return visits.Find(x => x.VisitId == id);
        }

        public List<Visit> GetVisitsByMemberId(int memberId)
        {
            var visits = GetVisits();

            return visits.FindAll(
                x => x.MemberId == memberId);
        }

        public void AddVisit(Visit visit)
        {
            var visits = GetVisits();

            visits.Add(visit);

            Save(visits);
        }

        public void DeleteVisit(string id)
        {
            var visits = GetVisits();

            var visit = visits.Find(x => x.VisitId == id);

            if (visit != null)
            {
                visits.Remove(visit);

                Save(visits);
            }
        }

        public void UpdateVisit(Visit visit)
        {
            var visits = GetVisits();

            var existingVisit =
                visits.Find(x => x.VisitId == visit.VisitId);

            if (existingVisit != null)
            {
                var index = visits.IndexOf(existingVisit);

                visits[index] = visit;

                Save(visits);
            }
        }

        private void Save(List<Visit> visits)
        {
            try
            {
                var json = JsonSerializer.Serialize(
                    visits,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(path, json);
            }
            catch (IOException ex)
            {
                FileLogger.Error(
                    $"Visit.json could not be saved: {path}", ex);

                throw new InvalidOperationException(
                    "Visit data could not be saved. " +
                    "See the error log for details.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                FileLogger.Error(
                    $"No write access to Visit.json: {path}", ex);

                throw new InvalidOperationException(
                    "Visit data could not be saved. " +
                    "See the error log for details.", ex);
            }
        }

        // Creates the folder and an empty JSON list when the file is missing.
        private void EnsureFileExists()
        {
            if (File.Exists(path))
                return;

            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, "[]");

            FileLogger.Info(
                $"Visit.json was missing and was created: {path}");
        }

        // Keeps a backup copy of the damaged file, logs the problem
        // and resets the file to an empty list so the app keeps working.
        private void HandleCorruptedFile(JsonException ex)
        {
            FileLogger.Error(
                $"Visit.json is corrupted: {path}", ex);

            try
            {
                string backupPath =
                    $"{path}.corrupt-{DateTime.Now:yyyyMMddHHmmss}";

                File.Copy(path, backupPath, overwrite: true);

                FileLogger.Info(
                    $"Backup of the damaged file saved: {backupPath}");

                File.WriteAllText(path, "[]");
            }
            catch (Exception backupEx)
            {
                FileLogger.Error(
                    "Could not back up or reset the damaged Visit.json.",
                    backupEx);
            }
        }
    }
}