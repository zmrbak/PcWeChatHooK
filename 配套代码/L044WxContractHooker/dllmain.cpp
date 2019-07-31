// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "resource.h"
#include "shellapi.h"
#include <string>
#include <tchar.h> 
#include <sstream>
#include <string>
#include <iomanip>
#include <strstream>
#include <list>
#include <iostream>
#include <map>
#include <tuple>
#include <CommCtrl.h>
#include<fstream>
#include<iomanip>

#pragma comment(lib, "Version.lib")

using namespace std;

//声明函数
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam);
VOID	ShowDemoUI(HMODULE hModule);
VOID	HookWx();
VOID	UnHookWx();
VOID	InsertUserList();
VOID	RecieveUserInfoHook();
string	Dec2Hex(DWORD i);
LPCWSTR String2LPCWSTR(string text);
LPCWSTR GetMsgByAddress(DWORD memAddress);
BOOL	IsWxVersionValid();
wstring String2Wstring(string str);
VOID	UnInject();
VOID	InitListContrl();
VOID	ListContrlSetItems();
wstring GetWStringByAddress(DWORD memAddress);
VOID	CheckLoginFinished();
VOID	SaveToTxtFie();
std::string wstringToString(const std::wstring& wstr);
//定义变量
DWORD wxBaseAddress = 0;
//HOOK标志
BOOL isWxHooked = FALSE;
//对话框句柄
HWND hWinDlg;
//跳回地址
DWORD jumBackAddress = 0;
//我们要提取的寄存器内容
DWORD r_esi = 0;
//此HOOK匹配的微信版本
const string wxVersoin = "2.6.7.57";
//我自己的微信ID
string myWxId = "";
HWND hListView = NULL;

DWORD overWritedCallAdd = 0;
DWORD hookAddress = 0;
BOOL isCheckLoginRunning = FALSE;


typedef tuple <
	//wxid1
	wstring,
	//wxName
	wstring,
	//v1
	wstring,
	//nickName
	wstring
> USER_INFO;


//定义7000个用户列表
list<USER_INFO> userInfoList(1);

//使用VS+Detours调试，必须一个没用的导出函数
VOID __declspec(dllexport) Test()
{
	//OutputDebugString(TEXT("开始调试"));
}


//DLL入口函数
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
	while (wxBaseAddress == 0)
	{
		wxBaseAddress = (DWORD)GetModuleHandle(TEXT("WeChatWin.dll"));
		Sleep(100);
	}

	DialogBox(hModule, MAKEINTRESOURCE(IDD_MAIN), NULL, &DialogProc);
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


//将int转成16进制字符串
string Dec2Hex(DWORD i)
{
	//定义字符串流
	stringstream ioss;

	//存放转化后字符
	string s_temp;

	//以8位十六制(大写)形式输出
	ioss.fill('0');
	ioss << setiosflags(ios::uppercase) << setw(8) << hex << i;
	ioss >> s_temp;

	return "0x" + s_temp;
}

