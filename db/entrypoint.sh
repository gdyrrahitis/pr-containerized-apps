#!/bin/bash
echo "$(ls)" 

echo 'start sql server process'
/opt/mssql/bin/sqlservr & ./setup-db.sh & bash