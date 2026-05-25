ARG KEYCLOAK_TAG=latest

# ================= BUILD STAGE =================
FROM quay.io/keycloak/keycloak:${KEYCLOAK_TAG} AS builder

ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true
ENV KC_DB=postgres
ENV KC_HTTP_RELATIVE_PATH=/auth
ENV KC_FEATURES=http-optimized-serializers

WORKDIR /opt/keycloak

# Copy certificates
COPY --chown=keycloak:keycloak certs/ca/ca.crt conf/root.crt
COPY --chown=keycloak:keycloak certs/keycloak/keycloak.cert.pem conf/server.crt
COPY --chown=keycloak:keycloak certs/keycloak/keycloak.key.pem conf/server.key

RUN chmod 600 conf/server.key

# Build optimized Keycloak distribution
RUN /opt/keycloak/bin/kc.sh build


# ================= RUNTIME STAGE =================
FROM quay.io/keycloak/keycloak:${KEYCLOAK_TAG}

WORKDIR /opt/keycloak

COPY --from=builder /opt/keycloak/ /opt/keycloak/

ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true
ENV KC_DB=postgres
ENV KC_TLS_HOSTNAME_VERIFIER=ANY

ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]
