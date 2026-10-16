using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Services.Interfaces
{
    public interface IBookingService
    {
        List<Booking> GetAllBookings();

        Booking GetBookingById(string id);

        List<Booking> GetMemberBookings(int memberId);

        void CreateBooking(
            int memberId,
            string sessionId);

        void CancelBooking(
            string bookingId,
            int memberId);

        bool CanBook(
            int memberId,
            string sessionId);

        // Manager-ის მიერ booking-ის გაუქმება
        void ManagerCancelBooking(
            string bookingId);

        // Trainer-ის მიერ საკუთარი session-ის წევრის ჩაწერა
        void TrainerCreateBooking(
            int trainerId,
            int memberId,
            string sessionId);

        // Trainer-ის მიერ საკუთარი session-ის booking-ის გაუქმება
        void TrainerCancelBooking(
            int trainerId,
            string bookingId);
    }
}