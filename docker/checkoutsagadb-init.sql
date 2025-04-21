CREATE
DATABASE IF NOT EXISTS CheckoutSagaDb;
CREATE
USER IF NOT EXISTS 'CheckoutSagaDb_user'@'%' IDENTIFIED BY 'CheckoutSagaDb_password';
GRANT ALL PRIVILEGES ON CheckoutSagaDb.* TO
'CheckoutSagaDb_user'@'%';
FLUSH
PRIVILEGES;