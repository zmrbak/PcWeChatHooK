// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "resource.h"
#include "shellapi.h"
#include <string>
#include <tchar.h> 
#include <iostream>
#include <sstream>
#include <string>
#include <iomanip>


using namespace std;

//声明函数
VOID ShowDemoUI(HMODULE hModule);
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam);
void SentTextMessage(HWND hwndDlg);
LPCWSTR String2LPCWSTR(string text);
string Dec2Hex(DWORD i);
WCHAR* CharToWChar(char* s);

//定义变量
DWORD wxBaseAddress = 0;

//DLL入口函数，该DLL被加载后，会调用该函数
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
	//获取WeChatWin.dll的基址
	wxBaseAddress = (DWORD)GetModuleHandle(TEXT("WeChatWin.dll"));
	string text = "微信基址：\t";
	text.append(Dec2Hex(wxBaseAddress));
	OutputDebugString(String2LPCWSTR(text));

	DialogBox(hModule, MAKEINTRESOURCE(IDD_MAIN), NULL, &DialogProc);
}

//窗口回调函数，处理窗口事件
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam)
{
	switch (uMsg)
	{
	case WM_INITDIALOG:
		break;

	case WM_CLOSE:
		//关闭窗口事件
		EndDialog(hwndDlg, 0);
		break;
	case WM_COMMAND:

		//发送消息
		if (wParam == ID_SEND)
		{
			OutputDebugString(TEXT("发送消息按钮被点击"));
			SentTextMessage(hwndDlg);
		}

		//打开视频帮助页面
		if (wParam == ID_HELP)
		{
			OutputDebugString(TEXT("帮助按钮被点击"));
			ShellExecute(hwndDlg, TEXT("open"), TEXT("http://t.cn/EXUbebQ"), NULL, NULL, NULL);
		}

		//把wxid设置为自己的wxid
		if (wParam == ID_SELF)
		{
			OutputDebugString(TEXT("自己的微信ID按钮被点击："));

			//判断微信是否登录
			int* loginStatus = (int*)(wxBaseAddress + 0x125D56C);
			if (*loginStatus == 0)
			{
				MessageBoxA(NULL, "请先登录微信", "错误", MB_OK | MB_ICONERROR);
				break;
			}

			string text = "MyWxid地址:";
			text.append(Dec2Hex(wxBaseAddress + 0x125D4EC));
			OutputDebugString(String2LPCWSTR(text));

			//二级指针，读出自己的wxid
			char** myWxid = (char**)(wxBaseAddress + 0x125D4EC);
			char p[50];
			strcpy_s(p, *myWxid);

			WCHAR* WCHAR_MyWxid = CharToWChar(p);
			//WCHAR WCHAR_MyWxid[21];
			//memset(WCHAR_MyWxid, 0, sizeof(WCHAR_MyWxid));

			MultiByteToWideChar(CP_ACP, 0, *myWxid, 21, WCHAR_MyWxid, 21);
			OutputDebugString(WCHAR_MyWxid);

			HWND hFileHelper = GetDlgItem(hwndDlg, IDC_WXID);
			SetWindowText(hFileHelper, WCHAR_MyWxid);
		}

		//QQ群交流
		if (wParam == ID_GITHUB)
		{
			OutputDebugString(TEXT("ID_GITHUB按钮被点击"));
			ShellExecute(hwndDlg, TEXT("open"), TEXT("https://github.com/zmrbak/PcWeChatHooK"), NULL, NULL, NULL);
		}

		//重启微信
		if (wParam == ID_RESTART)
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
				//WaitForSingleObject(procStruct.hProcess, INFINITE);
				CloseHandle(procStruct.hProcess);
				CloseHandle(procStruct.hThread);
			}

			//终止当前进程
			TerminateProcess(::GetCurrentProcess(), 0);
		}
		break;
	default:
		break;
	}
	return FALSE;
}

WCHAR* CharToWChar(char* s)
{
	int w_nlen = MultiByteToWideChar(CP_ACP, 0, s, -1, NULL, 0);
	WCHAR* ret = (WCHAR*)malloc(sizeof(WCHAR) * w_nlen);
	memset(ret, 0, sizeof(ret));
	MultiByteToWideChar(CP_ACP, 0, s, -1, ret, w_nlen);
	return ret;
}

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
	DWORD fill1;
	DWORD fill2;
};

