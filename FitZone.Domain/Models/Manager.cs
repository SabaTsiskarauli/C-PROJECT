using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Domain.Models
{
    public class Manager : Person
    {
        public int ManagerId { get; set; }

        public Manager()
        {
        }

        public Manager(
            int managerId,
            string fullName,
            string email,
            string password)
            : base(managerId, fullName, email, password)
        {
            ManagerId = managerId;
        }
    }
}
