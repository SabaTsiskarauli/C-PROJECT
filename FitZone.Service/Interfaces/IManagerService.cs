using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Services.Interfaces
{
    public interface IManagerService
    {
        List<Manager> GetAllManagers();

        Manager GetManagerById(int id);

        void AddManager(Manager manager);

        void UpdateManager(Manager manager);

        void DeleteManager(int id);
    }
}