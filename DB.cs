using System;
using MySql.Data.MySqlClient;

namespace OOP_backend_test
{
    public static class Database
    {
        // ─── connection string
        private const string ConnectionString =
            "Server=localhost;Database=oop_rental;User ID=root;Password=HuKr1k1k1FrMAKTN;";

        public static bool LoginAttempt(string loginHandle, string password)
        {
            // check empty inputs
            if (string.IsNullOrWhiteSpace(loginHandle) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Empty input fields!");
                return false;
            }

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    string sql = @"SELECT `Password`
                                   FROM   `userssecurity`
                                   WHERE  `LoginHandle` = @handle
                                   LIMIT  1";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@handle", loginHandle);

                        object result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            // No matching LoginHandle
                            Console.WriteLine("Incorrect login and/or password!");
                            return false;
                        }

                        string storedPassword = result.ToString();

                        if (storedPassword != password)
                        {
                            Console.WriteLine("Incorrect login and/or password!");
                            return false;
                        }

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LoginAttempt ERROR] {ex.Message}");
                    return false;
                }
            }
        }

        public static bool RegisterAttempt(string name, string surname, string loginHandle, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    //Check for duplicate LoginHandle
                    string checkSql = @"SELECT COUNT(*) FROM `userssecurity`
                                        WHERE `LoginHandle` = @handle";

                    using (MySqlCommand checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@handle", loginHandle);
                        long count = (long)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            Console.WriteLine("Duplicate logins, please attempt another!");
                            return false;
                        }
                    }

                    //Insert into UsersTable
                    //Name / Surname default to NULL if empty.
                    string? insertedName    = string.IsNullOrWhiteSpace(name)    ? null : name;
                    string? insertedSurname = string.IsNullOrWhiteSpace(surname) ? null : surname;

                    string insertUserSql = @"INSERT INTO `userstable`
                                                 (`Balance`, `IsBanned`, `Location`, `Name`, `Surname`)
                                             VALUES
                                                 (0, 0, POINT(0,0), @name, @surname)";

                    int newUserID;
                    using (MySqlCommand insertUser = new MySqlCommand(insertUserSql, conn))
                    {
                        insertUser.Parameters.AddWithValue("@name",    (object?)insertedName    ?? DBNull.Value);
                        insertUser.Parameters.AddWithValue("@surname", (object?)insertedSurname ?? DBNull.Value);

                        insertUser.ExecuteNonQuery();
                        newUserID = (int)insertUser.LastInsertedId;
                    }

                    //Insert into UsersSecurity
                    string insertSecSql = @"INSERT INTO `userssecurity`
                                                (`UserID`, `LoginHandle`, `Password`, `IsAdmin`)
                                            VALUES
                                                (@userId, @handle, @password, 0)";

                    using (MySqlCommand insertSec = new MySqlCommand(insertSecSql, conn))
                    {
                        insertSec.Parameters.AddWithValue("@userId",   newUserID);
                        insertSec.Parameters.AddWithValue("@handle",   loginHandle);
                        insertSec.Parameters.AddWithValue("@password", password);

                        insertSec.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RegisterAttempt ERROR] {ex.Message}");
                    return false;
                }
            }
        }

        public static void TestConnection()
        {
            MySqlConnection newthing = new MySqlConnection(ConnectionString);
            using (newthing)
            {
                try
                {
                    newthing.Open();
                    readAllTable(newthing, "testing");

                    string sqlCommand = "insert into `testing` (id, name, model) values (@id, @name, @model)";
                    using (MySqlCommand cmd = new MySqlCommand(sqlCommand, newthing))
                    {
                        Console.Write("ID:  ");
                        cmd.Parameters.AddWithValue("@id", Console.ReadLine());
                        Console.Write("NAME:  ");
                        cmd.Parameters.AddWithValue("@name", Console.ReadLine());
                        Console.Write("MODEL:  ");
                        cmd.Parameters.AddWithValue("@model", Console.ReadLine());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} row(s) inserted.");
                    }
                    readAllTable(newthing, "testing");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        public static void readAllTable(MySqlConnection newthing, string Table)
        {
            try
            {
                string sqlCommand = $"select * from {Table}";
                using (MySqlCommand cmd = new MySqlCommand(sqlCommand, newthing))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        Console.WriteLine($"ID: {reader["id"]}, Name: {reader["name"]}, Model: {reader["model"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static void writeToLog(MySqlConnection newthing, string type, int objId, int userid,
                                      float fee, TimeSpan renttime, DateTime starttime)
        {
            try
            {
                string sqlCommand =
                    @"INSERT INTO `logging` (id, userid, Type, OBJ_id, fee, `renting time`, starttime)
                      VALUES (null, @userid, @type, @objId, @fee, @rentTime, @startTime)";

                using (MySqlCommand cmd = new MySqlCommand(sqlCommand, newthing))
                {
                    cmd.Parameters.AddWithValue("@userid",   userid);
                    cmd.Parameters.AddWithValue("@type",     type);
                    cmd.Parameters.AddWithValue("@objId",    objId);
                    cmd.Parameters.AddWithValue("@fee",      fee);
                    cmd.Parameters.AddWithValue("@rentTime", renttime);
                    cmd.Parameters.AddWithValue("@startTime",starttime);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} row(s) inserted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
