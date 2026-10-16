using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;

namespace FitZone.Infrastructure.Repositories
{
    public class ManagerRepository : IManagerRepository
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
                    "Manager.json"
                )
            );

        private readonly JsonSerializerOptions options =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        public ManagerRepository()
        {
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            string? directory =
                Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory) &&
                !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(path))
            {
                File.WriteAllText(
                    path,
                    "[]"
                );
            }

            string json =
                File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                File.WriteAllText(
                    path,
                    "[]"
                );
            }
        }

        public List<Manager> GetManagers()
        {
            EnsureFileExists();

            string json =
                File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Manager>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<Manager>>(
                    json,
                    options
                ) ?? new List<Manager>();
            }
            catch (JsonException)
            {
                throw new InvalidOperationException(
                    "Manager.json contains invalid JSON."
                );
            }
        }

        public Manager GetManagerById(int id)
        {
            return GetManagers()
                .FirstOrDefault(
                    x => x.ManagerId == id
                );
        }

        public Manager GetByManagerName(
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return GetManagers()
                .FirstOrDefault(
                    x =>
                        x.FullName != null &&
                        x.FullName.Equals(
                            name,
                            StringComparison
                                .OrdinalIgnoreCase
                        )
                );
        }

        public void AddManager(
            Manager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(
                    nameof(manager)
                );
            }

            var managers =
                GetManagers();

            if (managers.Any(
                    x =>
                        x.ManagerId ==
                        manager.ManagerId))
            {
                throw new InvalidOperationException(
                    "Manager with this ID already exists."
                );
            }

            managers.Add(manager);

            Save(managers);
        }

        public void DeleteManager(int id)
        {
            var managers =
                GetManagers();

            var manager =
                managers.FirstOrDefault(
                    x =>
                        x.ManagerId == id
                );

            if (manager == null)
            {
                throw new InvalidOperationException(
                    "Manager not found."
                );
            }

            managers.Remove(manager);

            Save(managers);
        }

        public void UpdateManager(
            Manager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(
                    nameof(manager)
                );
            }

            var managers =
                GetManagers();

            var existing =
                managers.FirstOrDefault(
                    x =>
                        x.ManagerId ==
                        manager.ManagerId
                );

            if (existing == null)
            {
                throw new InvalidOperationException(
                    "Manager not found."
                );
            }

            existing.FullName =
                manager.FullName;

            existing.Email =
                manager.Email;

            existing.Password =
                manager.Password;

            Save(managers);
        }

        private void Save(
            List<Manager> managers)
        {
            string json =
                JsonSerializer.Serialize(
                    managers,
                    options
                );

            File.WriteAllText(
                path,
                json
            );
        }
    }
}