//窗口回调函数，处理窗口事件
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam)
{
	hWinDlg = hwndDlg;
	switch (uMsg)
	{
	case WM_INITDIALOG:
	{
		//检查微信版本
		if (IsWxVersionValid() == FALSE)
		{
			string msg = "微信版本不匹配\n";
			msg.append("请使用PC微信");
			msg.append(wxVersoin);
			MessageBox(NULL, String2LPCWSTR(msg), TEXT("错误"), MB_OK | MB_ICONERROR);

			EndDialog(hwndDlg, 0);
			UnInject();
			break;
		}

		//在登录微信前注入
		//判断微信是否登录
		int* loginStatus = (int*)(wxBaseAddress + 0x125D56C);
		if (*loginStatus != 0)
		{
			MessageBox(NULL, TEXT("请在登录微信前启用此功能"), TEXT("错误"), MB_OK | MB_ICONERROR);
			EndDialog(hwndDlg, 0);
			UnInject();
			break;
		}

		InitListContrl();

		//启动就HOOK
		//减少因不会使用带来的额外疑问
		HookWx();

		break;
	}
	case WM_CLOSE:
		//关闭窗口事件
		EndDialog(hwndDlg, 0);
		break;
	case WM_COMMAND:

		//接收消息
		if (wParam == IDC_START)
		{
			HookWx();
			break;
		}

		//保存到文件
		if (wParam == IDC_SAVE)
		{
			//UnHookWx();

			//HWND hFileHelper = GetDlgItem(hwndDlg, IDC_MSG);
			//SetWindowText(hFileHelper, TEXT("停止接收微信消息......"));
			SaveToTxtFie();
			break;
		}

		//IDC_REFRESH
		if (wParam == IDC_REFRESH)
		{
			ListContrlSetItems();
			break;
		}

		//打开视频帮助页面
		if (wParam == IDC_163CLASS)
		{
			ShellExecute(hwndDlg, TEXT("open"), TEXT("http://t.cn/EXUbebQ"), NULL, NULL, NULL);
			break;
		}


		//GitHub
		if (wParam == IDC_GITHUB)
		{
			ShellExecute(hwndDlg, TEXT("open"), TEXT("https://github.com/zmrbak/PcWeChatHooK"), NULL, NULL, NULL);
			break;
		}

		//重启微信
		if (wParam == IDC_WX_REBOOT)
		{
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

//Hook接收消息
VOID HookWx()
{
	//判断是否已经HOOK
	if (isWxHooked == FALSE)
	{
		isWxHooked = TRUE;

		//启动一个线程用来更新ListView
		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)CheckLoginFinished, NULL, NULL, 0);
		if (hANDLE != 0)
		{
			CloseHandle(hANDLE);
		}

		//WeChatWin.dll+175FA4 - 0F85 6B020000         - jne WeChatWin.dll+176215
		//WeChatWin.dll+175FAA - E8 0186EDFF           - call WeChatWin.dll+4E5B0
		//WeChatWin.dll+175FAF - 53                    - push ebx
		//#######################################################################
		//下面5个字节，进行HOOK
		////HOOK
		//WeChatWin.dll+175FB0 - E8 4BA60F00           - call WeChatWin.dll+270600
		//#######################################################################
		//WeChatWin.dll+175FB5 - 83 F8 03              - cmp eax,03 { 3 }
		//WeChatWin.dll+175FB8 - 0F85 43010000         - jne WeChatWin.dll+176101
		//WeChatWin.dll+175FBE - 8D 8D 28FFFFFF        - lea ecx,[ebp-000000D8]

		hookAddress = wxBaseAddress + 0x175FB0;
		string debugMsg = "Hook的地址：\t";
		debugMsg.append(Dec2Hex(hookAddress));
		OutputDebugString(String2LPCWSTR(debugMsg));

		//被覆盖的CALL目标
		overWritedCallAdd = wxBaseAddress + 0x270600;

		//跳回的地址
		jumBackAddress = hookAddress + 5;

		//组装跳转数据
		BYTE jmpCode[5] = { 0 };
		jmpCode[0] = 0xE9;

		//新跳转指令中的数据=跳转的地址-原地址（HOOK的地址）-跳转指令的长度
		*(DWORD*)& jmpCode[1] = (DWORD)RecieveUserInfoHook - hookAddress - 5;

		//覆盖指令
		//WeChatWin.dll + 310573 - B9 E8CF2B10 - mov ecx, WeChatWin.dll + 125CFE8{ (0) }
		WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, jmpCode, 5, 0);
	}


	HWND hEditor = GetDlgItem(hWinDlg, IDC_MSG);
	SetWindowText(hEditor, TEXT("开始准备接收微信消息......\r\n"));
}

//UnHook接收消息
VOID UnHookWx()
{
	if (isWxHooked == TRUE)
	{
		//恢复指令
		//#######################################################################
		//下面5个字节，进行HOOK
		////HOOK
		//WeChatWin.dll+175FB0 - E8 4BA60F00           - call WeChatWin.dll+270600
		//#######################################################################

		BYTE originalCode[5] = { 0xE8,0x4B,0xA6,0x0F,0x00 };

		//恢复指令
		WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, originalCode, 5, 0);

		isWxHooked = FALSE;
	}
}

