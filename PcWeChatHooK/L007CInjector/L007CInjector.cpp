// L007CInjector.cpp : 定义应用程序的入口点。
//
#include "framework.h"
#include "L007CInjector.h"
#include "resource1.h"
#include <string.h>
#include <TlHelp32.h>


INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam);

int APIENTRY wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR    lpCmdLine, _In_ int nCmdShow)
{
	DialogBox(NULL, MAKEINTRESOURCE(IDD_MAIN), NULL, &DialogProc);
	return 0;
}

char DllFileName[] = "C:\\Users\\Visual Studio 2019\\Desktop\\L004CHookDll.dll";
DWORD strSize = strlen(DllFileName) + 1;

INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam)
{
	switch (uMsg)
	{
	case WM_COMMAND:
		if (wParam == ID_INJECT)
		{
			wchar_t buff[0x100] = { 0 };
			DWORD weChatProcessID = 0;
			//1)	遍历系统中的进程，找到微信进程（CreateToolhelp32Snapshot、Process32Next）
			HANDLE handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
			swprintf_s(buff, L"CreateToolhelp32Snapshot=%p", handle);
			OutputDebugString(buff);

			PROCESSENTRY32 processentry32 = { 0 };
			processentry32.dwSize = sizeof(PROCESSENTRY32);

			BOOL next = Process32Next(handle, &processentry32);
			while (next == TRUE)
			{
				if (wcscmp(processentry32.szExeFile, L"WeChat.exe") == 0)
				{
					weChatProcessID = processentry32.th32ProcessID;
					break;
				}
				next = Process32Next(handle, &processentry32);
			}
			if (weChatProcessID == 0)
			{
				MessageBox(NULL, L"没找到微信", L"错误", MB_OK);
				return 0;
			}

			//2)	打开微信进程，获得HANDLE（OpenProcess）。
			HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, TRUE, weChatProcessID);
			if (hProcess == NULL)
			{
				MessageBox(NULL, L"打开微信进程失败", L"错误", MB_OK);
				return 0;
			}
			//3)	在微信进程中为DLL文件路径字符串申请内存空间（VirtualAllocEx）。
			LPVOID allocAddress = VirtualAllocEx(hProcess, NULL, strSize, MEM_COMMIT, PAGE_READWRITE);
			if (NULL == allocAddress)
			{
				MessageBox(NULL, L"分配内存空间失败", L"错误", MB_OK);
				return 0;
			}
			swprintf_s(buff, L"VirtualAllocEx=%p", allocAddress);
			OutputDebugString(buff);

			//4)	把DLL文件路径字符串写入到申请的内存中（WriteProcessMemory）
			BOOL result = WriteProcessMemory(hProcess, allocAddress, DllFileName, strSize, NULL);
			if (result == FALSE)
			{
				MessageBox(NULL, L"写入内存失败", L"错误", MB_OK);
				return 0;
			}

			//5)	从Kernel32.dll中获取LoadLibraryA的函数地址（GetModuleHandle、GetProcAddress）
			HMODULE hMODULE = GetModuleHandle(L"Kernel32.dll");
			FARPROC fARPROC = GetProcAddress(hMODULE, "LoadLibraryA");
			if (NULL == fARPROC)
			{
				MessageBox(NULL, L"查找LoadLibraryA失败", L"错误", MB_OK);
				return 0;
			}
			//6)	在微信中启动内存中指定了文件名路径的DLL（CreateRemoteThread）。
			//也就是调用DLL中的DllMain（以DLL_PROCESS_ATTACH为参数）。
			HANDLE hANDLE = CreateRemoteThread(hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)fARPROC, allocAddress, 0, NULL);
			if (NULL == hANDLE)
			{
				MessageBox(NULL, L"启动远程线程失败", L"错误", MB_OK);
				return 0;
			}
		}

		break;
	case WM_CLOSE:
		EndDialog(hwndDlg, 0);
		break;
	default:
		break;
	}
	return FALSE;
}