# Gamer Optimizer - Developer Quick Reference

## Key Files to Know

| File | Purpose | Modify For |
|------|---------|-----------|
| `LicenseActivationForm.cs` | License input screen | UI/UX changes |
| `GamerOptimizerForm.cs` | Main dashboard | Add features, change UI |
| `GamerOptimizerHelper.cs` | Optimization logic | Add/modify optimizations |
| `LicenseAPIHelper.cs` | API communication | Change server URL |
| `HWIDHelper.cs` | Hardware identification | Change HWID algorithm |
| `LicenseHelper.cs` | Encryption & storage | Change encryption key |
| `Program.cs` | Entry point | Change startup flow |
| `license-server.js` | Backend API | Change server logic |
| `database.sql` | Database schema | Add new fields |

## Common Tasks

### Add a New Optimization

1. **Add to Settings Model**:
   ```csharp
   // Models/LicenseKey.cs
   public bool MyNewFeature { get; set; }
   ```

2. **Implement Optimization**:
   ```csharp
   // GamerOptimizerHelper.cs
   internal static void MyNewOptimization()
   {
       try
       {
           // Implementation here
           ErrorLogger.Log("My optimization completed");
       }
       catch (Exception ex)
       {
           ErrorLogger.Log($"Error: {ex.Message}");
           throw;
       }
   }
   ```

3. **Add UI Checkbox**:
   ```csharp
   // GamerOptimizerForm.cs - in CreateUI()
   CheckBox myCheckBox = new CheckBox
   {
       Name = "myFeatureCheckBox",
       Text = "Enable My Feature",
       Location = new Point(10, yPosition),
       Checked = _settings.MyNewFeature
   };
   myCheckBox.CheckedChanged += (s, e) => 
       _settings.MyNewFeature = myCheckBox.Checked;
   optionsGroup.Controls.Add(myCheckBox);
   ```

4. **Call from OptimizeButton**:
   ```csharp
   // GamerOptimizerForm.cs - in OptimizeButton_Click()
   if (_settings.MyNewFeature)
   {
       statusLabel.Text = "Applying my feature...";
       GamerOptimizerHelper.MyNewOptimization();
       await Task.Delay(500);
   }
   ```

### Change API Endpoint

```csharp
// LicenseAPIHelper.cs - Line 11
private static readonly string APIBaseUrl = "http://YOUR_NEW_SERVER:8080";
```

### Change License Encryption Key

```csharp
// LicenseHelper.cs - Line 13
private static readonly string EncryptionKey = "YOUR_NEW_32_CHAR_KEY_HERE!!";
```

### Add License Field to Database

1. **Update database.sql**:
   ```sql
   ALTER TABLE licenses ADD COLUMN custom_field VARCHAR(255);
   ```

2. **Update LicenseKey model**:
   ```csharp
   public string CustomField { get; set; }
   ```

3. **Update API response in license-server.js** to include field

### Generate Test License

```bash
# SSH into VPS
mysql -u root -p gamer_optimizer

# Add license
INSERT INTO licenses (license_key, user_email, status, expiry_date)
VALUES ('GAMOPT-2026-DEV-001', 'dev@test.com', 'active', DATE_ADD(NOW(), INTERVAL 1 YEAR));

# Verify
SELECT * FROM licenses WHERE license_key = 'GAMOPT-2026-DEV-001';
```

## API Endpoints

### Validate License (Used by Desktop App)
```
POST /api/validate-key
Content-Type: application/json

{
  "license_key": "GAMOPT-2026-USER-001",
  "hwid": "A1B2C3D4E5F6G7H8..."
}

Response:
{
  "status": "valid|invalid|expired|error",
  "message": "Description",
  "expiry_date": "2026-12-31"
}
```

### Health Check
```
GET /api/health

Response:
{
  "status": "ok",
  "message": "License server is running"
}
```

## Environment Variables

Create `.env` file in backend directory:
```
DB_HOST=localhost
DB_USER=root
DB_PASSWORD=your_password
DB_NAME=gamer_optimizer
PORT=8080
NODE_ENV=production
```

## Build & Deploy Commands

### Build Application
```bash
# Command line
msbuild Optimizer.sln /p:Configuration=Release /p:Platform=AnyCPU

# Or use script
build.bat
```

