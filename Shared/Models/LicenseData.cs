

namespace Shared.Models
{
    public class LicenseData
    {
        public string CustomerIdentifier { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }  // Assuming you want an expiry date
        public string EncryptedLicenseKey { get; set; }
        public string DecryptedLicenseKey { get; set; }
    }

}
