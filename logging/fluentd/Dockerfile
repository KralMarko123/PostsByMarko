FROM fluent/fluentd:latest
RUN mkdir -p /fluentd/etc
RUN ["gem", "install", "fluent-plugin-elasticsearch", "--no-rdoc", "--no-ri"]

EXPOSE 24224