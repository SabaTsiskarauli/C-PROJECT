using FitZone.Domain.Interfaces;
using FitZone.Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FitZone.Infrastructure
{
    public class BookingRepository : IBookingRepository
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
"Booking.json"
));


        public List<Booking> GetBookings()
        {
            var json = File.ReadAllText(path);

            var bookings =
                JsonSerializer.Deserialize<List<Booking>>(json);

            return bookings ?? new List<Booking>();
        }

        public Booking GetBookingById(string id)
        {
            var bookings = GetBookings();

            return bookings.Find(x => x.BookingId == id);
        }

        public void AddBooking(Booking booking)
        {
            var bookings = GetBookings();

            bookings.Add(booking);

            Save(bookings);
        }

        public void DeleteBooking(string id)
        {
            var bookings = GetBookings();

            var booking = GetBookingById(id);

            if (booking != null)
            {
                bookings.Remove(booking);
                Save(bookings);
            }
        }

        public void UpdateBooking(Booking booking)
        {
            var bookings = GetBookings();

            var existingBooking =
                bookings.Find(x => x.BookingId == booking.BookingId);

            if (existingBooking != null)
            {
                var index = bookings.IndexOf(existingBooking);

                bookings[index] = booking;

                Save(bookings);
            }
        }

        private void Save(List<Booking> bookings)
        {
            var json = JsonSerializer.Serialize(
                bookings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(path, json);
        }
    }
}