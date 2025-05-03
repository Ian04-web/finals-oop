using System;
using System.Data;
using System.Windows.Forms;
using HotelManagementSystem.Database;
using Microsoft.Data.Sqlite;
using System.Drawing;

namespace HotelManagementSystem
{
    public class UsersForm : UserControl
    {
        private DataGridView dgvUsers = null!;
        private Button btnAddUser = null!;
        private Button btnRefresh = null!;

        public UsersForm()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            dgvUsers = new DataGridView
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
            dgvUsers.EnableHeadersVisualStyles = false;
            btnAddUser = new Button { Text = "Add User", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(10) };
            btnRefresh = new Button { Text = "Refresh", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(10) };

            btnAddUser.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatAppearance.BorderSize = 0;

            btnAddUser.Click += BtnAddUser_Click;
            btnRefresh.Click += (s, e) => LoadUsers();
            dgvUsers.CellContentClick += DgvUsers_CellContentClick;

            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnAddUser);
            this.Controls.Add(dgvUsers);
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void LoadUsers()
        {
            dgvUsers.Rows.Clear();
            dgvUsers.Columns.Clear();
            dgvUsers.Columns.Add("UserID", "User ID");
            dgvUsers.Columns.Add("Name", "Name");
            dgvUsers.Columns.Add("Email", "Email");
            dgvUsers.Columns.Add("Role", "Role");
            var editBtn = new DataGridViewButtonColumn { Name = "Edit", Text = "Edit", UseColumnTextForButtonValue = true };
            var deleteBtn = new DataGridViewButtonColumn { Name = "Delete", Text = "Delete", UseColumnTextForButtonValue = true };
            dgvUsers.Columns.Add(editBtn);
            dgvUsers.Columns.Add(deleteBtn);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT UserID, Name, Email, Role FROM Users";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dgvUsers.Rows.Add(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3)
                        );
                    }
                }
            }
        }

        private void DgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var userId = dgvUsers.Rows[e.RowIndex].Cells["UserID"].Value?.ToString();
            if (dgvUsers.Columns[e.ColumnIndex].Name == "Edit")
            {
                EditUser(userId);
            }
            else if (dgvUsers.Columns[e.ColumnIndex].Name == "Delete")
            {
                DeleteUser(userId);
            }
        }

        private void EditUser(string? userId)
        {
            if (string.IsNullOrEmpty(userId)) return;
            // Fetch user data
            string name = "", email = "", role = "";
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT Name, Email, Role FROM Users WHERE UserID = @id";
                cmd.Parameters.AddWithValue("@id", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        name = reader.GetString(0);
                        email = reader.GetString(1);
                        role = reader.GetString(2);
                    }
                }
            }
            var editForm = new AddUserForm();
            // Pre-fill fields
            var nameBox = editForm.Controls["txtName"] as TextBox;
            var emailBox = editForm.Controls["txtEmail"] as TextBox;
            var roleBox = editForm.Controls["cmbRole"] as ComboBox;
            if (nameBox != null) nameBox.Text = name;
            if (emailBox != null) emailBox.Text = email;
            if (roleBox != null) roleBox.SelectedItem = role;
            // Hide password field for edit
            var pwdBox = editForm.Controls["txtPassword"] as TextBox;
            if (pwdBox != null) pwdBox.Visible = false;
            editForm.Text = "Edit User";
            editForm.ShowDialog();
            if (editForm.UserAdded)
            {
                // Update user
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE Users SET Name=@name, Email=@email, Role=@role WHERE UserID=@id";
                    cmd.Parameters.AddWithValue("@name", nameBox?.Text ?? "");
                    cmd.Parameters.AddWithValue("@email", emailBox?.Text ?? "");
                    cmd.Parameters.AddWithValue("@role", roleBox?.SelectedItem?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
                LoadUsers();
            }
        }

        private void DeleteUser(string? userId)
        {
            if (string.IsNullOrEmpty(userId)) return;
            if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM Users WHERE UserID=@id";
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
                LoadUsers();
            }
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            var addForm = new AddUserForm();
            addForm.ShowDialog();
            if (addForm.UserAdded)
            {
                LoadUsers();
            }
        }
    }
} 