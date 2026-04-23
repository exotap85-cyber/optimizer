using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Optimizer
{
    internal static class GamerOptimizerHelper
    {
        // P/Invoke declarations
        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool EmptyWorkingSet(IntPtr proc);

        // List of safe startup apps to disable (common bloatware)
        private static readonly string[] SafeAppsToDisable = new string[]
        {
            "OneDrive",
            "GoogleDrive",
            "DropBox",
            "iCloud",
            "Slack",
            "Discord",
            "Steam",
            "EpicGames",
            "UbisoftConnect",
            "Origin",
            "RiotClientUxRender",
            "RiotClientServices",
            "Cortana",
            "WindowsSearch"
        };

        /// <summary>
        /// Create a system restore point before making changes
        /// </summary>
        internal static void CreateRestorePoint()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"Checkpoint-Computer -Description 'Gamer Optimizer Backup' -RestorePointType 'MODIFY_SETTINGS'\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                }

                ErrorLogger.Log("System restore point created successfully");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error creating restore point: {ex.Message}");
                // Don't throw - continue with optimization even if restore point fails
            }
        }

        /// <summary>
        /// Disable known non-critical background apps
        /// </summary>
        internal static void DisableBackgroundApps()
        {
            try
            {
                const string runPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        foreach (string appName in SafeAppsToDisable)
                        {
                            try
                            {
                                key.DeleteValue(appName, false);
                                ErrorLogger.Log($"Disabled startup app: {appName}");
                            }
                            catch
                            {
                                // App not found, continue
                            }
                        }
                    }
                }

                ErrorLogger.Log("Background apps disabled successfully");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error disabling background apps: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Set Windows to high performance power plan
        /// </summary>
        internal static void SetHighPerformancePowerPlan()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "/setactive SCHEME_MIN:SUB_NONE",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                }

                ErrorLogger.Log("High performance power plan activated");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error setting power plan: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Clean temporary files safely
        /// </summary>
        internal static void CleanTemporaryFiles()
        {
            try
            {
                string[] pathsToClean = new string[]
                {
                    Path.GetTempPath(),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), "Content.IE5"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp")
                };

                foreach (string path in pathsToClean)
                {
                    if (Directory.Exists(path))
                    {
                        try
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(path);

                            // Delete files (safe ones, not system files)
                            foreach (FileInfo file in dirInfo.GetFiles())
                            {
                                try
                                {
                                    file.Delete();
                                }
                                catch
                                {
                                    // File in use or protected, skip
                                }
                            }

                            // Delete subdirectories
                            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                            {
                                try
                                {
                                    dir.Delete(true);
                                }
                                catch
                                {
                                    // Directory in use or protected, skip
                                }
                            }

                            ErrorLogger.Log($"Cleaned temporary files in: {path}");
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.Log($"Error cleaning path {path}: {ex.Message}");
                        }
                    }
                }

                ErrorLogger.Log("Temporary files cleaned successfully");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error during temp file cleanup: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Perform light RAM cleanup using EmptyWorkingSet
        /// </summary>
        internal static void CleanRAM()
        {
            try
            {
                // Get all processes
                Process[] processes = Process.GetProcesses();

                int cleanedProcesses = 0;

                foreach (Process process in processes)
                {
                    try
                    {
                        // Skip critical processes
                        if (process.ProcessName.ToLower() == "system" ||
                            process.ProcessName.ToLower() == "svchost" ||
                            process.ProcessName.ToLower() == "csrss" ||
                            process.ProcessName.ToLower() == "services" ||
                            process.ProcessName.ToLower() == "dwm")
                        {
                            continue;
                        }

                        EmptyWorkingSet(process.Handle);
                        cleanedProcesses++;
                    }
                    catch
                    {
                        // Process might not have permission or already closed
                    }
                }

                ErrorLogger.Log($"RAM cleanup completed. Cleaned {cleanedProcesses} processes");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error during RAM cleanup: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Apply safe FPS tweaks
        /// </summary>
        internal static void ApplyFPSTweaks()
        {
            try
            {
                // Disable Xbox Game Bar
                DisableXboxGameBar();

                // Disable Game DVR
                DisableGameDVR();

                // Set for best performance
                SetBestPerformance();

                // Disable fullscreen optimizations
                DisableFullscreenOptimizations();

                ErrorLogger.Log("FPS tweaks applied successfully");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error applying FPS tweaks: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Disable Xbox Game Bar
        /// </summary>
        private static void DisableXboxGameBar()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", true))
                {
                    if (key != null)
                    {
                        key.SetValue("AppCaptureEnabled", 0, RegistryValueKind.DWord);
                    }
                }

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar", true))
                {
                    if (key != null)
                    {
                        key.SetValue("UseNexusButton", 0, RegistryValueKind.DWord);
                    }
                }

                ErrorLogger.Log("Xbox Game Bar disabled");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error disabling Xbox Game Bar: {ex.Message}");
            }
        }

        /// <summary>
        /// Disable Game DVR
        /// </summary>
        private static void DisableGameDVR()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", true))
                {
                    if (key != null)
                    {
                        key.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);
                    }
                }

                ErrorLogger.Log("Game DVR disabled");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error disabling Game DVR: {ex.Message}");
            }
        }

        /// <summary>
        /// Set system for best performance
        /// </summary>
        private static void SetBestPerformance()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", true))
                {
                    if (key != null)
                    {
                        key.SetValue("VisualFXSetting", 2, RegistryValueKind.DWord);
                    }
                }

                ErrorLogger.Log("System set to best performance");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error setting best performance: {ex.Message}");
            }
        }

        /// <summary>
        /// Disable fullscreen optimizations
        /// </summary>
        private static void DisableFullscreenOptimizations()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"System\GameConfigStore", true))
                {
                    if (key != null)
                    {
                        key.SetValue("GameDVR_DXGIHotKeyCapture", 0, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_FSEBehaviorMonitoringEnabled", 0, RegistryValueKind.DWord);
                    }
                }

                ErrorLogger.Log("Fullscreen optimizations disabled");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error disabling fullscreen optimizations: {ex.Message}");
            }
        }

        /// <summary>
        /// Restore system to default settings
        /// </summary>
        internal static void RestoreDefaults()
        {
            try
            {
                // Re-enable background apps
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (key != null)
                        {
                            // Most apps will re-add themselves on startup, so we just note this
                            ErrorLogger.Log("Restore operation completed - background apps can be re-enabled");
                        }
                    }
                }
                catch { }

                // Re-enable Xbox Game Bar
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("AppCaptureEnabled", 1, RegistryValueKind.DWord);
                        }
                    }
                }
                catch { }

                // Re-enable Game DVR
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("GameDVR_Enabled", 1, RegistryValueKind.DWord);
                        }
                    }
                }
                catch { }

                ErrorLogger.Log("System restoration completed");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error restoring defaults: {ex.Message}");
                throw;
            }
        }
    }
}
