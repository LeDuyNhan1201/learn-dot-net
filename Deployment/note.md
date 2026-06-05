```shell
docker exec keycloak0 /opt/keycloak/bin/kc.sh export \
		--realm learn-dot-netl-dev \
		--dir /opt/keycloak/data/export \
		--users realm_file \
		--log-level info
```

```shell
docker exec keycloak0 /opt/keycloak/bin/kc.sh export \
		--realm learn-dot-netl-dev \
		--dir /opt/keycloak/data/export \
        --users different_files --users-per-file 200 \
        --log-level info
```

```shell
docker run -d --name kafka-ui -p 8088:8080 \
-e KAFKA_CLUSTERS_0_NAME=local \
-e KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS=b-1-public.mskcluster.feggi5.c2.kafka.ap-southeast-2.amazonaws.com:9196,b-2-public.mskcluster.feggi5.c2.kafka.ap-southeast-2.amazonaws.com:9196 \
-e KAFKA_CLUSTERS_0_PROPERTIES_SECURITY_PROTOCOL=SASL_SSL \
-e KAFKA_CLUSTERS_0_PROPERTIES_SASL_MECHANISM=SCRAM-SHA-512 \
-e KAFKA_CLUSTERS_0_PROPERTIES_SASL_JAAS_CONFIG='org.apache.kafka.common.security.scram.ScramLoginModule required username="admin" password="Xk!J7%#r@avW67j*d3%M1JpM#i%jH*&5kX3&J2yjkZf2x";' \
provectuslabs/kafka-ui:latest
```