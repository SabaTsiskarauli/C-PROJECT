using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Services.Interfaces
{
    public interface IMemberService
    {
        List<Member> GetAllMembers();

        Member GetMemberById(
            int id);

        Member GetMemberByName(
            string name);

        void RegisterMember(
            Member member);

        void UpdateMember(
            Member member);

        void DeleteMember(
            int id);
    }
}