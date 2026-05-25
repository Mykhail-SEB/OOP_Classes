using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace OOP_backend_test
{
    public static class Program
    {
        public static void Main()
        {
            string connectionSTRING = "Server=localhost;Database=oop_rental;User ID=root;Password=HuKr1k1k1FrMAKTN;";
            MySqlConnection newthing = new MySqlConnection(connectionSTRING);
            

            using (newthing)
            {
                newthing.Open();
                while (true )
                {
                    Console.Read(); Console.Read();
                    try
                    {
                        MenuTestingThing(connectionSTRING, newthing);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }
        static string GetVehicleTypeIfValid(MySqlConnection newthing, int id)
        {
            {
                try
                {
                    // We select 'type' to verify existence and get the value simultaneously
                    string query = "SELECT type FROM vehicle WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, newthing))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        object result = cmd.ExecuteScalar();

                        // If result is not null, the ID exists
                        return result?.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking ID: {ex.Message}");
                    return null;
                }
            }
        }
        static void MenuTestingThing(string connectionSTRING, MySqlConnection newthing)
        {
            bool stopperthing = true;
            while (stopperthing)
            {
                try
                {
                    Console.WriteLine("\nPick a vehicle. ");

                    //get the IDs for the menu thing
                    int minId = 0;
                    int maxId = 0;
                    string rangeQuery = "SELECT MIN(id), MAX(id) FROM vehicle";

                    using (MySqlCommand cmd = new MySqlCommand(rangeQuery, newthing))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            minId = reader.GetInt32(0);
                            maxId = reader.GetInt32(1);
                        }
                        else
                        {
                            Console.WriteLine("The vehicle table is currently empty. 10 before retry.");
                            Thread.Sleep(10000);
                            continue;
                        }
                    }

                    //Take input from user
                    Console.Write($"Pick an ID from {minId} to {maxId}. Input of -1 will terminate the program. ");
                    string input = Console.ReadLine();

                    if (!int.TryParse(input, out int selectedId))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        continue;
                    }
                    if (selectedId == -1)
                    {
                        stopperthing = false;
                        continue;
                    }

                    //Check validity and get the vehicle type
                    string vehicleType = GetVehicleTypeIfValid(newthing, selectedId);
                    if (vehicleType != null)
                    {

                        Tracker.StartRent(selectedId);

                        Console.WriteLine("(2 seconds)");
                        Thread.Sleep(2000);

                        Tracker.FinishRent(newthing, 1, vehicleType, objId: selectedId, fee: 1);
                        Console.WriteLine("-----------------------------------");
                    }
                    else
                    {
                        Console.WriteLine($"ID {selectedId} does not exist in the database. Try again.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}
