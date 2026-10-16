using FitZone.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;



namespace FitZone.Infrastructure
{
    public class BasicMembershipRepository
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
"BasicMembership.json"
));



        public List<BasicMembership> GetBasicMemberships()
        {
            var json = File.ReadAllText(path);

            var memberships =
                JsonSerializer.Deserialize<List<BasicMembership>>(json);

            return memberships ?? new List<BasicMembership>();
        }

        public BasicMembership GetBasicMembershipById(string id)
        {
            var memberships = GetBasicMemberships();

            return memberships.Find(x => x.MembershipId == id);
        }

        public void AddBasicMembership(BasicMembership membership)
        {
            var memberships = GetBasicMemberships();

            memberships.Add(membership);

            var json = JsonSerializer.Serialize(
                memberships,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(path, json);
        }

        public void DeleteBasicMembership(string id)
        {
            var memberships = GetBasicMemberships();

            var membership = GetBasicMembershipById(id);

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



        public void UpdateBasicMembership(BasicMembership membership)
        {
            var memberships = GetBasicMemberships();

            var existingMembership =
                GetBasicMembershipById(membership.MembershipId);

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