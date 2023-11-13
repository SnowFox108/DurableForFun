FROM python:3.8-slim

RUN apt-get update && apt-get install krb5-user libkrb5-dev -y

#RUN addgroup --system --gid 1000 apps
#RUN adduser --system --uid 1000 --ingroup apps --shell /bin/false \ 
    #--disabled-password --no-create-home svc_app

WORKDIR /

COPY init.sh .

RUN chmod +x init.sh

#USER 1000

ENTRYPOINT ["bash", "init.sh"]