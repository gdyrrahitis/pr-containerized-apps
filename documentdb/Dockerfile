FROM mongo
EXPOSE 27019

RUN mkdir -p /usr/config

COPY documentdb/scripts .
COPY documentdb/entrypoint.sh .

RUN chmod +x entrypoint.sh

ENTRYPOINT ["/bin/sh", "entrypoint.sh"]
#CMD mongod --smallfiles && mongoimport --host=127.0.0.1 --db ordersDb --collection ordersCollection --drop --type json --file /init.json --jsonArray
