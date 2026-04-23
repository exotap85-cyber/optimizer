using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Optimizer
{
    internal static class HWIDHelper
    {
        /// <summary>
        /// Generate a unique HWID based on system information
        /// Uses CPU, Motherboard, and Windows SID
        /// </summary>
        internal static string GenerateHWID()
        {
            try
            {
                string cpuID = GetCPUID();
                string motherboardID = GetMotherboardID();
                string windowsSID = GetWindowsSID();

                string combined = $"{cpuID}_{motherboardID}_{windowsSID}";
                return HashString(combined);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error generating HWID: {ex.Message}");
                return "ERROR";
            }
        }

        /// <summary>
        /// Get CPU serial number/ID
        /// </summary>
        private static string GetCPUID()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["ProcessorId"]?.ToString() ?? "UNKNOWN";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error getting CPU ID: {ex.Message}");
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// Get motherboard serial number
        /// </summary>
        private static string GetMotherboardID()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["SerialNumber"]?.ToString() ?? "UNKNOWN";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error getting Motherboard ID: {ex.Message}");
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// Get Windows machine SID
        /// </summary>
        private static string GetWindowsSID()
        {
            try
            {
                string sidPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography";
                object sidValue = Microsoft.Win32.Registry.GetValue(sidPath, "MachineGuid", "");
                return sidValue?.ToString() ?? "UNKNOWN";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error getting Windows SID: {ex.Message}");
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// Hash a string using SHA256
        /// </summary>
        private static string HashString(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hashedBytes).Substring(0, 32).ToUpper();
            }
        }
    }
}
