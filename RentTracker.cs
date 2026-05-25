using System;
using System.Collections.Concurrent;
using MySql.Data.MySqlClient;

namespace OOP_backend_test
{
    public static class Tracker
    {
        // ── Dictionary now stores: vehicleID → (startTime, userID) ──────────
        private static readonly ConcurrentDictionary<int, (DateTime StartTime, int UserID)>
            _activeRents = new ConcurrentDictionary<int, (DateTime, int)>();

        // ────────────────────────────────────────────────────────────────────
        // START RENT
        // Extra guards: IsBanned == 1  →  end early
        //               Balance   < 0  →  end early
        // ────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Begins a rental session for <paramref name="objId"/>.
        /// </summary>
        /// <param name="objId">Vehicle ID to rent.</param>
        /// <param name="userID">UserID of the active user.</param>
        /// <param name="isBanned">IsBanned flag of the active user (1 = banned).</param>
        /// <returns>
        /// A <see cref="StartRentResult"/> indicating success or the reason for failure.
        /// </returns>
        public static StartRentResult StartRent(int objId, int userID, int isBanned)
        {
            // Guard 1 – user is banned
            if (isBanned == 1)
            {
                string msg = "[ERROR] Your account has been banned. Rental is not permitted.";
                Console.WriteLine(msg);
                return new StartRentResult(false, msg);
            }

            // Guard 2 – user balance is negative
            //           We need to read the balance from the DB.
            float balance = GetUserBalance(userID);
            if (balance < 0)
            {
                string msg = "[ERROR] Your balance is negative. Please top up before renting.";
                Console.WriteLine(msg);
                return new StartRentResult(false, msg);
            }

            // Attempt to register the rental
            var entry = (DateTime.Now, userID);
            if (_activeRents.TryAdd(objId, entry))
            {
                Console.WriteLine($"[INFO] Rental started for Object ID {objId} by User {userID} at {entry.Item1}");
                return new StartRentResult(true, string.Empty);
            }
            else
            {
                string msg = $"[ERROR] Object ID {objId} is already currently being rented.";
                Console.WriteLine(msg);
                return new StartRentResult(false, msg);
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // FINISH RENT
        // ────────────────────────────────────────────────────────────────────
        public static void FinishRent(MySqlConnection newthing, int userId, string type, int objId, int fee)
        {
            if (_activeRents.TryRemove(objId, out var entry))
            {
                DateTime startTime = entry.StartTime;
                DateTime endTime   = DateTime.Now;
                TimeSpan duration  = endTime - startTime;

                try
                {
                    Database.writeToLog(newthing, type, objId, userId, fee, duration, startTime);
                    Console.WriteLine($"[SUCCESS] Rent finished for {type} (ID: {objId}). " +
                                      $"Duration: {duration:hh\\:mm\\:ss}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DB ERROR] Failed to log: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[ERROR] No active rental session found for Object ID {objId}.");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // Helper – fetch balance for a user
        // ────────────────────────────────────────────────────────────────────
        private static float GetUserBalance(int userID)
        {
            const string connectionString =
                "Server=localhost;Database=oop_rental;User ID=root;Password=HuKr1k1k1FrMAKTN;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT `Balance` FROM `userstable` WHERE `UserID` = @id LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userID);
                        object result = cmd.ExecuteScalar();
                        return result == null ? 0f : Convert.ToSingle(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserBalance ERROR] {ex.Message}");
                return 0f;   // safe default – will not block the rental
            }
        }
    }

    // ── Small result DTO returned by StartRent ───────────────────────────────
    public record StartRentResult(bool Success, string Message);   
}
