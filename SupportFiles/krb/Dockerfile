FROM python:3.8-slim

RUN apt-get update && apt-get install krb5-user libkrb5-dev -y

WORKDIR /
COPY krb5.conf /krb/krb5.conf
COPY zhangm.keytab /krb/zhangm.keytab
COPY rekinit.sh .
RUN chmod 600 /krb/zhangm.keytab

RUN chmod +x rekinit.sh


ENTRYPOINT ["bash", "rekinit.sh"]
