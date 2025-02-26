using System;
using System.Collections.Generic;
using System.IO;

namespace MusicalServices
{
    public class Musical
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; } // Ticket price
    }

    public static class MusicalCatalog
    {
        private static string filePath = @"C:\Users\30111549\source\repos\BookMyShow\musicals.txt"; // ✅ Fixed Path

        public static List<Musical> LoadMusicals()
        {
            List<Musical> musicals = new List<Musical>();
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 3 &&
                        int.TryParse(parts[0], out int id) &&
                        decimal.TryParse(parts[2], out decimal price))
                    {
                        musicals.Add(new Musical { Id = id, Name = parts[1], Price = price });
                    }
                }
            }
            return musicals;
        }

        public static void SaveMusicals(List<Musical> musicals)
        {
            List<string> lines = new List<string>();
            foreach (var musical in musicals)
            {
                lines.Add($"{musical.Id}|{musical.Name}|{musical.Price}");
            }
            File.WriteAllLines(filePath, lines);
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

            List<Musical> musicals = LoadMusicals();
            int newId = (musicals.Count > 0) ? musicals[^1].Id + 1 : 1; // Auto-increment ID
            musicals.Add(new Musical { Id = newId, Name = name, Price = price });

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
                            Console.WriteLine($"ID: {musical.Id}, Name: {musical.Name}, Ticket Price: ${musical.Price}");
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
