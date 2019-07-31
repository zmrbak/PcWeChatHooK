// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "shellapi.h"
#include "resource.h"

VOID ShowDemoUI(HMODULE hModule);
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam);
VOID UnLoadMyself();

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)ShowDemoUI, hModule, NULL, 0);
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
VOID ShowDemoUI(HMODULE hModule)
{
	DialogBox(hModule, MAKEINTRESOURCE(IDD_MAIN), NULL, &DialogProc);
}

//窗口回调函数，处理窗口事件
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam)
{
	switch (uMsg)
	{
	case WM_INITDIALOG:
	{
		break;
	}
	case WM_CLOSE:
		//关闭窗口事件
		EndDialog(hwndDlg, 0);
		break;
	case WM_COMMAND:

		//接收消息
		if (wParam == IDC_UNLOAD)
		{
			//EndDialog(hwndDlg, 0);
			UnLoadMyself();
			break;
		}

		//打开视频帮助页面
		if (wParam == IDC_163CLASS)
		{
			OutputDebugString(TEXT("帮助按钮被点击"));
			ShellExecute(hwndDlg, TEXT("open"), TEXT("http://t.cn/EXUbebQ"), NULL, NULL, NULL);
			break;
		}


		//GIT源代码
		if (wParam == IDC_GIT)
		{
			OutputDebugString(TEXT("QQ群交流按钮被点击"));
			ShellExecute(hwndDlg, TEXT("open"), TEXT("https://github.com/zmrbak/PcWeChatHooK"), NULL, NULL, NULL);
			break;
		}

		//重启微信
		if (wParam == IDC_REBOOT)
		{
			OutputDebugString(TEXT("重启微信按钮被点击"));

			//获取微信程序路径
			TCHAR szAppName[MAX_PATH];
			GetModuleFileName(NULL, szAppName, MAX_PATH);

			//启动新进程
			STARTUPINFO StartInfo;
			ZeroMemory(&StartInfo, sizeof(StartInfo));
			StartInfo.cb = sizeof(StartInfo);

			PROCESS_INFORMATION procStruct;
			ZeroMemory(&procStruct, sizeof(procStruct));
			StartInfo.cb = sizeof(STARTUPINFO);

			if (CreateProcess((LPCTSTR)szAppName, NULL, NULL, NULL, FALSE, NORMAL_PRIORITY_CLASS, NULL, NULL, &StartInfo, &procStruct))
			{
				CloseHandle(procStruct.hProcess);
				CloseHandle(procStruct.hThread);
			}

			//终止当前进程
			TerminateProcess(GetCurrentProcess(), 0);
			break;
		}
		break;
	default:
		break;
	}
	return FALSE;
}

//卸载自己
VOID UnLoadMyself()
{
	HMODULE hModule = NULL;

	//GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS 会增加引用计数
	//因此，后面还需执行一次FreeLibrary
	//直接使用本函数（UnInject）地址来定位本模块
	GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (LPWSTR)& UnLoadMyself, &hModule);

	if (hModule != 0)
	{
		//减少一次引用计数
		FreeLibrary(hModule);

		//从内存中卸载
		FreeLibraryAndExitThread(hModule, 0);
	}
}

