using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Domain.Interfaces
{
    public interface IMemberRepository
    {
        List<Member> GetMembers();

        Member GetMemberById(int id);

        Member GetByMemberName(string username);

        void AddMember(Member member);

        void DeleteMember(int id);

        void UpdateMember(Member member);
    }
}