using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Optimizer
{
    public partial class LicenseActivationForm : Form
    {
        private bool _activationSuccessful = false;
        private string _currentHWID = string.Empty;

        public LicenseActivationForm()
        {
            InitializeComponent();
            _currentHWID = HWIDHelper.GenerateHWID();
        }

        private void LicenseActivationForm_Load(object sender, EventArgs e)
        {
            this.Text = "Gamer Optimizer - License Activation";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(500, 300);

            // Create UI elements
            CreateUI();

            // Check if license already exists
            if (LicenseHelper.IsLicenseValid())
            {
                _activationSuccessful = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void CreateUI()
        {
            // Title Label
            Label titleLabel = new Label
            {
                Text = "License Activation Required",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(450, 30),
                AutoSize = false
            };
            this.Controls.Add(titleLabel);

            // Description Label
            Label descriptionLabel = new Label
            {
                Text = "Enter your license key to activate Gamer Optimizer",
                Location = new Point(20, 60),
                Size = new Size(450, 30),
                AutoSize = false
            };
            this.Controls.Add(descriptionLabel);

            // License Key Label
            Label licenseLabel = new Label
            {
                Text = "License Key:",
                Location = new Point(20, 100),
                Size = new Size(100, 25)
            };
            this.Controls.Add(licenseLabel);

            // License Key TextBox
            TextBox licenseTextBox = new TextBox
            {
                Name = "licenseTextBox",
                Location = new Point(20, 125),
                Size = new Size(450, 30),
                Font = new Font("Arial", 11)
            };
            this.Controls.Add(licenseTextBox);

            // HWID Label
            Label hwidLabel = new Label
            {
                Text = "Your HWID:",
                Location = new Point(20, 165),
                Size = new Size(450, 20)
            };
            this.Controls.Add(hwidLabel);

            // HWID Value Label (Read-only)
            Label hwidValueLabel = new Label
            {
                Text = _currentHWID,
                Location = new Point(20, 185),
                Size = new Size(450, 20),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            this.Controls.Add(hwidValueLabel);

            // Status Label
            Label statusLabel = new Label
            {
                Name = "statusLabel",
                Text = "",
                Location = new Point(20, 215),
                Size = new Size(450, 30),
                AutoSize = false,
                ForeColor = Color.Red
            };
            this.Controls.Add(statusLabel);

            // Activate Button
            Button activateButton = new Button
            {
                Text = "Activate",
                Location = new Point(370, 255),
                Size = new Size(100, 35),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            activateButton.Click += async (s, e) => await ActivateButton_Click(s, e);
            this.Controls.Add(activateButton);

            // Copy HWID Button
            Button copyButton = new Button
            {
                Text = "Copy HWID",
                Location = new Point(260, 255),
                Size = new Size(100, 35),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(200, 200, 200),
                FlatStyle = FlatStyle.Flat
            };
            copyButton.Click += (s, e) =>
            {
                Clipboard.SetText(_currentHWID);
                MessageBox.Show("HWID copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            this.Controls.Add(copyButton);
        }

        private async Task ActivateButton_Click(object sender, EventArgs e)
        {
            TextBox licenseTextBox = (TextBox)this.Controls["licenseTextBox"];
            Label statusLabel = (Label)this.Controls["statusLabel"];

            if (string.IsNullOrWhiteSpace(licenseTextBox.Text))
            {
                statusLabel.Text = "Please enter a license key";
                statusLabel.ForeColor = Color.Red;
                return;
            }

            // Show loading state
            statusLabel.Text = "Validating license...";
            statusLabel.ForeColor = Color.Blue;
            ((Button)sender).Enabled = false;

            // Call API to validate license
            LicenseValidationResponse response = await LicenseAPIHelper.ValidateLicenseAsync(licenseTextBox.Text);

            ((Button)sender).Enabled = true;

            if (response.status == "valid")
            {
                // Save license locally
                LicenseKey license = new LicenseKey
                {
                    Key = licenseTextBox.Text,
                    HWID = _currentHWID,
                    ActivatedDate = DateTime.Now,
                    ExpiryDate = response.expiry_date ?? DateTime.Now.AddYears(1),
                    Status = "valid"
                };

                LicenseHelper.SaveLicense(license);

                statusLabel.Text = "License activated successfully!";
                statusLabel.ForeColor = Color.Green;

                _activationSuccessful = true;

                MessageBox.Show(
                    $"License activated successfully!\n\nExpiry: {license.ExpiryDate:yyyy-MM-dd}",
                    "Activation Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (response.status == "expired")
            {
                statusLabel.Text = "License has expired";
                statusLabel.ForeColor = Color.Red;
                MessageBox.Show($"License Error: {response.message}", "License Expired", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                statusLabel.Text = $"Error: {response.message}";
                statusLabel.ForeColor = Color.Red;
                MessageBox.Show($"License Error: {response.message}", "Activation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LicenseActivationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_activationSuccessful && this.DialogResult == DialogResult.None)
            {
                DialogResult result = MessageBox.Show(
                    "Exit without activating license?\n\nThe application will close.",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
