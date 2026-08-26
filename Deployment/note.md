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

```yaml
apiVersion: kafka.strimzi.io/v1
kind: KafkaNodePool
metadata:
  name: dual-broker
  labels:
    strimzi.io/cluster: cluster-0
spec:
  replicas: 3
  roles:
    - controller
    - broker
  storage:
    type: jbod
    volumes:
      - id: 0
        type: persistent-claim
        size: 100Gi
        kraftMetadata: shared
---
apiVersion: v1
kind: ConfigMap
metadata:
  name: kafka-metrics
  labels:
    app: strimzi
data:
  kafka-metrics-config.yml: |
    ...
---
apiVersion: kafka.strimzi.io/v1
kind: Kafka
metadata:
  name: cluster-0
spec:
  kafka:
    version: {{ .Values.kafka.version }}
    metadataVersion: {{ .Values.kafka.metadataVersion }}
    listeners:
      - name: ingress-mtls
        port: 9095
        type: ingress
        tls: true
        authentication:
          type: tls
          listenerConfig:
            ssl.client.auth: required
            ssl.principal.mapping.rules: RULE:^CN=(.*?),(.*)$/$1@cluster-0.com/
            ssl.truststore.location: /mnt/kafka-certs/kafka-ca.crt
            ssl.truststore.type: PEM

        configuration:
          hostTemplate: broker-{nodeId}.cluster-0-kafka.test
          bootstrap:
            host: bootstrap.cluster-0-kafka.test

      - name: lb-ssl
        port: 9094
        type: loadbalancer
        tls: true
        configuration:
          externalTrafficPolicy: Cluster # Local: avoids hops to other nodes and preserves the client IP
          brokerCertChainAndKey:
            secretName: server-certs
            certificate: server.crt
            key: server.key

      - name: scram-ssl
        port: 9092
        type: internal
        tls: true
        configuration:
          useServiceDnsDomain: true # false: ignore the suffix of the service DNS domain, eg: [dns.svc].cluster.local

        authentication:
          type: scram-sha-512

          listenerConfig:
            ssl.client.auth: required
            ssl.principal.mapping.rules: RULE:^CN=(.*?),(.*)$/$1@cluster-0.com/
            ssl.truststore.location: /mnt/kafka-certs/kafka-ca.crt
            ssl.truststore.type: PEM

      - name: sasl-ssl
        port: 9093
        type: internal
        tls: true
        configuration:
          useServiceDnsDomain: true # false: ignore the suffix of the service DNS domain, eg: [dns.svc].cluster.local

        authentication:
          type: custom
          sasl: true

          listenerConfig:
            sasl.enabled.mechanisms: OAUTHBEARER
            oauthbearer.sasl.server.callback.handler.class: io.strimzi.kafka.oauth.server.JaasServerOauthValidatorCallbackHandler
            oauthbearer.sasl.jaas.config: >
              org.apache.kafka.common.security.oauthbearer.OAuthBearerLoginModule required

              oauth.fail.fast=true
              oauth.check.audience=true
              oauth.check.issuer=true

              oauth.valid.issuer.uri={{ printf "https://%s/realms/%s" .Values.cluster.oauth.keycloak.baseUrl .Values.cluster.oauth.keycloak.realm | quote }}
              oauth.expected.audience={{ .Values.cluster.oauth.expectedAudience | quote }}
              oauth.username.claim={{ .Values.cluster.oauth.usernameClaim | quote }}
              oauth.jwks.endpoint.uri={{ printf "https://%s/realms/%s/protocol/openid-connect/certs" .Values.cluster.oauth.keycloak.baseUrl .Values.cluster.oauth.keycloak.realm | quote }}
              oauth.fallback.username.claim="preferred_username"

              oauth.jwks.refresh.seconds={{ .Values.cluster.oauth.jwks.refreshSeconds }}
              oauth.jwks.expiry.seconds={{ .Values.cluster.oauth.jwks.expirySeconds }}
              oauth.jwks.refresh.min.pause.seconds={{ .Values.cluster.oauth.jwks.refreshMinPauseSeconds }}

              oauth.ssl.endpoint.identification.algorithm="https"
              oauth.ssl.truststore.type="PEM"
              oauth.ssl.truststore.location="/mnt/oauth-certs/oauth-ca.crt"

              oauth.http.retries={{ .Values.cluster.oauth.http.retries }}
              oauth.http.retry.pause.millis={{ .Values.cluster.oauth.http.retryPauseMillis }}
              oauth.connect.timeout.seconds={{ .Values.cluster.oauth.http.connectTimeoutSeconds }}
              oauth.read.timeout.seconds={{ .Values.cluster.oauth.http.readTimeoutSeconds }}
              oauth.include.accept.header={{ .Values.cluster.oauth.http.includeAcceptHeader }}

              oauth.enable.metrics=true
              oauth.config.id=keycloak

            connections.max.reauth.ms: {{ .Values.cluster.oauth.maxReauthMs }}

    authorization:
      type: custom
      authorizerClass: io.strimzi.kafka.oauth.server.authorizer.KeycloakAuthorizer
      superUsers:
        - service-account-kafka

    config:
      # Strimzi authorization
      principal.builder.class: io.strimzi.kafka.oauth.server.OAuthKafkaPrincipalBuilder
      strimzi.authorization.client.id: kafka
      strimzi.authorization.kafka.cluster.name: cluster-0
      strimzi.authorization.token.endpoint.uri: {{ printf "https://%s/realms/%s/protocol/openid-connect/token" .Values.cluster.oauth.keycloak.baseUrl .Values.cluster.oauth.keycloak.realm | quote }}
      strimzi.authorization.delegate.to.kafka.acl: "false"

      # Grant refresh and caching
      # strimzi.authorization.reuse.grants: true # Default is true
      strimzi.authorization.grants.refresh.period.seconds: {{ .Values.cluster.authorization.grants.refreshPeriodSeconds }}
      strimzi.authorization.grants.refresh.pool.size: {{ .Values.cluster.authorization.grants.refreshPoolSize }}
      strimzi.authorization.grants.max.idle.time.seconds: {{ .Values.cluster.authorization.grants.maxIdleTimeSeconds }}
      strimzi.authorization.grants.gc.period.seconds: {{ .Values.cluster.authorization.grants.gcPeriodSeconds }}

      # HTTP behavior for grant retrieval
      strimzi.authorization.http.retries: {{ .Values.cluster.authorization.http.retries }}
      strimzi.authorization.connect.timeout.seconds: {{ .Values.cluster.authorization.http.connectTimeoutSeconds }}
      strimzi.authorization.read.timeout.seconds: {{ .Values.cluster.authorization.http.readTimeoutSeconds }}

      # TLS validation: Kafka verifies Keycloak server certificate
      strimzi.authorization.ssl.endpoint.identification.algorithm: https
      strimzi.authorization.ssl.truststore.type: PEM
      strimzi.authorization.ssl.truststore.location: /mnt/oauth-certs/oauth-ca.crt

      # Metrics
      strimzi.authorization.enable.metrics: true

      # Other Kafka configuration options
      default.replication.factor: {{ .Values.cluster.defaultReplicationFactor }}
      min.insync.replicas: {{ .Values.cluster.minInSyncReplicas }}
      offsets.topic.replication.factor: {{ .Values.cluster.offsetsTopicReplicationFactor }}
      transaction.state.log.replication.factor: {{ .Values.cluster.transaction.stateLogReplicationFactor }}
      transaction.state.log.min.isr: {{ .Values.cluster.transaction.stateLogMinInSyncReplicas }}

    template:
      pod:
        volumes:
          - name: oauth-certs
            secret:
              secretName: oauth-ca
          - name: kafka-certs
            secret:
              secretName: kafka-ca

      kafkaContainer:
        volumeMounts:
          - name: oauth-certs
            mountPath: /mnt/oauth-certs
            readOnly: true
          - name: kafka-certs
            mountPath: /mnt/kafka-certs
            readOnly: true

    metricsConfig:
      type: jmxPrometheusExporter
      valueFrom:
        configMapKeyRef:
          name: kafka-metrics
          key: kafka-metrics-config.yml

  entityOperator:
    topicOperator:
      watchedNamespace: cluster-0-topic
      reconciliationIntervalMs: 60000

      logging:
        type: inline # external

        valueFrom: # use if type is external
          configMapKeyRef:
            name: operator-logging # name and key are mandatory
            key: log4j2.properties

        loggers:
          rootLogger.level: INFO
          logger.jetty.level: WARN

    userOperator: {}
```