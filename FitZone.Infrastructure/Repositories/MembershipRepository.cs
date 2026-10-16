using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FitZone.Infrastructure
{
    public class MembershipRepository : IMembershipRepository
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
                "Membership.json"
            ));

        public List<Membership> GetMemberships()
        {
            EnsureFileExists();

            try
            {
                var json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<Membership>();

                var records =
                    JsonSerializer.Deserialize<List<MembershipRecord>>(json);

                if (records == null)
                    return new List<Membership>();

                var memberships = new List<Membership>();

                foreach (var record in records)
                {
                    if (record == null)
                        continue;

                    Membership membership;

                    if (string.Equals(
                            record.Type,
                            "Premium",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        membership =
                            new PremiumMembership(
                                record.MembershipId,
                                record.MemberId,
                                record.StartDate);
                    }
                    else
                    {
                        membership =
                            new BasicMembership(
                                record.MembershipId,
                                record.MemberId,
                                record.StartDate);
                    }

                    var visitLimit = record.VisitLimit;

                    if (visitLimit == 0)
                    {
                        visitLimit =
                            membership is PremiumMembership
                                ? -1
                                : 8;
                    }

                    membership.RestoreState(
                        record.StartDate,
                        record.EndDate,
                        record.RemainingVisits,
                        string.IsNullOrWhiteSpace(record.Status)
                            ? "Active"
                            : record.Status,
                        visitLimit);

                    memberships.Add(membership);
                }

                return memberships;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "Membership.json contains invalid JSON. Please fix the file format.",
                    ex);
            }
        }

        public Membership GetMembershipById(string id)
        {
            var memberships = GetMemberships();

            return memberships.Find(
                x => x.MembershipId == id);
        }

        public List<Membership> GetMembershipsByMemberId(
            int memberId)
        {
            var memberships = GetMemberships();

            return memberships.FindAll(
                x => x.MemberId == memberId);
        }

        public void AddMembership(
            Membership membership)
        {
            if (membership == null)
                throw new ArgumentNullException(
                    nameof(membership));

            var memberships = GetMemberships();

            if (memberships.Exists(
                    x => x.MembershipId ==
                         membership.MembershipId))
            {
                throw new InvalidOperationException(
                    "Membership with this ID already exists.");
            }

            memberships.Add(membership);

            Save(memberships);
        }

        public void DeleteMembership(string id)
        {
            var memberships = GetMemberships();

            var membership = memberships.Find(
                x => x.MembershipId == id);

            if (membership != null)
            {
                memberships.Remove(membership);

                Save(memberships);
            }
        }

        public void UpdateMembership(
            Membership membership)
        {
            if (membership == null)
                throw new ArgumentNullException(
                    nameof(membership));

            var memberships = GetMemberships();

            var existingMembership =
                memberships.Find(
                    x => x.MembershipId ==
                         membership.MembershipId);

            if (existingMembership == null)
                throw new InvalidOperationException(
                    "Membership not found.");

            var index =
                memberships.IndexOf(
                    existingMembership);

            memberships[index] = membership;

            Save(memberships);
        }

        private void Save(
            List<Membership> memberships)
        {
            EnsureFileExists();

            var records =
                new List<MembershipRecord>();

            foreach (var membership in memberships)
            {
                records.Add(
                    new MembershipRecord
                    {
                        MembershipId =
                            membership.MembershipId,

                        MemberId =
                            membership.MemberId,

                        Type =
                            membership.Type,

                        StartDate =
                            membership.StartDate,

                        EndDate =
                            membership.EndDate,

                        RemainingVisits =
                            membership.RemainingVisits,

                        Status =
                            membership.Status,

                        VisitLimit =
                            membership.VisitLimit
                    });
            }

            var json =
                JsonSerializer.Serialize(
                    records,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

            File.WriteAllText(
                path,
                json);
        }

        private void EnsureFileExists()
        {
            var directory =
                Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(directory) &&
                !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(path))
            {
                File.WriteAllText(
                    path,
                    "[]");
            }
        }

        private class MembershipRecord
        {
            public string MembershipId { get; set; }

            public int MemberId { get; set; }

            public string Type { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public int? RemainingVisits { get; set; }

            public string Status { get; set; }

            public int VisitLimit { get; set; }
        }
    }
}