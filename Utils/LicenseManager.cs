using Newtonsoft.Json;
using Shared.Models;
using System;
using System.Security.Cryptography;
using System.Text;

public static class LicenseManager
{
    public static void GenerateLicenseKeyAndSave(string customerIdentifier, DateTime issueDate, int validityDays, string filePath)
    {
        string licenseKey = GenerateLicenseKey(customerIdentifier, issueDate);
        string encryptedLicenseKey = EncryptLicenseKey(licenseKey, customerIdentifier);

        LicenseData licenseData = new LicenseData
        {
            CustomerIdentifier = customerIdentifier,
            IssueDate = issueDate,
            ExpiryDate = issueDate.AddDays(validityDays), // Set expiry date
            EncryptedLicenseKey = encryptedLicenseKey
        };

        SaveLicenseToFile(licenseData, filePath);
    }

    private static string GenerateLicenseKey(string customerIdentifier, DateTime issueDate)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string rawData = $"{customerIdentifier}-{issueDate:yyyyMMdd}";
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }
    }

    public static byte[] GenerateValidKey(string input, int size)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashedBytes = sha256.ComputeHash(keyBytes); // SHA256 creates a 256-bit hash.
            byte[] validSizeKey = new byte[size];
            Array.Copy(hashedBytes, validSizeKey, size);
            return validSizeKey;
        }
    }

    public static byte[] GenerateValidIV()
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.GenerateIV();
            return aesAlg.IV; // This will be 128 bits (16 bytes), which is valid for AES.
        }
    }

    private static string EncryptLicenseKey(string licenseKey, string customerIdentifier)
    {
        // Example: deriving an IV from the customer identifier or another consistent piece of data
        byte[] iv = GetConsistentIV(customerIdentifier); // Ensure this method generates a consistent IV
        byte[] key = GetEncryptionKey(); // Your encryption key retrieval logic

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(licenseKey);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
    private static void SaveLicenseToFile(LicenseData licenseData, string filePath)
    {
        string json = JsonConvert.SerializeObject(licenseData, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public static bool VerifyLicense(string filePath, string customerIdentifier)
    {
        try
        {
            LicenseData license = ReadLicenseFile(filePath);
            if (DateTime.Now > license.ExpiryDate)
            {
                Console.WriteLine("License has expired.");
                return false;
            }

            string newEncryptedKey = EncryptLicenseKey(GenerateLicenseKey(customerIdentifier, license.IssueDate), customerIdentifier);
            if (newEncryptedKey != license.EncryptedLicenseKey)
            {
                Console.WriteLine("Invalid license key.");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error verifying license");
            return false;
        }
    }

    //private static string DecryptLicenseKey(string encryptedKey)
    //{
    //    // Add your decryption logic here. This is a placeholder:
    //    byte[] keyBytes = Encoding.UTF8.GetBytes("your-encryption-key-123"); // Use secure management
    //    byte[] ivBytes = Encoding.UTF8.GetBytes("your-encryption-iv123");  // Use secure management
    //    using (Aes aesAlg = Aes.Create())
    //    {
    //        aesAlg.Key = keyBytes;
    //        aesAlg.IV = ivBytes;
    //        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

    //        byte[] encryptedBytes = Convert.FromBase64String(encryptedKey);
    //        using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
    //        {
    //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
    //            {
    //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
    //                {
    //                    return srDecrypt.ReadToEnd();
    //                }
    //            }
    //        }
    //    }
    //}

    public static LicenseData ReadLicenseFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: License file does not exist.");
            return null; // Or throw an appropriate exception
        }

        try
        {
            string fileContent = File.ReadAllText(filePath);
            var licenseData = JsonConvert.DeserializeObject<LicenseData>(fileContent);

            if (licenseData == null)
            {
                Console.WriteLine("Error: License data is corrupted or empty.");
                return null; // Or throw an appropriate exception
            }

            // Successfully read and parsed the license data.
            // No decryption of the license key is necessary if it's not required for further processing.
            Console.WriteLine("License data read successfully.");
            return licenseData;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error reading license file: {ex.Message}");
            return null; // Or throw an appropriate exception
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON deserialization error: {ex.Message}");
            return null; // Or throw an appropriate exception
        }
    }

    private static string DecryptLicenseKey(string encryptedKey)
    {
        byte[] keyBytes = GenerateValidKey("your-encryption-key-123", 256);  // Ensure this matches your encryption key size
        byte[] ivBytes = GenerateValidIV();  // Ensure this matches your encryption IV

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedKey);
            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    public static byte[] GetConsistentIV(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            byte[] iv = new byte[16]; // IV length for AES should be 128 bits (16 bytes)
            Array.Copy(hash, iv, 16);
            return iv;
        }
    }

    public static byte[] GetEncryptionKey()
    {
        // Example: Derive a key from a passphrase using PBKDF2
        string passphrase = "your-secure-passphrase"; // This should be securely stored or managed
        string salt = "your-secure-salt"; // Ensure salt is securely generated and stored

        using (var deriveBytes = new Rfc2898DeriveBytes(passphrase, Encoding.UTF8.GetBytes(salt), 10000))
        {
            return deriveBytes.GetBytes(32); // 32 bytes for AES-256 key
        }
    }
}