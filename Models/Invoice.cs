using System;

namespace HotelManagementSystem.Models
{
    public class Invoice
    {
        public int InvoiceID { get; set; }
        public int BookingID { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateIssued { get; set; }
    }
} 