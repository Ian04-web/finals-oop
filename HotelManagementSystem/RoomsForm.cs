using System;
using System.Data;
using System.Windows.Forms;
using HotelManagementSystem.Models;
using HotelManagementSystem.Database;
using Microsoft.Data.Sqlite;
using System.Drawing;

namespace HotelManagementSystem
{
    public class RoomsForm : UserControl
    {
        private DataGridView dgvRooms;
        private Button btnBookRoom;
        private Button btnRefresh;

        public RoomsForm()
        {
            InitializeComponent();
            LoadRooms();
        }

        private void InitializeComponent()
        {
            dgvRooms = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 350,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold) },
                DefaultCellStyle = { Font = new Font("Segoe UI", 11), SelectionBackColor = Color.FromArgb(220, 220, 220) }
            };
            dgvRooms.EnableHeadersVisualStyles = false;
            btnBookRoom = new Button { Text = "Book Selected Room", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(10) };
            btnRefresh = new Button { Text = "Refresh", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(10) };

            btnBookRoom.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatAppearance.BorderSize = 0;

            btnBookRoom.Click += BtnBookRoom_Click;
            btnRefresh.Click += (s, e) => LoadRooms();

            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnBookRoom);
            this.Controls.Add(dgvRooms);
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void LoadRooms()
        {
            dgvRooms.Rows.Clear();
            dgvRooms.Columns.Clear();
            dgvRooms.Columns.Add("RoomID", "Room ID");
            dgvRooms.Columns.Add("RoomType", "Room Type");
            dgvRooms.Columns.Add("PricePerNight", "Price Per Night");
            dgvRooms.Columns.Add("IsAvailable", "Available");

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT RoomID, RoomType, PricePerNight, IsAvailable FROM Rooms";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dgvRooms.Rows.Add(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetDecimal(2),
                            reader.GetInt32(3) == 1 ? "Yes" : "No"
                        );
                    }
                }
            }
        }

        private void BtnBookRoom_Click(object sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a room to book.");
                return;
            }
            var row = dgvRooms.SelectedRows[0];
            int roomId = Convert.ToInt32(row.Cells["RoomID"].Value);
            string roomType = row.Cells["RoomType"].Value.ToString();
            decimal price = Convert.ToDecimal(row.Cells["PricePerNight"].Value);
            string available = row.Cells["IsAvailable"].Value.ToString();
            if (available != "Yes")
            {
                MessageBox.Show("Room is not available.");
                return;
            }
            // For demo: just mark as unavailable
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE Rooms SET IsAvailable = 0 WHERE RoomID = @RoomID";
                cmd.Parameters.AddWithValue("@RoomID", roomId);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show($"Room {roomId} booked!");
            LoadRooms();
        }
    }
} 