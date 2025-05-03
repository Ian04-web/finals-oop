using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace HotelManagementSystem
{
    public class PaymentForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal Amount { get; private set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PaymentMethod { get; private set; } = "";
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Status { get; private set; } = "";
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime PaymentDate { get; private set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PaymentSaved { get; private set; } = false;

        private TextBox txtAmount = null!;
        private ComboBox cmbMethod = null!;
        private ComboBox cmbStatus = null!;
        private DateTimePicker dtpDate = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public PaymentForm(decimal? defaultAmount = null)
        {
            InitializeComponent();
            if (defaultAmount.HasValue)
                txtAmount.Text = defaultAmount.Value.ToString("F2");
            dtpDate.Value = DateTime.Now;
        }

        private void InitializeComponent()
        {
            this.Text = "Payment Details";
            this.Size = new Size(350, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblAmount = new Label { Text = "Amount:", Location = new Point(30, 30), AutoSize = true, Font = new Font("Segoe UI", 11) };
            txtAmount = new TextBox { Location = new Point(130, 25), Width = 160, Font = new Font("Segoe UI", 11) };

            Label lblMethod = new Label { Text = "Method:", Location = new Point(30, 75), AutoSize = true, Font = new Font("Segoe UI", 11) };
            cmbMethod = new ComboBox { Location = new Point(130, 70), Width = 160, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMethod.Items.AddRange(new string[] { "Cash", "Card", "Online" });
            cmbMethod.SelectedIndex = 0;

            Label lblStatus = new Label { Text = "Status:", Location = new Point(30, 120), AutoSize = true, Font = new Font("Segoe UI", 11) };
            cmbStatus = new ComboBox { Location = new Point(130, 115), Width = 160, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new string[] { "Paid", "Unpaid" });
            cmbStatus.SelectedIndex = 0;

            Label lblDate = new Label { Text = "Date:", Location = new Point(30, 165), AutoSize = true, Font = new Font("Segoe UI", 11) };
            dtpDate = new DateTimePicker { Location = new Point(130, 160), Width = 160, Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Short };

            btnSave = new Button { Text = "Save", Location = new Point(60, 220), Width = 90, Height = 35, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnCancel = new Button { Text = "Cancel", Location = new Point(180, 220), Width = 90, Height = 35, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnSave.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.BorderSize = 0;

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblAmount);
            this.Controls.Add(txtAmount);
            this.Controls.Add(lblMethod);
            this.Controls.Add(cmbMethod);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cmbStatus);
            this.Controls.Add(lblDate);
            this.Controls.Add(dtpDate);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal amt) || amt <= 0)
            {
                MessageBox.Show("Please enter a valid amount.");
                return;
            }
            Amount = amt;
            PaymentMethod = cmbMethod.SelectedItem?.ToString() ?? "";
            Status = cmbStatus.SelectedItem?.ToString() ?? "";
            PaymentDate = dtpDate.Value;
            PaymentSaved = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 