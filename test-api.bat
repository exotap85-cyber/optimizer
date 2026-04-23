@echo off
REM Test License Server Connection

echo.
echo ===============================================
echo  Gamer Optimizer - License Server Test
echo ===============================================
echo.

set API_URL=http://69.10.60.15:8080
set HEALTH_ENDPOINT=%API_URL%/api/health
set VALIDATE_ENDPOINT=%API_URL%/api/validate-key

echo Testing connection to: %API_URL%
echo.

REM Test health endpoint
echo [Test 1] Health Check...
curl -s %HEALTH_ENDPOINT% >nul 2>nul
if %ERRORLEVEL% EQU 0 (
    echo [✓] Server is reachable
    curl -s %HEALTH_ENDPOINT%
    echo.
) else (
    echo [✗] Server is not reachable
    echo.
    echo Troubleshooting:
    echo - Verify VPS is running: ssh root@69.10.60.15
    echo - Check port 8080 is open: telnet 69.10.60.15 8080
    echo - Verify license server is running on VPS
    pause
    exit /b 1
)

echo.
echo [Test 2] License Validation...
echo Sending test license key: GAMOPT-2026-TEST-001
echo.

REM Create test JSON payload
setlocal enabledelayedexpansion
set PAYLOAD={"license_key":"GAMOPT-2026-TEST-001","hwid":"TEST_HWID_12345"}

REM Use curl.exe and pipe JSON to avoid quoting issues across shells
echo %PAYLOAD% | curl.exe -s -X POST %VALIDATE_ENDPOINT% ^
  -H "Content-Type: application/json" ^
  --data-binary @-

echo.
echo.
echo ===============================================
echo  Test Complete
echo ===============================================
echo.

pause
