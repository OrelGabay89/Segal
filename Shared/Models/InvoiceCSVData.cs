using CsvHelper.Configuration.Attributes;

namespace IsraelTax.Shared.Models
{
    public class InvoiceCSVData
    {
        [Name("Invoice_ID")]
        public string Invoice_ID { get; set; }  // 1. מספר מזהה/קשר פנימי

        [Name("Invoice_Type")]
        public int Invoice_Type { get; set; }  // 2. סוג מסמך

        [Name("Vat_Number")]
        public string Vat_Number { get; set; }  // 3. מספר עוסק מורשה של מפיק המסמך

        [Name("Union_Vat_Number")]
        public string Union_Vat_Number { get; set; }  // 4. מספר עוסק המדווח במע"מ של מפיק המסמך

        [Name("Company_Authorized")]
        public string Company_Authorized { get; set; }  // 5. מספר תאגיד/גורם מוסמך (חדש)

        [Name("User_ID")]
        public string User_ID { get; set; }  // 6. תעודת זהות מפעיל השירות (חדש)

        [Name("User_Name")]
        public string User_Name { get; set; }  // 7. שם משתמש מפעיל השירות (חדש)

        [Name("Invoice_Reference_Number")]
        public string Invoice_Reference_Number { get; set; }  // 8. מספר מסמך (אסמכתא)

        [Name("Customer_VAT_Number")]
        public int Customer_VAT_Number { get; set; }  // 9. מספר עוסק מורשה לקוח

        [Name("Customer_Name")]
        public string Customer_Name { get; set; }  // 10. שם הלקוח

        [Name("Country_Code")]
        public string Country_Code { get; set; }  // 11. קוד מדינה של הלקוח (חדש)

        [Name("Invoice_Date")]
        public string Invoice_Date { get; set; }  // 12. תאריך המסמך

        [Name("Invoice_Issuance_Date")]
        public string Invoice_Issuance_Date { get; set; }  // 13. תאריך הפקת מסמך

        [Name("Branch_ID")]
        public string Branch_ID { get; set; }  // 14. מזהה סניף

        [Name("Accounting_Software_Number")]
        public int Accounting_Software_Number { get; set; }  // 15. מספר רישום תוכנה

        [Name("Client_Software_Key")]
        public string Client_Software_Key { get; set; }  // 16. מפתח הלקוח של מפיק החשבונית

        [Name("Amount_Before_Discount")]
        public decimal Amount_Before_Discount { get; set; }  // 17. סכום לפני הנחת מסמך

        [Name("Discount")]
        public decimal Discount { get; set; }  // 18. הנחת מסמך

        [Name("Payment_Amount")]
        public decimal Payment_Amount { get; set; }  // 19. סכום סופי ללא מע"מ

        [Name("VAT_Amount")]
        public decimal VAT_Amount { get; set; }  // 20. סכום המע"מ

        [Name("Payment_Amount_Including_VAT")]
        public decimal Payment_Amount_Including_VAT { get; set; }  // 21. סכום סופי כולל מע"מ

        [Name("Invoice_Note")]
        public string Invoice_Note { get; set; }  // הערה לחשבונית (לא קיים בטבלה שסיפקת)

        [Name("Action")]
        public int Action { get; set; }  // הפעולה שנבחרה (לא קיים בטבלה שסיפקת)

        [Name("Vehicle_License_Number")]
        public string Vehicle_License_Number { get; set; }  // מספר רישוי רכב (לא קיים בטבלה שסיפקת)

        [Name("Phone_Of_Driver")]
        public string Phone_Of_Driver { get; set; }  // מספר הטלפון של הנהג (לא קיים בטבלה שסיפקת)

        [Name("Arrival_Date")]
        public string Arrival_Date { get; set; }  // תאריך הגעה (לא קיים בטבלה שסיפקת)

        [Name("Estimated_Arrival_Time")]
        public string Estimated_Arrival_Time { get; set; }  // שעת הגעה משוערת (לא קיים בטבלה שסיפקת)

        [Name("Transition_Location")]
        public int Transition_Location { get; set; }  // מיקום המעבר (לא קיים בטבלה שסיפקת)

        [Name("Delivery_Address")]
        public string Delivery_Address { get; set; }  // כתובת אספקה (לא קיים בטבלה שסיפקת)

        [Name("Additional_Information")]
        public int Additional_Information { get; set; }  // מידע נוסף (לא קיים בטבלה שסיפקת)

        [Name("Confirmation_number")]
        public string ConfirmationNumber { get; set; }  // מספר אישור (לא קיים בטבלה שסיפקת)
    }
}
