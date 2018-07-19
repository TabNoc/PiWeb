cd dotnet/deploy/
sudo docker build . -t tabnoc/piweb
sudo docker run -it --init --rm -d -p 8080:8080 tabnoc/piweb