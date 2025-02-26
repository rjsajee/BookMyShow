using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MusicalServices
{
    public class Musical
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int SeatsAvailable { get; set; }
        public string Venue { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }

    public static class MusicalCatalog
    {
        private static string filePath = @"C:\Users\30111549\source\repos\BookMyShow\musicals.txt";

        public static List<Musical> LoadMusicals()
        {
            List<Musical> musicals = new List<Musical>();
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 7 && // ✅ Ensuring correct field count
                        int.TryParse(parts[0], out int id) &&
                        decimal.TryParse(parts[2], out decimal price) &&
                        int.TryParse(parts[3], out int seats))
                    {
                        musicals.Add(new Musical
                        {
                            Id = id,
                            Name = parts[1],
                            Price = price,
                            SeatsAvailable = seats,
                            Venue = parts[4],
                            Date = parts[5],
                            Time = parts[6]
                        });
                    }
                }
            }
            return musicals;
        }

        public static void SaveMusicals(List<Musical> musicals)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false)) // ✅ Ensures proper write operation
            {
                foreach (var musical in musicals)
                {
                    writer.WriteLine($"{musical.Id}|{musical.Name}|{musical.Price}|{musical.SeatsAvailable}|{musical.Venue}|{musical.Date}|{musical.Time}");
                }
                writer.Flush(); // ✅ Ensures data is written to disk
            }
        }

        public static void AddMusical()
        {
            Console.Write("Enter Musical Name: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter Ticket Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
            {
                Console.WriteLine("Invalid price. Must be a positive number.");
                return;
            }

            Console.Write("Enter Number of Seats Available: ");
            if (!int.TryParse(Console.ReadLine(), out int seats) || seats <= 0)
            {
                Console.WriteLine("Invalid number of seats. Must be a positive number.");
                return;
            }

            Console.Write("Enter Venue: ");
            string venue = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter Date (YYYY-MM-DD): ");
            string date = Console.ReadLine()?.Trim() ?? "";
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                Console.WriteLine("Invalid date format. Use YYYY-MM-DD.");
                return;
            }

            Console.Write("Enter Time (HH:mm): ");
            string time = Console.ReadLine()?.Trim() ?? "";
            if (!DateTime.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                Console.WriteLine("Invalid time format. Use HH:mm (24-hour format).");
                return;
            }

            List<Musical> musicals = LoadMusicals();
            int newId = (musicals.Count > 0) ? musicals[^1].Id + 1 : 1;
            musicals.Add(new Musical
            {
                Id = newId,
                Name = name,
                Price = price,
                SeatsAvailable = seats,
                Venue = venue,
                Date = date,
                Time = time
            });

            SaveMusicals(musicals);
            Console.WriteLine("Musical added successfully!");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("MusicalService is running...");

            while (true)
            {
                Console.WriteLine("\n1. Show Musicals\n2. Add Musical\n3. Exit");
                Console.Write("Choose an option: ");
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                if (choice == 1)
                {
                    var musicals = MusicalCatalog.LoadMusicals();
                    if (musicals.Count == 0)
                    {
                        Console.WriteLine("No musicals available.");
                    }
                    else
                    {
                        Console.WriteLine("\nAvailable Musicals:");
                        foreach (var musical in musicals)
                        {
                            Console.WriteLine($"ID: {musical.Id}, Name: {musical.Name}, Price: ${musical.Price}, " +
                                              $"Seats: {musical.SeatsAvailable}, Venue: {musical.Venue}, " +
                                              $"Date: {musical.Date}, Time: {musical.Time}");
                        }
                    }
                }
                else if (choice == 2)
                {
                    MusicalCatalog.AddMusical();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
