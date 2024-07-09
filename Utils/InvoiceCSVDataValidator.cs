using IsraelTax.Shared.Models;
using System.Globalization;


namespace Utils
{
    public static class InvoiceCSVDataValidator
    {
        public static bool Validate(InvoiceCSVData record, out List<string> validationErrors)
        {
            validationErrors = new List<string>();

            if (!DateTime.TryParseExact(record.Invoice_Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                validationErrors.Add("Invalid Invoice_Date format. Expected format: yyyy-MM-dd.");

            if (!DateTime.TryParseExact(record.Invoice_Issuance_Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                validationErrors.Add("Invalid Invoice_Issuance_Date format. Expected format: yyyy-MM-dd.");

            if (!DateTime.TryParseExact(record.Arrival_Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                validationErrors.Add("Invalid Arrival_Date format. Expected format: yyyy-MM-dd.");

            if (!decimal.TryParse(record.Payment_Amount.ToString(), out _))
                validationErrors.Add("Invalid Payment_Amount. Expected a decimal number.");

            if (!decimal.TryParse(record.Amount_Before_Discount.ToString(), out _))
                validationErrors.Add("Invalid Amount_Before_Discount. Expected a decimal number.");

            if (!decimal.TryParse(record.Discount.ToString(), out _))
                validationErrors.Add("Invalid Discount. Expected a decimal number.");

            if (!decimal.TryParse(record.VAT_Amount.ToString(), out _))
                validationErrors.Add("Invalid VAT_Amount. Expected a decimal number.");

            if (!decimal.TryParse(record.Payment_Amount_Including_VAT.ToString(), out _))
                validationErrors.Add("Invalid Payment_Amount_Including_VAT. Expected a decimal number.");

            // Additional validations can be added here as needed

            return validationErrors.Count == 0;
        }
    }
}
