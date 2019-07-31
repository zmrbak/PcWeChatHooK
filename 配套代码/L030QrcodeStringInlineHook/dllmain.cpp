// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
void Hook();
void Test();
void CallFunc();

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		HANDLE hANDLE = CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)Hook, NULL, NULL, NULL);
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
DWORD wechatWinBaseAddress = 0;
DWORD hookAddress = 0;
void Hook()
{
	wechatWinBaseAddress = (DWORD)GetModuleHandle(TEXT("WeChatWin.dll"));
	//WeChatWin.dll+39012A - 8B CC                 - mov ecx,esp
	hookAddress = wechatWinBaseAddress + 0x390125;

	//组装跳转数据
	BYTE JmpCode[5] = { 0 };
	JmpCode[0] = 0xE9;

	//新跳转指令中的数据=跳转的地址-原地址（HOOK的地址）-跳转指令的长度
	*(DWORD*)& JmpCode[1] = (DWORD)Test - hookAddress - 5;

	//先备份出来
	//再修改
	WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, JmpCode, 5, 0);

}

DWORD C_EAX;
DWORD C_EBX;
DWORD C_ECX;
DWORD C_EDX;

DWORD C_ESI;
DWORD C_EDI;
DWORD C_EBP;
DWORD C_ESP;
DWORD returnAddress;

char  QrcodeBytes[20] = { 0 };
__declspec(naked)  void Test()
{
	__asm
	{
		mov C_EAX, eax
		mov C_EBX, ebx
		mov C_ECX, ecx
		mov C_EDX, edx

		mov C_ESI, esi
		mov C_EDI, edi
		mov C_EBP, ebp
		mov C_ESP, esp
	}

	ReadProcessMemory(GetCurrentProcess(), (LPCVOID)C_EDX, QrcodeBytes, 20, 0);
	CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)CallFunc, NULL, NULL, NULL);
	returnAddress = wechatWinBaseAddress + 0x39012A;
	__asm
	{
		mov eax, C_EAX
		mov ebx, C_EBX
		mov ecx, C_ECX
		mov edx, C_EDX

		mov esi, C_ESI
		mov edi, C_EDI
		mov ebp, C_EBP
		mov esp, C_ESP

		//00B00000 - 83 EC 10              - sub esp,10 { 16 }
		//00B00003 - 8B CC                 - mov ecx,esp
		//00B00005 - E9 22018469           - jmp WeChatWin.dll+39012C
		sub esp, 0x10
		mov ecx, esp
		jmp returnAddress
	}
}

void CallFunc()
{
	TCHAR temp[20];
	MultiByteToWideChar(CP_ACP, NULL, QrcodeBytes, -1, temp, 20);
	MessageBox(NULL, temp, TEXT("OK"), NULL);
}

