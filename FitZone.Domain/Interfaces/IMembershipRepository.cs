using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Domain.Interfaces
{
    public interface IMembershipRepository
    {
        List<Membership> GetMemberships();

        Membership GetMembershipById(string id);

        List<Membership> GetMembershipsByMemberId(int memberId);

        void AddMembership(Membership membership);

        void DeleteMembership(string id);

        void UpdateMembership(Membership membership);
    }
}