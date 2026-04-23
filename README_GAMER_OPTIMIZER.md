# Gamer Optimizer - Desktop Application

Complete Windows gaming optimization application with license system and safe performance tweaks.

## Features

### 1. License System
- **License Key Validation**: Connect to remote API to validate license keys
- **Hardware ID (HWID)**: Generates unique HWID based on CPU, Motherboard, and Windows SID
- **Encrypted Storage**: Saves license locally with AES encryption
- **First Launch**: License screen appears on first launch
- **License Management**: View, validate, and manage licenses

### 2. Gamer Optimizer Dashboard
- **Gaming Optimizations**: One-click gaming mode for all optimizations
- **Selective Optimizations**: Choose individual optimizations
- **System Monitoring**: Real-time RAM, CPU, and disk usage display
- **Restore Defaults**: Safely restore system to default state

### 3. Optimization Features

#### A. Disable Background Apps
- Safely disables known non-critical startup applications
- Whitelist includes: OneDrive, Discord, Slack, Steam, etc.
- Uses Registry and Task Scheduler
- **Safe**: Does not disable system-critical services

#### B. High Performance Power Plan
- Activates Windows High Performance power plan
- Uses `powercfg /setactive SCHEME_MIN`
- Ensures maximum system performance
- Improves gaming FPS and responsiveness

#### C. Clean Temporary Files
- Clears %TEMP% directory
- Clears C:\Windows\Temp
- Clears Internet cache and browser temp files
- **Safe**: Only removes temporary files, not system files

#### D. Light RAM Cleanup
- Uses Windows EmptyWorkingSet API
- Frees unused RAM from running processes
- **Safe**: Non-destructive memory optimization
- Skips critical system processes

#### E. FPS Tweaks
- Disables Xbox Game Bar
- Disables Game DVR
- Sets system for best performance
- Disables fullscreen optimizations
- **Safe**: Registry-based changes, reversible

#### F. Gaming Mode
- Enables all optimizations simultaneously
- One-click performance boost
- Perfect for starting a gaming session

### 4. Safety Features
- **System Restore Point**: Creates restore point before any changes
- **Change Logging**: All modifications are logged
- **Restore Defaults**: Easy rollback to original settings
- **Protected System Processes**: Critical system services are never touched
- **Error Handling**: Comprehensive error messages and recovery

### 5. System Status Display
- **RAM Usage**: Current memory consumption
- **CPU Usage**: System processor load
- **Disk Space**: Available and total disk space
- **Refresh**: Real-time status updates

### 6. Additional Features
- **Update Checker**: Check for new versions
- **Error Logging**: All errors logged to file
- **User-Friendly UI**: Intuitive Windows Forms interface
- **Admin Required**: Runs with administrator privileges

## System Requirements

- **OS**: Windows 7, Windows 8, Windows 10, Windows 11
- **.NET Framework**: 4.7.2+
- **RAM**: 512 MB minimum
- **Disk Space**: 50 MB
- **Administrator Privileges**: Required for optimizations

## Installation

1. Download `GamerOptimizer.exe`
2. Run as Administrator
3. Enter your license key on first launch
4. Start optimizing!

## Usage

### First Launch
1. Open the application
2. License activation screen appears
3. Enter your license key
4. Click "Activate"
5. Dashboard opens

### Daily Usage
1. Open Gamer Optimizer
2. Select optimizations you want
3. Click "Optimize Now!" or enable "Gaming Mode"
4. Wait for optimization to complete
5. Close and launch your game

### Restore Defaults
1. Click "Restore Defaults" button
2. Confirm the restoration
3. System returns to original state

## Building from Source

### Prerequisites
- Visual Studio 2019 or higher
- .NET Framework 4.7.2
- Newtonsoft.Json NuGet package

### Build Steps
1. Open `Optimizer.sln` in Visual Studio
2. Restore NuGet packages
3. Build Solution (Release mode recommended)
4. Output: `bin\Release\GamerOptimizer.exe`

## API Integration

The application communicates with the license server at:
- **Endpoint**: `POST /api/validate-key`
- **Base URL**: `http://69.10.60.15:8080`
- **Request**:
  ```json
  {
    "license_key": "USER_KEY",
    "hwid": "GENERATED_HWID"
  }
  ```
- **Response**:
  ```json
  {
    "status": "valid|invalid|expired",
    "message": "Description",
    "expiry_date": "2026-12-31"
  }
  ```

## Configuration

The application stores configuration in:
- **License**: `%APPDATA%\GamerOptimizer\license.dat` (encrypted)
- **Logs**: `%APPDATA%\GamerOptimizer\logs\`
- **Settings**: `%APPDATA%\GamerOptimizer\settings.json`

## Troubleshooting

### "License Required" Error
- Check internet connection
- Verify license key is correct
- Ensure VPS license server is running
- Check firewall isn't blocking API calls

### "Admin Privileges Required"
- Run application as Administrator
- Right-click on .exe -> Run as Administrator

### Optimizations Not Working
- Ensure admin privileges
- Check Windows version compatibility
- Review error logs in %APPDATA%\GamerOptimizer\logs\

### License Server Connection Failed
- Verify VPS is running
- Check API endpoint: `http://69.10.60.15:8080/api/health`
- Check firewall/network settings
- Verify license server .env configuration

## Security

- **Encrypted Storage**: Licenses stored with AES-256 encryption
- **HWID Verification**: Unique device identification
- **Registry Safety**: Only modifies non-critical keys
- **Process Protection**: System processes never modified
- **Error Logging**: Comprehensive audit trail

## License

Proprietary software. Requires valid license key to operate.

## Support

For support or issues:
- Check application logs
- Verify system compatibility
- Contact support team

## Change Log

See CHANGELOG.md for version history and updates.

## Credits

- Based on Optimizer by deadmoon
- Modified for Gamer Optimizer with license system
- Gaming optimizations specifically tuned for performance

## Version

**Current Version**: 1.0.0
**Release Date**: April 2026
