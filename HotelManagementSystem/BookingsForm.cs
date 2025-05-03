using System;
using System.Data;
using System.Windows.Forms;
using HotelManagementSystem.Database;
using Microsoft.Data.Sqlite;
using System.Drawing;

namespace HotelManagementSystem
{
    public class BookingsForm : UserControl
    {
        private DataGridView dgvBookings = null!;
        private Button btnAddBooking = null!;
        private Button btnCheckIn = null!;
        private Button btnCheckOut = null!;
        private Button btnRefresh = null!;

        public BookingsForm()
        {
            InitializeComponent();
            LoadBookings();
        }

        private void InitializeComponent()
        {
            dgvBookings = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 300,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold) },
                DefaultCellStyle = { Font = new Font("Segoe UI", 11), SelectionBackColor = Color.FromArgb(220, 220, 220) }
            };
            dgvBookings.EnableHeadersVisualStyles = false;
            btnAddBooking = new Button { Text = "Add Booking", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(10) };
            btnCheckIn = new Button { Text = "Check-In", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(10) };
            btnCheckOut = new Button { Text = "Check-Out", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(255, 193, 7), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(10) };
            btnRefresh = new Button { Text = "Refresh", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(10) };

            btnAddBooking.FlatAppearance.BorderSize = 0;
            btnCheckIn.FlatAppearance.BorderSize = 0;
            btnCheckOut.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatAppearance.BorderSize = 0;

            btnAddBooking.Click += BtnAddBooking_Click;
            btnCheckIn.Click += BtnCheckIn_Click;
            btnCheckOut.Click += BtnCheckOut_Click;
            btnRefresh.Click += (s, e) => LoadBookings();
            dgvBookings.CellContentClick += DgvBookings_CellContentClick;

            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnCheckOut);
            this.Controls.Add(btnCheckIn);
            this.Controls.Add(btnAddBooking);
            this.Controls.Add(dgvBookings);
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void LoadBookings()
        {
            dgvBookings.Rows.Clear();
            dgvBookings.Columns.Clear();
            dgvBookings.Columns.Add("BookingID", "Booking ID");
            dgvBookings.Columns.Add("UserID", "User ID");
            dgvBookings.Columns.Add("RoomID", "Room ID");
            dgvBookings.Columns.Add("CheckIn", "Check-In");
            dgvBookings.Columns.Add("CheckOut", "Check-Out");
            dgvBookings.Columns.Add("Status", "Status");
            var editBtn = new DataGridViewButtonColumn { Name = "Edit", Text = "Edit", UseColumnTextForButtonValue = true };
            var deleteBtn = new DataGridViewButtonColumn { Name = "Delete", Text = "Delete", UseColumnTextForButtonValue = true };
            var invoiceBtn = new DataGridViewButtonColumn { Name = "Invoice", Text = "Invoice", UseColumnTextForButtonValue = true };
            dgvBookings.Columns.Add(editBtn);
            dgvBookings.Columns.Add(deleteBtn);
            dgvBookings.Columns.Add(invoiceBtn);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT BookingID, UserID, RoomID, CheckIn, CheckOut, Status FROM Bookings";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dgvBookings.Rows.Add(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetString(5)
                        );
                    }
                }
            }
        }

        private void DgvBookings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var bookingId = dgvBookings.Rows[e.RowIndex].Cells["BookingID"].Value?.ToString();
            if (dgvBookings.Columns[e.ColumnIndex].Name == "Edit")
            {
                EditBooking(bookingId);
            }
            else if (dgvBookings.Columns[e.ColumnIndex].Name == "Delete")
            {
                DeleteBooking(bookingId);
            }
            else if (dgvBookings.Columns[e.ColumnIndex].Name == "Invoice")
            {
                ShowInvoice(bookingId);
            }
        }

        private void EditBooking(string? bookingId)
        {
            if (string.IsNullOrEmpty(bookingId)) return;
            // Fetch booking data
            int userId = 0, roomId = 0;
            string checkIn = "", checkOut = "", status = "";
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT UserID, RoomID, CheckIn, CheckOut, Status FROM Bookings WHERE BookingID = @id";
                cmd.Parameters.AddWithValue("@id", bookingId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userId = reader.GetInt32(0);
                        roomId = reader.GetInt32(1);
                        checkIn = reader.GetString(2);
                        checkOut = reader.GetString(3);
                        status = reader.GetString(4);
                    }
                }
            }
            var editForm = new AddBookingForm();
            // Pre-fill fields
            var userBox = editForm.Controls["cmbUser"] as ComboBox;
            var roomBox = editForm.Controls["cmbRoom"] as ComboBox;
            var checkInBox = editForm.Controls["dtpCheckIn"] as DateTimePicker;
            var checkOutBox = editForm.Controls["dtpCheckOut"] as DateTimePicker;
            if (userBox != null)
            {
                for (int i = 0; i < userBox.Items.Count; i++)
                {
                    var item = userBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Id == userId)
                    {
                        userBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            if (roomBox != null)
            {
                for (int i = 0; i < roomBox.Items.Count; i++)
                {
                    var item = roomBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Id == roomId)
                    {
                        roomBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            if (checkInBox != null) checkInBox.Value = DateTime.TryParse(checkIn, out var ci) ? ci : DateTime.Now;
            if (checkOutBox != null) checkOutBox.Value = DateTime.TryParse(checkOut, out var co) ? co : DateTime.Now;
            editForm.Text = "Edit Booking";
            editForm.ShowDialog();
            if (editForm.BookingAdded)
            {
                // Update booking
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE Bookings SET UserID=@user, RoomID=@room, CheckIn=@checkin, CheckOut=@checkout WHERE BookingID=@id";
                    cmd.Parameters.AddWithValue("@user", (userBox?.SelectedItem as dynamic)?.Id ?? userId);
                    cmd.Parameters.AddWithValue("@room", (roomBox?.SelectedItem as dynamic)?.Id ?? roomId);
                    cmd.Parameters.AddWithValue("@checkin", checkInBox?.Value.ToString("yyyy-MM-dd") ?? checkIn);
                    cmd.Parameters.AddWithValue("@checkout", checkOutBox?.Value.ToString("yyyy-MM-dd") ?? checkOut);
                    cmd.Parameters.AddWithValue("@id", bookingId);
                    cmd.ExecuteNonQuery();
                }
                LoadBookings();
            }
        }

        private void DeleteBooking(string? bookingId)
        {
            if (string.IsNullOrEmpty(bookingId)) return;
            if (MessageBox.Show("Are you sure you want to delete this booking?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM Bookings WHERE BookingID=@id";
                    cmd.Parameters.AddWithValue("@id", bookingId);
                    cmd.ExecuteNonQuery();
                }
                LoadBookings();
            }
        }

        private void BtnAddBooking_Click(object sender, EventArgs e)
        {
            var addForm = new AddBookingForm();
            addForm.ShowDialog();
            if (addForm.BookingAdded)
            {
                LoadBookings();
            }
        }

        private void BtnCheckIn_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a booking to check-in.");
                return;
            }
            var row = dgvBookings.SelectedRows[0];
            int bookingId = Convert.ToInt32(row.Cells["BookingID"].Value);
            // Show payment dialog
            var paymentForm = new PaymentForm();
            if (paymentForm.ShowDialog() == DialogResult.OK && paymentForm.PaymentSaved)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Save payment
                    var payCmd = conn.CreateCommand();
                    payCmd.CommandText = "INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, Date) VALUES (@bid, @amt, @method, @status, @date)";
                    payCmd.Parameters.AddWithValue("@bid", bookingId);
                    payCmd.Parameters.AddWithValue("@amt", paymentForm.Amount);
                    payCmd.Parameters.AddWithValue("@method", paymentForm.PaymentMethod);
                    payCmd.Parameters.AddWithValue("@status", paymentForm.Status);
                    payCmd.Parameters.AddWithValue("@date", paymentForm.PaymentDate.ToString("yyyy-MM-dd"));
                    payCmd.ExecuteNonQuery();
                    // Update booking status
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE Bookings SET Status = 'Checked-in' WHERE BookingID = @BookingID";
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Checked in and payment recorded!");
                LoadBookings();
            }
        }

        private void BtnCheckOut_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a booking to check-out.");
                return;
            }
            var row = dgvBookings.SelectedRows[0];
            int bookingId = Convert.ToInt32(row.Cells["BookingID"].Value);
            // Show payment dialog
            var paymentForm = new PaymentForm();
            if (paymentForm.ShowDialog() == DialogResult.OK && paymentForm.PaymentSaved)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Save payment
                    var payCmd = conn.CreateCommand();
                    payCmd.CommandText = "INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, Date) VALUES (@bid, @amt, @method, @status, @date)";
                    payCmd.Parameters.AddWithValue("@bid", bookingId);
                    payCmd.Parameters.AddWithValue("@amt", paymentForm.Amount);
                    payCmd.Parameters.AddWithValue("@method", paymentForm.PaymentMethod);
                    payCmd.Parameters.AddWithValue("@status", paymentForm.Status);
                    payCmd.Parameters.AddWithValue("@date", paymentForm.PaymentDate.ToString("yyyy-MM-dd"));
                    payCmd.ExecuteNonQuery();
                    // Update booking status
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE Bookings SET Status = 'Checked-out' WHERE BookingID = @BookingID";
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Checked out and payment recorded!");
                LoadBookings();
            }
        }

        private void ShowInvoice(string? bookingId)
        {
            if (string.IsNullOrEmpty(bookingId)) return;
            MessageBox.Show($"Invoice for Booking ID: {bookingId}\n(Invoice details coming soon.)", "Invoice");
        }
    }
} 