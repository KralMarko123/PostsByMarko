#!/bin/bash
fluentdTemplateConf=$(<fluent.conf.template)
printf '%s\n' "$fluentdTemplateConf" > fluent.conf