//将int转成16进制字符串
string Dec2Hex(DWORD i)
{
	//定义字符串流
	stringstream ioss;
	//存放转化后字符
	string s_temp;
	//以十六制(大写)形式输出
	ioss.fill('0');
	ioss << setiosflags(ios::uppercase) << setw(8) << hex << i;
	//以十六制(小写)形式输出//取消大写的设置
	//ioss << resetiosflags(ios::uppercase) << hex << i;
	ioss >> s_temp;
	return "0x" + s_temp;
}

//把string 转换为 LPCWSTR
LPCWSTR String2LPCWSTR(string text)
{
	//原型：
	//typedef _Null_terminated_ CONST WCHAR *LPCWSTR, *PCWSTR;
	//typedef wchar_t WCHAR;

	size_t size = text.length();
	WCHAR* buffer = new WCHAR[size + 1];
	MultiByteToWideChar(CP_ACP, 0, text.c_str(), -1, buffer, size + 1);

	//确保以 '\0' 结尾
	buffer[size] = 0;
	return buffer;
}

string WcharToString(WCHAR* wchar)
{
	WCHAR* wText = wchar;
	// WideCharToMultiByte的运用
	DWORD dwNum = WideCharToMultiByte(CP_OEMCP, NULL, wText, -1, NULL, 0, NULL, FALSE);
	// psText为char*的临时数组，作为赋值给std::string的中间变量
	char* psText = new char[dwNum];
	// WideCharToMultiByte的再次运用
	WideCharToMultiByte(CP_OEMCP, NULL, wText, -1, psText, dwNum, NULL, FALSE);
	// std::string赋值
	return psText;
}

VOID SentTextMessage(HWND hwndDlg)
{
	string text = "";


	//判断微信是否登录
	int* loginStatus = (int*)(wxBaseAddress + 0x125D56C);
	if (*loginStatus == 0)
	{
		MessageBoxA(NULL, "请先登录微信", "错误", MB_OK | MB_ICONERROR);
		return;
	}

	//定位发送消息的Call的位置
	//WeChatWin.dll+C9BB8
	//	WeChatWin.dll + C9BB8 - E8 83B02100 - call WeChatWin.dll + 2E4C40

	DWORD callAddress_SendText = wxBaseAddress + 0x2E4C40;

	text = "Call地址:";
	text.append(Dec2Hex(callAddress_SendText));
	OutputDebugString(String2LPCWSTR(text));

	//组装wxid数据
	WCHAR wxid[50];
	UINT uINT = GetDlgItemText(hwndDlg, IDC_WXID, wxid, 50);
	if (uINT == 0)
	{
		MessageBoxA(NULL, "请填写wxid", "错误", MB_OK | MB_ICONERROR);
		return;
	}

	text = "目标wxid:\t";
	text.append(WcharToString(wxid));
	OutputDebugString(String2LPCWSTR(text));

	StructWxid structWxid = { 0 };
	structWxid.pWxid = wxid;
	structWxid.length = wcslen(wxid);
	structWxid.maxLength = wcslen(wxid) * 2;

	text = "微信ID长度:";
	text.append(Dec2Hex(structWxid.length));
	OutputDebugString(String2LPCWSTR(text));


	//structWxid.Init();
	//取wxid的地址
	DWORD* asmWxid = (DWORD*)& structWxid.pWxid;


	//组装发送的文本数据
	WCHAR wxMsg[1024];
	uINT = GetDlgItemText(hwndDlg, IDC_WXMSG, wxMsg, 1024);
	if (uINT == 0)
	{
		MessageBoxA(NULL, "请填写要发送的文本", "错误", MB_OK | MB_ICONERROR);
		return;
	}
	text = "发送内容:\t";
	text.append(WcharToString(wxMsg));
	OutputDebugString(String2LPCWSTR(text));

	StructWxid structMessage = { 0 };
	structMessage.pWxid = wxMsg;
	structMessage.length = wcslen(wxMsg);
	structMessage.maxLength = wcslen(wxMsg) * 2;

	text = "发送内容长度:";
	text.append(Dec2Hex(structMessage.length));
	OutputDebugString(String2LPCWSTR(text));

	//structMessage.Init();
	//取msg的地址
	DWORD* asmMsg = (DWORD*)& structMessage.pWxid;

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


		mov edx, asmWxid

		//传递参数
		push 0x1

		mov eax, 0x0
		push eax

		//微信消息内容
		mov ebx, asmMsg
		push ebx

		lea ecx, buff

		//调用函数
		call callAddress_SendText

		//平衡堆栈
		add esp, 0xC
	}
}
