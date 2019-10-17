// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include <stdio.h>
#include <string>
using namespace std;

void selectData();
DWORD dllAdress = 0;

typedef int (*sqlite3_callback)(void*, int, char**, char**);

typedef int(WINAPI* Sqlite3_exec)(
	DWORD,                /* The database on which the SQL executes */
	const char*,           /* The SQL to be executed */
	sqlite3_callback, /* Invoke this callback routine */
	void*,                 /* First argument to xCallback() */
	char**             /* Write error messages here */
	);

int MyCallback(void* para, int nColumn, char** colValue, char** colName);//回调函数

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

		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)selectData, 0, NULL, 0);
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
void selectData()
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
	////sqlite3_exec
	//基址 78CF0000
	//偏移：‭77F6C0‬
	//SQLITE_API int sqlite3_exec(
	//	sqlite3 * db,                /* The database on which the SQL executes */
	//	const char* zSql,           /* The SQL to be executed */
	//	sqlite3_callback xCallback, /* Invoke this callback routine */
	//	void* pArg,                 /* First argument to xCallback() */
	//	char** pzErrMsg             /* Write error messages here */
	//)

	//SQLITE_API int sqlite3_exec(
	//	sqlite3*,                                  /* An open database */
	//	const char* sql,                           /* SQL to be evaluated */
	//	int (*callback)(void*, int, char**, char**),  /* Callback function */
	//	void*,                                    /* 1st argument to callback */
	//	char** errmsg                              /* Error msg written here */
	//);

	Sqlite3_exec sqlite3_exec;
	sqlite3_exec = (Sqlite3_exec)(dllAdress + 0x77F6C0);

	DWORD db = 0x03111D88;
	const char* sql = "select * from sqlite_master";
	char* errmsg = NULL;
	DWORD i = sqlite3_exec(db, sql, MyCallback, NULL, &errmsg);
}

int MyCallback(void* para, int nColumn, char** colValue, char** colName)
{
	//OutputDebugStringA("----------------------------------------------------\n");

	CHAR sOut[MAX_PATH + 11] = { 0 };
	_snprintf_s(sOut, MAX_PATH + 11, "包含的列数：%d\n", nColumn);
	//OutputDebugStringA(sOut);
	MessageBoxA(NULL, sOut, "", MB_OK);

	for (int i = 0; i < nColumn; i++)
	{
		CHAR sOut[1024] = { 0 };
		_snprintf_s(sOut, 1024, "%s :%s\n", *(colName + i), colValue[i]);
		//OutputDebugStringA(sOut);
		MessageBoxA(NULL, sOut, "", MB_OK);
	}
	return 0;
}