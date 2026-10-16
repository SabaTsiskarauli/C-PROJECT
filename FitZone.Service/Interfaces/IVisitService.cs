using System.Collections.Generic;
using FitZone.Domain.Models;

namespace FitZone.Services.Interfaces
{
    public interface IVisitService
    {
        List<Visit> GetAllVisits();

        Visit GetVisitById(string id);

        List<Visit> GetMemberVisits(int memberId);

        void RegisterVisit(int memberId);

        void DeleteVisit(string id);

        // Finished sessions with active bookings are counted as visits
        int ProcessFinishedSessions();
    }
}