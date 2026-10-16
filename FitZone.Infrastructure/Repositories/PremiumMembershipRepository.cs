using FitZone.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FitZone.Infrastructure
{
    public class PremiumMembershipRepository
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
"PremiumMembership.json"
));

        public List<PremiumMembership> GetPremiumMemberships()
        {
            var json = File.ReadAllText(path);

            var memberships =
                JsonSerializer.Deserialize<List<PremiumMembership>>(json);

            return memberships ?? new List<PremiumMembership>();
        }

        public PremiumMembership GetPremiumMembershipById(string id)
        {
            var memberships = GetPremiumMemberships();

            return memberships.Find(x => x.MembershipId == id);
        }

        public void AddPremiumMembership(PremiumMembership membership)
        {
            var memberships = GetPremiumMemberships();

            memberships.Add(membership);

            var json = JsonSerializer.Serialize(
                memberships,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(path, json);
        }

        public void DeletePremiumMembership(string id)
        {
            var memberships = GetPremiumMemberships();

            var membership = GetPremiumMembershipById(id);

            if (membership != null)
            {
                memberships.Remove(membership);

                var json = JsonSerializer.Serialize(
                    memberships,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(path, json);
            }
        }

        public void UpdatePremiumMembership(PremiumMembership membership)
        {
            var memberships = GetPremiumMemberships();

            var existingMembership =
                GetPremiumMembershipById(membership.MembershipId);

            if (existingMembership != null)
            {
                var index = memberships.IndexOf(existingMembership);

                memberships[index] = membership;

                var json = JsonSerializer.Serialize(
                    memberships,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(path, json);
            }
        }
    }
}