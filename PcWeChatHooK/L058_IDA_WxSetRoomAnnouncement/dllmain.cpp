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
	DWORD callAdrress = (DWORD)dllAdress + 0x268500;

	//群号
	WCHAR wxid[50] = L"18042774402@chatroom";

	//公告消息
	WCHAR text[1024] = L"这是一个测试！";

	//组装数据
	CHAR bufferA[0xD0] = { 0 };
	DWORD* bufA = (DWORD*)& bufferA;

	CHAR buffer[0xD0] = { 0 };
	DWORD* buf = (DWORD*)& buffer;

	buf[0] = (DWORD)& wxid;
	buf[1] = wcslen(wxid);
	buf[2] = wcslen(wxid) * 2;
	buf[3] = 0;
	buf[4] = 0;

	buf[0 + 5] = (DWORD)& text;
	buf[1 + 5] = wcslen(text);
	buf[2 + 5] = wcslen(text) * 2;
	buf[3 + 5] = 0;
	buf[4 + 5] = 0;

	bufA[0] = (DWORD)& buffer;
	bufA[1] = bufA[0] + 0x60;
	bufA[2] = bufA[0] + 0x60;

	DWORD r_esp = 0;
	__asm
	{
		//保存堆栈寄存器
		mov r_esp, esp

		//654B85B1 | .  8D85 30FFFFFF lea eax, [local.52]
		//654B85B7 | .  50            push eax
		//654B85B8 | .E8 43FF2000   call WeChatWi.656C8500;  发布群公告
		lea eax, bufferA
		push eax
		call callAdrress

		//恢复堆栈寄存器
		mov eax, r_esp
		mov esp, eax
	}
}
