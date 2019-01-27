FROM microsoft/mssql-server-linux:2017-latest
EXPOSE 1433

RUN mkdir -p /usr/config

COPY db/sql .
COPY db/entrypoint.sh .
COPY db/setup-db.sh .

RUN chmod +x entrypoint.sh
RUN chmod +x setup-db.sh

ENTRYPOINT ["/bin/sh", "entrypoint.sh"]

CMD ["tail -f dev/null"]
HEALTHCHECK --interval=15s CMD /opt/mssql-tools/bin/sqlcmd -U sa -P Pass@word -Q "select 1" && grep -q "MSSQL CONFIG COMPLETE" ./config.log