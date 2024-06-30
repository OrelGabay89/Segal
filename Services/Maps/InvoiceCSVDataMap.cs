using CsvHelper.Configuration;
using IsraelTax.Shared.Models;

namespace Shared.Services
{
    public sealed class InvoiceCSVDataMap : ClassMap<InvoiceCSVData>
    {
        public InvoiceCSVDataMap()
        {
            Map(m => m.invoice_id).Name("invoice_id");
            Map(m => m.invoice_type).Name("invoice_type");
            Map(m => m.vat_number).Name("vat_number");
            Map(m => m.union_vat_number).Name("union_vat_number");
            Map(m => m.invoice_reference_number).Name("invoice_reference_number");
            Map(m => m.customer_vat_number).Name("customer_vat_number");
            Map(m => m.customer_name).Name("customer_name");
            Map(m => m.invoice_date).Name("invoice_date");
            Map(m => m.invoice_issuance_date).Name("invoice_issuance_date");
            Map(m => m.branch_id).Name("branch_id");
            Map(m => m.accounting_software_number).Name("accounting_software_number");
            Map(m => m.client_software_key).Name("client_software_key");
            Map(m => m.amount_before_discount).Name("amount_before_discount");
            Map(m => m.discount).Name("discount");
            Map(m => m.payment_amount).Name("payment_amount");
            Map(m => m.vat_amount).Name("vat_amount");
            Map(m => m.payment_amount_including_vat).Name("payment_amount_including_vat");
            Map(m => m.invoice_note).Name("invoice_note");
            Map(m => m.action).Name("action");
            Map(m => m.vehicle_license_number).Name("vehicle_license_number");
            Map(m => m.transition_location).Name("transition_location");
            Map(m => m.delivery_address).Name("delivery_address");
            Map(m => m.additional_information).Name("additional_information");
            References<ItemMap>(m => m.items);


        }
    }

    public sealed class ItemMap : ClassMap<Item>
    {
        public ItemMap()
        {
            Map(m => m.index).Name("index");
            Map(m => m.catalog_id).Name("catalog_id");
            Map(m => m.description).Name("description");
            Map(m => m.measure_unit_description).Name("measure_unit_description");
            Map(m => m.quantity).Name("quantity");
            Map(m => m.price_per_unit).Name("price_per_unit");
            Map(m => m.discount).Name("discount");
            Map(m => m.total_amount).Name("total_amount");
            Map(m => m.vat_rate).Name("vat_rate");
            Map(m => m.vat_amount).Name("vat_amount");
        }
    }
}
