#!/bin/bash
databaseName="CatalogDb"
wait_time=15s
password=Pass@word

# wait for SQL Server to come up
echo 'importing data will start in $wait_time...'
sleep $wait_time
echo 'importing data...'

echo 'executing init.sql'
# run the init script to create the DB and required table
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./init.sql"
echo 'database has been created'

echo 'executing table-init.sql'
# run the init script to create the DB and required table
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./table-init.sql" -d $databaseName
echo 'table has been created'

echo 'executing seed.sql'
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./seed.sql" -d $databaseName
echo 'table data has been seeded'

echo 'done seeding db...'