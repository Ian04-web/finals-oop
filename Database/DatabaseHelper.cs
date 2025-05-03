using System;
using System.Data.SQLite;
using System.IO;

namespace HotelManagementSystem.Database
{
    public class DatabaseHelper
    {
        private const string DatabaseFile = "hotel.db";
        private const string ConnectionString = "Data Source=" + DatabaseFile + ";Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(DatabaseFile))
            {
                SQLiteConnection.CreateFile(DatabaseFile);
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
            }
        }
    }
} 