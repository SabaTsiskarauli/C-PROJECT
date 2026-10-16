using System;
using System.Collections.Generic;
using System.Linq;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;

namespace FitZone.Services.Implementations
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMemberRepository _memberRepository;

        public MembershipService(
            IMembershipRepository membershipRepository,
            IMemberRepository memberRepository)
        {
            _membershipRepository = membershipRepository;
            _memberRepository = memberRepository;
        }

        public List<Membership> GetAllMemberships()
        {
            return _membershipRepository.GetMemberships();
        }

        public Membership GetMembershipById(string id)
        {
            return _membershipRepository.GetMembershipById(id);
        }

        public List<Membership> GetMemberMemberships(int memberId)
        {
            return _membershipRepository
                .GetMembershipsByMemberId(memberId);
        }

        public void PurchaseMembership(Membership membership)
        {
            if (membership == null)
                throw new ArgumentNullException(nameof(membership));

            var member =
                _memberRepository.GetMemberById(membership.MemberId);

            if (member == null)
                throw new InvalidOperationException(
                    "Member not found.");

            if (string.IsNullOrWhiteSpace(membership.MembershipId))
                throw new ArgumentException(
                    "Membership ID is required.");

            if (string.IsNullOrWhiteSpace(membership.Type))
                throw new ArgumentException(
                    "Membership type is required.");

            if (membership.Type != "Basic" &&
                membership.Type != "Premium")
            {
                throw new ArgumentException(
                    "Membership type must be Basic or Premium.");
            }

            if (_membershipRepository.GetMembershipById(
                    membership.MembershipId) != null)
            {
                throw new InvalidOperationException(
                    "Membership with this ID already exists.");
            }

            var existingMemberships =
                _membershipRepository
                    .GetMembershipsByMemberId(
                        membership.MemberId);

            var activeMembership =
                existingMemberships.FirstOrDefault(
                    x => x.Status == "Active" &&
                         x.EndDate >= DateTime.Now);

            if (activeMembership != null)
            {
                throw new InvalidOperationException(
                    "Member already has an active membership.");
            }

            _membershipRepository.AddMembership(membership);

            member.MembershipId = membership.MembershipId;

            _memberRepository.UpdateMember(member);
        }

        public Membership BuyMembership(
            int memberId,
            string membershipType)
        {
            if (string.IsNullOrWhiteSpace(membershipType))
            {
                throw new ArgumentException(
                    "Membership type is required.");
            }

            if (!membershipType.Equals(
                    "Basic",
                    StringComparison.OrdinalIgnoreCase) &&
                !membershipType.Equals(
                    "Premium",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Choose Basic or Premium.");
            }

            var member =
                _memberRepository.GetMemberById(memberId);

            if (member == null)
            {
                throw new InvalidOperationException(
                    "Member not found.");
            }

            var existingMemberships =
                _membershipRepository
                    .GetMembershipsByMemberId(memberId);

            var activeMembership =
                existingMemberships.FirstOrDefault(
                    x => x.Status == "Active" &&
                         x.EndDate >= DateTime.Now);

            if (activeMembership != null)
            {
                throw new InvalidOperationException(
                    "You already have an active membership. " +
                    "Use Upgrade to Premium if you have Basic.");
            }

            string membershipId = GenerateMembershipId();

            Membership membership;

            if (membershipType.Equals(
                    "Basic",
                    StringComparison.OrdinalIgnoreCase))
            {
                membership = new BasicMembership(
                    membershipId,
                    memberId,
                    DateTime.Now);
            }
            else
            {
                membership = new PremiumMembership(
                    membershipId,
                    memberId,
                    DateTime.Now);
            }

            _membershipRepository.AddMembership(membership);

            member.MembershipId = membership.MembershipId;

            _memberRepository.UpdateMember(member);

            return membership;
        }

        public void UpgradeToPremium(int memberId)
        {
            var member =
                _memberRepository.GetMemberById(memberId);

            if (member == null)
            {
                throw new InvalidOperationException(
                    "Member not found.");
            }

            var memberships =
                _membershipRepository
                    .GetMembershipsByMemberId(memberId);

            var activeMembership =
                memberships.FirstOrDefault(
                    x => x.Status == "Active" &&
                         x.EndDate >= DateTime.Now);

            if (activeMembership == null)
            {
                throw new InvalidOperationException(
                    "You do not have an active membership.");
            }

            if (activeMembership.Type.Equals(
                    "Premium",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "You already have Premium membership.");
            }

            if (!activeMembership.Type.Equals(
                    "Basic",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only Basic membership can be upgraded.");
            }

            // ვინარჩუნებთ ძველ Membership ID-ს და
            // მოქმედების ბოლო თარიღს.
            string membershipId =
                activeMembership.MembershipId;

            DateTime endDate =
                activeMembership.EndDate;

            var premiumMembership =
                new PremiumMembership(
                    membershipId,
                    memberId,
                    DateTime.Now);

            premiumMembership.Renew(
                endDate,
                null);

            _membershipRepository
                .DeleteMembership(
                    activeMembership.MembershipId);

            _membershipRepository
                .AddMembership(
                    premiumMembership);

            member.MembershipId =
                premiumMembership.MembershipId;

            _memberRepository.UpdateMember(member);
        }

        public void RenewMembership(
            string membershipId,
            DateTime newEndDate,
            int? remainingVisits)
        {
            var membership =
                _membershipRepository
                    .GetMembershipById(membershipId);

            if (membership == null)
                throw new InvalidOperationException(
                    "Membership not found.");

            if (newEndDate <= DateTime.Now)
                throw new ArgumentException(
                    "New end date must be in the future.");

            if (remainingVisits.HasValue &&
                remainingVisits.Value < 0)
            {
                throw new ArgumentException(
                    "Remaining visits cannot be negative.");
            }

            membership.Renew(
                newEndDate,
                remainingVisits);

            _membershipRepository
                .UpdateMembership(membership);
        }

        public void ExpireMembership(string membershipId)
        {
            var membership =
                _membershipRepository
                    .GetMembershipById(membershipId);

            if (membership == null)
                throw new InvalidOperationException(
                    "Membership not found.");

            membership.Expire();

            _membershipRepository
                .UpdateMembership(membership);
        }

        private string GenerateMembershipId()
        {
            var memberships =
                _membershipRepository.GetMemberships();

            if (memberships.Count == 0)
                return "MS101";

            var numbers = memberships
                .Select(x => x.MembershipId)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x) &&
                    x.StartsWith(
                        "MS",
                        StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    string numberPart =
                        x.Substring(2);

                    return int.TryParse(
                        numberPart,
                        out int number)
                        ? number
                        : 100;
                });

            int nextNumber =
                numbers.Any()
                    ? numbers.Max() + 1
                    : 101;

            return $"MS{nextNumber}";
        }
    }
}