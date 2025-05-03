using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using HotelManagementSystem.Database;

namespace HotelManagementSystem
{
    public class AddBookingForm : Form
    {
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool BookingAdded { get; private set; } = false;
        private ComboBox cmbUser = null!;
        private ComboBox cmbRoom = null!;
        private DateTimePicker dtpCheckIn = null!;
        private DateTimePicker dtpCheckOut = null!;
        private Button btnOK = null!;
        private Button btnCancel = null!;
        private Panel newUserPanel = null!;
        private TextBox txtNewUserName = null!;
        private TextBox txtNewUserEmail = null!;
        private TextBox txtNewUserPassword = null!;
        private ComboBox cmbNewUserRole = null!;

        public AddBookingForm()
        {
            InitializeComponent();
            LoadUsers();
            LoadAvailableRooms();
        }

        private void InitializeComponent()
        {
            this.Text = "Add Booking";
            this.Width = 350;
            this.Height = 420;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var lblUser = new Label { Text = "User:", Left = 20, Top = 20, Width = 80 };
            cmbUser = new ComboBox { Left = 120, Top = 20, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbUser.SelectedIndexChanged += CmbUser_SelectedIndexChanged;
            var lblRoom = new Label { Text = "Room:", Left = 20, Top = 60, Width = 80 };
            cmbRoom = new ComboBox { Left = 120, Top = 60, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            var lblCheckIn = new Label { Text = "Check-In:", Left = 20, Top = 100, Width = 80 };
            dtpCheckIn = new DateTimePicker { Left = 120, Top = 100, Width = 180 };
            var lblCheckOut = new Label { Text = "Check-Out:", Left = 20, Top = 140, Width = 80 };
            dtpCheckOut = new DateTimePicker { Left = 120, Top = 140, Width = 180 };

            // New User Panel
            newUserPanel = new Panel { Left = 20, Top = 180, Width = 300, Height = 150, Visible = false };
            var lblNewUser = new Label { Text = "New User Details", Top = 0, Width = 200 };
            var lblNewUserName = new Label { Text = "Name:", Top = 30, Width = 80 };
            txtNewUserName = new TextBox { Left = 90, Top = 30, Width = 180, Name = "txtNewUserName" };
            var lblNewUserEmail = new Label { Text = "Email:", Top = 60, Width = 80 };
            txtNewUserEmail = new TextBox { Left = 90, Top = 60, Width = 180, Name = "txtNewUserEmail" };
            var lblNewUserPassword = new Label { Text = "Password:", Top = 90, Width = 80 };
            txtNewUserPassword = new TextBox { Left = 90, Top = 90, Width = 180, UseSystemPasswordChar = true, Name = "txtNewUserPassword" };
            var lblNewUserRole = new Label { Text = "Role:", Top = 120, Width = 80 };
            cmbNewUserRole = new ComboBox { Left = 90, Top = 120, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList, Name = "cmbNewUserRole" };
            cmbNewUserRole.Items.AddRange(new string[] { "Guest", "Staff", "Admin" });
            cmbNewUserRole.SelectedIndex = 0;
            newUserPanel.Controls.Add(lblNewUser);
            newUserPanel.Controls.Add(lblNewUserName); newUserPanel.Controls.Add(txtNewUserName);
            newUserPanel.Controls.Add(lblNewUserEmail); newUserPanel.Controls.Add(txtNewUserEmail);
            newUserPanel.Controls.Add(lblNewUserPassword); newUserPanel.Controls.Add(txtNewUserPassword);
            newUserPanel.Controls.Add(lblNewUserRole); newUserPanel.Controls.Add(cmbNewUserRole);
            foreach (Control c in newUserPanel.Controls) if (c != lblNewUser) c.Left += 0; // align

            btnOK = new Button { Text = "OK", Left = 120, Top = 350, Width = 80 };
            btnCancel = new Button { Text = "Cancel", Left = 220, Top = 350, Width = 80 };

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblUser);
            this.Controls.Add(cmbUser);
            this.Controls.Add(lblRoom);
            this.Controls.Add(cmbRoom);
            this.Controls.Add(lblCheckIn);
            this.Controls.Add(dtpCheckIn);
            this.Controls.Add(lblCheckOut);
            this.Controls.Add(dtpCheckOut);
            this.Controls.Add(newUserPanel);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        private void LoadUsers()
        {
            cmbUser.Items.Clear();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT UserID, Name FROM Users";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbUser.Items.Add(new ComboBoxItem(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            cmbUser.Items.Add(new ComboBoxItem(-1, "<Add New User>"));
            if (cmbUser.Items.Count > 0) cmbUser.SelectedIndex = 0;
        }

        private void LoadAvailableRooms()
        {
            cmbRoom.Items.Clear();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT RoomID, RoomType FROM Rooms WHERE IsAvailable = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbRoom.Items.Add(new ComboBoxItem(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            if (cmbRoom.Items.Count > 0) cmbRoom.SelectedIndex = 0;
        }

        private void CmbUser_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbUser.SelectedItem is ComboBoxItem item && item.Id == -1)
                newUserPanel.Visible = true;
            else
                newUserPanel.Visible = false;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            int userId;
            if (cmbUser.SelectedItem is ComboBoxItem userItem && userItem.Id == -1)
            {
                // Add new user
                if (string.IsNullOrWhiteSpace(txtNewUserName.Text) || string.IsNullOrWhiteSpace(txtNewUserEmail.Text) || string.IsNullOrWhiteSpace(txtNewUserPassword.Text))
                {
                    MessageBox.Show("Please fill in all new user fields.");
                    return;
                }
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO Users (Name, Email, Password, Role) VALUES (@name, @email, @password, @role)";
                    cmd.Parameters.AddWithValue("@name", txtNewUserName.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtNewUserEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", PasswordHelper.HashPassword(txtNewUserPassword.Text.Trim()));
                    cmd.Parameters.AddWithValue("@role", cmbNewUserRole.SelectedItem?.ToString() ?? "Guest");
                    cmd.ExecuteNonQuery();
                    // Get new user ID
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    userId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            else if (cmbUser.SelectedItem is ComboBoxItem existingUser)
            {
                userId = existingUser.Id;
            }
            else
            {
                MessageBox.Show("Please select a user.");
                return;
            }
            if (cmbRoom.SelectedItem == null)
            {
                MessageBox.Show("Please select a room.");
                return;
            }
            if (dtpCheckOut.Value <= dtpCheckIn.Value)
            {
                MessageBox.Show("Check-out must be after check-in.");
                return;
            }
            var room = (ComboBoxItem)cmbRoom.SelectedItem;
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Bookings (UserID, RoomID, CheckIn, CheckOut, Status) VALUES (@user, @room, @checkin, @checkout, 'Reserved')";
                cmd.Parameters.AddWithValue("@user", userId);
                cmd.Parameters.AddWithValue("@room", room.Id);
                cmd.Parameters.AddWithValue("@checkin", dtpCheckIn.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@checkout", dtpCheckOut.Value.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();
                // Mark room as unavailable
                var cmd2 = conn.CreateCommand();
                cmd2.CommandText = "UPDATE Rooms SET IsAvailable = 0 WHERE RoomID = @RoomID";
                cmd2.Parameters.AddWithValue("@RoomID", room.Id);
                cmd2.ExecuteNonQuery();
            }
            BookingAdded = true;
            this.Close();
        }
    }
} 