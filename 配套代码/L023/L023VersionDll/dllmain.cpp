//被劫持的DLL文件：C:\WINDOWS\SYSTEM32\VERSION.dll
//在编译之后，请把文件名改为: VERSION.dll
//然后，放在此文件夹中:C:\Program Files (x86)\Tencent\WeChat\

//VS 2019中的编译设置
//配置属性->连接器->常规->启用增量链接:否 (/INCREMENTAL:NO)

//应用程序将在加载此DLL之后
//如果微信目录下存在WxHook.dll，则会自动加载此DLL
//你也可以将其指定为其它DLL文件，请自行修改LoadHookDll()中的相的参数。

//包含的头文件
#pragma once
#include "pch.h"
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <strsafe.h>

//宏定义
#define EXTERNC extern "C"
#define NAKED __declspec(naked)
#define EXPORT __declspec(dllexport)
#define ALCPP EXPORT NAKED
#define ALSTD EXTERNC EXPORT NAKED void __stdcall
#define ALCFAST EXTERNC EXPORT NAKED void __fastcall
#define ALCDECL EXTERNC EXPORT NAKED void __cdecl

//变量定义
#pragma region OriginalAddress
PVOID pfnGetFileVersionInfoA;
PVOID pfnGetFileVersionInfoByHandle;
PVOID pfnGetFileVersionInfoExA;
PVOID pfnGetFileVersionInfoExW;
PVOID pfnGetFileVersionInfoSizeA;
PVOID pfnGetFileVersionInfoSizeExA;
PVOID pfnGetFileVersionInfoSizeExW;
PVOID pfnGetFileVersionInfoSizeW;
PVOID pfnGetFileVersionInfoW;
PVOID pfnVerFindFileA;
PVOID pfnVerFindFileW;
PVOID pfnVerInstallFileA;
PVOID pfnVerInstallFileW;
PVOID pfnVerLanguageNameA;
PVOID pfnVerLanguageNameW;
PVOID pfnVerQueryValueA;
PVOID pfnVerQueryValueW;

#pragma endregion

// 原始模块句柄
HMODULE m_hModule = NULL;

// 微信HOOK的DLL句柄
HMODULE hModuleWxHook = NULL;

// 获取原始函数地址
FARPROC WINAPI GetAddress(PCSTR pszProcName)
{
	return GetProcAddress(m_hModule, pszProcName);
}

// 加载原始模块
inline BOOL WINAPI Load()
{
	TCHAR szPath[MAX_PATH] = {};
	//获取系统目录
	GetSystemDirectory(szPath, MAX_PATH);
	StringCchCat(szPath, MAX_PATH, TEXT("\\VERSION.dll"));

	m_hModule = ::LoadLibraryW(szPath);
	return (m_hModule != NULL);
}

// 卸载模块
inline VOID UnLoad()
{
	if (m_hModule)
	{
		::FreeLibrary(m_hModule);
		m_hModule = NULL;
	}
}


//预先获取所有原函数的地址
#pragma region GetOriginalAddress
FARPROC WINAPI GetOriginalAddress()
{
	pfnGetFileVersionInfoA = GetAddress("GetFileVersionInfoA");
	pfnGetFileVersionInfoByHandle = GetAddress("GetFileVersionInfoByHandle");
	pfnGetFileVersionInfoExA = GetAddress("GetFileVersionInfoExA");
	pfnGetFileVersionInfoExW = GetAddress("GetFileVersionInfoExW");
	pfnGetFileVersionInfoSizeA = GetAddress("GetFileVersionInfoSizeA");
	pfnGetFileVersionInfoSizeExA = GetAddress("GetFileVersionInfoSizeExA");
	pfnGetFileVersionInfoSizeExW = GetAddress("GetFileVersionInfoSizeExW");
	pfnGetFileVersionInfoSizeW = GetAddress("GetFileVersionInfoSizeW");
	pfnGetFileVersionInfoW = GetAddress("GetFileVersionInfoW");
	pfnVerFindFileA = GetAddress("VerFindFileA");
	pfnVerFindFileW = GetAddress("VerFindFileW");
	pfnVerInstallFileA = GetAddress("VerInstallFileA");
	pfnVerInstallFileW = GetAddress("VerInstallFileW");
	pfnVerLanguageNameA = GetAddress("VerLanguageNameA");
	pfnVerLanguageNameW = GetAddress("VerLanguageNameW");
	pfnVerQueryValueA = GetAddress("VerQueryValueA");
	pfnVerQueryValueW = GetAddress("VerQueryValueW");

	return NULL;
}
#pragma endregion

