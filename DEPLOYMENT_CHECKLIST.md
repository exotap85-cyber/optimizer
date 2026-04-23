# Gamer Optimizer - Deployment Checklist

## ✅ Pre-Deployment Verification

### Desktop Application (C#)

- [ ] All new classes compile without errors
  - [ ] `LicenseKey.cs`
  - [ ] `HWIDHelper.cs`
  - [ ] `LicenseHelper.cs`
  - [ ] `LicenseAPIHelper.cs`
  - [ ] `GamerOptimizerHelper.cs`
  - [ ] `LicenseActivationForm.cs`
  - [ ] `GamerOptimizerForm.cs`

- [ ] `Program.cs` modified correctly
  - [ ] `ValidateLicense()` method added
  - [ ] License check called before MainForm

- [ ] Building succeeds in Release mode
  - [ ] No compilation warnings (unless preexisting)
  - [ ] Output exe exists: `bin/Release/GamerOptimizer.exe`

- [ ] NuGet packages intact
  - [ ] Newtonsoft.Json included
  - [ ] No missing dependencies

### Backend API (Node.js)

- [ ] Files uploaded to VPS
  - [ ] `license-server.js`
  - [ ] `package.json`
  - [ ] `database.sql`
  - [ ] `.env` (customized with credentials)

- [ ] Node.js dependencies installed
  ```bash
  npm install
  # Should complete without errors
  ```

- [ ] MySQL database created
  - [ ] Database: `gamer_optimizer`
  - [ ] Tables created from `database.sql`
  - [ ] Sample data inserted

- [ ] Server starts without errors
  ```bash
  npm start
  # Should listen on port 8080
  ```

### Documentation

- [ ] All docs are in place
  - [ ] `README_GAMER_OPTIMIZER.md`
  - [ ] `SETUP_GUIDE.md`
  - [ ] `DEVELOPER_REFERENCE.md`
  - [ ] `ARCHITECTURE.md`
  - [ ] `IMPLEMENTATION_COMPLETE.md`
  - [ ] `backend/DEPLOYMENT.md`

- [ ] Scripts are functional
  - [ ] `build.bat`
  - [ ] `test-api.bat`

---

## 🚀 Deployment Steps

### Phase 1: Backend Server Setup (VPS)

**Time: ~30 minutes**

- [ ] SSH into VPS
  ```bash
  ssh root@69.10.60.15
  # Password: exo1tap
  ```

- [ ] Update system
  ```bash
  apt-get update && apt-get upgrade -y
  ```

- [ ] Install Node.js
  ```bash
  curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
  apt-get install -y nodejs
  ```

- [ ] Install MySQL (if not present)
  ```bash
  apt-get install -y mysql-server
  ```

- [ ] Create application directory
  ```bash
  mkdir -p /var/www/gamer-optimizer-license-server
  cd /var/www/gamer-optimizer-license-server
  ```

- [ ] Upload files via SCP
  ```bash
  scp license-server.js root@69.10.60.15:/var/www/gamer-optimizer-license-server/
  scp package.json root@69.10.60.15:/var/www/gamer-optimizer-license-server/
  scp database.sql root@69.10.60.15:/var/www/gamer-optimizer-license-server/
  ```

- [ ] Configure environment
  ```bash
  # On VPS
  nano .env
  # Set: DB_HOST, DB_USER, DB_PASSWORD, PORT=8080
  ```

- [ ] Install dependencies
  ```bash
  npm install
  # Should see: added X packages
  ```

- [ ] Create database
  ```bash
  mysql -u root -p < database.sql
  ```

- [ ] Test server startup
  ```bash
  npm start
  # Should see: "Gamer Optimizer License Server running on port 8080"
  ```

- [ ] Stop server
  ```bash
  # Press Ctrl+C
  ```

- [ ] Setup systemd service (optional but recommended)
  ```bash
  sudo nano /etc/systemd/system/gamer-optimizer-license.service
  # Copy service configuration from DEPLOYMENT.md
  
  sudo systemctl enable gamer-optimizer-license
  sudo systemctl start gamer-optimizer-license
  ```

### Phase 2: Test Backend

**Time: ~5 minutes**

- [ ] Test health endpoint
  ```bash
  curl http://69.10.60.15:8080/api/health
  # Should return: {"status":"ok",...}
  ```