//跳转到这里，让我们自己处理消息
__declspec(naked) VOID RecieveUserInfoHook()
{
	//#######################################################################
	//下面5个字节，进行HOOK
	////HOOK
	//WeChatWin.dll+175FB0 - E8 4BA60F00           - call WeChatWin.dll+270600
	//#######################################################################


	//保存现场
	__asm
	{
		//提取eax寄存器内容，放在一个变量中
		mov r_esi, esi

		//保存寄存器
		pushad
		pushf
	}

	//调用接收消息的函数
	InsertUserList();

	//恢复现场
	__asm
	{
		popf
		popad

		//补充被覆盖的代码
		call overWritedCallAdd

		//跳回被HOOK指令的下一条指令
		jmp jumBackAddress
	}
}

VOID InsertUserList()
{
	//ESI
	//群信息
	//$+8      >0E6663E8  UNICODE "10048068352@chatroom"
	//$+C      >00000014
	//$+10     >00000020
	//$+30     >0E754838  UNICODE "v1_861e5d0c5f981fd285a767a14d77ef0f84c9165df4b1f2a"
	//$+34     >0000006C
	//$+38     >00000080
	//$+48     >00000003
	//$+64     >0E78AB98  UNICODE "富足人生分享02"
	//$+68     >00000008
	//$+6C     >00000008
	//$+A0     >00000001
	//$+A4     >0E78AC18  UNICODE "FZRSFX02"
	//$+A8     >00000008
	//$+AC     >00000008
	//$+B8     >0E665CB8  UNICODE "fuzurenshengfenxiang02"
	//$+BC     >00000016
	//$+C0     >00000020
	//$+130    >0000000F
	//$+13C    >00000001
	//$+140    >00000001
	//$+148    >0E74E548
	//$+14C    >00000176

	//个人微信号信息
	//$+8      >0E715000  UNICODE "mu082921"
	//$+C      >00000008
	//$+10     >00000008
	//$+1C > 0E25E4A8  UNICODE "wz028a"
	//$+30     >0E754DB0  UNICODE "v1_299504fbcb8f9b4dd6bcd047a27e18592784624319e92ad"
	//$+34     >0000004C
	//$+38     >00000080
	//$+64     >0E253F30  UNICODE "青龙·嘉禾园微时代【招代理】"
	//$+68     >0000000E
	//$+6C     >00000010
	//$+A0     >00000001
	//$+A4     >0E253FC0  UNICODE "QLJHYWSDZDL"
	//$+A8     >0000000B
	//$+AC     >00000010
	//$+B8     >0E64A558  UNICODE "qinglongjiaheyuanweishidaizhaodaili"
	//$+BC     >00000023
	//$+C0     >00000040
	//$+130    >0000000F
	//$+13C    >00000001
	//$+140    >00000001
	//$+148    >047DB360
	//$+14C    >000002AA


	//公众号？
	//$ ==>    >00001000
	//$+8      >035FF3B0  UNICODE "weixin"
	//$+C      >00000006
	//$+10     >00000008
	//$+30     >0E754EC8  UNICODE "v1_c8d90e1960c4e8aca7ac312ad40f0b8696a323335d624b7"
	//$+34     >0000004C
	//$+38     >00000080
	//$+48     >00000001
	//$+4C     >00000038
	//$+64     >0480B520  UNICODE "微信团队"
	//$+68     >00000004
	//$+6C     >00000004
	//$+A0     >00000001
	//$+A4     >0480B610  UNICODE "WXTD"
	//$+A8     >00000004
	//$+AC     >00000004
	//$+B8     >0E25E0C0  UNICODE "weixintuandui"
	//$+BC     >0000000D
	//$+C0     >00000010
	//$+130    >0000000F
	//$+13C    >00000001
	//$+140    >00000001
	//$+148    >0E7512A8
	//$+14C    >0000028E


	//个人微信号、群号
	wstring wxid1 = GetMsgByAddress(r_esi + 0x8);
	wstring wxName = GetMsgByAddress(r_esi + 0x1C);
	wstring v1 = GetMsgByAddress(r_esi + 0x30);
	//个人微信昵称、群昵称
	wstring nickName = GetMsgByAddress(r_esi + 0x64);
	USER_INFO userInfo(wxid1, wxName, v1, nickName);

	for (auto& userInfoOld : userInfoList)
	{
		wstring wxid = get<0>(userInfoOld);
		if (wxid == wxid1)
		{
			return;
		}
	}

	userInfoList.push_front(userInfo);
}