//装载微信HOOK
VOID LoadHookDll()
{
	//只对WeChat.exe进行注入的DLL加载
	HMODULE hModule = GetModuleHandle(TEXT("WeChat.exe"));
	if (hModule == 0)
	{
		return;
	}

	WIN32_FIND_DATA FindFileData;
	HANDLE  hFind = FindFirstFile(TEXT("L023WeChatHookDll.dll"), &FindFileData);

	if (hFind != INVALID_HANDLE_VALUE)
	{
		hModuleWxHook = ::LoadLibraryW(TEXT("L023WeChatHookDll.dll"));
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 入口函数
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hModule);
		if (Load())
		{
			GetOriginalAddress();
			LoadHookDll();
		}
		else
		{
			return FALSE;
		}
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		UnLoad();
		break;
	}
	return TRUE;
}

// 函数被调用时，转发到系统原始DLL的函数
#pragma region OriginalLibraryApi
namespace OriginalLibraryApi
{

	ALCDECL GetFileVersionInfoA(void)
	{
		__asm	JMP pfnGetFileVersionInfoA;
	}

	ALCDECL GetFileVersionInfoByHandle(void)
	{
		__asm	JMP pfnGetFileVersionInfoByHandle;
	}

	ALCDECL GetFileVersionInfoExA(void)
	{
		__asm	JMP pfnGetFileVersionInfoExA;
	}

	ALCDECL GetFileVersionInfoExW(void)
	{
		__asm	JMP pfnGetFileVersionInfoExW;
	}

	ALCDECL GetFileVersionInfoSizeA(void)
	{
		__asm	JMP pfnGetFileVersionInfoSizeA;
	}

	ALCDECL GetFileVersionInfoSizeExA(void)
	{
		__asm	JMP pfnGetFileVersionInfoSizeExA;
	}

	ALCDECL GetFileVersionInfoSizeExW(void)
	{
		__asm	JMP pfnGetFileVersionInfoSizeExW;
	}

	ALCDECL GetFileVersionInfoSizeW(void)
	{
		__asm	JMP pfnGetFileVersionInfoSizeW;
	}

	ALCDECL GetFileVersionInfoW(void)
	{
		__asm	JMP pfnGetFileVersionInfoW;
	}

	ALCDECL VerFindFileA(void)
	{
		__asm	JMP pfnVerFindFileA;
	}

	ALCDECL VerFindFileW(void)
	{
		__asm	JMP pfnVerFindFileW;
	}

	ALCDECL VerInstallFileA(void)
	{
		__asm	JMP pfnVerInstallFileA;
	}

	ALCDECL VerInstallFileW(void)
	{
		__asm	JMP pfnVerInstallFileW;
	}

	ALCDECL VerLanguageNameA(void)
	{
		__asm	JMP pfnVerLanguageNameA;
	}

	ALCDECL VerLanguageNameW(void)
	{
		__asm	JMP pfnVerLanguageNameW;
	}

	ALCDECL VerQueryValueA(void)
	{
		__asm	JMP pfnVerQueryValueA;
	}

	ALCDECL VerQueryValueW(void)
	{
		__asm	JMP pfnVerQueryValueW;
	}
}
#pragma endregion