### Start Backend Server
```bash
# Development (with auto-reload)
npm run dev

# Production (background)
nohup npm start > logs.txt 2>&1 &
```

### Test API
```bash
# Health check
curl http://69.10.60.15:8080/api/health

# Validate license
curl -X POST http://69.10.60.15:8080/api/validate-key \
  -H "Content-Type: application/json" \
  -d '{"license_key":"GAMOPT-2026-TEST-001","hwid":"TEST"}'

# Or use script
test-api.bat
```

## Code Patterns Used

### Safe Registry Modification
```csharp
using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Path\To\Key", true))
{
    if (key != null)
    {
        key.SetValue("ValueName", value, RegistryValueKind.DWord);
    }
}
```

### Process Execution
```csharp
ProcessStartInfo psi = new ProcessStartInfo
{
    FileName = "command.exe",
    Arguments = "arguments",
    UseShellExecute = false,
    RedirectStandardOutput = true,
    CreateNoWindow = true,
    Verb = "runas" // Admin privilege
};

using (Process process = Process.Start(psi))
{
    process.WaitForExit();
}
```

### Error Logging
```csharp
try
{
    // Your code
}
catch (Exception ex)
{
    ErrorLogger.Log($"Description: {ex.Message}");
    throw; // or handle
}
```

### Async API Call
```csharp
LicenseValidationResponse response = 
    await LicenseAPIHelper.ValidateLicenseAsync(licenseKey);

if (response.status == "valid")
{
    // Success
}
else
{
    // Handle error
}
```

## File Locations

| Item | Path |
|------|------|
| License Storage | `%APPDATA%\GamerOptimizer\license.dat` |
| Application Logs | `%APPDATA%\GamerOptimizer\logs\` |
| VPS Files | `/var/www/gamer-optimizer-license-server/` |
| MySQL Database | `gamer_optimizer` |
| Config Template | `backend/.env.example` |

## Testing Scenarios

### Test 1: Valid License
```
Key: GAMOPT-2026-TEST-001
Status: active
Expiry: 2026-12-31
Expected: Valid, app launches
```

### Test 2: Expired License
```
Key: GAMOPT-2026-EXPIRED
Status: active
Expiry: 2020-01-01
Expected: Expired error
```

### Test 3: Invalid Key
```
Key: INVALID-KEY-12345
Expected: License not found error
```

### Test 4: Offline Mode
```
Scenario: No internet connection
Expected: Uses cached local license
```

### Test 5: Hardware Change
```
Scenario: Move license.dat to different PC
Expected: HWID mismatch error
```

## Performance Optimization

- Cache license validation for 24 hours (optional feature)
- Lazy load system info (only when user clicks refresh)
- Use connection pooling for database (already done)
- Minimize registry access
- Queue long-running operations

## Security Checklist

- [ ] API uses HTTPS on production
- [ ] Database passwords not in code
- [ ] Sensitive data encrypted
- [ ] Input validation on all fields
- [ ] SQL injection protection (parameterized queries)
- [ ] Admin privileges required for app
- [ ] Audit logging enabled
- [ ] Error messages don't leak system info

## Debugging Tips

1. **Check VPS Connection**:
   ```bash
   ping 69.10.60.15
   telnet 69.10.60.15 8080
   ```

2. **View Server Logs**:
   ```bash
   tail -f /var/log/gamer-optimizer.log
   ```

3. **Check Database**:
   ```bash
   mysql> SELECT * FROM licenses;
   mysql> SELECT * FROM license_logs ORDER BY created_at DESC;
   ```

4. **Monitor Processes**:
   ```bash
   ps aux | grep node
   ```

5. **Test API Locally**:
   ```bash
   test-api.bat
   ```

## Version Management

Update these files when releasing new version:
1. `version.txt` - Version number
2. `CHANGELOG.md` - Changes made
3. `Program.cs` - Update `Major` and `Minor` constants
4. `package.json` - Backend version

## Useful Links

- GitHub Optimizer: https://github.com/hellzerg/optimizer
- .NET Framework: https://dotnet.microsoft.com/
- Node.js: https://nodejs.org/
- MySQL: https://www.mysql.com/

---

**Tips**: 
- Keep backups of `.env` and `license.dat`
- Test all changes on dev system first
- Monitor license_logs for suspicious activity
- Regular database backups recommended

