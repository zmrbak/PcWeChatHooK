// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include <Windows.h>
#include <string>

using namespace std;

class TEXT_WX
{
public:
	wchar_t* pWxid = nullptr;
	DWORD length = 0;
	DWORD maxLength = 0;
	DWORD fill1 = 0;
	DWORD fill2 = 0;
	wchar_t wxid[1024] = { 0 };

	TEXT_WX(wstring wsWxid)
	{
		const wchar_t* temp = wsWxid.c_str();
		wmemcpy(wxid, temp, wsWxid.length());
		length = wsWxid.length();
		maxLength = wsWxid.capacity();
		fill1 = 0;
		fill2 = 0;
		pWxid = wxid;
	}
};


class  TEXT_WXID
{
public:
	wchar_t* pWxid = nullptr;
	DWORD length = 0;
	DWORD maxLength = 0;
	DWORD fill1 = 0;
	DWORD fill2 = 0;
};



class ROOM_AT
{
public:
	DWORD at_WxidList = 0;
	DWORD at_end1 = 0;
	DWORD at_end2 = 0;
};

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
	DWORD callAddress_SendText = (DWORD)dllAdress + 0x2E4C40;

	//122A8258  13A9A3F0  UNICODE "5847657683@chatroom"
	TEXT_WX wxId(L"5847657683@chatroom");


	//14139DEC  14139CF8  UNICODE "@马天佑 hahhaa"
	TEXT_WX wxMsg(L"@asda 121按设计很好");


	//012CE028  13573FA0  UNICODE "wxid_k2d9oduqc9lc22"
	WCHAR atIt[50] = L"wxid_k2d9oduqc9lc22";
	TEXT_WXID wxAtId;
	wxAtId.pWxid = atIt;
	wxAtId.length = wcslen(atIt);
	wxAtId.maxLength = wcslen(atIt) * 2;
	wxAtId.fill1 = 0;
	wxAtId.fill2 = 0;
	//DWORD* asmWxid = (DWORD*)& wxAtId.pWxid;

	ROOM_AT roomAt;
	roomAt.at_WxidList = (DWORD)& wxAtId.pWxid;
	roomAt.at_end1 = roomAt.at_WxidList + 5 * 4;
	roomAt.at_end2 = roomAt.at_end1;

	//定义一个缓冲区
	BYTE buff[0x81C] = { 0 };

	//执行汇编调用
	__asm
	{
		//wxid
		//0F149BA8    8B55 CC         mov edx, dword ptr ss : [ebp - 0x34]
		//0F149BAB    8D43 14         lea eax, dword ptr ds : [ebx + 0x14]
		//0F149BAE    6A 01           push 0x1
		//0F149BB0    50              push eax
		//0F149BB1    53              push ebx
		//0F149BB2    8D8D E4F7FFFF   lea ecx, dword ptr ss : [ebp - 0x81C]
		//0F149BB8    E8 83B02100     call WeChatWi.0F364C40
		//0F149BBD    83C4 0C         add esp, 0xC


		//mov edx, asmWxid
		lea edx, wxId

		//传递参数
		push 0x1

		//mov eax, 0x0
		lea eax, roomAt
		push eax

		//微信消息内容
		//mov ebx, asmMsg
		lea ebx, wxMsg

		push ebx

		lea ecx, buff

		//调用函数
		call callAddress_SendText

		//平衡堆栈
		add esp, 0xC
	}
}
