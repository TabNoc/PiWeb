FROM python:2
#RUN apt-get install python-smbus
COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt
COPY . .
CMD [ "python", "./webserver.py" ]