//读取内存中的字符串
//存储格式
//xxxxxxxx:字符串地址（memAddress）
//xxxxxxxx:字符串长度（memAddress +4）
LPCWSTR GetMsgByAddress(DWORD memAddress)
{
	//获取字符串长度
	DWORD msgLength = *(DWORD*)(memAddress + 4);
	DWORD msgMaxLength = *(DWORD*)(memAddress + 4);
	if (msgLength == 0 || msgLength > msgMaxLength)
	{
		WCHAR* msg = new WCHAR[1];
		msg[0] = 0;
		return msg;
	}

	WCHAR* msg = new WCHAR[msgLength + 1];
	ZeroMemory(msg, msgLength + 1);

	//复制内容
	wmemcpy_s(msg, msgLength + 1, (WCHAR*)(*(DWORD*)memAddress), msgLength + 1);
	return msg;
}

wstring GetHexStringByAddress(DWORD memAddress)
{
	//获取字符串长度
	DWORD data = *(DWORD*)(memAddress);

	//定义字符串流
	wstringstream ioss;

	//存放转化后字符
	wstring s_temp;

	//以8位十六制(大写)形式输出
	ioss.fill('0');
	ioss << setiosflags(ios::uppercase) << setw(8) << hex << data;
	ioss >> s_temp;

	return L"0x" + s_temp;
}

