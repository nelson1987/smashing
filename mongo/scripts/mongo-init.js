
print('Start #################################################################');
db = db.getSiblingDB('sales')

db.createUser({
  user: 'sales',
  pwd: '$SALES_PASSWORD',
  roles: [{ role: 'readWrite', db: 'sales' }],
});
db.createCollection('receipts')
db.createCollection('documents')
db.createCollection('invoices')

db = db.getSiblingDB('warehouse')

db.createUser({
  user: 'warehouse',
  pwd: '$WAREHOUSE_PASSWORD',
  roles: [{ role: 'readWrite', db: 'warehouse' }],
});
db.createCollection('documents')
db.createCollection('stocks')
db.createCollection('invoices')
db.createCollection('orders')

db = db.getSiblingDB('api_prod_db');
db.createUser(
  {
    user: 'api_user',
    pwd: 'api1234',
    roles: [{ role: 'readWrite', db: 'api_prod_db' }],
  },
);
db.createCollection('users');

db = db.getSiblingDB('api_dev_db');
db.createUser(
  {
    user: 'api_user',
    pwd: 'api1234',
    roles: [{ role: 'readWrite', db: 'api_dev_db' }],
  },
);
db.createCollection('users');

db = db.getSiblingDB('api_test_db');
db.createUser(
  {
    user: 'api_user',
    pwd: 'api1234',
    roles: [{ role: 'readWrite', db: 'api_test_db' }],
  },
);
db.createCollection('users');

print('END #################################################################');