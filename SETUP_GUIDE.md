# Quick Start Guide - Gamer Optimizer

## What You're Getting

A complete Windows gaming optimization application with:
- ✅ License system with API validation
- ✅ Hardware ID generation and encryption
- ✅ Safe gaming optimizations
- ✅ System monitoring dashboard
- ✅ Backend API server for license management

## File Structure

```
optimizer-16.7/
├── Optimizer/                          # Main C# Application
│   ├── Forms/
│   │   ├── LicenseActivationForm.cs   # License screen
│   │   ├── GamerOptimizerForm.cs       # Main dashboard
│   │   └── ...
│   ├── Models/
│   │   └── LicenseKey.cs              # License models
│   ├── HWIDHelper.cs                  # Hardware ID generation
│   ├── LicenseHelper.cs               # Encryption & storage
│   ├── LicenseAPIHelper.cs            # API communication
│   ├── GamerOptimizerHelper.cs        # Optimization features
│   └── ...
│
├── backend/                            # Node.js License Server
│   ├── license-server.js              # Main API server
│   ├── package.json                   # Dependencies
│   ├── database.sql                   # MySQL schema
│   ├── .env.example                   # Configuration template
│   ├── DEPLOYMENT.md                  # VPS setup guide
│   └── ...
│
├── README_GAMER_OPTIMIZER.md          # Full documentation
└── ...
```

## Step-by-Step Setup

### Phase 1: Backend Setup (VPS)

1. **SSH into VPS**:
   ```bash
   ssh root@69.10.60.15
   # Password: exo1tap
   ```

2. **Follow VPS deployment guide**:
   - Read: `backend/DEPLOYMENT.md`
   - Setup Node.js
   - Create MySQL database
   - Configure .env file
   - Start license server

3. **Test the server**:
   ```bash
   curl http://69.10.60.15:8080/api/health
   # Should return: {"status":"ok","message":"License server is running"}
   ```

### Phase 2: Building the Application

1. **Open Visual Studio**:
   - Load `Optimizer.sln`
   - Restore NuGet packages (Newtonsoft.Json should be included)

2. **Build the Project**:
   - Select "Release" configuration
   - Build > Build Solution
   - Output: `bin/Release/GamerOptimizer.exe`

3. **Test Locally**:
   - Ensure API endpoint in `LicenseAPIHelper.cs` matches your VPS
   - Run as Administrator
   - License activation screen should appear
   - Enter test key: `GAMOPT-2026-TEST-001`

### Phase 3: License Management

1. **Add a New License** (on VPS):
   ```bash
   mysql -u root -p gamer_optimizer
   INSERT INTO licenses (license_key, user_email, status, expiry_date)
   VALUES ('YOUR-KEY-HERE', 'customer@email.com', 'active', DATE_ADD(NOW(), INTERVAL 1 YEAR));
   ```

2. **View Licenses**:
   ```bash
   SELECT * FROM licenses;
   ```

3. **Check Validation Logs**:
   ```bash
   SELECT * FROM license_logs ORDER BY created_at DESC;
   ```

## Key Components Explained

### 1. License Activation Flow

```
User Launch
    ↓
Check Local License
    ↓
If Valid → Show Dashboard
If Not → Show Activation Screen
    ↓
User Enters License Key
    ↓
Generate HWID (CPU + Motherboard + Windows SID)
    ↓
Send to API: {license_key, hwid}
    ↓
API Validates in Database
    ↓
If Valid → Encrypt & Save Locally → Show Dashboard
If Invalid → Show Error Message
```

### 2. HWID Generation

HWID is a unique 32-character hash of:
- CPU Serial Number
- Motherboard Serial Number
- Windows Machine GUID (SID)

This ensures license is tied to specific hardware.

### 3. Encryption

Licenses stored locally using AES-256:
- Location: `%APPDATA%\GamerOptimizer\license.dat`
- Encrypted with: `GAMOPT2026SECKEY`
- Contains: Key, HWID, dates, status

### 4. API Communication

POST to: `http://69.10.60.15:8080/api/validate-key`

