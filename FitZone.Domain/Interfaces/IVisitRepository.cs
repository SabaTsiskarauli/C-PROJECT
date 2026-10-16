using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Domain.Interfaces
{
    public interface IVisitRepository
    {
        List<Visit> GetVisits();

        Visit GetVisitById(string id);

        List<Visit> GetVisitsByMemberId(int memberId);

        void AddVisit(Visit visit);

        void DeleteVisit(string id);

        void UpdateVisit(Visit visit);
    }
}