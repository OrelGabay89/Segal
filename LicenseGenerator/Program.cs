// See https://aka.ms/new-console-template for more information
using Utils;

DateTime today = DateTime.Now;
string customerIdentifier = "Segal";
LicenseManager.GenerateLicenseKeyAndSave(customerIdentifier, today, 5, "c:\\temp\\license.lic");

