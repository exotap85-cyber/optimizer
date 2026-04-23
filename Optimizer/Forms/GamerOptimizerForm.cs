using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;

namespace Optimizer
{
    public partial class GamerOptimizerForm : Form
    {
        private GamerOptimizerSettings _settings;
        private bool _isOptimizing = false;

        public GamerOptimizerForm()
        {
            InitializeComponent();
            _settings = new GamerOptimizerSettings();
        }

        private void GamerOptimizerForm_Load(object sender, EventArgs e)
        {
            this.Text = "Gamer Optimizer - Dashboard";
            this.Size = new Size(600, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateUI();
            RefreshSystemStatus();
        }

        private void CreateUI()
        {
            // Title
            Label titleLabel = new Label
            {
                Text = "Gamer Optimizer",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(560, 40)
            };
            this.Controls.Add(titleLabel);

            // System Status Section
            GroupBox statusGroup = new GroupBox
            {
                Text = "System Status",
                Location = new Point(20, 70),
                Size = new Size(560, 100),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            Label ramLabel = new Label
            {
                Name = "ramLabel",
                Text = "RAM Usage: Calculating...",
                Location = new Point(10, 25),
                Size = new Size(540, 20)
            };
            statusGroup.Controls.Add(ramLabel);

            Label cpuLabel = new Label
            {
                Name = "cpuLabel",
                Text = "CPU Usage: Calculating...",
                Location = new Point(10, 50),
                Size = new Size(540, 20)
            };
            statusGroup.Controls.Add(cpuLabel);

            Label diskLabel = new Label
            {
                Name = "diskLabel",
                Text = "Disk Space: Calculating...",
                Location = new Point(10, 75),
                Size = new Size(540, 20)
            };
            statusGroup.Controls.Add(diskLabel);

            this.Controls.Add(statusGroup);

            // Optimization Options Section
            GroupBox optionsGroup = new GroupBox
            {
                Text = "Optimization Options",
                Location = new Point(20, 180),
                Size = new Size(560, 380),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            int yPosition = 25;

            // Disable Background Apps
            CheckBox backgroundAppsCheckBox = new CheckBox
            {
                Name = "backgroundAppsCheckBox",
                Text = "Disable Background Apps (Safe List)",
                Location = new Point(10, yPosition),
                Size = new Size(500, 25),
                Checked = _settings.DisableBackgroundApps,
                Font = new Font("Arial", 10)
            };
            backgroundAppsCheckBox.CheckedChanged += (s, e) => _settings.DisableBackgroundApps = backgroundAppsCheckBox.Checked;
            optionsGroup.Controls.Add(backgroundAppsCheckBox);
            yPosition += 40;

            // High Performance Power Plan
            CheckBox powerPlanCheckBox = new CheckBox
            {
                Name = "powerPlanCheckBox",
                Text = "Set High Performance Power Plan",
                Location = new Point(10, yPosition),
                Size = new Size(500, 25),
                Checked = _settings.HighPerformancePowerPlan,
                Font = new Font("Arial", 10)
            };
            powerPlanCheckBox.CheckedChanged += (s, e) => _settings.HighPerformancePowerPlan = powerPlanCheckBox.Checked;
            optionsGroup.Controls.Add(powerPlanCheckBox);
            yPosition += 40;

            // Clean Temp Files
            CheckBox cleanTempCheckBox = new CheckBox
            {
                Name = "cleanTempCheckBox",
                Text = "Clean Temporary Files",
                Location = new Point(10, yPosition),
                Size = new Size(500, 25),
                Checked = _settings.CleanTempFiles,
                Font = new Font("Arial", 10)
            };
            cleanTempCheckBox.CheckedChanged += (s, e) => _settings.CleanTempFiles = cleanTempCheckBox.Checked;
            optionsGroup.Controls.Add(cleanTempCheckBox);
            yPosition += 40;

            // Light RAM Cleanup
            CheckBox ramCleanupCheckBox = new CheckBox
            {
                Name = "ramCleanupCheckBox",
                Text = "Light RAM Cleanup",
                Location = new Point(10, yPosition),
                Size = new Size(500, 25),
                Checked = _settings.LightRAMCleanup,
                Font = new Font("Arial", 10)
            };
            ramCleanupCheckBox.CheckedChanged += (s, e) => _settings.LightRAMCleanup = ramCleanupCheckBox.Checked;
            optionsGroup.Controls.Add(ramCleanupCheckBox);
            yPosition += 40;

            // FPS Tweaks
            CheckBox fpsCheckBox = new CheckBox
            {
                Name = "fpsCheckBox",
                Text = "FPS Tweaks (Disable Xbox Game Bar, Game DVR)",
                Location = new Point(10, yPosition),
                Size = new Size(500, 25),
                Checked = _settings.FPSTweaks,
                Font = new Font("Arial", 10)
            };
            fpsCheckBox.CheckedChanged += (s, e) => _settings.FPSTweaks = fpsCheckBox.Checked;
            optionsGroup.Controls.Add(fpsCheckBox);
            yPosition += 40;

            // Gaming Mode
            CheckBox gamingModeCheckBox = new CheckBox
            {
                Name = "gamingModeCheckBox",
                Text = "Enable Gaming Mode (All Optimizations)",
                Location = new Point(10, yPosition),
                Size = new Size(500, 25),
                Checked = _settings.GamingMode,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };
            gamingModeCheckBox.CheckedChanged += (s, e) =>
            {
                _settings.GamingMode = gamingModeCheckBox.Checked;
                if (gamingModeCheckBox.Checked)
                {
                    backgroundAppsCheckBox.Checked = true;
                    powerPlanCheckBox.Checked = true;
                    cleanTempCheckBox.Checked = true;
                    ramCleanupCheckBox.Checked = true;
                    fpsCheckBox.Checked = true;
                }
            };
            optionsGroup.Controls.Add(gamingModeCheckBox);
            yPosition += 40;

            // Status Label
            Label statusLabel = new Label
            {
                Name = "optimizeStatusLabel",
                Text = "",
                Location = new Point(10, yPosition),
                Size = new Size(500, 30),
                Font = new Font("Arial", 9),
                ForeColor = Color.Blue
            };
            optionsGroup.Controls.Add(statusLabel);

            this.Controls.Add(optionsGroup);

            // Buttons Section
            Button optimizeButton = new Button
            {
                Text = "Optimize Now!",
                Location = new Point(20, 580),
                Size = new Size(200, 50),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 176, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            optimizeButton.Click += async (s, e) => await OptimizeButton_Click(s, e);
            this.Controls.Add(optimizeButton);

            Button restoreButton = new Button
            {
                Text = "Restore Defaults",
                Location = new Point(230, 580),
                Size = new Size(170, 50),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 192, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            restoreButton.Click += (s, e) => RestoreButton_Click(s, e);
            this.Controls.Add(restoreButton);

            Button refreshButton = new Button
            {
                Text = "Refresh Status",
                Location = new Point(410, 580),
                Size = new Size(170, 50),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            refreshButton.Click += (s, e) => RefreshSystemStatus();
            this.Controls.Add(refreshButton);
        }

        private void RefreshSystemStatus()
        {
            Label ramLabel = (Label)this.Controls["ramLabel"];
            Label cpuLabel = (Label)this.Controls["cpuLabel"];
            Label diskLabel = (Label)this.Controls["diskLabel"];

            try
            {
                // RAM Usage
                long totalMemory = GC.GetTotalMemory(false) / (1024 * 1024);
                ramLabel.Text = $"RAM Usage: {totalMemory} MB";

                // CPU Usage (simplified)
                cpuLabel.Text = $"CPU Usage: Monitoring...";

                // Disk Space
                string systemDrive = "C:";
                try
                {
                    DriveInfo drive = new DriveInfo(systemDrive);
                    long freeSpace = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
                    long totalSpace = drive.TotalSize / (1024 * 1024 * 1024);
                    diskLabel.Text = $"Disk Space: {freeSpace} GB free / {totalSpace} GB total";
                }
                catch { }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log($"Error refreshing system status: {ex.Message}");
            }
        }

        private async Task OptimizeButton_Click(object sender, EventArgs e)
        {
            if (_isOptimizing)
            {
                MessageBox.Show("Optimization is already in progress!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Button button = (Button)sender;
            button.Enabled = false;
            _isOptimizing = true;

            Label statusLabel = (Label)this.Controls["optimizeStatusLabel"];
            statusLabel.Text = "Starting optimization...";
            statusLabel.ForeColor = Color.Blue;

            try
            {
                // Create system restore point
                statusLabel.Text = "Creating system restore point...";
                GamerOptimizerHelper.CreateRestorePoint();

                await Task.Delay(500);

                // Disable Background Apps
                if (_settings.DisableBackgroundApps)
                {
                    statusLabel.Text = "Disabling background apps...";
                    GamerOptimizerHelper.DisableBackgroundApps();
                    await Task.Delay(500);
                }

                // Set High Performance Mode
                if (_settings.HighPerformancePowerPlan)
                {
                    statusLabel.Text = "Setting high performance power plan...";
                    GamerOptimizerHelper.SetHighPerformancePowerPlan();
                    await Task.Delay(500);
                }

                // Clean Temp Files
                if (_settings.CleanTempFiles)
                {
                    statusLabel.Text = "Cleaning temporary files...";
                    GamerOptimizerHelper.CleanTemporaryFiles();
                    await Task.Delay(500);
                }

                // RAM Cleanup
                if (_settings.LightRAMCleanup)
                {
                    statusLabel.Text = "Performing light RAM cleanup...";
                    GamerOptimizerHelper.CleanRAM();
                    await Task.Delay(500);
                }

                // FPS Tweaks
                if (_settings.FPSTweaks)
                {
                    statusLabel.Text = "Applying FPS tweaks...";
                    GamerOptimizerHelper.ApplyFPSTweaks();
                    await Task.Delay(500);
                }

                statusLabel.Text = "Optimization completed successfully!";
                statusLabel.ForeColor = Color.Green;

                MessageBox.Show(
                    "Gaming optimization completed successfully!\n\nYour system is now optimized for gaming.",
                    "Optimization Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                RefreshSystemStatus();
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
                ErrorLogger.Log($"Optimization error: {ex.Message}");
                MessageBox.Show($"Error during optimization:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isOptimizing = false;
                button.Enabled = true;
            }
        }

        private void RestoreButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Restore system to default settings?\n\nThis will reverse all optimizations.",
                "Confirm Restore",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    GamerOptimizerHelper.RestoreDefaults();
                    MessageBox.Show("System restored to default settings.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error restoring defaults:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
