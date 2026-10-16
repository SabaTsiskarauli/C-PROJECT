using FitZone.Domain.Models;
using System.Collections.Generic;

namespace FitZone.Domain.Interfaces
{
    public interface IBookingRepository
    {
        List<Booking> GetBookings();

        Booking GetBookingById(string id);

        void AddBooking(Booking booking);

        void DeleteBooking(string id);

        void UpdateBooking(Booking booking);
    }
}
