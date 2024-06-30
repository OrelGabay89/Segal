

using CsvHelper.Configuration.Attributes;

namespace IsraelTax.Shared.Models
{
    public class InvoiceCSVData
    {
        public string invoice_id { get; set; }
        public int invoice_type { get; set; }
        public int vat_number { get; set; }
        public int union_vat_number { get; set; }
        public string invoice_reference_number { get; set; }
        public int customer_vat_number { get; set; }
        public string customer_name { get; set; }
        public DateTime invoice_date { get; set; }
        public DateTime invoice_issuance_date { get; set; }
        public string branch_id { get; set; }
        public int accounting_software_number { get; set; }
        public string client_software_key { get; set; }
        public decimal amount_before_discount { get; set; }
        public decimal discount { get; set; }
        public decimal payment_amount { get; set; }
        public decimal vat_amount { get; set; }
        public decimal payment_amount_including_vat { get; set; }
        public string invoice_note { get; set; }
        public int action { get; set; }
        public int vehicle_license_number { get; set; }
        public int transition_location { get; set; }
        public string delivery_address { get; set; }
        public int additional_information { get; set; }
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public int index { get; set; }
        public string catalog_id { get; set; }
        public string description { get; set; }
        public string measure_unit_description { get; set; }
        public double quantity { get; set; }
        public decimal price_per_unit { get; set; }
        public decimal discount { get; set; }
        public decimal total_amount { get; set; }
        public int vat_rate { get; set; }
        public decimal vat_amount { get; set; }
    }
}