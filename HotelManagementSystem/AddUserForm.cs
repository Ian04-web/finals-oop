using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using HotelManagementSystem.Database;

namespace HotelManagementSystem
{
    public class AddUserForm : Form
    {
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool UserAdded { get; private set; } = false;
        private TextBox txtName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPassword = null!;
        private ComboBox cmbRole = null!;
        private Button btnOK = null!;
        private Button btnCancel = null!;

        public AddUserForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Add User";
            this.Width = 350;
            this.Height = 270;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var lblName = new Label { Text = "Name:", Left = 20, Top = 20, Width = 80 };
            txtName = new TextBox { Left = 120, Top = 20, Width = 180 };
            var lblEmail = new Label { Text = "Email:", Left = 20, Top = 60, Width = 80 };
            txtEmail = new TextBox { Left = 120, Top = 60, Width = 180 };
            var lblPassword = new Label { Text = "Password:", Left = 20, Top = 100, Width = 80 };
            txtPassword = new TextBox { Left = 120, Top = 100, Width = 180, UseSystemPasswordChar = true };
            var lblRole = new Label { Text = "Role:", Left = 20, Top = 140, Width = 80 };
            cmbRole = new ComboBox { Left = 120, Top = 140, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "Admin", "Staff", "Guest" });
            cmbRole.SelectedIndex = 0;

            btnOK = new Button { Text = "OK", Left = 120, Top = 180, Width = 80 };
            btnCancel = new Button { Text = "Cancel", Left = 220, Top = 180, Width = 80 };

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblRole);
            this.Controls.Add(cmbRole);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Users (Name, Email, Password, Role) VALUES (@name, @email, @password, @role)";
                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                cmd.Parameters.AddWithValue("@role", cmbRole.SelectedItem.ToString());
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return;
                }
            }
            UserAdded = true;
            this.Close();
        }
    }
} 