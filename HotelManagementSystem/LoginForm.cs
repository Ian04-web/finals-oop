using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using HotelManagementSystem.Database;

namespace HotelManagementSystem
{
    public class LoginForm : Form
    {
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int? LoggedInUserId { get; private set; } = null;
        private TextBox txtEmail = null!;
        private TextBox txtPassword = null!;
        private Button btnLogin = null!;
        private Label lblError = null!;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Hotel Management Login";
            this.Width = 350;
            this.Height = 260;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Segoe UI", 11);

            var lblTitle = new Label { Text = "Hotel Management System", Left = 30, Top = 10, Width = 280, Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
            var lblEmail = new Label { Text = "Email:", Left = 30, Top = 50, Width = 80 };
            txtEmail = new TextBox { Left = 120, Top = 50, Width = 180 };
            var lblPassword = new Label { Text = "Password:", Left = 30, Top = 90, Width = 80 };
            txtPassword = new TextBox { Left = 120, Top = 90, Width = 180, UseSystemPasswordChar = true };
            btnLogin = new Button { Text = "Login", Left = 120, Top = 140, Width = 180, Height = 35 };
            btnLogin.Click += BtnLogin_Click;
            lblError = new Label { Text = "", Left = 30, Top = 180, Width = 270, ForeColor = System.Drawing.Color.Red };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblError);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Please enter both email and password.";
                return;
            }
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT UserID, Password FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int userId = reader.GetInt32(0);
                        string hash = reader.GetString(1);
                        if (PasswordHelper.VerifyPassword(txtPassword.Text.Trim(), hash))
                        {
                            LoggedInUserId = userId;
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            return;
                        }
                    }
                }
            }
            lblError.Text = "Invalid email or password.";
        }
    }
} 