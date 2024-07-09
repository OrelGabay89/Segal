using CsvHelper.Configuration.Attributes;

namespace IsraelTax.Shared.Models
{
    public class InvoiceCSVData
    {
        [Name("Invoice_ID")]
        public string Invoice_ID { get; set; }

        [Name("Invoice_Type")]
        public int Invoice_Type { get; set; }

        [Name("Vat_Number")]
        public string Vat_Number { get; set; }

        [Name("Union_Vat_Number")]
        public string Union_Vat_Number { get; set; }

        [Name("Invoice_Reference_Number")]
        public string Invoice_Reference_Number { get; set; }

        [Name("Customer_VAT_Number")]
        public int Customer_VAT_Number { get; set; }

        [Name("Customer_Name")]
        public string Customer_Name { get; set; }

        [Name("Invoice_Date")]
        public string Invoice_Date { get; set; }

        [Name("Invoice_Issuance_Date")]
        public string Invoice_Issuance_Date { get; set; }

        [Name("Branch_ID")]
        public string Branch_ID { get; set; }

        [Name("Accounting_Software_Number")]
        public int Accounting_Software_Number { get; set; }

        [Name("Client_Software_Key")]
        public string Client_Software_Key { get; set; }

        [Name("Amount_Before_Discount")]
        public decimal Amount_Before_Discount { get; set; }

        [Name("Discount")]
        public decimal Discount { get; set; }

        [Name("Payment_Amount")]
        public decimal Payment_Amount { get; set; }

        [Name("VAT_Amount")]
        public decimal VAT_Amount { get; set; }

        [Name("Payment_Amount_Including_VAT")]
        public decimal Payment_Amount_Including_VAT { get; set; }

        [Name("Invoice_Note")]
        public string Invoice_Note { get; set; }

        [Name("Action")]
        public int Action { get; set; }

        [Name("Vehicle_License_Number")]
        public string Vehicle_License_Number { get; set; }

        [Name("Phone_Of_Driver")]
        public string Phone_Of_Driver { get; set; }

        [Name("Arrival_Date")]
        public string Arrival_Date { get; set; }

        [Name("Estimated_Arrival_Time")]
        public string Estimated_Arrival_Time { get; set; }

        [Name("Transition_Location")]
        public int Transition_Location { get; set; }

        [Name("Delivery_Address")]
        public string Delivery_Address { get; set; }

        [Name("Additional_Information")]
        public int Additional_Information { get; set; }

        [Name("Confirmation_number")]
        public string ConfirmationNumber { get; set; }
    }
}
