// Gamer Optimizer License Server API
// Deploy on VPS at: http://69.10.60.15:8080

const express = require('express');
const mysql = require('mysql2/promise');
const bodyParser = require('body-parser');
const cors = require('cors');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 8080;

// Middleware
app.use(bodyParser.json());
app.use(cors());

// MySQL Connection Pool
const pool = mysql.createPool({
  host: process.env.DB_HOST || 'localhost',
  user: process.env.DB_USER || 'root',
  password: process.env.DB_PASSWORD || '',
  database: process.env.DB_NAME || 'gamer_optimizer',
  waitForConnections: true,
  connectionLimit: 10,
  queueLimit: 0
});

/**
 * POST /api/validate-key
 * Validate a license key
 * 
 * Request body:
 * {
 *   "license_key": "USER_LICENSE_KEY",
 *   "hwid": "HARDWARE_ID"
 * }
 * 
 * Response:
 * {
 *   "status": "valid" | "invalid" | "expired" | "error",
 *   "message": "Description",
 *   "expiry_date": "2026-12-31"
 * }
 */
app.post('/api/validate-key', async (req, res) => {
  try {
    const { license_key, hwid } = req.body;

    // Validate input
    if (!license_key || !hwid) {
      return res.status(400).json({
        status: 'error',
        message: 'Missing license_key or hwid'
      });
    }

    // Get database connection
    const connection = await pool.getConnection();

    try {
      // Query for license
      const [licenses] = await connection.execute(
        'SELECT * FROM licenses WHERE license_key = ? LIMIT 1',
        [license_key]
      );

      if (licenses.length === 0) {
        return res.json({
          status: 'invalid',
          message: 'License key not found'
        });
      }

      const license = licenses[0];

      // Check if license is active
      if (license.status !== 'active') {
        return res.json({
          status: 'invalid',
          message: 'License is not active'
        });
      }

      // Check if license is locked to a specific HWID
      if (license.hwid_locked && license.hwid_locked !== hwid) {
        return res.json({
          status: 'invalid',
          message: 'License is locked to a different device'
        });
      }

      // Check expiry date
      const expiryDate = new Date(license.expiry_date);
      const now = new Date();

      if (expiryDate < now) {
        return res.json({
          status: 'expired',
          message: 'License has expired',
          expiry_date: license.expiry_date
        });
      }

      // Update last validation date
      await connection.execute(
        'UPDATE licenses SET last_validated = NOW() WHERE id = ?',
        [license.id]
      );

      // Log successful validation
      await connection.execute(
        'INSERT INTO license_logs (license_id, hwid, status, created_at) VALUES (?, ?, ?, NOW())',
        [license.id, hwid, 'validated']
      );

      return res.json({
        status: 'valid',
        message: 'License is valid',
        expiry_date: license.expiry_date
      });

    } finally {
      connection.release();
    }

  } catch (error) {
    console.error('License validation error:', error);
    res.status(500).json({
      status: 'error',
      message: 'Server error during validation'
    });
  }
});

/**
 * POST /api/activate-key
 * Activate a new license key (for first-time activation)
 */
app.post('/api/activate-key', async (req, res) => {
  try {
    const { license_key, hwid } = req.body;

    if (!license_key || !hwid) {
      return res.status(400).json({
        status: 'error',
        message: 'Missing license_key or hwid'
      });
    }

    const connection = await pool.getConnection();

    try {
      const [licenses] = await connection.execute(
        'SELECT * FROM licenses WHERE license_key = ? LIMIT 1',
        [license_key]
      );

      if (licenses.length === 0) {
        return res.json({
          status: 'invalid',
          message: 'License key not found'
        });
      }

      const license = licenses[0];

      // Update HWID if locking to device
      if (!license.hwid_locked) {
        await connection.execute(
          'UPDATE licenses SET hwid_locked = ?, activated_date = NOW() WHERE id = ?',
          [hwid, license.id]
        );
      }

      return res.json({
        status: 'valid',
        message: 'License activated successfully',
        expiry_date: license.expiry_date
      });

    } finally {
      connection.release();
    }

  } catch (error) {
    console.error('License activation error:', error);
    res.status(500).json({
      status: 'error',
      message: 'Server error during activation'
    });
  }
});

/**
 * GET /api/health
 * Health check endpoint
 */
app.get('/api/health', (req, res) => {
  res.json({ status: 'ok', message: 'License server is running' });
});

// Start server
app.listen(PORT, () => {
  console.log(`Gamer Optimizer License Server running on port ${PORT}`);
});

module.exports = app;