VOID CheckLoginFinished()
{
	if (isCheckLoginRunning == TRUE)
	{
		return;
	}

	isCheckLoginRunning = TRUE;
	DWORD oldListLenA = 0;
	DWORD oldListLenB = 0;

	while (true)
	{
		DWORD listLen = userInfoList.size();
		if (oldListLenA != listLen)
		{
			oldListLenA = listLen;
			oldListLenB = 0;
			Sleep(1000);
			continue;
		}

		if (oldListLenB == 0)
		{
			//更新列表
			ListContrlSetItems();
			oldListLenB = oldListLenA;
		}

		Sleep(500);
	}
}
//检查微信版本是否匹配
BOOL IsWxVersionValid()
{
	WCHAR VersionFilePath[MAX_PATH];
	if (GetModuleFileName((HMODULE)wxBaseAddress, VersionFilePath, MAX_PATH) == 0)
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
			if (VerQueryValue(pBuf, TEXT("\\"), (void**)& pVsInfo, &iFileInfoSize)) {
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


//将string转换成wstring  
wstring String2Wstring(string str)
{
	wstring result;
	//获取缓冲区大小，并申请空间，缓冲区大小按字符计算  
	int len = MultiByteToWideChar(CP_ACP, 0, str.c_str(), str.size(), NULL, 0);
	TCHAR* buffer = new TCHAR[len + 1];
	//多字节编码转换成宽字节编码  
	MultiByteToWideChar(CP_ACP, 0, str.c_str(), str.size(), buffer, len);
	buffer[len] = '\0';             //添加字符串结尾  
	//删除缓冲区并返回值  
	result.append(buffer);
	delete[] buffer;
	return result;
}

//注入的DLL自己卸载自己
VOID UnInject()
{
	HMODULE hModule = NULL;

	//GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS 会增加引用计数
	//因此，后面还需执行一次FreeLibrary
	//直接使用本函数（UnInject）地址来定位本模块
	GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (LPWSTR)& UnInject, &hModule);

	if (hModule != 0)
	{
		//减少一次引用计数
		FreeLibrary(hModule);
		//从内存中卸载
		FreeLibraryAndExitThread(hModule, 0);
	}
}

//向初始化ListView
void InitListContrl()
{
	hListView = GetDlgItem(hWinDlg, IDC_LIST);

	ListView_SetExtendedListViewStyleEx(hListView,
		LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES,
		LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES
	);

	DWORD i = 0;

	//添加ListView表头
	LVCOLUMN pcol = { 0 };
	pcol.mask = LVCF_FMT | LVCF_WIDTH | LVCF_TEXT | LVCF_SUBITEM;
	pcol.fmt = LVCFMT_LEFT;
	pcol.cx = 50;
	pcol.pszText = (LPWSTR)L"序号";
	ListView_InsertColumn(hListView, i++, &pcol);

	pcol.cx = 140;
	pcol.pszText = (LPWSTR)L"微信ID";
	ListView_InsertColumn(hListView, i++, &pcol);

	pcol.cx = 120;
	pcol.pszText = (LPWSTR)L"微信名";
	ListView_InsertColumn(hListView, i++, &pcol);

	pcol.cx = 200;
	pcol.pszText = (LPWSTR)L"昵称";
	ListView_InsertColumn(hListView, i++, &pcol);
}

//向ListView中添加数据
VOID ListContrlSetItems()
{
	//列表清空
	ListView_DeleteAllItems(hListView);

	//添加数据项
	LVITEM item = { 0 };
	item.mask = LVIF_TEXT | LVCF_FMT | LVCF_WIDTH;

	//行号++
	DWORD index = 0;
	for (auto& userInfoOld : userInfoList)
	{

		wstring wxid1 = get<0>(userInfoOld);
		wstring wxName = get<1>(userInfoOld);
		//wstring v1 = get<2>(userInfoOld);
		wstring nickName = get<3>(userInfoOld);

		if (wxid1 == L"" && wxName == L"") continue;

		//行号++
		index++;

		//单元格序号
		DWORD i = 0;

		//插入一行
		item.iItem = index;
		item.iSubItem = i++;
		wstring id(to_wstring(index));
		item.pszText = (LPWSTR)id.c_str();
		ListView_InsertItem(hListView, &item);

		//添加单元格数据
		//微信ID
		item.iItem = index - 1;
		item.iSubItem = i++;
		item.pszText = (LPWSTR)wxid1.c_str();
		ListView_SetItem(hListView, &item);

		//添加单元格数据
		//微信名
		item.iSubItem = i++;
		item.pszText = (LPWSTR)wxName.c_str();
		ListView_SetItem(hListView, &item);

		//添加单元格数据
		//昵称
		item.iSubItem = i++;
		item.pszText = (LPWSTR)nickName.c_str();
		ListView_SetItem(hListView, &item);
	}
}

VOID SaveToTxtFie()
{
	wstring wxUserFileName = L"WxUserLists.txt";

	DWORD index = 0;



	//作为输出文件打开
	ofstream ofile;
	ofile.open(wxUserFileName, ios_base::trunc | ios_base::binary | ios_base::in);
	//char const* const utf16head = "\xFF\xFE ";
	//ofile.write(utf16head, 2);

	for (auto& userInfoOld : userInfoList)
	{
		wstring wxid1 = get<0>(userInfoOld);
		wstring wxName = get<1>(userInfoOld);
		wstring nickName = get<3>(userInfoOld);

		if (wxid1 == L"" && wxName == L"" && nickName == L"") continue;

		index++;

		wstring userText = L"";
		userText.append(to_wstring(index))
			.append(L"\t")
			.append(wxid1)
			.append(L"\t")
			.append(wxName)
			.append(L"\t")
			.append(nickName)
			.append(L"\r\n");


		string strintStr = wstringToString(userText);
		char const* pos = (char const*)strintStr.c_str();

		////写入文件
		ofile.write(pos, strintStr.length());
		
	}
	ofile.flush();
	ofile.close();
	ShellExecute(NULL, NULL, L"notepad.exe", wxUserFileName.c_str(), L".\\", SW_SHOW);
}

std::string wstringToString(const std::wstring& wstr)
{
	LPCWSTR pwszSrc = wstr.c_str();
	int nLen = WideCharToMultiByte(CP_ACP, 0, pwszSrc, -1, NULL, 0, NULL, NULL);
	if (nLen == 0)
		return std::string("");

	char* pszDst = new char[nLen];
	if (!pszDst)
		return std::string("");

	WideCharToMultiByte(CP_ACP, 0, pwszSrc, -1, pszDst, nLen, NULL, NULL);
	std::string str(pszDst);
	delete[] pszDst;
	pszDst = NULL;

	return str;
}