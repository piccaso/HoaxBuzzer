FROM postgres
RUN \
    apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y ca-certificates postgresql-plpython-10 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*

ADD db.sql    /docker-entrypoint-initdb.d/01_db.sql
ADD tofDE.sql /docker-entrypoint-initdb.d/02_tofDE.sql