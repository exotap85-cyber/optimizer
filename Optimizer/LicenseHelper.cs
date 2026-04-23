using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Optimizer
{
    internal static class LicenseHelper
    {
        private static readonly string LicenseFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GamerOptimizer",
            "license.dat"
        );

        private static readonly string EncryptionKey = "GAMOPT2026SECKEY";

        /// <summary>
        /// Save encrypted license key locally
        /// </summary>
        internal static void SaveLicense(LicenseKey license)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LicenseFile));

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(license);
                string encrypted = EncryptString(json);

                File.WriteAllText(LicenseFile, encrypted);
                ErrorLogger.Log("License saved successfully");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error saving license: {ex.Message}");
            }
        }

        /// <summary>
        /// Load and decrypt license from local storage
        /// </summary>
        internal static LicenseKey LoadLicense()
        {
            try
            {
                if (!File.Exists(LicenseFile))
                {
                    return null;
                }

                string encrypted = File.ReadAllText(LicenseFile);
                string json = DecryptString(encrypted);

                LicenseKey license = Newtonsoft.Json.JsonConvert.DeserializeObject<LicenseKey>(json);
                return license;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error loading license: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Check if license is valid and not expired
        /// </summary>
        internal static bool IsLicenseValid()
        {
            try
            {
                LicenseKey license = LoadLicense();
                if (license == null) return false;

                if (license.Status != "valid") return false;

                if (DateTime.Now > license.ExpiryDate) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete license file
        /// </summary>
        internal static void DeleteLicense()
        {
            try
            {
                if (File.Exists(LicenseFile))
                {
                    File.Delete(LicenseFile);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error deleting license: {ex.Message}");
            }
        }

        /// <summary>
        /// Encrypt a string using AES
        /// </summary>
        private static string EncryptString(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);

                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt a string using AES
        /// </summary>
        private static string DecryptString(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] buffer = Convert.FromBase64String(cipherText);

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    byte[] iv = new byte[aes.IV.Length];
                    ms.Read(iv, 0, iv.Length);

                    using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
