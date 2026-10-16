using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FitZone.Infrastructure
{
    public class MemberRepository : IMemberRepository
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
                "Member.json"
            ));

        public List<Member> GetMembers()
        {
            EnsureFileExists();

            try
            {
                var json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<Member>();

                var members =
                    JsonSerializer.Deserialize<List<Member>>(json);

                return members ?? new List<Member>();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "Member.json contains invalid JSON. Please fix the file format.",
                    ex);
            }
        }

        public Member GetMemberById(int id)
        {
            var members = GetMembers();

            return members.Find(
                x => x.UserId == id);
        }

        public Member GetByMemberName(string username)
        {
            var members = GetMembers();

            return members.Find(
                x => x.FullName != null &&
                     x.FullName.Equals(
                         username,
                         StringComparison.OrdinalIgnoreCase));
        }

        public void AddMember(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            var members = GetMembers();

            if (members.Exists(
                    x => x.UserId == member.UserId))
            {
                throw new InvalidOperationException(
                    "Member with this ID already exists.");
            }

            members.Add(member);

            Save(members);
        }

        public void DeleteMember(int id)
        {
            var members = GetMembers();

            var member = members.Find(
                x => x.UserId == id);

            if (member != null)
            {
                members.Remove(member);

                Save(members);
            }
        }

        public void UpdateMember(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            var members = GetMembers();

            var existingMember = members.Find(
                x => x.UserId == member.UserId);

            if (existingMember == null)
                throw new InvalidOperationException(
                    "Member not found.");

            var index = members.IndexOf(existingMember);

            members[index] = member;

            Save(members);
        }

        private void Save(List<Member> members)
        {
            EnsureFileExists();

            var json = JsonSerializer.Serialize(
                members,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(path, json);
        }

        private void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(path);

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
    }
}