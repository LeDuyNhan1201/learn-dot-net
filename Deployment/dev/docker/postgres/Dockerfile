ARG POSTGRES_TAG=latest
FROM postgres:${POSTGRES_TAG}

RUN mkdir -p /etc/postgres

COPY certs/postgres/postgres.key.pem /etc/postgres/server.key
COPY certs/postgres/postgres.cert.pem /etc/postgres/server.crt
COPY certs/ca/ca.crt /etc/postgres/ca.crt

RUN chmod 600 /etc/postgres/server.key && chown postgres:postgres /etc/postgres/server.key
