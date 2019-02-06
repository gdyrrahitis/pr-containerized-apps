#!/bin/bash
echo "$(ls)" 

echo 'start sql server process'
/opt/mssql/bin/sqlservr & sh ./setup-db.sh & bash