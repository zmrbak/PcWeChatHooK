// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "Windows.h"
#include "shellapi.h"
#include <tchar.h>
#include <stdio.h>
#include <list>
#include <string>
#include <fstream>
#include <strstream>
#pragma comment(lib, "Version.lib")

using namespace std;

void inlineHook();
void InlinkHookJump();
void OutPutData(int,int);
VOID SaveUserToTxtFie();
BOOL IsWxVersionValid();
VOID CALLBACK TimerProc(HWND hwnd, UINT uMsg, UINT_PTR idEvent, DWORD dwTime);
BOOL IsWxVersionValid();

//定时器ID
DWORD dwTimeId = 0;
//微信基址
DWORD dllAdress = 0;
//跳回地址
DWORD jumpBackAddress = 0;
//此HOOK匹配的微信版本
const string wxVersoin = "2.6.8.65";
//数据库的数量
DWORD dbCount = 0;


//定义一个结构体来存储 数据库句柄-->数据库名
struct DbNameHandle
{
	int DBHandler;
	char DBName[MAX_PATH + 11];
};

//在内存中存储一个“数据库句柄-->数据库名”的链表，
list<DbNameHandle> dbList;


//DLL入口函数
BOOL APIENTRY DllMain(HMODULE hModule,	DWORD  ul_reason_for_call,	LPVOID lpReserved)
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

		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)inlineHook, 0, NULL, 0);
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

//开始InlineHook
void inlineHook()
{
	//循环等待获取微信基址
	while (TRUE)
	{
		dllAdress = (DWORD)GetModuleHandleA("WeChatWin.dll");
		if (dllAdress == 0)
		{
			Sleep(100);
		}
		else
		{
			break;
		}
	}
	if (IsWxVersionValid() == FALSE)
	{
		MessageBoxA(NULL,"微信版本不匹配，请使用2.6.8.65版本！","错误",MB_OK);
		return;
	}

	//7B3F0FA3  |.  8B75 EC       mov esi,[local.5]
	//7B3F0FA6 | .  83C4 08       add esp, 0x8
	//0x‭430FA3‬
	DWORD hookAddress = dllAdress + 0x430FA3;
	
	jumpBackAddress = hookAddress + 6;

	//组装跳转数据
	BYTE JmpCode[6] = { 0 };
	JmpCode[0] = 0xE9;
	JmpCode[6 - 1] = 0x90;

	//新跳转指令中的数据=跳转的地址-原地址（HOOK的地址）-跳转指令的长度
	*(DWORD*)&JmpCode[1] = (DWORD)InlinkHookJump - hookAddress - 5;

	WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, JmpCode, 6, 0);
}

//InlineHook完成后，程序在Hook点跳转到这里执行。
//这里必须是裸函数
__declspec(naked) void InlinkHookJump()
{
	//补充代码
	__asm
	{
		//补充被覆盖的代码
		mov esi, dword ptr ss : [ebp - 0x14]
		add esp, 0x8

		//保存寄存器
		pushad

		//参数2，数据库句柄
		push [ebp - 0x14]
		//参数1，数据库路径地址，ASCII
		push [ebp - 0x28]
		//调用我们的处理函数
		call OutPutData
		add esp, 8

		//恢复寄存器
		popad

		//跳回去接着执行
		jmp jumpBackAddress
	}
}

//把内存中HOOK到的数据存储在链表中，
//重置定时器，5秒钟后激活定时器
void OutPutData(int dbAddress,int dbHandle)
{
	DbNameHandle db = { 0 };
	db.DBHandler = dbHandle;
	_snprintf_s(db.DBName, MAX_PATH + 11, "%s",(char*)dbAddress);
	dbList.push_back(db);
	
	//定时器
	dwTimeId = SetTimer(NULL, 1, 5000, TimerProc);
}

//定时器回调函数
//到时候，把内存中HOOK来的数据保存到一个文本文件中
VOID CALLBACK TimerProc(HWND hwnd, UINT uMsg, UINT_PTR idEvent, DWORD dwTime)
{
	if (dwTimeId == idEvent)
	{
		//关闭定时器
		KillTimer(NULL, 1);
		//把“数据库句柄-->数据库名”的链表保存到一个文本文件中
		SaveUserToTxtFie();
	}
}

//把“数据库句柄-->数据库名”的链表保存到一个文本文件中
VOID SaveUserToTxtFie()
{
	if (dbCount == dbList.size())
	{
		return;
	}
	wstring wxUserFileName = L"DB_Hander.txt";
	DWORD index = 0;

	//作为输出文件打开
	ofstream ofile;
	ofile.open(wxUserFileName, ios_base::trunc | ios_base::binary | ios_base::in);
	/*char const* const utf16head = "\xFF\xFE ";
	ofile.write(utf16head, 2);*/
	DWORD i = 0;
	for (auto& db : dbList)
	{
		i++;
		CHAR sOut[MAX_PATH+11] = {0};
		_snprintf_s(sOut, MAX_PATH + 11, "%02d\t0x%08X\t%s\n", i,db.DBHandler, db.DBName);

		string strintStr = string(sOut);
		char const* pos = (char const*)strintStr.c_str();

		////写入文件
		ofile.write(pos, strintStr.length());
	}
	dbCount = i;
	ofile.flush();
	ofile.close();
	ShellExecute(NULL, NULL, L"notepad.exe", wxUserFileName.c_str(), L".\\", SW_SHOW);
}

//检查微信版本是否匹配
BOOL IsWxVersionValid()
{
	WCHAR VersionFilePath[MAX_PATH];
	if (GetModuleFileName((HMODULE)dllAdress, VersionFilePath, MAX_PATH) == 0)
	{
		return FALSE;
	}

	string asVer = "";
	VS_FIXEDFILEINFO* pVsInfo;
	unsigned int iFileInfoSize = sizeof(VS_FIXEDFILEINFO);
	int iVerInfoSize = GetFileVersionInfoSize(VersionFilePath, NULL);
	if (iVerInfoSize != 0) {
		char* pBuf = new char[iVerInfoSize];
		if (GetFileVersionInfo(VersionFilePath, 0, iVerInfoSize, pBuf)) {
			if (VerQueryValue(pBuf, TEXT("\\"), (void**)&pVsInfo, &iFileInfoSize)) {
				//主版本2.6.7.57
				//2
				int s_major_ver = (pVsInfo->dwFileVersionMS >> 16) & 0x0000FFFF;
				//6
				int s_minor_ver = pVsInfo->dwFileVersionMS & 0x0000FFFF;
				//7
				int s_build_num = (pVsInfo->dwFileVersionLS >> 16) & 0x0000FFFF;
				//57
				int s_revision_num = pVsInfo->dwFileVersionLS & 0x0000FFFF;

				//把版本变成字符串
				strstream wxVer;
				wxVer << s_major_ver << "." << s_minor_ver << "." << s_build_num << "." << s_revision_num;
				wxVer >> asVer;
			}
		}
		delete[] pBuf;
	}

	//版本匹配
	if (asVer == wxVersoin)
	{
		return TRUE;
	}

	//版本不匹配
	return FALSE;
}


