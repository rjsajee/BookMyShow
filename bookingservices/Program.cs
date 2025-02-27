using System;
using System.Collections.Generic;
using System.IO;
using MusicalServices;

namespace BookingServices
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int MusicalId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public static class BookingManager
    {
        private static string bookingFilePath = @"C:\Users\30111549\source\repos\BookMyShow\bookings.txt";
        private static string musicalFilePath = @"C:\Users\30111549\source\repos\BookMyShow\musicals.txt";

       // private static string bookingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "bookings.txt");
       // private static string musicalFilePath = Path.Combine("/app/data", "musicals.txt");

        public static List<Booking> LoadBookings()
        {
            List<Booking> bookings = new List<Booking>();
            if (File.Exists(bookingFilePath))
            {
                string[] lines = File.ReadAllLines(bookingFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 4 &&
                        int.TryParse(parts[0], out int id) &&
                        int.TryParse(parts[1], out int musicalId) &&
                        int.TryParse(parts[2], out int quantity) &&
                        decimal.TryParse(parts[3], out decimal totalPrice))
                    {
                        bookings.Add(new Booking { BookingId = id, MusicalId = musicalId, Quantity = quantity, TotalPrice = totalPrice });
                    }
                }
            }
            return bookings;
        }

        public static void SaveBookings(List<Booking> bookings)
        {
            List<string> lines = new List<string>();
            foreach (var booking in bookings)
            {
                lines.Add($"{booking.BookingId}|{booking.MusicalId}|{booking.Quantity}|{booking.TotalPrice}");
            }
            File.WriteAllLines(bookingFilePath, lines);
        }

        public static void CreateBooking()
        {
            List<Musical> musicals = MusicalCatalog.LoadMusicals(); // Load musical shows

            if (musicals.Count == 0)
            {
                Console.WriteLine("No musicals available for booking.");
                return;
            }

            Console.Write("Enter Musical ID: ");
            if (!int.TryParse(Console.ReadLine(), out int musicalId))
            {
                Console.WriteLine("Invalid Musical ID.");
                return;
            }

            var musical = musicals.Find(m => m.Id == musicalId);

            if (musical == null)
            {
                Console.WriteLine("Musical not found.");
                return;
            }

            Console.Write($"Enter Number of Tickets (Available: {musical.SeatsAvailable}): ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid ticket count. Must be a positive number.");
                return;
            }

            if (musical.SeatsAvailable < quantity)
            {
                Console.WriteLine($"Only {musical.SeatsAvailable} seats are available. Cannot book {quantity} tickets.");
                return;
            }

            // Update available seats in the musical
            musical.SeatsAvailable -= quantity;

            // Save updated musicals back to file
            MusicalCatalog.SaveMusicals(musicals);

            // Create new booking
            List<Booking> bookings = LoadBookings();
            int newId = (bookings.Count > 0) ? bookings[^1].BookingId + 1 : 1;
            var booking = new Booking
            {
                BookingId = newId,
                MusicalId = musicalId,
                Quantity = quantity,
                TotalPrice = musical.Price * quantity
            };

            bookings.Add(booking);
            SaveBookings(bookings);
            Console.WriteLine($"Booking successful! {quantity} tickets booked for {musical.Name}.");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("BookingService is running...");

            while (true)
            {
                Console.WriteLine("\n1. Show Bookings\n2. Create Booking\n3. Exit");
                Console.Write("Choose an option: ");
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                if (choice == 1)
                {
                    var bookings = BookingManager.LoadBookings();
                    if (bookings.Count == 0)
                    {
                        Console.WriteLine("No bookings found.");
                    }
                    else
                    {
                        Console.WriteLine("\nBooking History:");
                        foreach (var booking in bookings)
                        {
                            Console.WriteLine($"Booking ID: {booking.BookingId}, Musical ID: {booking.MusicalId}, Tickets: {booking.Quantity}, Total Price: {booking.TotalPrice:C}");
                        }
                    }
                }
                else if (choice == 2)
                {
                    BookingManager.CreateBooking();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
