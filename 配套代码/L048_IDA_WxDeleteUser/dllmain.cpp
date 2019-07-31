// dllmain.cpp : 定义 DLL 应用程序的入口点。

#include "pch.h"
#include <Windows.h>

VOID DoAction();
//文本消息结构体
struct StructWxid
{
	//发送的文本消息指针
	wchar_t* pWxid;
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
	WCHAR wxid[50] = L"wxid_mss8evt475qp22";
	HMODULE dllAdress = GetModuleHandleA("WeChatWin.dll");
	DWORD callAdrress = (DWORD)dllAdress + 0x26FB50;

	//构建Wxid结构体
	StructWxid structWxid = { 0 };
	structWxid.pWxid = wxid;
	structWxid.length = wcslen(wxid);
	structWxid.maxLength = wcslen(wxid) * 2;

	//取wxid的地址
	DWORD* asmWxid = (DWORD*)& structWxid.pWxid;

	//0FC5DBDD    51              push ecx
	//0FC5DBDE    57              push edi
	//0FC5DBDF    E8 6C1F1000     call WeChatWi.0FD5FB50                   ; 删除用户的ＣＡＬＬ
	//0FC5DBE4    8D8D 58FFFFFF   lea ecx,dword ptr ss:[ebp-0xA8]

	__asm
	{
		mov ecx, 0
		push ecx
		mov edi, asmWxid
		push edi
		call callAdrress
	}
}

