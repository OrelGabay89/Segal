
namespace Services.Interfaces
{
    public interface IInvoicService
    {
        Task<string> GetInvoiceNumber();
    }
}
