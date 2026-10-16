using FitZone.Domain.Models;
using System;
using System.Collections.Generic;

namespace FitZone.Services.Interfaces
{
    public interface IMembershipService
    {
        List<Membership> GetAllMemberships();

        Membership GetMembershipById(
            string id);

        List<Membership> GetMemberMemberships(
            int memberId);

        void PurchaseMembership(
            Membership membership);

        Membership BuyMembership(
            int memberId,
            string membershipType);

        void UpgradeToPremium(
            int memberId);

        void RenewMembership(
            string membershipId,
            DateTime newEndDate,
            int? remainingVisits);

        void ExpireMembership(
            string membershipId);
    }
}