**Request**:
```json
{
  "license_key": "GAMOPT-2026-USER-001",
  "hwid": "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6"
}
```

**Response**:
```json
{
  "status": "valid",
  "message": "License is valid",
  "expiry_date": "2026-12-31"
}
```

## Testing Scenarios

### Test 1: Valid License
1. Database has active license: `GAMOPT-2026-TEST-001`
2. Run app, enter key
3. Should activate successfully

### Test 2: Expired License
1. Create license with past expiry date
2. Run app, enter key
3. Should show "expired" error

### Test 3: Invalid HWID
1. Activate on Computer A
2. Copy `license.dat` to Computer B
3. Should reject with "device mismatch" error

### Test 4: Network Failure
1. Turn off internet
2. Run app with stored license
3. Should work offline (uses cached license)

## Optimization Features Explained

| Feature | What It Does | Safety | Reversible |
|---------|------------|--------|-----------|
| **Disable Background Apps** | Removes startup bloatware | Very Safe | Yes - via restore |
| **High Performance Power Plan** | Enables max performance mode | Safe | Yes - change in settings |
| **Clean Temp Files** | Removes cache/temp files | Very Safe | N/A - removes junk |
| **Light RAM Cleanup** | Frees unused process memory | Very Safe | N/A - automatic |
| **FPS Tweaks** | Disables Game Bar/DVR | Very Safe | Yes - via restore |
| **Gaming Mode** | Enables all above | Very Safe | Yes - via restore |

## Deployment to Users

1. **Build Release Version**:
   ```bash
   # Visual Studio: Build > Build Solution (Release)
   ```

2. **Create Installer** (Optional):
   - Use NSIS, Inno Setup, or similar
   - Include: GamerOptimizer.exe + dependencies

3. **Distribute**:
   - Website download link
   - Direct file sharing
   - Auto-update system (can add later)

4. **User License Keys**:
   - Generate in database
   - Send to users via email
   - They enter on first launch

## Monitoring & Administration

### Check Active Users
```bash
SELECT license_key, hwid_locked, last_validated 
FROM licenses 
WHERE status = 'active';
```

### Find Usage by Date
```bash
SELECT DATE(created_at), COUNT(*) 
FROM license_logs 
GROUP BY DATE(created_at) 
ORDER BY created_at DESC;
```

### Suspend a License
```bash
UPDATE licenses 
SET status = 'suspended' 
WHERE license_key = 'GAMOPT-2026-USER-001';
```

### View Failed Validations
```bash
SELECT * FROM license_logs 
WHERE status != 'validated' 
ORDER BY created_at DESC;
```

## Next Steps (Optional Enhancements)

1. **Web Admin Panel** - Manage licenses via web interface
2. **Update Checker** - Built-in update notifications
3. **Usage Statistics** - Track active users
4. **Custom SMTP** - Email license delivery
5. **2FA Support** - Two-factor authentication
6. **Trial Keys** - Time-limited trial licenses
7. **Volume Licensing** - Bulk user management
8. **API Key System** - For integrations

## Troubleshooting

### "License Server Unreachable"
- ✓ Check VPS is running: `ssh root@69.10.60.15`
- ✓ Test endpoint: `curl http://69.10.60.15:8080/api/health`
- ✓ Check .env configuration
- ✓ Verify firewall allows port 8080

### "License Activation Failed"
- ✓ Verify license key in database
- ✓ Check license isn't expired
- ✓ Check license isn't suspended
- ✓ Verify API connectivity

### "Application Won't Start"
- ✓ Run as Administrator
- ✓ Check .NET Framework 4.7.2+
- ✓ Check error logs in %APPDATA%\GamerOptimizer\logs\

### "HWID Mismatch"
- ✓ This is expected - license locked to one device
- ✓ Create new license for different device
- ✓ Or use license unlock feature (admin only)

## Support & Updates

- Monitor license server logs
- Check application event viewer
- Review database license_logs table
- Update security patches regularly

---

**Ready to deploy?** Start with the VPS backend setup, then build and test the application locally!
