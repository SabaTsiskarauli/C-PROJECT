using System;
using System.Collections.Generic;
using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using FitZone.Services.Interfaces;
using FitZone.Services.Security;

namespace FitZone.Services.Implementations
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository
            memberRepository;

        public MemberService(
            IMemberRepository memberRepository)
        {
            this.memberRepository =
                memberRepository;
        }

        public List<Member> GetAllMembers()
        {
            return memberRepository.GetMembers();
        }

        public Member GetMemberById(int id)
        {
            return memberRepository
                .GetMemberById(id);
        }

        public Member GetMemberByName(
            string name)
        {
            return memberRepository
                .GetByMemberName(name);
        }

        public void RegisterMember(
            Member member)
        {
            if (member == null)
                throw new ArgumentNullException(
                    nameof(member));

            if (string.IsNullOrWhiteSpace(
                    member.Password))
                throw new ArgumentException(
                    "Password is required.");

            member.Password =
                HashIfNeeded(member.Password);

            memberRepository.AddMember(member);
        }

        public void UpdateMember(
            Member member)
        {
            if (member == null)
                throw new ArgumentNullException(
                    nameof(member));

            // If the password was not changed, it is already
            // a hash and stays untouched. If a new plain
            // password was set, it is hashed here.
            if (!string.IsNullOrWhiteSpace(
                    member.Password))
            {
                member.Password =
                    HashIfNeeded(member.Password);
            }

            memberRepository.UpdateMember(
                member
            );
        }

        public void DeleteMember(int id)
        {
            memberRepository.DeleteMember(id);
        }

        private static string HashIfNeeded(
            string password)
        {
            return IsBCryptHash(password)
                ? password
                : PasswordHasher.Hash(password);
        }

        private static bool IsBCryptHash(
            string value)
        {
            return value != null &&
                   value.Length == 60 &&
                   (value.StartsWith("$2a$") ||
                    value.StartsWith("$2b$") ||
                    value.StartsWith("$2x$") ||
                    value.StartsWith("$2y$"));
        }
    }
}