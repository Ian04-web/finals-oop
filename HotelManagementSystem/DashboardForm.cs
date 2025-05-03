using System.Windows.Forms;
using System.Data;
using Microsoft.Data.Sqlite;
using HotelManagementSystem.Database;
using System.Drawing;

namespace HotelManagementSystem
{
    public class DashboardForm : Form
    {
        private Panel sidebarPanel = null!;
        private Button btnDashboard = null!;
        private Button btnRooms = null!;
        private Button btnBookings = null!;
        private Button btnPayments = null!;
        private Button btnUsers = null!;
        private Panel mainPanel = null!;
        private RoomsForm roomsForm = null!;
        private BookingsForm bookingsForm = null!;
        private UsersForm usersForm = null!;

        // Dashboard controls
        private Label lblWelcome = null!;
        private Panel summaryPanel = null!;
        private Label lblTotalBookings = null!;
        private Label lblCheckedIn = null!;
        private Label lblCheckedOut = null!;
        private DataGridView dgvRecentBookings = null!;

        public DashboardForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Hotel Management and Booking System";
            this.Width = 1100;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 11);

            sidebarPanel = new Panel { Width = 200, Dock = DockStyle.Left, BackColor = Color.FromArgb(34, 40, 49) };
            mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 245, 245) };

            // Logo/App Name
            var lblLogo = new Label { Text = "🏨 HotelSys", Dock = DockStyle.Top, Height = 70, ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            sidebarPanel.Controls.Add(lblLogo);

            btnDashboard = CreateSidebarButton("Dashboard", ""); // MDL2: Home
            btnRooms = CreateSidebarButton("Rooms", ""); // MDL2: Room
            btnBookings = CreateSidebarButton("Bookings", ""); // MDL2: Calendar
            btnPayments = CreateSidebarButton("Payments", ""); // MDL2: Money
            btnUsers = CreateSidebarButton("Users", ""); // MDL2: Contact

            btnDashboard.Click += (s, e) => { HighlightSidebar(btnDashboard); ShowDashboard(); };
            btnRooms.Click += (s, e) => { HighlightSidebar(btnRooms); ShowRooms(); };
            btnBookings.Click += (s, e) => { HighlightSidebar(btnBookings); ShowBookings(); };
            btnPayments.Click += (s, e) => { HighlightSidebar(btnPayments); ShowPayments(); };
            btnUsers.Click += (s, e) => { HighlightSidebar(btnUsers); ShowUsers(); };

            sidebarPanel.Controls.Add(btnUsers);
            sidebarPanel.Controls.Add(btnPayments);
            sidebarPanel.Controls.Add(btnBookings);
            sidebarPanel.Controls.Add(btnRooms);
            sidebarPanel.Controls.Add(btnDashboard);

            this.Controls.Add(mainPanel);
            this.Controls.Add(sidebarPanel);

            HighlightSidebar(btnDashboard);
            ShowDashboard();
        }

        private Button CreateSidebarButton(string text, string icon)
        {
            var btn = new Button
            {
                Text = $"  {icon}  {text}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(34, 40, 49),
                Cursor = Cursors.Hand
            };
            btn.MouseEnter += (s, e) => { if (!btn.Tag?.Equals("active") ?? true) btn.BackColor = Color.FromArgb(57, 62, 70); };
            btn.MouseLeave += (s, e) => { if (!btn.Tag?.Equals("active") ?? true) btn.BackColor = Color.FromArgb(34, 40, 49); };
            btn.Font = new Font("Segoe MDL2 Assets", 16, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            return btn;
        }

        private void HighlightSidebar(Button activeBtn)
        {
            foreach (Control ctrl in sidebarPanel.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = Color.FromArgb(34, 40, 49);
                    btn.Tag = null;
                }
            }
            activeBtn.BackColor = Color.FromArgb(0, 123, 255);
            activeBtn.Tag = "active";
        }

        private void ShowDashboard()
        {
            mainPanel.Controls.Clear();
            // Header
            lblWelcome = new Label { Text = "Welcome to Hotel Management Dashboard", Dock = DockStyle.Top, Height = 50, Font = new Font("Segoe UI", 18, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(20, 10, 0, 0), ForeColor = Color.FromArgb(34, 40, 49) };
            mainPanel.Controls.Add(lblWelcome);

            // Card-style Summary Panel
            summaryPanel = new Panel { Height = 120, Dock = DockStyle.Top, BackColor = Color.Transparent };
            lblTotalBookings = CreateSummaryCard("Total Bookings", Color.FromArgb(0, 123, 255), 30);
            lblCheckedIn = CreateSummaryCard("Checked-Ins", Color.FromArgb(40, 167, 69), 250);
            lblCheckedOut = CreateSummaryCard("Checked-Outs", Color.FromArgb(255, 193, 7), 470);
            summaryPanel.Controls.Add(lblTotalBookings);
            summaryPanel.Controls.Add(lblCheckedIn);
            summaryPanel.Controls.Add(lblCheckedOut);
            mainPanel.Controls.Add(summaryPanel);

            // Recent Bookings
            var lblRecent = new Label { Text = "Recent Bookings", Left = 20, Top = 160, Width = 300, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(34, 40, 49) };
            dgvRecentBookings = new DataGridView { Left = 20, Top = 200, Width = 900, Height = 250, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false, ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold) }, DefaultCellStyle = { Font = new Font("Segoe UI", 11), SelectionBackColor = Color.FromArgb(220, 220, 220) } };
            dgvRecentBookings.EnableHeadersVisualStyles = false;
            dgvRecentBookings.Columns.Add("Guest", "Guest");
            dgvRecentBookings.Columns.Add("Room", "Room");
            dgvRecentBookings.Columns.Add("CheckIn", "Check-In");
            dgvRecentBookings.Columns.Add("CheckOut", "Check-Out");
            dgvRecentBookings.Columns.Add("Status", "Status");
            mainPanel.Controls.Add(lblRecent);
            mainPanel.Controls.Add(dgvRecentBookings);

            LoadDashboardSummary();
            LoadRecentBookings();
        }

        private Label CreateSummaryCard(string title, Color color, int left)
        {
            return new Label
            {
                Text = $"{title}\n0",
                Width = 200,
                Height = 90,
                Left = left,
                Top = 15,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = color,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(0, 10, 0, 0),
                Margin = new Padding(10),
                FlatStyle = FlatStyle.Flat,
                Tag = title,
                // Rounded corners (simulate with region)
            };
        }

        private void LoadDashboardSummary()
        {
            int total = 0, checkedIn = 0, checkedOut = 0;
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM Bookings";
                total = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "SELECT COUNT(*) FROM Bookings WHERE Status = 'Checked-in'";
                checkedIn = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "SELECT COUNT(*) FROM Bookings WHERE Status = 'Checked-out'";
                checkedOut = Convert.ToInt32(cmd.ExecuteScalar());
            }
            lblTotalBookings.Text = $"Total Bookings\n{total}";
            lblCheckedIn.Text = $"Checked-Ins\n{checkedIn}";
            lblCheckedOut.Text = $"Checked-Outs\n{checkedOut}";
        }

        private void LoadRecentBookings()
        {
            dgvRecentBookings.Rows.Clear();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT U.Name, R.RoomType, B.CheckIn, B.CheckOut, B.Status
                                    FROM Bookings B
                                    JOIN Users U ON B.UserID = U.UserID
                                    JOIN Rooms R ON B.RoomID = R.RoomID
                                    ORDER BY B.BookingID DESC LIMIT 5";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dgvRecentBookings.Rows.Add(
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4)
                        );
                    }
                }
            }
        }

        private void ShowRooms()
        {
            mainPanel.Controls.Clear();
            if (roomsForm == null) roomsForm = new RoomsForm();
            mainPanel.Controls.Add(roomsForm);
        }

        private void ShowBookings()
        {
            mainPanel.Controls.Clear();
            if (bookingsForm == null) bookingsForm = new BookingsForm();
            mainPanel.Controls.Add(bookingsForm);
        }

        private void ShowPayments()
        {
            mainPanel.Controls.Clear();
            var label = new Label { Text = "Payments", Dock = DockStyle.Top, Font = new System.Drawing.Font("Segoe UI", 20), Height = 60 };
            mainPanel.Controls.Add(label);
            // Add payment/invoice controls here
        }

        private void ShowUsers()
        {
            mainPanel.Controls.Clear();
            if (usersForm == null) usersForm = new UsersForm();
            mainPanel.Controls.Add(usersForm);
        }
    }
} 