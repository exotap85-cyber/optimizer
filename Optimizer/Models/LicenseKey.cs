using System;

namespace Optimizer
{
    [Serializable]
    public class LicenseKey
    {
        public string Key { get; set; }
        public string HWID { get; set; }
        public DateTime ActivatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; }
        public string UserEmail { get; set; }
    }

    [Serializable]
    public class LicenseValidationRequest
    {
        public string license_key { get; set; }
        public string hwid { get; set; }
    }

    [Serializable]
    public class LicenseValidationResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public DateTime? expiry_date { get; set; }
    }

    public class GamerOptimizerSettings
    {
        public bool DisableBackgroundApps { get; set; }
        public bool HighPerformancePowerPlan { get; set; }
        public bool CleanTempFiles { get; set; }
        public bool LightRAMCleanup { get; set; }
        public bool FPSTweaks { get; set; }
        public bool GamingMode { get; set; }

        public GamerOptimizerSettings()
        {
            DisableBackgroundApps = false;
            HighPerformancePowerPlan = false;
            CleanTempFiles = false;
            LightRAMCleanup = false;
            FPSTweaks = false;
            GamingMode = false;
        }
    }
}
