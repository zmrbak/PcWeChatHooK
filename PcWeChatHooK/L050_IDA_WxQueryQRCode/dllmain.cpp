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
	DWORD call1 = (DWORD)dllAdress + 0x1E99D0;
	DWORD call2 = (DWORD)dllAdress + 0x2DC880;

	//65D8D34F    E8 AC4CECFF     call WeChatWi.65C52000
	//65D8D354    E8 77C6FFFF     call WeChatWi.65D899D0                   ; 二维码1调用
	//65D8D359    8BC8            mov ecx,eax
	//65D8D35B    E8 20F50E00     call WeChatWi.65E7C880                   ; 二维码2调用
	__asm
	{
		call call1
		mov ecx, eax
		call call2
	}
}