- [ ] Test license validation
  ```bash
  curl -X POST http://69.10.60.15:8080/api/validate-key \
    -H "Content-Type: application/json" \
    -d '{"license_key":"GAMOPT-2026-TEST-001","hwid":"TEST"}'
  # Should return: {"status":"valid",...}
  ```

- [ ] Check database records
  ```bash
  mysql -u root -p gamer_optimizer
  SELECT * FROM licenses;
  # Should show test licenses
  ```

- [ ] Verify logs
  ```bash
  SELECT * FROM license_logs ORDER BY created_at DESC LIMIT 5;
  ```

### Phase 3: Build Desktop Application

**Time: ~10 minutes**

- [ ] Update API endpoint (if needed)
  - [ ] File: `LicenseAPIHelper.cs`
  - [ ] Line: `private static readonly string APIBaseUrl`
  - [ ] Verify: `"http://69.10.60.15:8080"`

- [ ] Verify encryption key (optional)
  - [ ] File: `LicenseHelper.cs`
  - [ ] Line: `private static readonly string EncryptionKey`
  - [ ] Use default or customize

- [ ] Open Visual Studio
  - [ ] Load `Optimizer.sln`
  - [ ] Restore NuGet packages

- [ ] Build Release version
  ```bash
  # Visual Studio: Build > Build Solution (Release)
  # Or command line:
  msbuild Optimizer.sln /p:Configuration=Release /p:Platform=AnyCPU
  ```

- [ ] Verify output file
  - [ ] Location: `Optimizer\bin\Release\GamerOptimizer.exe`
  - [ ] Size: ~2-3 MB (with dependencies)

- [ ] Test locally (optional but recommended)
  - [ ] Run as Administrator
  - [ ] Enter test key: `GAMOPT-2026-TEST-001`
  - [ ] Should activate and show dashboard

### Phase 4: Create License Keys

**Time: ~5 minutes**

- [ ] Add test licenses for users
  ```bash
  mysql -u root -p gamer_optimizer
  
  INSERT INTO licenses (license_key, user_email, status, expiry_date)
  VALUES 
    ('GAMOPT-2026-USER-001', 'user1@example.com', 'active', DATE_ADD(NOW(), INTERVAL 1 YEAR)),
    ('GAMOPT-2026-USER-002', 'user2@example.com', 'active', DATE_ADD(NOW(), INTERVAL 6 MONTH));
  
  SELECT * FROM licenses;
  ```

### Phase 5: Distribution

**Time: ~15 minutes**

- [ ] Prepare distribution package
  - [ ] GamerOptimizer.exe
  - [ ] License key (per user)
  - [ ] Instructions document

- [ ] Create download method
  - [ ] Upload exe to website/storage
  - [ ] Create download link
  - [ ] Or distribute via email

- [ ] Document for users
  - [ ] How to download
  - [ ] How to install
  - [ ] How to activate (license key)
  - [ ] Troubleshooting steps

- [ ] Send to beta testers
  - [ ] Include test license key
  - [ ] Request feedback
  - [ ] Monitor for issues

- [ ] Launch to production
  - [ ] Announce on website
  - [ ] Generate customer licenses
  - [ ] Send to customers with instructions

---

## 🧪 Testing Scenarios

### Test Suite 1: License Activation

- [ ] **Valid License**
  - Enter: `GAMOPT-2026-TEST-001`
  - Expected: Activate successfully
  - Result: ✅ Pass / ❌ Fail

- [ ] **Invalid License**
  - Enter: `INVALID-KEY-XXXX`
  - Expected: Show error "not found"
  - Result: ✅ Pass / ❌ Fail

- [ ] **Expired License**
  - Create license with past expiry
  - Enter: that key
  - Expected: Show "expired" error
  - Result: ✅ Pass / ❌ Fail

- [ ] **Copy HWID Button**
  - Click "Copy HWID"
  - Expected: Clipboard has HWID value
  - Result: ✅ Pass / ❌ Fail

### Test Suite 2: Dashboard

- [ ] **System Monitoring**
  - Click "Refresh Status"
  - Expected: RAM, CPU, Disk values update
  - Result: ✅ Pass / ❌ Fail

- [ ] **Individual Optimizations**
  - Enable each checkbox
  - Click "Optimize Now!"
  - Expected: Each optimization completes
  - Result: ✅ Pass / ❌ Fail

