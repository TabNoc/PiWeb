#!/bin/bash
#
# die erste Zeile gibt mit welchem Programm das nachfolgende Scipt susgefuert werder soll. Sinnvoll ist das Script mit x Rechten auszustatten chmod +x script
#Progamm xyz >! .... !, fuert das Programm xyz aus und ÃƒÂ¼bergibt den zwischen ! gespeicherten Text, der Variablen ${V}  enthalten kann.

echo
echo %%%   Hi, das ist das Docker Script fuer PiWeb   %%%
echo
echo
cd /home/pi/dotnet/deploy/

dockerWebsiteTagName="tabnoc/piweb"
dockerServerTagName="tabnoc/wateringserver"
dockerAktorTagName="tabnoc/wateringaktor"
echo Der Docker TagName fuer die Website lautet: $dockerWebsiteTagName
echo Der Docker TagName fuer den Server lautet:  $dockerServerTagName
echo Der Docker TagName fuer den Aktor lautet:  $dockerAktorTagName

echo
dockerRunningWebsiteContainer=$(sudo docker ps -a -q --filter ancestor=$dockerWebsiteTagName --format="{{.ID}}")
dockerRunningServerContainer=$(sudo docker ps -a -q --filter ancestor=$dockerServerTagName --format="{{.ID}}")
dockerRunningAktorContainer=$(sudo docker ps -a -q --filter ancestor=$dockerAktorTagName --format="{{.ID}}")
echo

if [[ $1 = [Ww][Ee][Bb][Ss][Ii][Tt][Ee] ]]; then
cd PiWeb
selectedWebSite=true
selectedServer=false
selectedAktor=false
fi
if [[ $1 = [Ss][Ee][Rr][Vv][Ee][Rr] ]]; then
cd WateringServer
selectedWebSite=false
selectedServer=true
selectedAktor=false
fi
if [[ $1 = [Aa][Kk][Tt][Oo][Rr] ]]; then
cd Aktor
selectedWebSite=false
selectedServer=false
selectedAktor=true
fi

if [[ $2 = [Ll][Oo][Gg][Ss] ]]; then 
	if [ "$selectedServer" = "true" ]; then
		sudo docker logs WebPiServer -f
	fi

	if [ "$selectedWebSite" = "true" ]; then
		sudo docker logs WebPiSite -f
	fi

	if [ "$selectedAktor" = "true" ]; then
		sudo docker logs WebPiAktor -f
	fi
	exit 0
fi


if [ "$selectedWebSite" = "true" ]; then
	if [ -n "$dockerRunningWebsiteContainer" ]; then
		echo Es wurde der Container $dockerRunningWebsiteContainer mit dem TagName $dockerWebsiteTagName gefunden. 
		echo Dieser wird nun gestoppt!
		echo
		sudo docker stop $dockerRunningWebsiteContainer
		echo
	else
		echo Es wurde kein laufender Docker Container mit dem TagName $dockerWebsiteTagName gefunden
		echo
	fi
fi
if [ "$selectedServer" = "true" ]; then
	if [ -n "$dockerRunningServerContainer" ]; then
		echo Es wurde der Container $dockerRunningServerContainer mit dem TagName $dockerServerTagName gefunden. 
		echo Dieser wird nun gestoppt!
		echo
		sudo docker stop $dockerRunningServerContainer
		echo
	else
		echo Es wurde kein laufender Docker Container mit dem TagName $dockerServerTagName gefunden
		echo
	fi
fi
if [ "$selectedAktor" = "true" ]; then
	if [ -n "$dockerRunningAktorContainer" ]; then
		echo Es wurde der Container $dockerRunningAktorContainer mit dem TagName $dockerAktorTagName gefunden. 
		echo Dieser wird nun gestoppt!
		echo
		sudo docker stop $dockerRunningAktorContainer
		echo
	else
		echo Es wurde kein laufender Docker Container mit dem TagName $dockerAktorTagName gefunden
		echo
	fi
fi

#if ! [[ $1 = [Ss][Tt][Aa][Rr][Tt] ]]; then 
	#echo
	#echo Der Tag wird von dem alten Image entfernt
	#sudo docker rmi $dockerTagName
#fi

## if [ $# -gt 0 ]; then 
if [[ $2 = [Ss][Tt][Oo][Pp] ]]; then 
	echo
	echo Der Docker Container sollte nur gestoppt werden!
	echo
	exit 0
fi

if [[ $2 = [Ss][Tt][Aa][Rr][Tt] ]]; then 
	echo
	echo Der Docker Container sollte nur gestartet werden, build wird uebersprungen!
	echo
else
	echo
	echo Der Container wird erstellt
	echo
	if [ "$selectedServer" = "true" ]; then
		sudo docker build . -t $dockerServerTagName
	fi

	if [ "$selectedWebSite" = "true" ]; then
		sudo docker build . -t $dockerWebsiteTagName
	fi

	if [ "$selectedAktor" = "true" ]; then
		cp /home/pi/remote/webserver.py .
		cp /home/pi/remote/WateringCtrl.py .
		cp /home/pi/remote/BusIO.py .
		cp /home/pi/remote/Raspberry-key.pem .
		cp /home/pi/remote/Raspberry-pub.pem .
		sudo docker build . -t $dockerAktorTagName
	fi
	echo
fi

echo Der Conatiner wird gestartet und anschliessend beobachtet
echo
if [ "$selectedServer" = "true" ]; then
	echo Der alte Container fuer den Namen WebPiServer wird zuerst noch geloescht
	sudo docker rm WebPiServer
	sudo docker run -it --init --restart unless-stopped -d -p 5000:5000 --net PiWeb --add-host=database:172.18.0.1 --name WebPiServer $dockerServerTagName
	echo Anwendung wurde mit IP \"$(sudo docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' WebPiServer)\" gestartet
	sudo docker logs WebPiServer -f
fi

if [ "$selectedWebSite" = "true" ]; then
	echo Der alte Container fuer den Namen WebPiSite wird zuerst noch geloescht
	sudo docker rm WebPiSite
	sudo docker run -it --init --restart unless-stopped -d -p 8080:8080 --net PiWeb --name WebPiSite $dockerWebsiteTagName
	echo Anwendung wurde mit IP \"$(sudo docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' WebPiSite)\" gestartet
	sudo docker logs WebPiSite -f
fi

if [ "$selectedAktor" = "true" ]; then
	echo Der alte Container fuer den Namen WebPiAktor wird zuerst noch geloescht
	sudo docker rm WebPiAktor
	sudo docker run -it --init --restart unless-stopped -d -p 443:443 --net PiWeb --name WebPiAktor --device /dev/i2c-1 $dockerAktorTagName
	echo Anwendung wurde mit IP \"$(sudo docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' WebPiAktor)\" gestartet
	sudo docker logs WebPiAktor -f
fi
echo

echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo !!Wenn diese Zeile zu lesen ist, ist ein Fehler aufgetreten!!
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo
#echo Schoenen Tag noch
