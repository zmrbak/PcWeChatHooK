// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
DWORD wxBaseAddress = 0;
DWORD jumBackAddress = 0;
DWORD callAddress = 0;
void WriteDebugString(DWORD oldESP);
VOID OutPutDebugStr(DWORD oldESP);
VOID OpenDebugString(HMODULE hModule);

//DLL入口函数
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		//##########################################################
		//
		//注意：仅适配PC微信2.6.8.65版本，其它版本不可用
		//
		//##########################################################

		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)OpenDebugString, hModule, NULL, 0);
		if (hANDLE != 0)
		{
			CloseHandle(hANDLE);
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
//显示操作的窗口
VOID OpenDebugString(HMODULE hModule)
{
	//获取WeChatWin.dll的基址
	while (wxBaseAddress == 0)
	{
		Sleep(100);
		wxBaseAddress = (DWORD)GetModuleHandle(TEXT("WeChatWin.dll"));
	}

	DWORD TLogLevel = 0;
	DWORD Xlogger_IsEnabled = 1;

	DWORD TLogLevel_Address = wxBaseAddress + 0x1208280;
	DWORD Xlogger_IsEnabled_Address = wxBaseAddress + 0x126317D;

	//修改内存
	*((int*)TLogLevel_Address) = TLogLevel;
	*((int*)Xlogger_IsEnabled_Address) = Xlogger_IsEnabled;

	// inline hook
	//6BA6E0E8 | .E8 F38B82FF   call WeChatWi.6B296CE0;  生成信息
	//WeChatWin.dll+A5E0E8 - E8 F38B82FF           - call WeChatWin.dll+286CE0
	DWORD hookAddress = wxBaseAddress + 0xA5E0E8;

	//返回地址
	jumBackAddress = hookAddress + 5;

	//Call的偏移
	DWORD offset = *((int*)((BYTE*)hookAddress + 1));
	callAddress = hookAddress + offset + 5;

	//组装指令
	BYTE jmpCode[5] = { 0 };
	jmpCode[0] = 0xE9;

	*((int*)& jmpCode[1]) = (DWORD)OutPutDebugStr - hookAddress - 5;

	//保存数据
	//BYTE originalCode[5] = { 0 };
	//ReadProcessMemory(GetCurrentProcess(), (LPCVOID)hookAddress, originalCode,5,0);

	//覆盖数据
	WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, jmpCode, 5, 0);
}
__declspec(naked) VOID OutPutDebugStr(DWORD oldESP)
{
	_asm
	{
		//补充被覆盖的代码
		call callAddress

		//保存现场
		mov oldESP, esp

		//添加参数
		push oldESP

		//调用函数
		call WriteDebugString

		//恢复堆栈
		mov esp, oldESP

		//跳回被HOOK指令的下一条指令
		jmp jumBackAddress
	}
}
void WriteDebugString(DWORD oldESP)
{
	DWORD logAddress = *((int*)oldESP);
	char buffer[0x1000] = { 0 };
	ReadProcessMemory(GetCurrentProcess(), (LPCVOID)logAddress, buffer, 0x1000, 0);

	OutputDebugStringA(buffer);
}
