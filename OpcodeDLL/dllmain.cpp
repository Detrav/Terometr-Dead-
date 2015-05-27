#include "stdafx.h"
#include <Windows.h>
#include <string>
#include <fstream>

//Usinng
using namespace std;

//OpCodeAdress TERA CLIENT 30.09 // Ru off 2805
string OpCodeVersion = "2904";
//string OpCodeVersion = "2805";
DWORD GetOpCodeNameAddress = 0x018D5960;
//DWORD GetOpCodeNameAddress = 0x012A1230;//0191AE60; //for Find I_TELEPORT


string GetOpCodeName(DWORD Opcode)
{
	unsigned int pointer = 0;

	_asm
	{
		    push Opcode
			call GetOpCodeNameAddress
			add esp, 4
			mov [pointer], eax
	}

	return string((LPCSTR)pointer);
}

DWORD WINAPI Thread(LPVOID nothing)
{
	ofstream fileOut;
	ofstream fileOut2;
	ofstream fileOutcs;
	fileOut.open(OpCodeVersion + "_OpCodes_Dec.txt", ios::out);
	fileOut2.open(OpCodeVersion + "_OpCodes_Hex.txt", ios::out);
	fileOutcs.open(OpCodeVersion + "_OpCodes.cs", ios::out);
	
	string name = "";
	fileOutcs << " enum OpCode" << OpCodeVersion << " : ushort {" << endl;
	for (int i = 0; i < 0x10000; i++)
	{
		name = GetOpCodeName(i);
		if (name != "")
		{
			//file dec output
			fileOut << name << " = " << i << endl;

			//file2 hex output
			fileOut2 << name << " = 0x" << std::hex << i << endl; //this will print the number in hexadecimal

			//file3 cs version of enum for c#.net
			fileOutcs << name << " = " << i << "," << endl;
		}
		
	}
	fileOutcs << "}";

	fileOut.close();
	fileOut2.close();
	fileOutcs.close();

	return 1;
}


BOOL WINAPI DllMain(HINSTANCE Hinst, DWORD  Reason, LPVOID nothing)
{
	switch (Reason)
	{

		case DLL_PROCESS_ATTACH:
			MessageBoxA(0, "Injected", "Desu", 0);
			CreateThread(0, 0, Thread, 0, 0, 0);
		case DLL_PROCESS_DETACH:
			MessageBoxA(0, "Finished.", "Desu", 0);
	}

	return 1;
}