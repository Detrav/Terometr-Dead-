========================================================================
    DYNAMIC LINK LIBRARY: OpcodeDLL-Projektübersicht
========================================================================

Diese OpcodeDLL-DLL wurde vom Anwendungs-Assistenten für Sie erstellt.

Diese Datei bietet eine Übersicht über den Inhalt der einzelnen Dateien, aus
denen Ihre OpcodeDLL-Anwendung besteht.


OpcodeDLL.vcxproj
    Dies ist die Hauptprojektdatei für VC++-Projekte, die mit dem Anwendungs-Assistenten generiert werden. Sie enthält Informationen über die Version von Visual C++, mit der die Datei generiert wurde, sowie über die Plattformen, Konfigurationen und Projektfunktionen, die im Anwendungs-Assistenten ausgewählt wurden.

OpcodeDLL.vcxproj.filters
    Dies ist die Filterdatei für VC++-Projekte, die mithilfe eines Anwendungs-Assistenten erstellt werden. Sie enthält Informationen über die Zuordnung zwischen den Dateien im Projekt und den Filtern. Diese Zuordnung wird in der IDE zur Darstellung der Gruppierung von Dateien mit ähnlichen Erweiterungen unter einem bestimmten Knoten verwendet (z. B. sind CPP-Dateien dem Filter "Quelldateien" zugeordnet).

OpcodeDLL.cpp
    Dies ist die Hauptquelldatei der DLL.

	Diese DLL exportiert keine Symbole, Dies führt dazu, dass sie beim Buildvorgang keine LIB-Datei generiert. Wenn dieses Projekt eine Projektabhängigkeit eines anderen Projekts sein soll, müssen Sie entweder Code zum Exportieren einiger Symbole aus der DLL hinzufügen, damit eine Exportbibliothek erstellt wird, oder Sie können im Ordner "Linker" auf der Eigenschaftenseite "Allgemein" im Dialogfeld "Eigenschaftenseiten" des Projekts die Eigenschaft "Ignore Input Library" auf "Ja" festlegen.

/////////////////////////////////////////////////////////////////////////////
Andere Standarddateien:

StdAfx.h, StdAfx.cpp
    Mit diesen Dateien werden eine vorkompilierte Headerdatei (PCH) mit dem Namen OpcodeDLL.pch und eine vorkompilierte Typendatei mit dem Namen StdAfx.obj erstellt.

/////////////////////////////////////////////////////////////////////////////
Weitere Hinweise:

Der Anwendungs-Assistent weist Sie mit "TODO:"-Kommentaren auf Teile des
Quellcodes hin, die Sie ergänzen oder anpassen sollten.

/////////////////////////////////////////////////////////////////////////////



Guide to the Opcode DLL:

Code from tagyourit50, released at ragezone: http://forum.ragezone.com/f797/release-tera-live-packet-sniffer-1052922-post8369480/#post8369480


#########
Post Copy:
For those wondering how to get the opcode names here's a little guide.

1. In cheat engine, find the function that references the string "I_TELEPORT". Look just above that and find the start of the function. (In this case 0x0191D0A0)



2. Make a c++ dll that looks like http://pastebin.com/qTGzrW8w with the address that you got in the previous step. 

3. Now inject it into the Tera process, after which you should see some message boxes.
Click OK on those, then in your TERA\Client\Binaries folder you should see a file called opcodes.txt full of opcodes. 
(You can just change the output directory if you want)

#######


If you want to use the file with the packet editor, use the decimal output file, copy it to the packetviewer directory and name it "opcodefile.txt"

######
GoneUp
