FROM python:3.8-slim

RUN apt-get update && apt-get install krb5-user libkrb5-dev -y

#RUN addgroup --system --gid 1000 apps
#RUN adduser --system --uid 1000 --ingroup apps --shell /bin/false \ 
    #--disabled-password --no-create-home svc_app

WORKDIR /
COPY krb5.conf /krb/krb5.conf
COPY zhangm.keytab /krb/zhangm.keytab
COPY rekinit.sh .
RUN chmod 600 /krb/zhangm.keytab
#RUN kinit ZhangM -k -t /etc/zhangm.keytab

RUN chmod +x rekinit.sh

#USER 1000

ENTRYPOINT ["bash", "rekinit.sh"]
