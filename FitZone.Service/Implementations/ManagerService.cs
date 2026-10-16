using System;
using System.Collections.Generic;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;

namespace FitZone.Services.Implementations
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public List<Manager> GetAllManagers()
        {
            return _managerRepository.GetManagers();
        }

        public Manager GetManagerById(int id)
        {
            return _managerRepository.GetManagerById(id);
        }

        public void AddManager(Manager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (string.IsNullOrWhiteSpace(manager.FullName))
                throw new ArgumentException("Manager name is required.");

            if (string.IsNullOrWhiteSpace(manager.Email))
                throw new ArgumentException("Manager email is required.");

            if (string.IsNullOrWhiteSpace(manager.Password))
                throw new ArgumentException("Manager password is required.");

            if (_managerRepository.GetManagerById(manager.ManagerId) != null)
                throw new InvalidOperationException(
                    "Manager with this ID already exists.");

            _managerRepository.AddManager(manager);
        }

        public void UpdateManager(Manager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (_managerRepository.GetManagerById(manager.ManagerId) == null)
                throw new InvalidOperationException("Manager not found.");

            _managerRepository.UpdateManager(manager);
        }

        public void DeleteManager(int id)
        {
            if (_managerRepository.GetManagerById(id) == null)
                throw new InvalidOperationException("Manager not found.");

            _managerRepository.DeleteManager(id);
        }
    }
}