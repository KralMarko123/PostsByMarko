FROM debian:buster-slim as build
RUN mkdir -p /fluentd/conf
WORKDIR /fluentd/conf
COPY generate_config.sh .
COPY conf .
RUN chmod +x generate_config.sh
RUN /bin/bash generate_config.sh

FROM fluent/fluentd:v1.12.0-debian-1.0
USER root
COPY --from=build /fluentd/conf/ /fluentd/etc/
RUN ["gem", "install", "fluent-plugin-elasticsearch", "--no-document", "--version", "4.3.3"]
USER fluent

EXPOSE 24224