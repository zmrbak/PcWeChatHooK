// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include <Windows.h>

VOID DoAction();

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		//##########################################################
		//
		//注意：仅适配PC微信2.6.7.57版本，其它版本不可用
		//
		//##########################################################
		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)DoAction, NULL, NULL, 0);
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

VOID DoAction()
{
	HMODULE dllAdress = GetModuleHandleA("WeChatWin.dll");
	DWORD address = (DWORD)dllAdress + 0x125D050 + 0x194;
	if (*(DWORD*)address == 0)
	{
		MessageBox(NULL, L"微信未登录", L"信息", MB_OK);
	}
	else
	{
		MessageBox(NULL, L"微信已登陆", L"信息", MB_OK);
	}
}