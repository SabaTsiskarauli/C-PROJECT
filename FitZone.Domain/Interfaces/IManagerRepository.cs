using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Domain.Interfaces
{
    public interface IManagerRepository
    {
        List<Manager> GetManagers();

        Manager GetManagerById(int id);

        Manager GetByManagerName(string name);

        void AddManager(Manager manager);

        void DeleteManager(int id);

        void UpdateManager(Manager manager);
    }
}