// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"

DWORD WINAPI UnloadProc(PVOID param)
{
	while (true)
	{
		Sleep(100);
	}
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		HANDLE hThread = CreateThread(NULL, 0, UnloadProc, NULL, 0, NULL);
		if (hThread != 0)
		{
			CloseHandle(hThread);
		}
		break;
	}
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

