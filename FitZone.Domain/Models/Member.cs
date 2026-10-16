using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Domain.Models
{
    public class Member : Person
    {
        public int UserId { get; set; }

        public string MembershipId { get; set; }

        public Member()
        {
        }

        public Member(
            int userId,
            string fullName,
            string email,
            string password,
            string membershipId)
            : base(userId, fullName, email, password)
        {
            UserId = userId;
            MembershipId = membershipId;
        }
    }
}
