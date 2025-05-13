CREATE
DATABASE IF NOT EXISTS CheckoutSagaDb;
CREATE
DATABASE IF NOT EXISTS CheckoutOutboxDb;

CREATE
USER IF NOT EXISTS 'CheckoutSagaDb_user'@'%' IDENTIFIED BY 'CheckoutSagaDb_password';
CREATE
USER IF NOT EXISTS 'CheckoutOutboxDb_user'@'%' IDENTIFIED BY 'CheckoutOutboxDb_password';

GRANT ALL PRIVILEGES ON CheckoutSagaDb.* TO
'CheckoutSagaDb_user'@'%';
GRANT ALL PRIVILEGES ON CheckoutOutboxDb.* TO
'CheckoutOutboxDb_user'@'%';

FLUSH
PRIVILEGES;