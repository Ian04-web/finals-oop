using System;
using Microsoft.Data.Sqlite;
using System.IO;

namespace HotelManagementSystem.Database
{
    public class DatabaseHelper
    {
        private const string DatabaseFile = "hotel.db";
        private const string ConnectionString = "Data Source=" + DatabaseFile + ";";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(DatabaseFile))
            {
                File.Create(DatabaseFile).Dispose();
            }

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                // Users table
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Users (
                    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL
                );";
                cmd.ExecuteNonQuery();
                // Rooms table
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Rooms (
                    RoomID INTEGER PRIMARY KEY AUTOINCREMENT,
                    RoomType TEXT NOT NULL,
                    PricePerNight REAL NOT NULL,
                    IsAvailable INTEGER NOT NULL
                );";
                cmd.ExecuteNonQuery();
                // Bookings table
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Bookings (
                    BookingID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER NOT NULL,
                    RoomID INTEGER NOT NULL,
                    CheckIn TEXT NOT NULL,
                    CheckOut TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    FOREIGN KEY(UserID) REFERENCES Users(UserID),
                    FOREIGN KEY(RoomID) REFERENCES Rooms(RoomID)
                );";
                cmd.ExecuteNonQuery();
                // Invoices table
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Invoices (
                    InvoiceID INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookingID INTEGER NOT NULL,
                    Amount REAL NOT NULL,
                    DateIssued TEXT NOT NULL,
                    FOREIGN KEY(BookingID) REFERENCES Bookings(BookingID)
                );";
                cmd.ExecuteNonQuery();
                // Create Payments table
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Payments (
                    PaymentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookingID INTEGER NOT NULL,
                    Amount REAL NOT NULL,
                    PaymentMethod TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    Date TEXT NOT NULL,
                    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
                )";
                cmd.ExecuteNonQuery();
            }
            SeedInitialData();
        }

        private static void SeedInitialData()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                // Seed Rooms if empty
                cmd.CommandText = "SELECT COUNT(*) FROM Rooms";
                long roomCount = (long)cmd.ExecuteScalar();
                if (roomCount == 0)
                {
                    cmd.CommandText = "INSERT INTO Rooms (RoomType, PricePerNight, IsAvailable) VALUES (@type, @price, 1)";
                    cmd.Parameters.AddWithValue("@type", "Single");
                    cmd.Parameters.AddWithValue("@price", 50);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters["@type"].Value = "Double";
                    cmd.Parameters["@price"].Value = 80;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters["@type"].Value = "Suite";
                    cmd.Parameters["@price"].Value = 150;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters["@type"].Value = "Deluxe";
                    cmd.Parameters["@price"].Value = 200;
                    cmd.ExecuteNonQuery();
                }
                // Seed Users if empty
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT COUNT(*) FROM Users";
                long userCount = (long)cmd.ExecuteScalar();
                if (userCount == 0)
                {
                    cmd.CommandText = "INSERT INTO Users (Name, Email, Password, Role) VALUES (@name, @email, @password, @role)";
                    cmd.Parameters.AddWithValue("@name", "Admin");
                    cmd.Parameters.AddWithValue("@email", "Admin");
                    cmd.Parameters.AddWithValue("@password", HotelManagementSystem.PasswordHelper.HashPassword("123"));
                    cmd.Parameters.AddWithValue("@role", "Admin");
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}   