-- Gamer Optimizer License Database Schema

CREATE DATABASE IF NOT EXISTS gamer_optimizer;
USE gamer_optimizer;

-- Licenses Table
CREATE TABLE licenses (
  id INT PRIMARY KEY AUTO_INCREMENT,
  license_key VARCHAR(255) UNIQUE NOT NULL,
  user_email VARCHAR(255),
  status ENUM('active', 'inactive', 'suspended') DEFAULT 'active',
  created_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  activated_date DATETIME,
  expiry_date DATETIME NOT NULL,
  hwid_locked VARCHAR(255),
  last_validated DATETIME,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_license_key (license_key),
  INDEX idx_email (user_email),
  INDEX idx_status (status)
);

-- License Logs Table (for auditing)
CREATE TABLE license_logs (
  id INT PRIMARY KEY AUTO_INCREMENT,
  license_id INT NOT NULL,
  hwid VARCHAR(255),
  status VARCHAR(50),
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (license_id) REFERENCES licenses(id) ON DELETE CASCADE,
  INDEX idx_license_id (license_id),
  INDEX idx_created_at (created_at)
);

-- Admin Users Table (optional - for admin panel)
CREATE TABLE admin_users (
  id INT PRIMARY KEY AUTO_INCREMENT,
  username VARCHAR(100) UNIQUE NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  email VARCHAR(255),
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_username (username)
);

-- Insert sample data
INSERT INTO licenses (license_key, user_email, expiry_date, status)
VALUES 
  ('GAMOPT-2026-TEST-001', 'test@example.com', DATE_ADD(NOW(), INTERVAL 1 YEAR), 'active'),
  ('GAMOPT-2026-TEST-002', 'user@example.com', DATE_ADD(NOW(), INTERVAL 6 MONTH), 'active');
