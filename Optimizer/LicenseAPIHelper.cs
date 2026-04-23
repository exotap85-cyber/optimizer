using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Optimizer
{
    internal static class LicenseAPIHelper
    {
        private static readonly string APIBaseUrl = "http://69.10.60.15:8080";
        private static readonly string ValidationEndpoint = "/api/validate-key";

        /// <summary>
        /// Validate license key with remote API
        /// </summary>
        internal static async Task<LicenseValidationResponse> ValidateLicenseAsync(string licenseKey)
        {
            try
            {
                string hwid = HWIDHelper.GenerateHWID();

                LicenseValidationRequest request = new LicenseValidationRequest
                {
                    license_key = licenseKey,
                    hwid = hwid
                };

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    string json = JsonConvert.SerializeObject(request);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(
                        APIBaseUrl + ValidationEndpoint,
                        content
                    );

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        LicenseValidationResponse apiResponse = JsonConvert.DeserializeObject<LicenseValidationResponse>(responseContent);

                        return apiResponse;
                    }
                    else
                    {
                        return new LicenseValidationResponse
                        {
                            status = "error",
                            message = $"API Error: {response.StatusCode}"
                        };
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorLogger.Log($"Network error during license validation: {ex.Message}");
                return new LicenseValidationResponse
                {
                    status = "error",
                    message = $"Network Error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error validating license: {ex.Message}");
                return new LicenseValidationResponse
                {
                    status = "error",
                    message = $"Error: {ex.Message}"
                };
            }
        }
    }
}
