using FitZone.Domain.Models;

namespace FitZone.Services.Interfaces
{
    public interface IAuthService
    {
        Member LoginMember(
            string email,
            string password);

        Trainer LoginTrainer(
            string email,
            string password);

        Manager LoginManager(
            string email,
            string password);

        Member RegisterMember(
            string fullName,
            string email,
            string password);
    }
}