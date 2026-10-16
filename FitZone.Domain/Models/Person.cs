namespace FitZone.Domain.Models
{
    public abstract class Person
    {
        public int PersonId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        protected Person()
        {
        }

        protected Person(
            int personId,
            string fullName,
            string email,
            string password)
        {
            PersonId = personId;
            FullName = fullName;
            Email = email;
            Password = password;
        }
    }
}