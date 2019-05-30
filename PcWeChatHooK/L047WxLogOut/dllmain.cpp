// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "Windows.h"

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		HMODULE dllAdress = GetModuleHandleA("WeChatWin.dll");
		DWORD callAdrress = (DWORD)dllAdress + 0x3F2A20;
		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)callAdrress, 0, NULL, 0);
		if (hANDLE != 0)
		{
			CloseHandle(hANDLE);
		}
	}
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}


