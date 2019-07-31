// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include <stdio.h>
HMODULE g_hDll = NULL;

DWORD WINAPI UnloadProc(PVOID param)
{
	MessageBox(
		NULL,
		TEXT("Press ok to unload me."),
		TEXT("MsgBox in dll"),
		MB_OK);
	HINSTANCE hDLL = LoadLibrary(TEXT("C:\\Users\\libit\\Desktop\\演示代码\\DllTest\\Debug\\DllTest.dll"));

	char str[100];
	sprintf_s(str, "%d", (int)hDLL);
	LPWSTR lpszPath = new WCHAR[100];
	MultiByteToWideChar(CP_ACP, 0, str, 100, lpszPath, 100);
	MessageBox(
		NULL,
		lpszPath,
		TEXT("MsgBox in dll--1"),
		MB_OK);

	if (hDLL != 0)
	{
		FreeLibrary(hDLL);
		FreeLibrary(hDLL);
	}

	MessageBox(
		NULL,
		TEXT("OK"),
		TEXT("MsgBox in dll--2"),
		MB_OK);

	FreeLibraryAndExitThread(g_hDll, 0);
	//FreeLibrary(g_hDll);
	// oops!  
	return 0;
}


BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		g_hDll = (HMODULE)hModule;
		HANDLE hThread = CreateThread(NULL, 0, UnloadProc, NULL, 0, NULL);
		if (hThread != 0)
		{
			CloseHandle(hThread);
		}
		break;
	}
	//FreeLibrary（）
	//FreeLibraryAndExitThread
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