- [ ] **Gaming Mode**
  - Enable "Gaming Mode" checkbox
  - All other checkboxes auto-enable
  - Expected: All optimizations enabled
  - Result: ✅ Pass / ❌ Fail

- [ ] **Restore Defaults**
  - Click "Restore Defaults"
  - Expected: System settings reverted
  - Result: ✅ Pass / ❌ Fail

### Test Suite 3: Offline Mode

- [ ] **Cached License**
  - Activate on connected system
  - Disconnect from internet
  - Restart application
  - Expected: Works with cached license
  - Result: ✅ Pass / ❌ Fail

### Test Suite 4: Hardware Lock

- [ ] **HWID Binding**
  - Activate on Computer A
  - Copy license.dat to Computer B
  - Run on Computer B
  - Expected: HWID mismatch error
  - Result: ✅ Pass / ❌ Fail

### Test Suite 5: Error Handling

- [ ] **No Internet**
  - Turn off internet
  - Try to activate new license
  - Expected: Show connection error
  - Result: ✅ Pass / ❌ Fail

- [ ] **Server Down**
  - Stop backend server
  - Try to activate
  - Expected: Show connection error
  - Result: ✅ Pass / ❌ Fail

- [ ] **Invalid Response**
  - Mock invalid JSON response
  - Expected: Handle gracefully
  - Result: ✅ Pass / ❌ Fail

---

## 🔍 Post-Deployment Monitoring

### Daily Checks

- [ ] Server is running
  ```bash
  ps aux | grep node
  ```

- [ ] Database is accessible
  ```bash
  mysql -u root -p gamer_optimizer -e "SELECT COUNT(*) FROM licenses;"
  ```

- [ ] API responds
  ```bash
  curl http://69.10.60.15:8080/api/health
  ```

### Weekly Checks

- [ ] Monitor license_logs for errors
  ```bash
  mysql -u root -p gamer_optimizer -e "SELECT COUNT(*), status FROM license_logs GROUP BY status;"
  ```

- [ ] Check active licenses
  ```bash
  mysql -u root -p gamer_optimizer -e "SELECT COUNT(*) FROM licenses WHERE status='active';"
  ```

- [ ] Review server logs
  ```bash
  tail -50 /var/log/gamer-optimizer.log
  ```

### Monthly Tasks

- [ ] Backup database
  ```bash
  mysqldump -u root -p gamer_optimizer > backup_$(date +%Y%m%d).sql
  ```

- [ ] Review audit logs
  ```bash
  mysql -u root -p gamer_optimizer -e "SELECT * FROM license_logs WHERE DATE(created_at) >= DATE_SUB(NOW(), INTERVAL 30 DAY);"
  ```

- [ ] Check for suspicious activity
  ```bash
  mysql -u root -p gamer_optimizer -e "SELECT license_key, COUNT(*) as validations FROM license_logs WHERE DATE(created_at) >= DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY license_key ORDER BY validations DESC LIMIT 10;"
  ```

---

## 🆘 Troubleshooting Quick Reference

| Problem | Solution |
|---------|----------|
| **API Unreachable** | Check VPS running, port 8080 open, Node process active |
| **License Not Found** | Verify license key in database, check if inserted correctly |
| **HWID Mismatch** | Normal for different devices; create new license for each user |
| **Build Fails** | Ensure Visual Studio installed, NuGet packages restored |
| **Permission Denied** | Run as Administrator, check user permissions |
| **Database Connection Error** | Check MySQL credentials in .env, verify MySQL running |

---

## 📋 Final Sign-Off

- [ ] **Development**: All features working locally
- [ ] **Backend**: Server running on VPS, database populated
- [ ] **Testing**: All test scenarios passed
- [ ] **Documentation**: All guides complete and accurate
- [ ] **Security**: Encryption enabled, credentials not in code
- [ ] **Performance**: Response time acceptable (<1 second)
- [ ] **Monitoring**: Logging and audit trail working
- [ ] **Backup**: Database backup procedure in place
- [ ] **Support**: Documentation complete for users and developers

---

## 🎉 Deployment Complete!

Once all checkboxes are complete, your system is ready for production!

**Deployment Date**: _________________  
**Deployed By**: _________________  
**Version**: 1.0.0  
**Status**: 🟢 PRODUCTION READY

---

**Next Steps**:
1. Monitor system for first 48 hours
2. Respond to user feedback
3. Plan enhancements for v1.1
4. Maintain regular backups

