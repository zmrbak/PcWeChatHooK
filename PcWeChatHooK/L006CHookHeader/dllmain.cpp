// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
using namespace L006CsHookDll;

VOID Test()
{
	Class1^ class1 = gcnew Class1();
	class1->ShowMessage();
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		Test();
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}