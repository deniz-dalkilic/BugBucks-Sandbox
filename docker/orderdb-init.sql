CREATE DATABASE IF NOT EXISTS OrderDb;
CREATE USER IF NOT EXISTS 'OrderDb_user'@'%' IDENTIFIED BY 'OrderDb_password';
GRANT ALL PRIVILEGES ON OrderDb.* TO 'OrderDb_user'@'%';
FLUSH PRIVILEGES;