// dllmain.cpp : 定义 DLL 应用程序的入口点。

#include "pch.h"
#include <Windows.h>

VOID DoAction();
//文本消息结构体
struct STRUCT_TEXT
{
	//发送的文本消息指针
	wchar_t* pText;
	//字符串长度
	DWORD length;
	//字符串最大长度
	DWORD maxLength;

	//补充两个占位数据
	DWORD fill1 = 0;
	DWORD fill2 = 0;
};


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
	//0D91BF18  0F211828  UNICODE "wxid_wy7g93iuyjml22"
	//0D91BF18  0F211828  UNICODE "xmxm659878"
	//0D97D8C8  0DB0BAB0  UNICODE "wxid_k2d9oduqc9lc22"

	//0D97D8C8  0DB0BAB0  UNICODE "4342363136@chatroom"




	WCHAR wxid[50] = L"wxid_wy7g93iuyjml22";
	HMODULE dllAdress = GetModuleHandleA("WeChatWin.dll");
	DWORD callAdrress = (DWORD)dllAdress + 0x2E4C40;

	//构建Wxid结构体
	STRUCT_TEXT structWxid = { 0 };
	structWxid.pText = wxid;
	structWxid.length = wcslen(wxid);
	structWxid.maxLength = structWxid.length * 2;

	//取wxid的地址
	DWORD* asmWxid = (DWORD*)& structWxid.pText;


	//构建消息内容
	WCHAR wxText[50] = L"你好，我好，大家好！";
	STRUCT_TEXT structText = { 0 };
	structText.pText = wxText;
	structText.length = wcslen(wxText);
	structText.maxLength = structText.length * 2;

	//取文本内容地址
	DWORD* asmText = (DWORD*)& structText.pText;

	CHAR buffer[0x81C] = { 0 };

	//DWORD returnValue = 0;

//5E709BA8  |.  8B55 CC       |mov edx,[local.13]                      ;  WXID
//5E709BAB  |.  8D43 14       |lea eax,dword ptr ds:[ebx+0x14]         ;  0
//5E709BAE  |.  6A 01         |push 0x1                                ;  1
//5E709BB0  |.  50            |push eax                                ;  0
//5E709BB1  |.  53            |push ebx                                ;  WXMSG
//5E709BB2  |.  8D8D E4F7FFFF |lea ecx,[local.519]                     ;  BUFFER 0x81C
//5E709BB8  |.  E8 83B02100   |call WeChatWi.5E924C40
//5E709BBD  |.  83C4 0C       |add esp,0xC
//5E709BC0  |.  50            |push eax



	__asm
	{
		mov edx, asmWxid
		lea eax, buffer
		mov ebx, asmText
		lea ecx, buffer

		push 0x1
		push eax
		push ebx
		call callAdrress
		add esp, 0xC

		//mov returnValue,eax
	}
}