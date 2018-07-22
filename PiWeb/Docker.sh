#!/bin/bash
#
# die erste Zeile gibt mit welchem Programm das nachfolgende Scipt susgefuert werder soll. Sinnvoll ist das Script mit x Rechten auszustatten chmod +x script
#Progamm xyz >! .... !, fuert das Programm xyz aus und Ã¼bergibt den zwischen ! gespeicherten Text, der Variablen ${V}  enthalten kann.

echo
echo %%%   Hi, das ist das Docker Script fuer PiWeb   %%%
echo
echo
cd dotnet/deploy/

dockerTagName="tabnoc/piweb"
echo Der Docker TagName lautet: $dockerTagName

echo
dockerRunningContainer=$(sudo docker ps -a -q --filter ancestor=$dockerTagName --format="{{.ID}}")
echo

if [ -n "$dockerRunningContainer" ]; then
	echo Es wurde der Container $dockerRunningContainer mit dem TagName $dockerTagName gefunden. 
	echo Dieser wird nun gestoppt!
	echo
	sudo docker stop $dockerRunningContainer
	echo
else
	echo Es wurde kein laufender Docker Container mit dem TagName $dockerTagName gefunden
	echo
fi

#if ! [[ $1 = [Ss][Tt][Aa][Rr][Tt] ]]; then 
	#echo
	#echo Der Tag wird von dem alten Image entfernt
	#sudo docker rmi $dockerTagName
#fi

## if [ $# -gt 0 ]; then 
if [[ $1 = [Ss][Tt][Oo][Pp] ]]; then 
	echo
	echo Der Docker Container sollte nur gestoppt werden!
	echo
	exit 0
fi

if [[ $1 = [Ss][Tt][Aa][Rr][Tt] ]]; then 
	echo
	echo Der Docker Container sollt<e nur gestartet werden, build wird uebersprungen!
	echo
else
	echo
	echo Der Container wird erstellt
	echo
	sudo docker build . -t $dockerTagName
	echo
fi

echo Der Conatiner wird gestartet und anschliessend beobachtet
echo
sudo docker logs $(sudo docker run -it --init --restart unless-stopped -d -p 8080:8080 $dockerTagName) -f
echo

echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo !!Wenn diese Zeile zu lesen ist, ist ein Fehler aufgetreten!!
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo
#echo Schoenen Tag noch
